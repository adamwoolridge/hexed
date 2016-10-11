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


	[SerializeField]
 	HexCell[] neighbors;

	public HexCell GetNeighbor (HexDirection direction) 
	{
 		return neighbors[(int)direction];
 	}

	public void SetNeighbor (HexDirection direction, HexCell cell) 
	{
 		neighbors[(int)direction] = cell;
		cell.neighbors[(int)direction.Opposite()] = this;
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
 			
		Vector3 bottom = transform.parent.localPosition;
		Vector3 top = bottom + new Vector3(0f, Height, 0f);



		// Top
		for (HexDirection d = HexDirection.NE; d <= HexDirection.NW; d++) 
		{
			Vector3 bridge = HexMetrics.GetBridge(d);

			Vector3 v1 = top + HexMetrics.GetFirstSolidCorner(d);
			Vector3 v2 = top + HexMetrics.GetSecondSolidCorner(d);
			Vector3 v3 = v1 + bridge;
			Vector3 v4 = v2 + bridge;

			AddTriangle(top, v1, v2);
			if (d == HexDirection.NE) 
			{
				TriangulateConnection(d, v1, v2);
			}
			if (d <= HexDirection.SE) {
				TriangulateConnection(d, v1, v2);
			}	
//			AddQuad(v1, v2, v3, v4);
//
//			AddTriangle(v1, top + HexMetrics.GetFirstCorner(d), v3);
//			AddTriangle(v2, v4, top + HexMetrics.GetSecondCorner(d));
		}
	
//		// Sides
//		for (int i=0; i<6; i++)
//			AddSide(i, bottom, top);
//
//		// Bottom
//		for (int i = 0; i < 6; i++) 
//			AddTriangle(bottom, bottom + HexMetrics.corners[i], bottom + HexMetrics.corners[i + 1] );
	
		hexMesh.vertices = vertices.ToArray();
		hexMesh.colors = colors.ToArray();
 		hexMesh.triangles = triangles.ToArray();
 		hexMesh.RecalculateNormals();

		meshCollider.sharedMesh = hexMesh;
	}

	void TriangulateConnection (HexDirection direction, Vector3 v1, Vector3 v2) 
	{
		HexCell neighbor = GetNeighbor(direction);
		if (neighbor == null) return;
		
		Vector3 bridge = HexMetrics.GetBridge(direction);
		Vector3 v3 = v1 + bridge;
		Vector3 v4 = v2 + bridge;

		AddQuad(v1, v2, v3, v4);

		HexCell nextNeighbor = GetNeighbor(direction.Next());
		if (direction <= HexDirection.E &&  nextNeighbor != null) 
			AddTriangle(v2, v4, v2 + HexMetrics.GetBridge(direction.Next()));
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

	void AddQuad (Vector3 v1, Vector3 v2, Vector3 v3, Vector3 v4) 
	{
		int vertexIndex = vertices.Count;
		vertices.Add(v1);
		vertices.Add(v2);
		vertices.Add(v3);
		vertices.Add(v4);
		triangles.Add(vertexIndex);
		triangles.Add(vertexIndex + 2);
		triangles.Add(vertexIndex + 1);
		triangles.Add(vertexIndex + 1);
		triangles.Add(vertexIndex + 2);
		triangles.Add(vertexIndex + 3);

		colors.Add(Height > 0.0f ? SandColour : SeaColour);
		colors.Add(Height > 0.0f ? SandColour : SeaColour);
		colors.Add(Height > 0.0f ? SandColour : SeaColour);
		colors.Add(Height > 0.0f ? SandColour : SeaColour);
	}

 	void AddSide(int i, Vector3 bottom, Vector3 top)
 	{
		AddTriangle(top+HexMetrics.corners[i], bottom + HexMetrics.corners[i], bottom + HexMetrics.corners[i+1]);
		AddTriangle(bottom+HexMetrics.corners[i+1], top+HexMetrics.corners[i+1], top+HexMetrics.corners[i]);

 	}

	public void Clicked(float mouseHeight)
 	{ 		
 		Height = mouseHeight;

 		Triangulate();
 	}
}
