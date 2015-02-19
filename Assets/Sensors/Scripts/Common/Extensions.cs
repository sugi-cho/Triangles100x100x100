using UnityEngine;
using System.Collections;

static class Extensions
{
	
	public static Vector2 XY (this Vector3 vec3)
	{
		return new Vector2 (vec3.x, vec3.y);
	}
	public static Vector2 YZ (this Vector3 vec3)
	{
		return new Vector2 (vec3.y, vec3.z);
	}
	public static Vector2 ZX (this Vector3 vec3)
	{
		return new Vector2 (vec3.z, vec3.x);
	}
	
	public static Vector2 XY (this Vector4 vec4)
	{
		return new Vector2 (vec4.x, vec4.y);
	}
	public static Vector2 YZ (this Vector4 vec4)
	{
		return new Vector2 (vec4.y, vec4.z);
	}
	public static Vector2 ZW (this Vector4 vec4)
	{
		return new Vector2 (vec4.z, vec4.w);
	}
	public static Vector2 WX (this Vector4 vec4)
	{
		return new Vector2 (vec4.w, vec4.x);
	}
	
	public static Vector3 Position (this MonoBehaviour mono)
	{
		return mono.transform.position;
	}
	public static Quaternion Rotation (this MonoBehaviour mono)
	{
		return mono.transform.rotation;
	}
	
	static Material bMat {
		get {
			if (_bMat == null)
				_bMat = new Material (Shader.Find ("Hidden/FastBlur"));
			return _bMat;
		}
	}
	static Material _bMat;
	public static RenderTexture GetBlur (this RenderTexture s, float bSize, int iteration = 1, int ds = 0)
	{
		float 
		widthMod = 1f / (1f * (1 << ds));
		
		int
		rtW = s.width >> ds,
		rtH = s.height >> ds;
		
		RenderTexture rt = RenderTexture.GetTemporary (rtW, rtH, s.depth, s.format);
		Graphics.Blit (s, rt);
		
		for (int i = 0; i < iteration; i++) {
			float iterationOffs = (float)i;
			bMat.SetVector ("_Parameter", new Vector4 (bSize * widthMod + iterationOffs, -bSize * widthMod - iterationOffs, 0, 0));
			
			RenderTexture rt2 = RenderTexture.GetTemporary (rtW, rtH, 0, rt.format);
			rt2.filterMode = FilterMode.Bilinear;
			Graphics.Blit (rt, rt2, bMat, 1);
			RenderTexture.ReleaseTemporary (rt);
			rt = rt2;
			
			rt2 = RenderTexture.GetTemporary (rtW, rtH, 0, rt.format);
			rt2.filterMode = FilterMode.Bilinear;
			Graphics.Blit (rt, rt2, bMat, 2);
			RenderTexture.ReleaseTemporary (rt);
			rt = rt2;
		}
		Graphics.Blit (rt, s);
		RenderTexture.ReleaseTemporary (rt);
		return s;
	}
	public static RenderTexture CreateRenderTexture (int width, int height, RenderTexture rt = null, FilterMode mode = FilterMode.Bilinear)
	{
		if (rt != null)
			ReleaseRenderTexture (rt);
		rt = new RenderTexture (width, height, 16, RenderTextureFormat.ARGBHalf);
		rt.wrapMode = TextureWrapMode.Repeat;
		rt.filterMode = mode;
		rt.Create ();
		RenderTexture.active = rt;
		GL.Clear (true, true, Color.clear);
		return rt;
	}
	public static RenderTexture CreateRenderTexture (RenderTexture s, RenderTexture rt = null)
	{
		if (rt != null)
			Extensions.ReleaseRenderTexture (rt);
		rt = CreateRenderTexture (s.width, s.height);
		return rt;
	}
	public static void ReleaseRenderTexture (RenderTexture rt)
	{
		if (rt == null)
			return;
		rt.Release ();
		Object.Destroy (rt);
	}
	static public void DrawFullscreenQuad (this Material mat, int pass = 0, float z=1.0f)
	{
		mat.SetPass (pass);
		GL.Begin (GL.QUADS);
		GL.Vertex3 (-1.0f, -1.0f, z);
		GL.Vertex3 (1.0f, -1.0f, z);
		GL.Vertex3 (1.0f, 1.0f, z);
		GL.Vertex3 (-1.0f, 1.0f, z);
		
		GL.Vertex3 (-1.0f, 1.0f, z);
		GL.Vertex3 (1.0f, 1.0f, z);
		GL.Vertex3 (1.0f, -1.0f, z);
		GL.Vertex3 (-1.0f, -1.0f, z);
		GL.End ();
	}
}
