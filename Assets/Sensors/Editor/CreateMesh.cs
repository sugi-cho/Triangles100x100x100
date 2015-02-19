using UnityEngine;
using UnityEditor;

public class CreateMesh : MonoBehaviour
{
	[MenuItem("sugi.cho/Create/Triangle")]
	public static void CreateTriangleMesh ()
	{
		var mesh = new Mesh ();
		Vector3[] vertices = new Vector3[]{
			new Vector3 (0, 0, 0.5f),
			new Vector3 (0.5f, 0, -0.5f),
			new Vector3 (-0.5f, 0, -0.5f)
		};
		mesh.vertices = vertices;
		mesh.SetIndices (new int[]{0,1,2}, MeshTopology.Triangles, 0);
		mesh.RecalculateNormals ();
		mesh.RecalculateBounds ();

		AssetDatabase.CreateAsset (mesh, "Assets/triangle.asset");
		AssetDatabase.SaveAssets ();
		AssetDatabase.Refresh ();
	}
}
