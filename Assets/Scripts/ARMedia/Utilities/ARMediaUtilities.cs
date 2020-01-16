//
//  ARMediaUtilities.cs
//
//  ARMedia SDK Unity Plugin & Examples
//
//  Copyright (c) 2017 Inglobe Technologies. All rights reserved.
//

using UnityEngine;
using System.Collections;
using System.Runtime.InteropServices;
using System.IO;

#if UNITY_EDITOR
using UnityEditor;
using System.Reflection;
#endif

public class ARMediaUtilities
{
	public static void NormalizeQuaternion (ref Quaternion q)
	{
		float sum = 0;
		for (int i = 0; i < 4; ++i)
			sum += q[i] * q[i];
		float magnitudeInverse = 1 / Mathf.Sqrt(sum);
		for (int i = 0; i < 4; ++i)
			q[i] *= magnitudeInverse;
	}

	public static Quaternion ConvertRotation(Quaternion q)
	{
		// Converts the rotation from right handed to left handed.
		return new Quaternion(q.x, q.y, -q.z, -q.w);	
	}

#if UNITY_EDITOR
	public static bool GetImageFilename(Texture2D asset, out string filename)
	{
		if (asset != null) 
		{
			string assetPath = AssetDatabase.GetAssetPath (asset);
			filename = Path.GetFileName (assetPath);
			return true;
		}

		filename = "";
		return false;
	}
	
	public static bool GetImageExtension(Texture2D asset, out string extension)
	{
		if (asset != null) 
		{
			string assetPath = AssetDatabase.GetAssetPath(asset);
			extension = Path.GetExtension(assetPath);
			return true;
		}
		
		extension = "";
		return false;
	}
	
	public static bool GetImageSize(Texture2D asset, out float width, out float height) 
	{
		if (asset != null) 
		{
			string assetPath = AssetDatabase.GetAssetPath(asset);
			TextureImporter importer = AssetImporter.GetAtPath(assetPath) as TextureImporter;
			
			if (importer != null) 
			{
				object[] args = new object[2] { 0, 0 };
				MethodInfo mi = typeof(TextureImporter).GetMethod("GetWidthAndHeight", BindingFlags.NonPublic | BindingFlags.Instance);
				mi.Invoke(importer, args);
				
				width = (int)args[0];
				height = (int)args[1];
				
				return true;
			}
		}
		
		height = width = 0;
		return false;
	}
#endif
}