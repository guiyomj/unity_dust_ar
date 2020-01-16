//
//  ARMediaObjectTarget.cs
//
//  ARMedia SDK Unity Plugin & Examples
//
//  Copyright (c) 2017 Inglobe Technologies. All rights reserved.
//

using UnityEngine;
using System.Collections;

#if UNITY_EDITOR
using UnityEditor;
#endif

/*
 * ARMediaObjectTarget:
 * 
 * Use this object to receive tracked poses from the ARMediaObjectTracker tracker and display content accordingly. In order 
 * to display content, just add one or more GameObjects as children of this object. You can specify a 'referenceObject' in order
 * to help you positioning the augmented content with respect to the real object being tracked: the 'referenceObject' is meant to 
 * be set to the very same mesh that you used when you created the object tracking data on the ARMedia SDK Developer Portal
 * (http://dev.inglobetechnologies.com) and will be diplayed only in the Editor but will be removed automatically during the app execution.
 * 
 * NOTE: this object is meant to be used ONLY as a visaul hint to place content with respect to the real object being tracked, 
 * but the real tracking data is defined elsewhere (specifically by providing a configuration file and pointcloud to the 
 * ARMediaObjectTracker object).
 * 
 */

[ExecuteInEditMode]
public class ARMediaObjectTarget : MonoBehaviour 
{
	// the reference mesh of the object that is meant to be tracked...
	public GameObject referenceObject;

	// the reference mesh (instance) of the object that is meant to be tracked...
	[HideInInspector, SerializeField]
	private GameObject referenceObjectInstance;

	void Start ()
	{
#if UNITY_EDITOR
#else
		UnSetObjectReferences();
#endif
	}

	void Update () 
	{
#if UNITY_EDITOR
		SetObjectReferences();
#else
#endif
	}

#if UNITY_EDITOR
	private void SetObjectReferences()
	{
		if(UnityEditor.EditorApplication.isPlayingOrWillChangePlaymode)
			return;
		
		if(null == referenceObject)
			return;
		
		if(null == referenceObjectInstance)
		{
			referenceObjectInstance = (GameObject)Instantiate(referenceObject);
			referenceObjectInstance.transform.SetParent(gameObject.transform);
			EditorUtility.SetDirty(this);
		}
	}
#endif

	private void UnSetObjectReferences()
	{
		referenceObject = null;

		if(null != referenceObjectInstance)
		{
			referenceObjectInstance.transform.SetParent(null);
			DestroyObject(referenceObjectInstance);
		}
		referenceObjectInstance = null;
	}
}