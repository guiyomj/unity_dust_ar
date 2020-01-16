//
//  ARMediaLocationTracker.cs
//
//  ARMedia SDK Unity Plugin & Examples
//
//  Copyright (c) 2017 Inglobe Technologies. All rights reserved.
//

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.IO;

/*
 * ARMedia Location Tracker:
 * 
 * Using this tracker you can use the device locations services to track the user's position and display geolocated content 
 * accordingly. In order to geolocate a GameObject you must instantiate a ARMediaLocationTarget object and specify its latitude,
 * longitude and altitude (if it matters) and add it to the target as a child. If can choose to ignore the user's altitude (and
 * hence the geolocated objects' altitude as well) by setting the 'ignoreAltitude' parameter to 'true'. A default user's height 
 * is assumed to be 1.70 meters, feel free to modify it if it makes sense in your app.
 *
 * NOTE: Use this script as is or modify it to adapt it to your app's needs.
 * 
 */

public class ARMediaLocationTracker : ARMediaTracker 
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
	private static extern void setApplicationKeyForLocationTracker(string applicationKey);

	// tracker...
	[DllImport (ARMEDIA_TRACKING_LIB, CallingConvention=CallingConvention.Cdecl)]
	private static extern void initARMediaLocationTracker();

	[DllImport (ARMEDIA_TRACKING_LIB, CallingConvention=CallingConvention.Cdecl)]
	private static extern void cleanupARMediaLocationTracker();

	[DllImport(ARMEDIA_TRACKING_LIB, CallingConvention=CallingConvention.Cdecl)]
	private static extern void startARMediaLocationTracker();

	[DllImport(ARMEDIA_TRACKING_LIB, CallingConvention=CallingConvention.Cdecl)]
	private static extern void stopARMediaLocationTracker();

#elif UNITY_ANDROID

	// application...
	public static void setApplicationKeyForLocationTracker(string applicationKey)
	{
		using (AndroidJavaClass javaClass = new AndroidJavaClass("com.inglobetechnologies.armedia.sdk.tracking.ARMediaTrackingUnityPlugin"))
		{
			object[] args = {applicationKey};
			javaClass.CallStatic("setApplicationKeyForLocationTracker", args);
		}
	}

	// tracker...
	public static void initARMediaLocationTracker()
	{
		using (AndroidJavaClass javaClass = new AndroidJavaClass("com.inglobetechnologies.armedia.sdk.tracking.ARMediaTrackingUnityPlugin"))
		{
			javaClass.CallStatic("initARMediaLocationTracker");
		}
	}

	public static void cleanupARMediaLocationTracker()
	{
		using (AndroidJavaClass javaClass = new AndroidJavaClass("com.inglobetechnologies.armedia.sdk.tracking.ARMediaTrackingUnityPlugin"))
		{
			javaClass.CallStatic("cleanupARMediaLocationTracker");
		}
	}

	public static void startARMediaLocationTracker()
	{
		using (AndroidJavaClass javaClass = new AndroidJavaClass("com.inglobetechnologies.armedia.sdk.tracking.ARMediaTrackingUnityPlugin"))
		{
			javaClass.CallStatic("startARMediaLocationTracker");
		}
	}

	public static void stopARMediaLocationTracker()
	{
		using (AndroidJavaClass javaClass = new AndroidJavaClass("com.inglobetechnologies.armedia.sdk.tracking.ARMediaTrackingUnityPlugin"))
		{
			javaClass.CallStatic("stopARMediaLocationTracker");
		}
	}
#endif

	[DllImport(ARMEDIA_TRACKING_LIB, CallingConvention=CallingConvention.Cdecl)]
	public static extern void getProjectionMatrixForLocationTracker(double[] matrix);
	
	[DllImport(ARMEDIA_TRACKING_LIB, CallingConvention=CallingConvention.Cdecl)]
	private static extern void updateLocationTracker();

	[DllImport(ARMEDIA_TRACKING_LIB, CallingConvention=CallingConvention.Cdecl)]
	public static extern bool isTrackingLocations();

	[DllImport(ARMEDIA_TRACKING_LIB, CallingConvention=CallingConvention.Cdecl)]
	public static extern void getPoseForLocationTracker(double[] pose);

	[DllImport(ARMEDIA_TRACKING_LIB, CallingConvention=CallingConvention.Cdecl)]
	public static extern void getLocation(double[] location_coordinates);

	[DllImport(ARMEDIA_TRACKING_LIB, CallingConvention=CallingConvention.Cdecl)]
	public static extern void getCoordinatesForLocation(double[] cartesian_coordinates, double[] location_coordinates);

	[DllImport(ARMEDIA_TRACKING_LIB, CallingConvention=CallingConvention.Cdecl)]
	public static extern void ignoreAltitudeForLocationTracker(bool ignore_altitude);
	
#endregion
	
	// set this to 'true' to ignore location altitude data...
	public bool ignoreAltitude = false;

	// user's eyes height...
	public float usersHeight = 1.70f;
	
	// the locations targets where objects are going to be displayed...
	public List<ARMediaLocationTarget> trackableLocations;
	
	// gyros and compass...
	private bool gyroEnabled = false;
	private bool gyroCapable = false;
	private bool compassInitd = false;
	private bool needRefreshMotion = true;

	private Quaternion orientationQuaternion;
	private Quaternion calibration =  Quaternion.identity;
	private Quaternion referenceRotation = Quaternion.identity;

	private double[] userLocation;
	private double[] previousUserLocation;

