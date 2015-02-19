using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class MassMeshDrawer : MonoBehaviour
{
	public Material mat;
	Mesh[] ms;
	
	// Use this for initialization
	void Start ()
	{
		var mfs = GetComponentsInChildren<MeshFilter> ();
		ms = mfs.Select (mf => mf.sharedMesh).ToArray ();
		
		if (mat == null)
			mat = mfs [0].renderer.sharedMaterial;
		for (int i = 0; i < mfs.Length; i++)
			Destroy (mfs [i].gameObject);
	}
	
	// Update is called once per frame
	void Update ()
	{
		var pos = transform.position;
		var rot = transform.rotation;
		foreach (var m in ms) {
			Graphics.DrawMesh (m, pos, rot, mat, 0);
		}
	}
}
