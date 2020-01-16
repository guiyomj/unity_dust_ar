//
//  ARMediaLocationTracker.cs
//
//  ARMedia SDK Unity Plugin & Examples
//
//  Copyright (c) 2017 Inglobe Technologies. All rights reserved.
//

using UnityEngine;
using System.Collections;
using System.Runtime.InteropServices;
using System.IO;

/*
 * ARMedia Motion Tracker:
 *
 * Using this tracker you can use the device motion sensors to control either the main Camera's or a generic GameObject
 * orientation. In order to control the main Camera's attitude you set the 'trackCameraMode' parameter to 'true' while if you
 * want to control a GameObject's orientation you set it to 'false'. When you set the 'useInitialOrientation' parameter to 'true' 
 * then the initial orientation of the device will be used a reference for the motion whereas, when it is set to 'false', the 
 * North direction will be used as reference.
 * 
 * NOTE: Use this script as is or modify it to adapt it to your app's needs.
 * 
 */

public class ARMediaMotionTracker : ARMediaTracker 
{
#if UNITY_IPHONE 
	public const string ARMEDIA_TRACKING_LIB = "__Internal";
#else
	public const string ARMEDIA_TRACKING_LIB = "ARMediaTracking";
#endif

#region Native APIs...

#if !UNITY_ANDROID

	// application...
	[DllImport(ARMEDIA_TRACKING_LIB, CallingConvention=CallingConvention.Cdecl)]
	private static extern void setApplicationKeyForMotionTracker(string applicationKey);

	// tracker...
	[DllImport (ARMEDIA_TRACKING_LIB, CallingConvention=CallingConvention.Cdecl)]
	private static extern void initARMediaMotionTracker();

	[DllImport (ARMEDIA_TRACKING_LIB, CallingConvention=CallingConvention.Cdecl)]
	private static extern void cleanupARMediaMotionTracker();

	[DllImport(ARMEDIA_TRACKING_LIB, CallingConvention=CallingConvention.Cdecl)]
	private static extern void startARMediaMotionTracker();

	[DllImport(ARMEDIA_TRACKING_LIB, CallingConvention=CallingConvention.Cdecl)]
	private static extern void stopARMediaMotionTracker();

#elif UNITY_ANDROID

	// application...
	public static void setApplicationKeyForMotionTracker(string applicationKey)
	{
		using (AndroidJavaClass javaClass = new AndroidJavaClass("com.inglobetechnologies.armedia.sdk.tracking.ARMediaTrackingUnityPlugin"))
		{
			object[] args = {applicationKey};
			javaClass.CallStatic("setApplicationKeyForMotionTracker", args);
		}
	}

	// tracker...
	public static void initARMediaMotionTracker()
	{
		using (AndroidJavaClass javaClass = new AndroidJavaClass("com.inglobetechnologies.armedia.sdk.tracking.ARMediaTrackingUnityPlugin"))
		{
			javaClass.CallStatic("initARMediaMotionTracker");
		}
	}

	public static void cleanupARMediaMotionTracker()
	{
		using (AndroidJavaClass javaClass = new AndroidJavaClass("com.inglobetechnologies.armedia.sdk.tracking.ARMediaTrackingUnityPlugin"))
		{
			javaClass.CallStatic("cleanupARMediaMotionTracker");
		}
	}

	public static void startARMediaMotionTracker()
	{
		using (AndroidJavaClass javaClass = new AndroidJavaClass("com.inglobetechnologies.armedia.sdk.tracking.ARMediaTrackingUnityPlugin"))
		{
			javaClass.CallStatic("startARMediaMotionTracker");
		}
	}

	public static void stopARMediaMotionTracker()
	{
		using (AndroidJavaClass javaClass = new AndroidJavaClass("com.inglobetechnologies.armedia.sdk.tracking.ARMediaTrackingUnityPlugin"))
		{
			javaClass.CallStatic("stopARMediaMotionTracker");
		}
	}
#endif

	[DllImport(ARMEDIA_TRACKING_LIB, CallingConvention=CallingConvention.Cdecl)]
	public static extern void getProjectionMatrixForMotionTracker(double[] matrix);
	
	[DllImport(ARMEDIA_TRACKING_LIB, CallingConvention=CallingConvention.Cdecl)]
	private static extern void updateMotionTracker();

	[DllImport(ARMEDIA_TRACKING_LIB, CallingConvention=CallingConvention.Cdecl)]
	public static extern bool isTrackingMotion();

	[DllImport(ARMEDIA_TRACKING_LIB, CallingConvention=CallingConvention.Cdecl)]
	public static extern void getPoseForMotionTracker(double[] pose);
	
#endregion

	// set this to 'true' to match the initial viewing direction to the devices' orientation regardless of the North...
	public bool useInitialOrientation = false;

	// update camera's pose vs. objects'...
	public bool trackCameraMode = false;

	// the game object we want to apply the pose to (if not using track camera mode)...
	public GameObject trackableObject;
	
	// screen status...
	private ScreenOrientation screenOrientation;

	// gyros and compass...
	private bool gyroEnabled = false;
	private bool gyroCapable = false;
	private bool compassInitd = false;
	private bool needRefreshMotion = true;

	private Quaternion orientationQuaternion;
	private Quaternion calibration =  Quaternion.identity;
	private Quaternion referenceRotation = Quaternion.identity;

#if UNITY_ANDROID
    private bool androidInit = false;
	private float compassValue = 0.0f;
#endif

