//
//  ARMediaSDK.cs
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
 * ARMediaSDK:
 * 
 * This is the main component that must be always present in any scene that uses the SDK facilities. You need to have one and 
 * only one of such component in your scene. Using the properties of this component:
 * 
 *  - you specify the Application Key (received from the ARMedia SDK Developer Portal (http://dev.inglobetechnologies.com) 
 *    specific to the app you are developing
 *  - you set the actual tracker/scanner to use in your scene (one among ARMediaObjectTracker, ARMediaPlanarTracker, 
 *    ARMediaLocationTracker, ARMediaMotionTracker and ARMediaQRCodeScanner)
 *  - you choose the camera resolution (either LOW or HIGH) for to be used to capture the video from the mobile device's camera
 *  - and you specify the Camera that will be used to render the augmented content (required to correctly adjust projection
 *    and other properties requried for the augmentation)
 * 
 * NOTE: Use this script as is or modify it to adapt it to your app's needs.
 * 
 */

public class ARMediaSDKMessage
{
	public string sender = null;
	public string message = null;
}

public class ARMediaSDK : MonoBehaviour 
{
#if UNITY_IPHONE 
	public const string ARMEDIA_TRACKING_LIB = "__Internal";
#else
	public const string ARMEDIA_TRACKING_LIB = "ARMediaTracking";	
#endif
	
#region Native APIs...
	
#if !UNITY_ANDROID

	// camera...
	[DllImport (ARMEDIA_TRACKING_LIB, CallingConvention=CallingConvention.Cdecl)]
	private static extern void configureARMediaCamera(bool highQuality);
	
	[DllImport (ARMEDIA_TRACKING_LIB, CallingConvention=CallingConvention.Cdecl)]
	private static extern void initARMediaCamera();
	
	[DllImport (ARMEDIA_TRACKING_LIB, CallingConvention=CallingConvention.Cdecl)]
	private static extern void cleanupARMediaCamera();
	
	[DllImport (ARMEDIA_TRACKING_LIB, CallingConvention=CallingConvention.Cdecl)]
	private static extern void startARMediaCamera();
	
	[DllImport (ARMEDIA_TRACKING_LIB, CallingConvention=CallingConvention.Cdecl)]
	private static extern void stopARMediaCamera();

#elif UNITY_ANDROID

	// camera...
	public static void configureARMediaCamera(bool highQuality)
	{
		using (AndroidJavaClass javaClass = new AndroidJavaClass("com.inglobetechnologies.armedia.sdk.tracking.ARMediaTrackerUnityWrapper"))
		{
			object[] args = {highQuality};
			javaClass.CallStatic("configureARMediaCamera", args);
		}
	}
	
	public static void initARMediaCamera()
	{
		using (AndroidJavaClass javaClass = new AndroidJavaClass("com.inglobetechnologies.armedia.sdk.tracking.ARMediaTrackerUnityWrapper"))
		{
			javaClass.CallStatic("initARMediaCamera");
		}
	}
	
	public static void cleanupARMediaCamera()
	{
		using (AndroidJavaClass javaClass = new AndroidJavaClass("com.inglobetechnologies.armedia.sdk.tracking.ARMediaTrackerUnityWrapper"))
		{
			javaClass.CallStatic("cleanupARMediaCamera");
		}
	}
	
	public static void startARMediaCamera()
	{
		using (AndroidJavaClass javaClass = new AndroidJavaClass("com.inglobetechnologies.armedia.sdk.tracking.ARMediaTrackerUnityWrapper"))
		{
			javaClass.CallStatic("startARMediaCamera");
		}
	}
	
	public static void stopARMediaCamera()
	{
		using (AndroidJavaClass javaClass = new AndroidJavaClass("com.inglobetechnologies.armedia.sdk.tracking.ARMediaTrackerUnityWrapper"))
		{
			javaClass.CallStatic("stopARMediaCamera");
		}
	}

#endif

	[DllImport (ARMEDIA_TRACKING_LIB, CallingConvention=CallingConvention.Cdecl)]
	private static extern void updateARMediaCamera();
	
	[DllImport(ARMEDIA_TRACKING_LIB, CallingConvention=CallingConvention.Cdecl)]
	private static extern uint getARMediaCameraWidth();	

	[DllImport(ARMEDIA_TRACKING_LIB, CallingConvention=CallingConvention.Cdecl)]
	private static extern uint getARMediaCameraHeight();

	[DllImport(ARMEDIA_TRACKING_LIB, CallingConvention=CallingConvention.Cdecl)]
	private static extern System.IntPtr createNativeTexture();

	[DllImport(ARMEDIA_TRACKING_LIB, CallingConvention=CallingConvention.Cdecl)]
	private static extern void destroyNativeTexture(System.IntPtr texture);

#endregion

	// application signature...
	public string applicationKey;
	
	// the video background...
	public enum CAMERA_RESOLUTION { LOW, HIGH };	
	public CAMERA_RESOLUTION cameraResolution = CAMERA_RESOLUTION.LOW;
	private GameObject backgroundCameraObject = null;
	private Camera backgroundCamera = null;
	private GameObject backgroundPlane = null;
	private int backgroundPlaneLayer = 20;		// NOTE: we use layer 20 to render the video background, if you need it for other purposes feel free to change to the layer number to any available).
	private Texture2D videoTex;
	private System.IntPtr videoTexPtr;

	// the trackers...
	public List<GameObject> trackersObjects;
	private List<ARMediaTracker> trackers = new List<ARMediaTracker>();
	private Hashtable trackersByName = new Hashtable();
	private Hashtable trackersInitdByName = new Hashtable();

	// the main camera...
	public Camera mainCamera;
	private double[] projection;
	private Matrix4x4 projectionMatrix;

	// init status...
	private bool initd = false;
	private bool initdCamera = false;

	// screen status...
	private ScreenOrientation screenOrientation;
	private float screenWidth;
	private float screenHeight;
	private float screenAspectRatio;

	// video details...
	private float videoWidth;
	private float videoHeight;
	private float videoAspectRatio;
	private float scaledVideoWidth;
	private float scaledVideoHeight;

	// other data...
	private Matrix4x4 projectionFixMatrix;

	public void setupVideoBackground()
	{
		// setup layers, culling & clear color...
		LayerMask backgroundPlaneLayerMask = 1 << backgroundPlaneLayer;
		backgroundPlane.layer = backgroundPlaneLayer;
		backgroundCamera.cullingMask = backgroundPlaneLayerMask;
		backgroundCamera.clearFlags = CameraClearFlags.Skybox;
		mainCamera.cullingMask &= ~backgroundPlaneLayerMask;
		mainCamera.clearFlags = CameraClearFlags.Depth;

		// setup texture for video feed...
		videoTexPtr = createNativeTexture();
		videoTex = Texture2D.CreateExternalTexture((int)videoWidth, (int)videoHeight, TextureFormat.RGBA32, false, false, videoTexPtr);

		backgroundPlane.GetComponent<Renderer>().material.mainTexture = videoTex; 

		// set UVs to match native part...
		MeshFilter meshFilter = (MeshFilter)backgroundPlane.GetComponent(typeof(MeshFilter));
		if(meshFilter)
			meshFilter.mesh.uv = new Vector2[] { new Vector2 (0, 1), new Vector2 (1, 0), new Vector2 (1, 1), new Vector2 (0, 0) };
		else
			Debug.LogError("ERROR: could not find mesh filter component of video background object.");

		if(screenOrientation == ScreenOrientation.LandscapeLeft || screenOrientation == ScreenOrientation.LandscapeRight)
			scaledVideoWidth = screenWidth;
		else
			scaledVideoWidth = screenHeight;

		scaledVideoHeight = (scaledVideoWidth / videoAspectRatio);
		
		backgroundPlane.transform.localScale = new Vector3(scaledVideoWidth, scaledVideoHeight, 1.0f);
		backgroundCamera.orthographicSize = screenHeight/2.0f;
	}

	public void updateVideoBackground()
	{
		backgroundCamera.orthographicSize = screenHeight/2.0f;

		switch(screenOrientation)
		{
		case ScreenOrientation.LandscapeRight:
			backgroundPlane.transform.localRotation = Quaternion.AngleAxis(180.0f, Vector3.forward);
			break;
		case ScreenOrientation.LandscapeLeft:
			backgroundPlane.transform.localRotation = Quaternion.AngleAxis(0.0f, Vector3.forward);
			break;
		case ScreenOrientation.Portrait:
			backgroundPlane.transform.localRotation = Quaternion.AngleAxis(270.0f, Vector3.forward);
			break;
		case ScreenOrientation.PortraitUpsideDown:
			backgroundPlane.transform.localRotation = Quaternion.AngleAxis(90.0f, Vector3.forward);
			break;
		}
	}

	public void updateProjectionMatrix(ARMediaTracker tracker)
	{
		if(null == tracker && null == trackers)
			return;

		if(null == tracker)
			tracker = trackers[0];

		if(!tracker.IsTrackingCameraMode())
		{
			switch(screenOrientation)
			{
			case ScreenOrientation.LandscapeRight:
				mainCamera.transform.localRotation = Quaternion.AngleAxis(180.0f, Vector3.forward);
				break;
			case ScreenOrientation.LandscapeLeft:
				mainCamera.transform.localRotation = Quaternion.AngleAxis(0.0f, Vector3.forward);
				break;
			case ScreenOrientation.Portrait:
				mainCamera.transform.localRotation = Quaternion.AngleAxis(90.0f, Vector3.forward);
				break;
			case ScreenOrientation.PortraitUpsideDown:
				mainCamera.transform.localRotation = Quaternion.AngleAxis(270.0f, Vector3.forward);
				break;
			}
		}
	
		// Retrieve camera projection matrix (<- native)...
		tracker.GetProjectionMatrix(projection);

		projectionMatrix.m00 = (float)projection[0];
		projectionMatrix.m10 = (float)projection[1];
		projectionMatrix.m20 = (float)projection[2];
		projectionMatrix.m30 = (float)projection[3];
		
		projectionMatrix.m01 = (float)projection[4];
		projectionMatrix.m11 = (float)projection[5];
		projectionMatrix.m21 = (float)projection[6];
		projectionMatrix.m31 = (float)projection[7];
		
		projectionMatrix.m02 = (float)projection[8];
		projectionMatrix.m12 = (float)projection[9];
		projectionMatrix.m22 = (float)projection[10];
		projectionMatrix.m32 = (float)projection[11];
		
		projectionMatrix.m03 = (float)projection[12];
		projectionMatrix.m13 = (float)projection[13];
		projectionMatrix.m23 = (float)projection[14];
		projectionMatrix.m33 = (float)projection[15];
		
		// NOTE: before changing the actual projection matrix, we need to also adjust the viewport (to match video size)...
		float xOffset = 0.0f;
		float yOffset = 0.0f;
		
		if(screenOrientation == ScreenOrientation.LandscapeLeft || screenOrientation == ScreenOrientation.LandscapeRight)
		{
			xOffset = (screenWidth - scaledVideoWidth)/2.0f;
			yOffset = (screenHeight - scaledVideoHeight)/2.0f;
		}
		else
		{
			xOffset = (screenWidth - scaledVideoHeight)/2.0f;
			yOffset = (screenHeight - scaledVideoWidth)/2.0f;
		}
		
		Rect cameraViewportNormalized = mainCamera.rect;
		cameraViewportNormalized.xMin = xOffset/screenWidth;
		cameraViewportNormalized.yMin = yOffset/screenHeight;
		cameraViewportNormalized.xMax = (scaledVideoWidth-Mathf.Abs(xOffset))/screenWidth;
		cameraViewportNormalized.yMax = (scaledVideoHeight-Mathf.Abs(yOffset))/screenHeight;
		
		// correct projection because Unity will modify projection to fit new viewport...
		projectionFixMatrix = Matrix4x4.TRS(new Vector3(0, 0, 0), Quaternion.identity, new Vector3 (cameraViewportNormalized.width, cameraViewportNormalized.height, 1.0f));
		
		// apply actual 'corrected' projection...
		mainCamera.projectionMatrix = projectionFixMatrix * projectionMatrix;
	}

	IEnumerator Start ()
	{
		if ((Application.platform != RuntimePlatform.IPhonePlayer) && (Application.platform != RuntimePlatform.Android))
		{
			Debug.Log("WARNING: you need to execute the app on a real device (Android or iOS).");
			Destroy(gameObject);
			yield return null;
		}

		Destroy(GetComponent("ARMediaAutoName"));

		if(0 == trackersObjects.Count)
		{
			Debug.LogWarning("WARNING: no trackers set.");
			Destroy(gameObject);
			yield return null;
		}

		foreach(GameObject trackerObject in trackersObjects)
		{
			if(null == trackerObject)
				continue;

			// get actual tracker object...
			ARMediaTracker tracker = trackerObject.GetComponent<ARMediaTracker>();
			if(!tracker)
			{
				Debug.Log("WARNING: tracker not valid.");
				trackers.Add(null);
			}
			else
			{
				// add the tracker to the list of trackers...
				trackers.Add(tracker);
				trackersByName.Add(tracker.getName(), tracker);
				trackersInitdByName.Add(tracker.getName(), "false");
			}
		}

		// init some data...
		backgroundCameraObject = (GameObject)Instantiate(Resources.Load("ARMedia/Prefabs/Utilities/ARMediaBackgroundCamera"));
		backgroundPlane = (GameObject)Instantiate(Resources.Load("ARMedia/Prefabs/Utilities/ARMediaBackgroundPlane"));
		backgroundCamera = backgroundCameraObject.GetComponent<Camera>();

		projection = new double[16];

		foreach(ARMediaTracker tracker in trackers)
		{
			if(null == tracker)
				continue;

			// set main camera...
			tracker.SetCamera(mainCamera);

			// reset main camera's transforms (this will also modify near and far planes)...
			tracker.ResetCameraTransforms();

			// set app key...
			tracker.setApplicationKey(applicationKey);
		}

		// setup the camera to use choosen resolution...
		configureARMediaCamera(CAMERA_RESOLUTION.HIGH == cameraResolution);

		// setup screen status & background (must be done after camera configuration)...
		screenOrientation = Screen.orientation;
		screenWidth = Screen.width;
		screenHeight = Screen.height;
		
		// NOTE: sometimes, when the app starts the device orientation may be Unknown, so we must setup things in a different way (you may want to handle this in a different way)...
		screenAspectRatio = (screenWidth / screenHeight);
		if(screenOrientation == ScreenOrientation.Unknown)
		{
			if (screenAspectRatio > 1.0f)
				screenOrientation = ScreenOrientation.LandscapeLeft;
			else
				screenOrientation = ScreenOrientation.Portrait;
		}

		// setup video background...
		videoWidth = getARMediaCameraWidth();
		videoHeight = getARMediaCameraHeight();
		videoAspectRatio = (videoWidth / videoHeight);
		
		setupVideoBackground();

		// issue device's camera initialization: this could take some time, a message will be sent upon completion (->OnInitCamera)...
		initARMediaCamera();

		foreach(ARMediaTracker tracker in trackers)
		{
			if(null == tracker)
				continue;

			if(!tracker.Configure((int)videoWidth, (int)videoHeight))
			{
				Debug.LogError("ERROR: could not configure the tracker");
				trackersInitdByName[tracker.getName()] = "false";
			}
		}

#if UNITY_IPHONE
		Handheld.SetActivityIndicatorStyle(UnityEngine.iOS.ActivityIndicatorStyle.White);
#elif UNITY_ANDROID
		Handheld.SetActivityIndicatorStyle(AndroidActivityIndicatorStyle.Small);
#endif
		Handheld.StartActivityIndicator();

		// issue tracker initialization: will usually take some time, a message will be sent upon completion (->OnInitTracker)...
		foreach(ARMediaTracker tracker in trackers)
		{
			if(null == tracker)
				continue;
			tracker.Init();
		}

		// main camera projection & viewport (must be done here and also after tracker initialization)...
		updateVideoBackground();

		updateProjectionMatrix(null);
	}

	void Update ()
	{
		// if screen orientation or size has not changed, update background and projection...
		if (screenWidth != Screen.width || screenHeight != Screen.height || screenOrientation == ScreenOrientation.Unknown || (screenOrientation != Screen.orientation))
		{
			// update data  to take into account new screen orientation and size...
			screenOrientation = Screen.orientation;
			screenWidth = Screen.width;
			screenHeight = Screen.height;

			// NOTE: sometimes, since we have the navigation bar, width and height are not simply swapped when changing orientation, we must re-set them accordingly (and we have to deal with the 'Unknown' orientation)...
			screenAspectRatio = (screenWidth / screenHeight);
			if(screenOrientation == ScreenOrientation.Unknown)
			{
				if (screenAspectRatio > 1.0f)
					screenOrientation = ScreenOrientation.LandscapeLeft;
				else
					screenOrientation = ScreenOrientation.Portrait;
			}

			if(screenOrientation == ScreenOrientation.LandscapeLeft || screenOrientation == ScreenOrientation.LandscapeRight)
				scaledVideoWidth = screenWidth;
			else if(screenOrientation == ScreenOrientation.Portrait || screenOrientation == ScreenOrientation.PortraitUpsideDown)
				scaledVideoWidth = screenHeight;
					
			scaledVideoHeight = (scaledVideoWidth / videoAspectRatio);
			backgroundPlane.transform.localScale = new Vector3(scaledVideoWidth, scaledVideoHeight, 1.0f);
			backgroundCamera.orthographicSize = screenHeight/2.0f;

			// adjust background and projection...
			updateVideoBackground();

			foreach(ARMediaTracker tracker in trackers)
			{
				if("true" == (string)trackersInitdByName[tracker.getName()])
					updateProjectionMatrix(tracker);
				break;
			}
		}

		// update camera...
		updateARMediaCamera();

		// update trackers...
		foreach(ARMediaTracker tracker in trackers)
		{
			if(null == tracker)
				continue;

			tracker.UpdateTracker();
		}

		// get poses (if tracking)...
		foreach(ARMediaTracker tracker in trackers)
		{
			if(null == tracker)
				continue;

			if("true" == (string)trackersInitdByName[tracker.getName()])
				tracker.UpdatePoses();
		}
	}

#region Native Plugin Callbacks...

	/* 
	 * IMPORTANT: the gameobject this component is attached to must be called "ARMediaSDK" in order to receive the following callbacks.
	 */ 

	// callbacks from native plugin...
	void OnInitCamera(string status)
	{
		//Screen.sleepTimeout = SleepTimeout.SystemSetting;

		if("OK" == status)
		{
			// start the camera...
			startARMediaCamera();

			initdCamera = true;
        }
		else
		{
			Debug.LogError("ERROR: could not setup camera module.");
			initdCamera = false;
		}

		bool initdTrackers = true;
		foreach(ARMediaTracker tracker in trackers)
		{
			if(null == tracker)
				continue;
			initdTrackers = initdTrackers && ("true" == (string)trackersInitdByName[tracker.getName()]);
		}
		initd = initdCamera && initdTrackers;
	}

	void OnInitTracker(string message)
	{
		Handheld.StopActivityIndicator();

		ARMediaSDKMessage sdk_message = JsonUtility.FromJson<ARMediaSDKMessage>(message);
		ARMediaTracker sender_tracker = (ARMediaTracker)trackersByName[sdk_message.sender];

		if (null == sender_tracker) 
		{
			Debug.Log("Could not find tracker: "+sdk_message.sender);
			return;
		}

		if("OK" == sdk_message.message)
		{
			Debug.Log("Starting tracker...");

			// start the tracker...
			sender_tracker.StartTracker();

			// setup main camera projection & viewport (must be done after tracker initialization)...
			updateProjectionMatrix(sender_tracker);

			trackersInitdByName[sender_tracker.getName()] = "true";

			Debug.Log("Starting tracker...done.");
		}
		else
		{
			Debug.Log("ERROR: could not start tracker");
			trackersInitdByName[sender_tracker.getName()] = "false";
		}

		//initd = initdCamera && initdTracker;
		bool initdTrackers = true;
		foreach(ARMediaTracker tracker in trackers)
		{
			if(null == tracker)
				continue;
			initdTrackers = initdTrackers && ("true" == (string)trackersInitdByName[tracker.getName()]);
		}
		initd = initdCamera && initdTrackers;
	}

	// Object Tracker Callbacks...
	void OnTrackerFoundObject(string message)
	{
		ARMediaSDKMessage sdk_message = JsonUtility.FromJson<ARMediaSDKMessage>(message);
		Debug.Log("Tracker ["+sdk_message.sender+"] found object.");

		// do whatever you want here...
	}

	void OnTrackerLostObject(string message)
	{
		ARMediaSDKMessage sdk_message = JsonUtility.FromJson<ARMediaSDKMessage>(message);
		Debug.Log("Tracker ["+sdk_message.sender+"] lost object.");

		// do whatever you want here...
	}

	// Planar Tracker Callbacks...
	void OnTrackerFoundKeyframeWithName(string message)
	{
		ARMediaSDKMessage sdk_message = JsonUtility.FromJson<ARMediaSDKMessage>(message);
		Debug.Log("Tracker ["+sdk_message.sender+"] found keyframe: "+sdk_message.message);
		
		// do whatever you want here...
	}
	
	void OnTrackerLostKeyframeWithName(string message)
	{
		ARMediaSDKMessage sdk_message = JsonUtility.FromJson<ARMediaSDKMessage>(message);
		Debug.Log("Tracker ["+sdk_message.sender+"] lost keyframe: "+sdk_message.message);
		
		// do whatever you want here...
	}

	// Location Tracker Callbacks...
	void OnTrackerDidUpdateLocation(string message)
	{
		ARMediaSDKMessage sdk_message = JsonUtility.FromJson<ARMediaSDKMessage>(message);
		Debug.Log("Tracker ["+sdk_message.sender+"] updated.");

		// do whatever you want here...
	}

	// QR Code Scanner Callbacks...
	void OnScannerFoundQRCode(string message)
	{
		ARMediaSDKMessage sdk_message = JsonUtility.FromJson<ARMediaSDKMessage>(message);
		Debug.Log("Scanner ["+sdk_message.sender+"] found QR code: "+sdk_message.message);

		// do whatever you want here...
	}
#endregion

	// Unity callbacks...
	void OnEnable()
	{
		if ((Application.platform != RuntimePlatform.IPhonePlayer) && (Application.platform != RuntimePlatform.Android))
		{
			Debug.Log("WARNING: you need to execute the app on a real device (Android or iOS).");
			return;
		}

		if(initdCamera)
			startARMediaCamera();

		foreach(ARMediaTracker tracker in trackers)
		{
			if(null == tracker)
				continue;

			if("true" == (string)trackersInitdByName[tracker.getName()])
				tracker.StartTracker();
		}

		// enable related objects as well...
		if(backgroundPlane)
			backgroundPlane.SetActive(true);
		if(backgroundCameraObject)
			backgroundCameraObject.SetActive(true);
	}

	void OnDisable()
	{
		if ((Application.platform != RuntimePlatform.IPhonePlayer) && (Application.platform != RuntimePlatform.Android))
		{
			Debug.Log("WARNING: you need to execute the app on a real device (Android or iOS).");
			return;
		}

		foreach(ARMediaTracker tracker in trackers)
		{
			if(null == tracker)
				continue;
			tracker.StopTracker();
		}
		stopARMediaCamera();

		// disable related objects as well...
		if(backgroundCameraObject)
			backgroundCameraObject.SetActive(false);
		if(backgroundPlane)
			backgroundPlane.SetActive(false);
	}

	void OnApplicationPause(bool pause)
	{
		if(!initd)
			return;

		if (pause)
		{
			foreach(ARMediaTracker tracker in trackers)
			{
				if(null == tracker)
					continue;
				tracker.StopTracker();
			}
			stopARMediaCamera();
		}
		else
		{
			if(initdCamera)
				startARMediaCamera();

			foreach(ARMediaTracker tracker in trackers)
			{
				if(null == tracker)
					continue;

				if("true" == (string)trackersInitdByName[tracker.getName()])
					tracker.StartTracker();
			}
		}
	}

	void OnDestroy()
	{
		if ((Application.platform != RuntimePlatform.IPhonePlayer) && (Application.platform != RuntimePlatform.Android))
			return;

		foreach(ARMediaTracker tracker in trackers)
		{
			if(null == tracker)
				continue;
			tracker.StopTracker();
		}
		stopARMediaCamera();
		
		// cleanup tracker & camera...
		foreach(ARMediaTracker tracker in trackers)
		{
			if(null == tracker)
				continue;
			tracker.CleanUpTracker();
			trackersInitdByName[tracker.getName()] = "false";
		}
		cleanupARMediaCamera();
		
		initdCamera = false;
		initd = false;

		// destroy related objects as well...
		Destroy(backgroundPlane);
		Destroy(backgroundCameraObject);

		destroyNativeTexture(videoTexPtr);
	}
}
