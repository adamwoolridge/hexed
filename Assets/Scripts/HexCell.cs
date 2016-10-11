using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class HexCell : MonoBehaviour {

	public HexCoordinates coordinates;
	public Color SeaColour = Color.blue;
	public Color SandColour = Color.yellow;

	Mesh hexMesh;
	List<Vector3> vertices;
 	List<int> triangles;
	List<Color> colors;
	MeshCollider meshCollider;

	public float Height = 0f;


	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void Triangulate()
	{
		if (hexMesh == null)
		{
			GetComponent<MeshFilter>().mesh = hexMesh = new Mesh();
			meshCollider = gameObject.AddComponent<MeshCollider>();
 			hexMesh.name = "Hex Mesh";
 			vertices = new List<Vector3>();
 			triangles = new List<int>();
			colors = new List<Color>();
		}

		hexMesh.Clear();
 		vertices.Clear();
 		triangles.Clear();
 		colors.Clear();
 			
		Vector3 center = transform.parent.localPosition + new Vector3(0f, Height, 0f);

		for (int i = 0; i < 6; i++) 
    		AddTriangle(center, center + HexMetrics.corners[i], center + HexMetrics.corners[i + 1] );
	
		hexMesh.vertices = vertices.ToArray();
		hexMesh.colors = colors.ToArray();
 		hexMesh.triangles = triangles.ToArray();
 		hexMesh.RecalculateNormals();

		meshCollider.sharedMesh = hexMesh;
	}

	void AddTriangle (Vector3 v1, Vector3 v2, Vector3 v3) 
	{
 		int vertexIndex = vertices.Count;
 		vertices.Add(v1);
		vertices.Add(v2);
		vertices.Add(v3);
 		triangles.Add(vertexIndex);
 		triangles.Add(vertexIndex + 1);
		triangles.Add(vertexIndex + 2);

 		colors.Add(Height > 0.0f ? SandColour : SeaColour);
		colors.Add(Height > 0.0f ? SandColour : SeaColour);
		colors.Add(Height > 0.0f ? SandColour : SeaColour);
 	}

 	public void Clicked(bool left=true)
 	{ 		
 		Height += left? 3.0f : -3f;

 		Triangulate();
		Debug.Log(coordinates.ToString());	
 	}
}
