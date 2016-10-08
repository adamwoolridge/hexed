using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class HexGrid : MonoBehaviour 
{
	public HexCell cellPrefab; 
 	public int width = 6;
 	public int height;

 	HexCell [] cells;
 	HexMesh hexMesh;

 	public Text cellTextPrefab;
 	Canvas gridCanvas;

	void Awake () 
	{
 		cells = new HexCell[height * width];
 		gridCanvas = GetComponentInChildren<Canvas>();
		hexMesh = GetComponentInChildren<HexMesh>();

 		int i = 0;

 		for (int z = 0; z < height; z++) 
 		{
 			for (int x = 0; x < width; x++) 
 			{
 				CreateCell(x, z, i);
 				i++;
 			}
 		}
 	}

 	void Start()
 	{
 		hexMesh.Triangulate(cells);
 	}

	void CreateCell (int x, int z, int i) 
	{
 		Vector3 position;
		position.x = (x + z * 0.5f - z / 2) * (HexMetrics.innerRadius * 2f);
		position.y = 0f;
		position.z = z * (HexMetrics.outerRadius * 1.5f);

 		HexCell cell = cells[i] = Instantiate<HexCell>(cellPrefab);
 		cell.transform.SetParent(transform, false);
 		cell.transform.localPosition = position;
		cell.coordinates = HexCoordinates.FromOffsetCoordinates(x, z);

		Text label = Instantiate<Text>(cellTextPrefab);
 		label.rectTransform.SetParent(gridCanvas.transform, false);
 		label.rectTransform.anchoredPosition = new Vector2(position.x, position.z);
		label.text = cell.coordinates.ToStringOnSeparateLines();
 	}

	void Update () 
	{
 		if (Input.GetMouseButtonDown(0)) 
 		{
 			HandleInput();
 		} 
 	}

	void HandleInput () 
	{
 		Ray inputRay = Camera.main.ScreenPointToRay(Input.mousePosition);
 		RaycastHit hit;
 		if (Physics.Raycast(inputRay, out hit)) 
 		{
 			TouchCell(hit.point);
 		}
 	}

   	void TouchCell (Vector3 position) 
   	{
 		position = transform.InverseTransformPoint(position);
		HexCoordinates coordinates = HexCoordinates.FromPosition(position);
		Debug.Log(coordinates.ToString());
	}
}
