//
//  ARMediaLocationTarget.cs
//
//  ARMedia SDK Unity Plugin & Examples
//
//  Copyright (c) 2017 Inglobe Technologies. All rights reserved.
//

using UnityEngine;
using System.Collections;
using System;

/*
 * ARMedia Location Target: 
 * 
 * Use an object of this class to add locations to a ARMediaLocationTracker tracker. An instance of this class does not display
 * any content by itself, in order to add content to the location itself, just add one or more GameObjects as children of this
 * object.
 * 
 * NOTE: even though you can specify the altitude information, this is not strictly required and can be ignored by setting a 
 * property in the ARMediaLocationTracker.
 * 
 */

[ExecuteInEditMode]
public class ARMediaLocationTarget : MonoBehaviour 
{
	public string locationName;

	public string locationLatitude;
	public string locationLongitude;
	public string locationAltitude;

	[HideInInspector]
	public double latitude;
	[HideInInspector]
	public double longitude;
	[HideInInspector]
	public double altitude;

	[HideInInspector]
	public bool valid;

	void Start () 
	{
		SetCoordinates();
	}

	void Update ()
	{
#if UNITY_EDITOR
		SetCoordinates();
#endif
	}

	private void SetCoordinates()
	{
		valid = true;

		if (!Double.TryParse(locationLatitude, out latitude))
		{
			locationLatitude = "Please enter a valid value.";
			latitude = 0.0;
			valid = false;
		}
		
		if (!Double.TryParse(locationLongitude, out longitude))
		{
			locationLongitude = "Please enter a valid value.";
			longitude = 0.0;
			valid = false;
		}
		
		if (!Double.TryParse(locationAltitude, out altitude))
		{
			locationAltitude = "Please enter a valid value.";
			altitude = 0.0;
			valid = false;
		}
	}
}