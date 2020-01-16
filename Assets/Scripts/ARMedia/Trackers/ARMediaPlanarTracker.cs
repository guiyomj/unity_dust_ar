//
//  ARMediaPlanarTracker.cs
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
 * ARMedia Planar Tracker:
 * 
 * Using this tracker you can recognize and track images specified by a list of ARMediaPlanarTargets attached to the tracker itself.
 * You specify the images meant to be recognized and tracked using the 'trackableKeyframes' parameter and a proper configuration 
 * file using the 'ConfigurationFile' parameter. Note that you must specify a 'StreamingAssets' relative path for the 
 * configuration file and in that same path also the tracking data (images or cache) must be found.
 * 
 * When you set the 'trackCameraMode' parameter to 'true' the tracked pose will be applied to the main Camera instead of the 
 * 'trackableKeyframes', this allows you to correctly handle physics simulations if you need so (the 'trackableKeyframes' are 
 * assumed to be fixed while the camera moves).
 * 
 * NOTE: Use this script as is or modify it to adapt it to your app's needs.
 * 
 */

public class ARMediaPlanarTracker : ARMediaTracker 
{
#if UNITY_IPHONE 
	public const string ARMEDIA_TRACKING_LIB = "__Internal";
#else
	public const string ARMEDIA_TRACKING_LIB = "ARMediaTracking";	
#endif

#region Native APIs...


#if !UNITY_ANDROID

// trackers manager...
	[DllImport (ARMEDIA_TRACKING_LIB, CallingConvention=CallingConvention.Cdecl)]
	private static extern void addVisualTrackerWithNameAndType(string trackerName, bool isObjectTracker);

	[DllImport (ARMEDIA_TRACKING_LIB, CallingConvention=CallingConvention.Cdecl)]
	private static extern void removeVisualTrackerWithName(string trackerName);

	[DllImport (ARMEDIA_TRACKING_LIB, CallingConvention=CallingConvention.Cdecl)]
	private static extern void configureVisualTrackerWithName(string trackerName, string configurationFile, int width, int height);

	[DllImport (ARMEDIA_TRACKING_LIB, CallingConvention=CallingConvention.Cdecl)]
	private static extern void initVisualTrackerWithNameAndType(string trackerName, bool isObjectTracker);

	[DllImport (ARMEDIA_TRACKING_LIB, CallingConvention=CallingConvention.Cdecl)]
	private static extern void cleanupVisualTrackerWithName(string trackerName);

	[DllImport (ARMEDIA_TRACKING_LIB, CallingConvention=CallingConvention.Cdecl)]
	private static extern void startVisualTrackerWithName(string trackerName);

	[DllImport (ARMEDIA_TRACKING_LIB, CallingConvention=CallingConvention.Cdecl)]
	private static extern void stopVisualTrackerWithName(string trackerName);

	[DllImport (ARMEDIA_TRACKING_LIB, CallingConvention=CallingConvention.Cdecl)]
	private static extern void setApplicationKeyForVisualTrackerWithName(string applicationKey, string trackerName);


#elif UNITY_ANDROID

	public static void addVisualTrackerWithNameAndType(string trackerName, bool isObjectTracker)
	{
		using (AndroidJavaClass javaClass = new AndroidJavaClass("com.inglobetechnologies.armedia.sdk.tracking.ARMediaTrackingUnityPlugin"))
		{
			object[] args = {trackerName, isObjectTracker};
			javaClass.CallStatic("addVisualTrackerWithNameAndType", args);
		}
	}

	public static void removeVisualTrackerWithName(string trackerName)
	{
		using (AndroidJavaClass javaClass = new AndroidJavaClass("com.inglobetechnologies.armedia.sdk.tracking.ARMediaTrackingUnityPlugin"))
		{
			object[] args = {trackerName};
			javaClass.CallStatic("removeVisualTrackerWithName", args);
		}
	}

	public static void configureVisualTrackerWithName(string trackerName, string configurationFile, int width, int height)
	{
		using (AndroidJavaClass javaClass = new AndroidJavaClass("com.inglobetechnologies.armedia.sdk.tracking.ARMediaTrackingUnityPlugin"))
		{
			object[] args = {trackerName, configurationFile, width, height};
			javaClass.CallStatic("configureVisualTrackerWithName", args);
		}
	}

