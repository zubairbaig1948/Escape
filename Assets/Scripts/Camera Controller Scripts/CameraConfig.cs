﻿using UnityEngine;
using System.Collections;

public class CameraConfig : MonoBehaviour {

	public bool automatic = true;
	private Bounds combinedBounds;

	public float distance = 10f;
	public float height = 5f;

	void OnEnable(){

		if(automatic){

			Quaternion orgRotation = transform.rotation;
			transform.rotation = Quaternion.identity;

			distance = MaxBoundsExtent(transform) * 2.75f;
			height = MaxBoundsExtent(transform) * .6f;

			if (height < 1)
				height = 1;

			transform.rotation = orgRotation;

		}

	}

	public void SetCameraSettings () {

		CameraController cam = GameObject.FindObjectOfType<CameraController> ();
		 
		if(!cam)
			return;
			
		cam.TPSDistance = distance;
		cam.TPSHeight = height;

	}

	public static float MaxBoundsExtent(Transform obj){
		
		// get the maximum bounds extent of object, including all child renderers,
		// but excluding particles and trails, for FOV zooming effect.

		var renderers = obj.GetComponentsInChildren<Renderer>();

		Bounds bounds = new Bounds();
		bool initBounds = false;
		foreach (Renderer r in renderers)
		{
			if (!((r is TrailRenderer) || (r is ParticleSystemRenderer)))
			{
				if (!initBounds)
				{
					initBounds = true;
					bounds = r.bounds;
				}
				else
				{
					bounds.Encapsulate(r.bounds);
				}
			}
		}
		float max = Mathf.Max(bounds.extents.x, bounds.extents.y, bounds.extents.z);
		return max;

	}

}
