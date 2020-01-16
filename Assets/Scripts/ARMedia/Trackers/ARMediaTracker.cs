//
//  ARMediaTracker.cs
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
 * ARMedia Tracker: 
 * 
 * Abstract base class for any tracker. You do not use this class directly, instead refer to any of the derived classes
 * to use specific features of the SDK.
 * 
 * Classes derived from this include:
 * 
 * - ARMediaObjectTracker: used to recognise and track generic objects (car engine, buildings, toys, ...)
 * - ARMediaPlanarTracker: used to recognise and track images (posters, blue prints, magazines, books, ...)
 * - ARMediaLocationTracker: used to track the user's position on Earth and display geolocated content around him/her
 * - ARMediaMotionTracker: used to track the mobile device's orientation and display content around the user
 * 
 */

public abstract class ARMediaTracker : MonoBehaviour 
{
	public abstract string getName();

	public abstract void setApplicationKey(string applicationKey);

	public abstract bool Configure(int width, int height);
	
 	public abstract void Init();

	public abstract void CleanUpTracker();

	public abstract void StartTracker();

	public abstract void StopTracker();

	public abstract bool IsTracking();

	public abstract void UpdateTracker();

	public abstract void UpdatePoses();

	public abstract void GetProjectionMatrix(double[] projectionMatrix);

	public abstract bool IsTrackingCameraMode();

	public void SetCamera(Camera main_camera)
	{
		mainCamera = main_camera;
	}

	public abstract void ResetCameraTransforms();

	protected Camera mainCamera;
}