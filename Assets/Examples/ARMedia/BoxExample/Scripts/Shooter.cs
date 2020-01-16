//
//  Shooter.cs
//
//  ARMedia SDK Unity Plugin & Examples
//
//  Copyright (c) 2015 Inglobe Technologies. All rights reserved.
//

using UnityEngine;
using System.Collections;

public class Shooter : MonoBehaviour 
{
	public Rigidbody projectile;
	public Transform shotPos;

	public float shotForce = 1000.0f;
	public float moveSpeed = 10.0f;
	
	private Color[] colors;
	
	void Start ()
	{
		colors = new Color[6];
		colors[0] = new Color(1.0f, 0.0f, 0.0f);
		colors[1] = new Color(0.0f, 1.0f, 0.0f);
		colors[2] = new Color(0.0f, 0.0f, 1.0f);
		colors[3] = new Color(0.0f, 1.0f, 1.0f);
		colors[4] = new Color(1.0f, 0.0f, 1.0f);
		colors[5] = new Color(1.0f, 1.0f, 0.0f);
	}

	void Update ()
	{
		if(Application.isEditor)
		{
			float h = Input.GetAxis("Horizontal") * Time.deltaTime * moveSpeed;
			float v = Input.GetAxis("Vertical") * Time.deltaTime * moveSpeed;

			transform.Translate(new Vector3(h, v, 0.0f));

			if(Input.GetButtonUp("Fire1"))
			{
				Rigidbody shot = Instantiate(projectile, shotPos.position, shotPos.rotation) as Rigidbody;
				shot.gameObject.GetComponent<Renderer>().material.color = colors[Random.Range(0, colors.Length)];
				shot.AddForce(shotPos.forward * shotForce);
			}

			return;
		}

        int fingerCount = 0;
        foreach (Touch touch in Input.touches) 
		{
            if (touch.phase != TouchPhase.Ended && touch.phase != TouchPhase.Canceled)
                fingerCount++;
        }

		if (fingerCount > 0)
		{
			Rigidbody shot = Instantiate(projectile, shotPos.position, shotPos.rotation) as Rigidbody;
			shot.gameObject.transform.parent = gameObject.transform.parent;
			shot.gameObject.GetComponent<Renderer>().material.color = colors[Random.Range(0, colors.Length)];
			shot.AddForce(shotPos.forward * shotForce);
		}
	}
}