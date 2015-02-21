using UnityEngine;
using System.Collections;

public class ParticleUpdater : MonoBehaviour
{
	public int texSize = 1000;
	public Material updateMat;

	const string
		PropColTex = "_ColTex",
		PropVelTex = "_VelTex",
		PropRotTex = "_RotTex",
		PropPosTex = "_PosTex",
		PropMousePos = "_MousePos",
		PropMouseTime = "_MouseTime",
		propMVP = "_MVP";
	[SerializeField]
	RenderTexture[]
		colTex = new RenderTexture[2],
		velTex = new RenderTexture[2],
		rotTex = new RenderTexture[2],
		posTex = new RenderTexture[2];
	float mouseTime, waitCouter;

	void SwapRts (RenderTexture[] rts)
	{
		RenderTexture tmp = rts [0];
		rts [0] = rts [1];
		rts [1] = tmp;
	}
	void SwapRts ()
	{
		SwapRts (colTex);
		SwapRts (velTex);
		SwapRts (rotTex);
		SwapRts (posTex);
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

	void OnEnable ()
	{
		CreateTextures (colTex);
		CreateTextures (velTex);
		CreateTextures (rotTex);
		CreateTextures (posTex);
	}
	void OnDisable ()
	{
		ReleaseTextures (colTex);
		ReleaseTextures (velTex);
		ReleaseTextures (rotTex);
		ReleaseTextures (posTex);
	}
	void Start ()
	{
		Matrix4x4 P = GL.GetGPUProjectionMatrix (Camera.main.projectionMatrix, false);
		Matrix4x4 V = Camera.main.worldToCameraMatrix;
		Matrix4x4 M = transform.localToWorldMatrix;
		Matrix4x4 MVP = P * V * M;
		updateMat.SetMatrix (propMVP, MVP);
		WriteRenderBuffers (updateMat, 0);
		Application.targetFrameRate = 30;
		Shader.SetGlobalFloat (PropMouseTime, mouseTime);
	}
	void Update ()
	{
		if (Input.GetMouseButtonDown (0)) {
			mouseTime = 0;
			if (waitCouter > 0) {
				Shader.SetGlobalFloat (PropMouseTime, mouseTime);
				WriteRenderBuffers (updateMat, 0);
			}
			waitCouter = 0;
		}
		if (Input.GetMouseButton (0)) {
			var pos = Input.mousePosition;
			pos.z = 40f;
			pos = Camera.main.ScreenToWorldPoint (pos);
			Shader.SetGlobalVector (PropMousePos, pos);
			mouseTime += Time.deltaTime;
			WriteRenderBuffers (updateMat, 1);
		} else if (Input.GetMouseButtonUp (0)) {
			if (mouseTime > 20f)
				waitCouter = 10f;
		} else {
			if (waitCouter > 0)
				WriteRenderBuffers (updateMat, 2);
			else {
				mouseTime = 0;
				WriteRenderBuffers (updateMat, 0);
			}
			waitCouter -= Time.deltaTime;
		}
		Shader.SetGlobalFloat (PropMouseTime, mouseTime);
	}

	void WriteRenderBuffers (Material mat, int pass = 0)
	{
		Graphics.SetRenderTarget (
			new RenderBuffer[]{
			colTex [0].colorBuffer,
			velTex [0].colorBuffer,
			rotTex [0].colorBuffer,
			posTex [0].colorBuffer
		},
		colTex [0].depthBuffer
		);
		mat.DrawFullscreenQuad (pass);
		
		Shader.SetGlobalTexture (PropColTex, colTex [0]);
		Shader.SetGlobalTexture (PropVelTex, velTex [0]);
		Shader.SetGlobalTexture (PropRotTex, rotTex [0]);
		Shader.SetGlobalTexture (PropPosTex, posTex [0]);
		SwapRts ();
		Graphics.SetRenderTarget (null);
	}
}
