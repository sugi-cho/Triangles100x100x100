using UnityEngine;
using System.Collections;

public class Quat2Mat : MonoBehaviour
{
	public Material mat;
	// Use this for initialization
	void Start ()
	{
	
	}
	
	// Update is called once per frame
	void Update ()
	{
		Quaternion r = transform.rotation;
		mat.SetVector ("_Q", new Vector4 (r.x, r.y, r.z, r.w));
	}
}