	public static void initVisualTrackerWithNameAndType(string trackerName, bool isObjectTracker)
	{
		using (AndroidJavaClass javaClass = new AndroidJavaClass("com.inglobetechnologies.armedia.sdk.tracking.ARMediaTrackingUnityPlugin"))
		{
			object[] args = {trackerName, isObjectTracker};
			javaClass.CallStatic("initVisualTrackerWithNameAndType", args);
		}
	}

	public static void cleanupVisualTrackerWithName(string trackerName)
	{
		using (AndroidJavaClass javaClass = new AndroidJavaClass("com.inglobetechnologies.armedia.sdk.tracking.ARMediaTrackingUnityPlugin"))
		{
			object[] args = {trackerName};
			javaClass.CallStatic("cleanupVisualTrackerWithName", args);
		}
	}

	public static void startVisualTrackerWithName(string trackerName)
	{
		using (AndroidJavaClass javaClass = new AndroidJavaClass("com.inglobetechnologies.armedia.sdk.tracking.ARMediaTrackingUnityPlugin"))
		{
			object[] args = {trackerName};
			javaClass.CallStatic("startVisualTrackerWithName", args);
		}
	}

	public static void stopVisualTrackerWithName(string trackerName)
	{
		using (AndroidJavaClass javaClass = new AndroidJavaClass("com.inglobetechnologies.armedia.sdk.tracking.ARMediaTrackingUnityPlugin"))
		{
			object[] args = {trackerName};
			javaClass.CallStatic("stopVisualTrackerWithName", args);
		}
	}

	public static void setApplicationKeyForVisualTrackerWithName(string applicationKey, string trackerName)
	{
		using (AndroidJavaClass javaClass = new AndroidJavaClass("com.inglobetechnologies.armedia.sdk.tracking.ARMediaTrackingUnityPlugin"))
		{
			object[] args = {applicationKey, trackerName};
			javaClass.CallStatic("setApplicationKeyForVisualTrackerWithName", args);
		}
	}

	public static void notifyVisualListeners(string trackerName)
	{
		using (AndroidJavaClass javaClass = new AndroidJavaClass("com.inglobetechnologies.armedia.sdk.tracking.ARMediaTrackingUnityPlugin"))
		{
			object[] args = {trackerName};
			javaClass.CallStatic("notifyVisualListeners", args);
		}
	}

#endif

	[DllImport (ARMEDIA_TRACKING_LIB, CallingConvention=CallingConvention.Cdecl)]
	private static extern void updateVisualTrackerWithName(string trackerName);

	[DllImport(ARMEDIA_TRACKING_LIB, CallingConvention=CallingConvention.Cdecl)]
	public static extern void getProjectionMatrixForVisualTrackerWithName(double[] matrix, string trackerName);

	[DllImport(ARMEDIA_TRACKING_LIB, CallingConvention=CallingConvention.Cdecl)]
	public static extern bool isTrackingTrackerWithName(string trackerName);

	[DllImport(ARMEDIA_TRACKING_LIB, CallingConvention=CallingConvention.Cdecl)]
	public static extern bool isTrackingKeyframeForVisualTrackerWithName(string keyframe, string trackerName);

	[DllImport(ARMEDIA_TRACKING_LIB, CallingConvention=CallingConvention.Cdecl)]
	public static extern void getPoseForKeyframeForVisualTrackerWithName(string keyframe, double[] pose, string trackerName);

#endregion

	public string TrackerName;

	// the list of trackable images...
	public List<ARMediaPlanarTarget> trackableKeyframes;

	// tracker configuration...
	public string ConfigurationFile;

	// update camera's pose vs. objects'...
	public bool trackCameraMode = false;

	// temporaries for poses data...
	private double[] pose;
	private Matrix4x4 poseMatrix;

	// the camera projection matrix...
	private Matrix4x4 projectionMatrix;
	
	// screen status...
	private ScreenOrientation screenOrientation;