	public override string getName()
	{
		// WARNING: Do not modify this...
		return "motion_tracker";
	}

	public override void setApplicationKey(string applicationKey)
	{
		setApplicationKeyForMotionTracker(applicationKey);
	}
	
	public override bool Configure(int width, int height)
	{
		return true;
	}
	
	public override void Init()
	{
		initARMediaMotionTracker();
	}
	
	public override void CleanUpTracker()
	{
		cleanupARMediaMotionTracker();
	}

	public override void StartTracker()
	{
		startARMediaMotionTracker();

		if(needRefreshMotion)
		{
			SetupMotionCalibration();
			needRefreshMotion = false;
		}
	}
	
	public override void UpdateTracker()
	{
		screenOrientation = Screen.orientation;

#if UNITY_ANDROID
		if (compassValue == 0 || !androidInit)
		{
			SetupMotionCalibration();
			androidInit = true;
		}
#endif

		updateMotionTracker();
	}
	
	public override void StopTracker()
	{
		CleanupMotionCalibration();

		stopARMediaMotionTracker();
	}
	
	public override bool IsTracking()
	{
		return isTrackingMotion();
	}
	
	public override void UpdatePoses()
	{
		if(!isTrackingMotion())
			return;

		Quaternion rot = Quaternion.identity;
		if(gyroEnabled)
			rot = Input.gyro.attitude;
		else
			Debug.LogWarning("WARNING: gyros not available.");

		if(trackCameraMode)
			setCameraPosition(mainCamera.transform.position, rot);
		else
			setObjectPosition(trackableObject.transform.position, rot);
	}
	
	public override void GetProjectionMatrix(double[] projectionMatrix)
	{
		getProjectionMatrixForMotionTracker(projectionMatrix);
	}

	public override bool IsTrackingCameraMode()
	{
		return trackCameraMode;
	}

	public override void ResetCameraTransforms()
	{
		// for motion tracking we do not need to reset anything because camera control is totally up to the user...
	}

	private void CleanupMotionCalibration()
	{
		compassInitd = false;
		Input.compass.enabled = false;
	}

	private void SetupMotionCalibration()
	{
		Input.gyro.enabled = true;
		Input.compass.enabled = true;

		gyroCapable = SystemInfo.supportsGyroscope;

		if(gyroCapable)
		{
			gyroEnabled = true;

			var fw = (Input.gyro.attitude) * (-Vector3.forward);
			fw.z = 0;

			if (fw == Vector3.zero)
				calibration = Quaternion.identity;
			else
				calibration = (Quaternion.FromToRotation(Vector3.up, fw));

#if UNITY_ANDROID
			compassValue = Input.compass.trueHeading;
#endif

			if(!useInitialOrientation)
			{
				Quaternion compass = Quaternion.AngleAxis(Input.compass.trueHeading, Vector3.up);
				referenceRotation = Quaternion.Inverse(compass) * Quaternion.Inverse(Quaternion.Euler(90, 0, 0)) * Quaternion.Inverse(calibration);
				
				InvokeRepeating("UpdateHeading", 0.5f, 0.5f);
			}
			else
				referenceRotation = Quaternion.Inverse(Quaternion.Euler(90, 0, 0)) * Quaternion.Inverse(calibration);
		}
		else
		{
			Debug.LogWarning("WARNING: gyros not available.");
			gyroEnabled = false;
		}
	}

	private void setObjectPosition(Vector3 position, Quaternion rotation)
	{
		orientationQuaternion = Quaternion.AngleAxis(0.0f, Vector3.forward);

		switch(screenOrientation)
		{
		case ScreenOrientation.LandscapeRight:
			orientationQuaternion = Quaternion.AngleAxis(180.0f, Vector3.forward);
			break;
		case ScreenOrientation.LandscapeLeft:
			orientationQuaternion = Quaternion.AngleAxis(0.0f, Vector3.forward);
			break;
		case ScreenOrientation.Portrait:
			orientationQuaternion = Quaternion.AngleAxis(90.0f, Vector3.forward);
			break;
		case ScreenOrientation.PortraitUpsideDown:
			orientationQuaternion = Quaternion.AngleAxis(270.0f, Vector3.forward);
			break;
		}

		trackableObject.transform.rotation = orientationQuaternion * Quaternion.Inverse(ARMediaUtilities.ConvertRotation(referenceRotation * rotation));
		trackableObject.transform.position = position;
	}

	private void setCameraPosition(Vector3 p, Quaternion q)
	{
		mainCamera.transform.rotation = ARMediaUtilities.ConvertRotation( referenceRotation * q);
		mainCamera.transform.position = p;
	}

	private void UpdateHeading()
	{
		if(compassInitd)
			return;

		if((Input.compass.trueHeading != 0.0) && (Input.compass.headingAccuracy >= 0.0))
		{
			compassInitd = true;

			Quaternion compass = Quaternion.AngleAxis(Input.compass.trueHeading, Vector3.up);			
			referenceRotation = Quaternion.Inverse(compass) * Quaternion.Inverse(Quaternion.Euler(90, 0, 0)) * Quaternion.Inverse(calibration);

			CancelInvoke("UpdateHeading");

			Input.compass.enabled = false;

			return;
		}
	}
}
