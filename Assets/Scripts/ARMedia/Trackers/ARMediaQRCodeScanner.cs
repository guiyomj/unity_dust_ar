//
//  ARMediaQRCodeScanner.cs
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
 * ARMedia QR Code Scanner:
 * 
 * Using this scanner you can recognize and decode QR codes. The 'ARMediaSDK' GameObject will receive notifications about 
 * detected codes.
 * 
 * NOTE: Use this script as is or modify it to adapt it to your app's needs.
 * 
 */

public class ARMediaQRCodeScanner : ARMediaTracker 
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
	private static extern void setApplicationKeyForQRCodeScanner(string applicationKey);

	// scanner...
	[DllImport (ARMEDIA_TRACKING_LIB, CallingConvention=CallingConvention.Cdecl)]
	private static extern void initARMediaQRCodeScanner();

	[DllImport (ARMEDIA_TRACKING_LIB, CallingConvention=CallingConvention.Cdecl)]
	private static extern void cleanupARMediaQRCodeScanner();

	[DllImport(ARMEDIA_TRACKING_LIB, CallingConvention=CallingConvention.Cdecl)]
	private static extern void startARMediaQRCodeScanner();

	[DllImport(ARMEDIA_TRACKING_LIB, CallingConvention=CallingConvention.Cdecl)]
	private static extern void stopARMediaQRCodeScanner();

#elif UNITY_ANDROID

	// application...
	public static void setApplicationKeyForQRCodeScanner(string applicationKey)
	{
		using (AndroidJavaClass javaClass = new AndroidJavaClass("com.inglobetechnologies.armedia.sdk.tracking.ARMediaTrackingUnityPlugin"))
		{
			object[] args = {applicationKey};
			javaClass.CallStatic("setApplicationKeyForQRCodeScanner", args);
		}
	}

	// scanner...
	public static void initARMediaQRCodeScanner()
	{
		using (AndroidJavaClass javaClass = new AndroidJavaClass("com.inglobetechnologies.armedia.sdk.tracking.ARMediaTrackingUnityPlugin"))
		{
			javaClass.CallStatic("initARMediaQRCodeScanner");
		}
	}

	public static void cleanupARMediaQRCodeScanner()
	{
		using (AndroidJavaClass javaClass = new AndroidJavaClass("com.inglobetechnologies.armedia.sdk.tracking.ARMediaTrackingUnityPlugin"))
		{
			javaClass.CallStatic("cleanupARMediaQRCodeScanner");
		}
	}

	public static void startARMediaQRCodeScanner()
	{
		using (AndroidJavaClass javaClass = new AndroidJavaClass("com.inglobetechnologies.armedia.sdk.tracking.ARMediaTrackingUnityPlugin"))
		{
			javaClass.CallStatic("startARMediaQRCodeScanner");
		}
	}

	public static void stopARMediaQRCodeScanner()
	{
		using (AndroidJavaClass javaClass = new AndroidJavaClass("com.inglobetechnologies.armedia.sdk.tracking.ARMediaTrackingUnityPlugin"))
		{
			javaClass.CallStatic("stopARMediaQRCodeScanner");
		}
	}
#endif

	[DllImport(ARMEDIA_TRACKING_LIB, CallingConvention=CallingConvention.Cdecl)]
	public static extern void getProjectionMatrixForQRCodeScanner(double[] matrix);

	[DllImport(ARMEDIA_TRACKING_LIB, CallingConvention=CallingConvention.Cdecl)]
	private static extern void updateQRCodeScanner();

	[DllImport(ARMEDIA_TRACKING_LIB, CallingConvention=CallingConvention.Cdecl)]
	public static extern bool isScanningQRCodes();

	[DllImport(ARMEDIA_TRACKING_LIB, CallingConvention=CallingConvention.Cdecl)]
	public static extern void setDebugModeForQRCodeScanner(bool debug_mode);

#endregion

	public bool showDetectedQRCodes = true;			// set this to 'true' to display a rectangle surrounding any found QR code...

	// screen status...
	private ScreenOrientation screenOrientation;

	public override string getName()
	{
		// WARNING: Do not modify this...
		return "qr_scanner";
	}

	public override void setApplicationKey(string applicationKey)
	{
		setApplicationKeyForQRCodeScanner(applicationKey);
	}
	
	public override bool Configure(int width, int height)
	{
		return true;
	}
	
	public override void Init()
	{
		setDebugModeForQRCodeScanner(showDetectedQRCodes);

		initARMediaQRCodeScanner();
	}
	
	public override void CleanUpTracker()
	{
		cleanupARMediaQRCodeScanner();
	}
	
	public override void StartTracker()
	{
		startARMediaQRCodeScanner();
	}
	
	public override void UpdateTracker()
	{
		screenOrientation = Screen.orientation;

		updateQRCodeScanner();
	}
	
	public override void StopTracker()
	{
		stopARMediaQRCodeScanner();
	}
	
	public override bool IsTracking()
	{
		return isScanningQRCodes();
	}
	
	public override void UpdatePoses()
	{
		return;
	}
	
	public override void GetProjectionMatrix(double[] projectionMatrix)
	{
		getProjectionMatrixForQRCodeScanner(projectionMatrix);
	}

	public override bool IsTrackingCameraMode()
	{
		return false;
	}

	public override void ResetCameraTransforms()
	{
		return;
	}
}