	// other data...
	private Quaternion rot90x, rot90xneg, rot180y, rot180yneg, zeroQuat, orientationQuaternion;
	private Matrix4x4 rotationMatrix, rotation90Matrix, rotation180Matrix, translationMatrix, composedMatrix, orientationMatrix, invertedMatrix, projectionFixMatrix;

	public override string getName()
	{
		return TrackerName;
	}

	public override void setApplicationKey(string applicationKey)
	{
		setApplicationKeyForVisualTrackerWithName(applicationKey, TrackerName);
	}

	public override bool Configure(int width, int height)
	{
		// set tracking configuration...
#if !UNITY_ANDROID
		string confFilePAth = Application.streamingAssetsPath + "/" + ConfigurationFile;
		if (!System.IO.File.Exists(confFilePAth))
		{
			Debug.LogError("ERROR: could not find configuration file.");
			return false;
		}
#else
		string confFilePAth = "jar:file://"+Application.dataPath+"!/assets/" + ConfigurationFile;
 	
		WWW www = new WWW(confFilePAth);

		while(!www.isDone)
		{ 
			// do nothing or handle it as you prefer...
		}

		if(!string.IsNullOrEmpty(www.error))
		{
			Debug.LogError("ERROR: could not find configuration file.");
			return false;
		}
#endif

		// setup the tracker with provided configuration file and specified capture resolution/quality...
		configureVisualTrackerWithName(TrackerName, confFilePAth, width, height);

		return true;
	}
	
	public override void Init()
	{
		if(null == TrackerName || TrackerName == "")
		{
			Debug.LogError("ERROR: Cannot init tracker with no name.");
			return;
		}

		initVisualTrackerWithNameAndType(TrackerName, false);
	}
	
	public override void CleanUpTracker()
	{
		cleanupVisualTrackerWithName(TrackerName);
	}

	public override void StartTracker()
	{
		startVisualTrackerWithName(TrackerName);
	}

	public override void StopTracker()
	{
		stopVisualTrackerWithName(TrackerName);
	}

	public override bool IsTracking()
	{
		return isTrackingTrackerWithName(TrackerName);
	}

	public override void UpdateTracker()
	{
		updateVisualTrackerWithName(TrackerName);

#if UNITY_ANDROID
		notifyVisualListeners(TrackerName);
#endif
	}

	public override void UpdatePoses()
	{
		if(0 == trackableKeyframes.Count)
		{
			Debug.LogWarning("WARNING: no keyframes available.");
			return;
		}

		if(!isTrackingTrackerWithName(TrackerName))
		{
			// hide objects...
			foreach(ARMediaPlanarTarget keyframe in trackableKeyframes)
			{
				if(null == keyframe)
					continue;
				keyframe.gameObject.SetActive(false);
			}
			return;
		}

		foreach(ARMediaPlanarTarget keyframe in trackableKeyframes)
		{
			if(null == keyframe)
				continue;

			if(isTrackingKeyframeForVisualTrackerWithName(keyframe.keyframeName, TrackerName))
			{
				keyframe.gameObject.SetActive(true);

				// retrieve last pose matrix (<- native)...
				getPoseForKeyframeForVisualTrackerWithName(keyframe.keyframeName, pose, TrackerName);

				// need to negate Z because Unity uses a different coordinates reference system wrt native part...
				poseMatrix.m00 = (float)pose[0];
				poseMatrix.m10 = (float)pose[1];
				poseMatrix.m20 = -(float)pose[2];
				poseMatrix.m30 = (float)pose[3];
				
				poseMatrix.m01 = (float)pose[4];
				poseMatrix.m11 = (float)pose[5];
				poseMatrix.m21 = -(float)pose[6];
				poseMatrix.m31 = (float)pose[7];
				
				poseMatrix.m02 = (float)pose[8];
				poseMatrix.m12 = (float)pose[9];
				poseMatrix.m22 = -(float)pose[10];
				poseMatrix.m32 = (float)pose[11];
				
				poseMatrix.m03 = (float)pose[12];
				poseMatrix.m13 = (float)pose[13];
				poseMatrix.m23 = -(float)pose[14];
				poseMatrix.m33 = (float)pose[15];
				
				if(trackCameraMode)
				{
					// reset transforms in case user moved targets in the Editor...
					keyframe.transform.position = Vector3.zero;
					keyframe.transform.rotation = zeroQuat;
					keyframe.transform.localScale = Vector3.one;

					setCameraPosition(poseMatrix.GetColumn(3),
					                  Quaternion.LookRotation(poseMatrix.GetColumn(2), poseMatrix.GetColumn(1)),
					                  new Vector3(keyframe.keyframeScale, keyframe.keyframeScale, keyframe.keyframeScale)
					                  );
				}
				else
					setObjectPosition(keyframe.gameObject, 
					                  poseMatrix.GetColumn(3), 
					                  Quaternion.LookRotation(poseMatrix.GetColumn(2), poseMatrix.GetColumn(1)),
					                  new Vector3(keyframe.keyframeScale, keyframe.keyframeScale, keyframe.keyframeScale)
					                  );
			}
			else
			{
				keyframe.gameObject.SetActive(false);
			}
		}
	}
	
