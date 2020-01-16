//
//  ARMediaAutoName.cs
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

[ExecuteInEditMode]
public class ARMediaAutoName : MonoBehaviour
{
	// WARNING: Do not modify this...
	public string defaultName = "ARMediaSDK";

	void Start ()
	{
#if UNITY_EDITOR
		this.hideFlags = HideFlags.HideInInspector;
#endif
	}

	void Update () 
	{
#if UNITY_EDITOR
		if(UnityEditor.EditorApplication.isPlayingOrWillChangePlaymode)
			return;
		
		// reset the game object name to <defaultName> in order to be sure that the object will receive events callbacks from the native plugin.
		if(gameObject.name != defaultName)
		{
			gameObject.name= defaultName;
			EditorUtility.SetDirty(this);
		}
#endif
	}
}