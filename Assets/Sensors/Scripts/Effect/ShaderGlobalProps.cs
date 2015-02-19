using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

[ExecuteInEditMode]
public class ShaderGlobalProps : MonoBehaviour
{
	public FloatProp[] fProps;
	public VectorProp[] vProps;
	public TextureProp[] tProps;
	public KeyCode[]
		keys,
		toggleKeys;

	Dictionary<KeyCode,bool> toggleMap = new Dictionary<KeyCode, bool> ();
	// Use this for initialization
	void Start ()
	{
		SetProps ();

		foreach (var k in toggleKeys) {
			toggleMap.Add (k, false);
			Shader.SetGlobalInt (string.Format ("_Toggle{0}", k.ToString ()), toggleMap [k] ? 1 : 0);
		}
	}
	void SetProps ()
	{
		foreach (var p in fProps)
			Shader.SetGlobalFloat (p.name, p.val);
		foreach (var p in vProps)
			Shader.SetGlobalVector (p.name, p.val);
		foreach (var p in tProps)
			if (p.val != null)
				Shader.SetGlobalTexture (p.name, p.val);
	}

	void Update ()
	{
		foreach (var k in keys)
			Shader.SetGlobalInt (string.Format ("_Key{0}", k.ToString ()), Input.GetKey (k) ? 1 : 0);
		if (Input.anyKeyDown) {
			foreach (var k in toggleKeys) {
				if (Input.GetKeyDown (k)) {
					toggleMap [k] = !toggleMap [k];
					Shader.SetGlobalInt (string.Format ("_Toggle{0}", k.ToString ()), toggleMap [k] ? 1 : 0);
				}
			}
		}
	}

	[System.Serializable]
	public class FloatProp
	{
		public string name;
		public float val;
	}
	[System.Serializable]
	public class VectorProp
	{
		public string name;
		public Vector4 val;
	}
	[System.Serializable]
	public class TextureProp
	{
		public string name = "_Tex";
		public Texture val;
	}
}
