using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
public class MeshParticle : MonoBehaviour
{
	public Mesh originalMesh;
	public Material originalMat;
	public int
		numParticles = 20000,
		numMeshes = 50;

	Mesh
		particleMesh;
	Material[]
		mats;

	void OnEnable ()
	{
		if (originalMesh == null || originalMat == null)
			return;
		CreateParticle ();
	}
	
	// Update is called once per frame
	void Update ()
	{
		Matrix4x4 matrix = transform.localToWorldMatrix;
//		foreach (var mat in mats)
		for (var i = 0; i < mats.Length; i++) {
			mats [i].SetInt ("_Offset", i * numParticles);
			Graphics.DrawMesh (particleMesh, matrix, mats [i], 0);
		}
	}

	void CreateParticle ()
	{
		particleMesh = new Mesh ();
		var vCount = originalMesh.vertexCount;
		var iCount = originalMesh.GetIndices (0).Length;
		numParticles = Mathf.Min (65000 / vCount, numParticles);

		Vector3[] vertices = new Vector3[numParticles * vCount];
		Vector3[] normals = new Vector3[numParticles * vCount];
		Vector2[] uv2 = new Vector2[numParticles * vCount];
		int[] indices = new int[numParticles * iCount];
		mats = new Material[numMeshes];

		for (var i = 0; i < numParticles; i++) {
			for (var j = 0; j < vCount; j++) {
				vertices [i * vCount + j] = originalMesh.vertices [j];
				normals [i * vCount + j] = originalMesh.normals [j];
				uv2 [i * vCount + j] = new Vector2 ((float)i + 0.5f, Random.value);
			}
			for (var j = 0; j <iCount; j++)
				indices [i * iCount + j] = originalMesh.GetIndices (0) [j] + i * vCount;
		}
		for (var i = 0; i < numMeshes; i++) {
			mats [i] = new Material (originalMat);
			mats [i].SetInt ("_Offset", i * numParticles);
			mats [i].hideFlags = HideFlags.HideAndDontSave;
		}

		particleMesh.vertices = vertices;
		particleMesh.normals = normals;
		particleMesh.uv2 = uv2;
		particleMesh.SetIndices (indices, MeshTopology.Triangles, 0);
		particleMesh.hideFlags = HideFlags.HideAndDontSave;
	}

	void OnDisable ()
	{
		DestroyImmediate (particleMesh);
		foreach (var m in mats)
			DestroyImmediate (m);
	}
}