#if UNITY_ANDROID
	private bool androidInit = false;
	private float compassValue = 0.0f;
#endif

	public override string getName()
	{
		// WARNING: Do not modify this...
		return "location_tracker";
	}

	public override void setApplicationKey(string applicationKey)
	{
		setApplicationKeyForLocationTracker(applicationKey);
	}
	
	public override bool Configure(int width, int height)
	{
		return true;
	}
	
	public override void Init()
	{
		userLocation = new double[3];
		previousUserLocation = new double[3];

		initARMediaLocationTracker();

		ignoreAltitudeForLocationTracker(ignoreAltitude);
	}
	
	public override void CleanUpTracker()
	{
		cleanupARMediaLocationTracker();

		userLocation = null;
		previousUserLocation = null;
	}

	public override void StartTracker()
	{
		startARMediaLocationTracker();

		if(needRefreshMotion)
		{
			SetupMotionCalibration();
			needRefreshMotion = false;
		}
	}
	
	public override void UpdateTracker()
	{
		updateLocationTracker();

		// get current location
		getLocation(userLocation);

		// if different from previous location, update registered locations...
		if((userLocation[0] != previousUserLocation[0]) || (userLocation[1] != previousUserLocation[1]) || (userLocation[2] != previousUserLocation[2] && !ignoreAltitude))
		{
			previousUserLocation[0] = userLocation[0];
			previousUserLocation[1] = userLocation[1];
			previousUserLocation[2] = userLocation[2];

			UpdateLocations();
		}

#if UNITY_ANDROID
		if (compassValue == 0 || !androidInit)
		{
			SetupMotionCalibration();
			androidInit = true;
		}
#endif
	}
	
	public override void StopTracker()
	{
		CleanupMotionCalibration();

		stopARMediaLocationTracker();
	}
	
	public override bool IsTracking()
	{
		return isTrackingLocations();
	}
	
	public override void UpdatePoses()
	{
		if(!isTrackingLocations())
			return;

		Quaternion rot = Quaternion.identity;
		if(gyroEnabled)
			rot = Input.gyro.attitude;
		else
			Debug.LogWarning("WARNING: gyros not available.");

		setCameraPosition(mainCamera.transform.position, rot);
	}
	
	public override void GetProjectionMatrix(double[] projectionMatrix)
	{
		getProjectionMatrixForLocationTracker(projectionMatrix);
	}

	public override bool IsTrackingCameraMode()
	{
		return true;
	}

	public override void ResetCameraTransforms()
	{
		if(null == mainCamera)
		{
			Debug.LogWarning("Could not reset camera transform.");
			return;
		}

		mainCamera.transform.parent = null;
		mainCamera.transform.position = new Vector3(0, 0, 0);
		mainCamera.transform.localRotation = Quaternion.identity;

		mainCamera.nearClipPlane = 0.03f;
		mainCamera.farClipPlane = 1000.0f;
	}

	private void CleanupMotionCalibration()
	{
		compassInitd = false;

		Input.compass.enabled = false;
		Input.location.Stop();
	}

	private void SetupMotionCalibration()
	{
		Input.compass.enabled = true;
		Input.location.Start();

		gyroCapable = SystemInfo.supportsGyroscope;

		Input.gyro.enabled = true;

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

			Quaternion compass = Quaternion.AngleAxis(Input.compass.trueHeading, Vector3.up);
			referenceRotation = Quaternion.Inverse(compass) * Quaternion.Inverse(Quaternion.Euler(90, 0, 0)) * Quaternion.Inverse(calibration);

			InvokeRepeating("UpdateHeading", 0.5f, 0.5f);
		}
		else
		{
			Debug.LogWarning("WARNING: gyros not available.");
			gyroEnabled = false;
		}
	}
	
	private void setCameraPosition(Vector3 p, Quaternion q)
	{
		mainCamera.transform.rotation = ARMediaUtilities.ConvertRotation( referenceRotation * q);
		mainCamera.transform.position = new Vector3(0.0f, usersHeight, 0.0f);
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
			Input.location.Stop();

			return;
		}
	}

	private void UpdateLocations()
	{
		// for each registered location, updates its cartesian coordinates...
		foreach(ARMediaLocationTarget location in trackableLocations)
		{
			if(!location.valid)
				continue;

			double[] location_coordinates = new double[3];
			double[] cartesian_coordinates = new double[3];

			location_coordinates[0] = location.latitude;
			location_coordinates[1] = location.longitude;
			location_coordinates[2] = location.altitude;

			getCoordinatesForLocation(cartesian_coordinates, location_coordinates);

			location.gameObject.transform.position = new Vector3((float)cartesian_coordinates[0],	// X (east)
			                                                     (float)cartesian_coordinates[1],	// Y (height)
			                                                     (float)cartesian_coordinates[2]	// Z (north)
			                                                     );
		}
	}
}
