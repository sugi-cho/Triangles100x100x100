using UnityEngine;
using System.Collections;

public class ParticleUpdater : MonoBehaviour
{
	public int texSize = 1024;
	public Material updateMat;

	const string
		PropTarTex = "_TarTex",
		PropVelTex = "_VelTex",
		PropRotTex = "_RotTex",
		PropPosTex = "_PosTex";
	[SerializeField]
	RenderTexture[]
		tarTex = new RenderTexture[2],
		velTex = new RenderTexture[2],
		rotTex = new RenderTexture[2],
		posTex = new RenderTexture[2];

	void SwapRts (RenderTexture[] rts)
	{
		RenderTexture tmp = rts [0];
		rts [0] = rts [1];
		rts [1] = tmp;
	}
	void SwapRts ()
	{
		SwapRts (tarTex);
		SwapRts (velTex);
		SwapRts (rotTex);
		SwapRts (posTex);
	}
	// Use this for initialization
	void OnEnable ()
	{
		CreateTextures (tarTex);
		CreateTextures (velTex);
		CreateTextures (rotTex);
		CreateTextures (posTex);

		if (camera == null) {
			gameObject.AddComponent<Camera> ();
			camera.clearFlags = CameraClearFlags.Nothing;
			camera.depth = -10;
			camera.hdr = true;
			camera.cullingMask = 0;
			camera.hideFlags = HideFlags.HideAndDontSave;
		}
	}
	void OnDisable ()
	{
		ReleaseTextures (tarTex);
		ReleaseTextures (velTex);
		ReleaseTextures (rotTex);
		ReleaseTextures (posTex);
	}
	void CreateTextures (RenderTexture[] rts)
	{
		for (var i = 0; i < 2; i++)
			rts [i] = Extensions.CreateRenderTexture (texSize, texSize, rts [i], FilterMode.Point);
	}
	void ReleaseTextures (RenderTexture[] rts)
	{
		for (var i = 0; i < 2; i ++)
			Extensions.ReleaseRenderTexture (rts [i]);
	}

	void OnPreRender ()
	{
		Graphics.SetRenderTarget (
			new RenderBuffer[]{
				tarTex [0].colorBuffer,
				velTex [0].colorBuffer,
				rotTex [0].colorBuffer,
				posTex [0].colorBuffer
			},
			tarTex [0].depthBuffer
		);
		updateMat.DrawFullscreenQuad ();
	}
	void OnPostRender ()
	{
		Shader.SetGlobalTexture (PropTarTex, tarTex [0]);
		Shader.SetGlobalTexture (PropVelTex, tarTex [0]);
		Shader.SetGlobalTexture (PropRotTex, rotTex [0]);
		Shader.SetGlobalTexture (PropPosTex, posTex [0]);
		SwapRts ();
		Graphics.SetRenderTarget (null);
	}
}
