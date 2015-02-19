using UnityEngine;
using System.Collections;

public class CalcDot : MonoBehaviour
{
	public Vector3
		v1, v2;
	public float d;
	public Vector3 cr;
	public Vector4 q;
	// Use this for initialization
	void Start ()
	{
	
	}
	
	// Update is called once per frame
	void Update ()
	{
		d = Vector3.Dot (v1, v2);
		cr = Vector3.Cross (v1, v2);
		q = new Vector4 (cr.x, cr.y, cr.z, 1f + d);
		Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);
		Debug.DrawRay (ray.origin, ray.direction * 10, Color.yellow);
	}
}