	public override void GetProjectionMatrix(double[] projectionMatrix)
	{
		getProjectionMatrixForVisualTrackerWithName(projectionMatrix, TrackerName);
	}

	public override bool IsTrackingCameraMode()
	{
		return trackCameraMode;
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

	void Start ()
	{	
		rot180y = Quaternion.AngleAxis(180, Vector3.up);
		rot180yneg = Quaternion.AngleAxis(-180, Vector3.up);
		rotation180Matrix = new Matrix4x4();
		rotation180Matrix.SetTRS(Vector3.zero, rot180yneg, Vector3.one);
		rot90x = Quaternion.AngleAxis(90, Vector3.right);
		rot90xneg = Quaternion.AngleAxis(-90, Vector3.right);
		zeroQuat = new Quaternion(0.0f, 0.0f, 0.0f, 1.0f);
		rotationMatrix = new Matrix4x4();
		rotation90Matrix = new Matrix4x4();
		rotation90Matrix.SetTRS(Vector3.zero, rot90xneg, Vector3.one);
		translationMatrix = new Matrix4x4();
		composedMatrix = new Matrix4x4();
		orientationMatrix = new Matrix4x4();
		invertedMatrix = new Matrix4x4();
		
		pose = new double[16];
	}

	void Awake ()
	{
		// add tracker to tracker manager...
		if(null == TrackerName || TrackerName == "")
		{
			Debug.LogError("ERROR: Cannot create tracker with no name.");
			return;
		}

		addVisualTrackerWithNameAndType(TrackerName, false);
	}

	void OnDestroy()
	{
		// add tracker to tracker manager...
		if(null == TrackerName || TrackerName == "")
			return;

		removeVisualTrackerWithName(TrackerName);
	}

	private void setObjectPosition(GameObject target, Vector3 position, Quaternion rotation, Vector3 scale)
	{
		target.transform.rotation = rotation * rot90x * rot180y;
		target.transform.position = position;

		target.transform.localScale = scale;
	}

	private void setCameraPosition(Vector3 p, Quaternion q, Vector3 scale)
	{
		ARMediaUtilities.NormalizeQuaternion(ref q);

		rotationMatrix.SetTRS(Vector3.zero, q, Vector3.one);
		translationMatrix.SetTRS(p, zeroQuat, scale);

		composedMatrix = translationMatrix * rotationMatrix;

		if(trackCameraMode)
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

			orientationMatrix.SetTRS(Vector3.zero, orientationQuaternion, Vector3.one);
		}
		else
			orientationMatrix = Matrix4x4.identity;

		// need to invert transforms...
		invertedMatrix = rotation180Matrix * rotation90Matrix * composedMatrix.inverse * orientationMatrix;

		// update transform...		
		mainCamera.transform.rotation = Quaternion.LookRotation(invertedMatrix.GetColumn(2), invertedMatrix.GetColumn(1));
		mainCamera.transform.position = invertedMatrix.GetColumn(3);
	}
}
