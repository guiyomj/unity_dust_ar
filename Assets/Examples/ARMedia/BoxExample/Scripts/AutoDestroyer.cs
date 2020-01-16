//
//  AutoDestroyer.cs
//
//  ARMedia SDK Unity Plugin & Examples
//
//  Copyright (c) 2015 Inglobe Technologies. All rights reserved.
//

using UnityEngine;
using System.Collections;

public class AutoDestroyer : MonoBehaviour
{
	void Start () 
	{
		Destroy(gameObject, 10.0f);
	}
}
