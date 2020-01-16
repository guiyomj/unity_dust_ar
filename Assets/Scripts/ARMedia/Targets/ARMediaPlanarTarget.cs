//
//  ARMediaPlanarTarget.cs
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
 * ARMediaPlanarTarget: 
 * 
 * Using this object you specify an image (keyframe) that will be recognized and tracked by the ARMediaPlanarTracker tracker. 
 * Using the 'referenceKeyframe' parameter you can display the image in the Editor, specify it's real-world size and place content
 * accordingly by adding one or more GameObjects as children of this object and transforming them with respect to the displayed 
 * image.
 * 
 * NOTE: this object is meant to be used mainly as a visaul hint to place content with respect to the real image being tracked, 
 * but the real tracking data is defined elsewhere (specifically by providing a configuration file and images to the 
 * ARMediaPlanarTracker object).
 * 
 */

[ExecuteInEditMode]
public class ARMediaPlanarTarget : MonoBehaviour 
{
	[HideInInspector]
	public string keyframeName;
	
	public Texture2D referenceKeyframe;
	[HideInInspector, SerializeField]
	private GameObject keyframeQuad;

	public float keyframeWidth;
	public float keyframeHeight;

	[HideInInspector, SerializeField] 
	private float aspect = 1.0f;

	[HideInInspector]
	public float keyframeScale = 1.0f;
	
	private Renderer keyframe_renderer;

	[HideInInspector, SerializeField]
	private float previousWidth = 0.0f;
	[HideInInspector, SerializeField]
	private float previousHeight = 0.0f;
	[HideInInspector, SerializeField]
	private Texture2D previousTexture = null;

	void Start () 
	{
#if UNITY_EDITOR
#else
		UnSetKeyframeReference();
#endif
	}

	void Update () 
	{
#if UNITY_EDITOR
		if(UnityEditor.EditorApplication.isPlayingOrWillChangePlaymode)
			return;

		if(null == keyframeQuad)
		{
			keyframeQuad = (GameObject)Instantiate(Resources.Load("ARMedia/Prefabs/Utilities/ARMediaKeyframeQuad"));
			keyframeQuad.transform.SetParent(gameObject.transform);

			// setup keyframe texture...
			keyframe_renderer = keyframeQuad.GetComponent<Renderer>();
			if(keyframe_renderer)
			{
				var tempMaterial = new Material(keyframe_renderer.sharedMaterial);
				tempMaterial.mainTexture = referenceKeyframe;
				keyframe_renderer.sharedMaterial = tempMaterial;
			}
		}

		// check what changed in the editor...
		if(previousTexture != referenceKeyframe)
		{
			previousTexture = referenceKeyframe;
			
			// update aspect/size...
			OnKeyframeChanged();

			return;
		}

		if(previousWidth != keyframeWidth)
		{
			// user changed width, adjust height accordingly...
			previousWidth = keyframeWidth;
			keyframeHeight = keyframeWidth / aspect;
			previousHeight = keyframeHeight;

			OnKeyframeSizeChanged();
			
			return;
		}

		if(previousHeight != keyframeHeight)
		{
			// user changed height, adjust width accordingly...
			previousHeight = keyframeHeight;
			keyframeWidth = keyframeHeight * aspect;
			previousWidth = keyframeWidth;

			OnKeyframeSizeChanged();

			return;
		}
#else
#endif	
	}

	private void UnSetKeyframeReference()
	{
		if(null != keyframeQuad)
			DestroyObject(keyframeQuad.gameObject);
	}

#if UNITY_EDITOR
	private void OnKeyframeSizeChanged()
	{
		if(aspect>=1.0f)
			keyframeScale = 1.0f/keyframeWidth;
		else
			keyframeScale = 1.0f/keyframeHeight;

		keyframeQuad.transform.localScale = new Vector3(keyframeWidth, keyframeHeight, 1.0f);

		EditorUtility.SetDirty(this);
	}

	private void OnKeyframeChanged()
	{
		// get new keyframe name...
		if(!ARMediaUtilities.GetImageFilename(referenceKeyframe, out keyframeName))
			Debug.LogWarning("Cannot get keyframe filename.");

		// update keyframe texture...
		keyframe_renderer = keyframeQuad.GetComponent<Renderer>();
		if(keyframe_renderer)
		{
			var tempMaterial = new Material(keyframe_renderer.sharedMaterial);
			tempMaterial.mainTexture = referenceKeyframe;
			keyframe_renderer.sharedMaterial = tempMaterial;
		}

		// get new keyframe size...

		// retrieve the texture size in pixels...
		ARMediaUtilities.GetImageSize(referenceKeyframe, out keyframeWidth, out keyframeHeight);
		aspect = keyframeWidth/keyframeHeight;

		// make it smaller (Unity units are in meters)...
		if(keyframeWidth>100.0f || keyframeHeight>100.0f)
		{
			keyframeWidth = keyframeWidth/100.0f;
			keyframeHeight = keyframeHeight/100.0f;
		}

		OnKeyframeSizeChanged();

		// store values to track changes...
		previousWidth = keyframeWidth;
		previousHeight = keyframeHeight;
		previousTexture = referenceKeyframe;
	}
#endif
}