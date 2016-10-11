using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class HexGrid : MonoBehaviour 
{
	public HexCell cellPrefab; 

 	public int width = 6;
 	public int height = 6;

 	HexCell [] cells;

 	public Text cellTextPrefab;
 	Canvas gridCanvas;

 	private int curWidth = 6;
 	private int curHeight = 6;
 
	void Awake () 
	{
		curWidth = -1;
		curHeight = -1;

		BuildGrid(width, height);
 	}

 	void BuildGrid(int w, int h)
 	{	
 		curHeight = h;
 		curWidth = w;

		cells = new HexCell[height * width];
 		gridCanvas = GetComponentInChildren<Canvas>();
	
 		int i = 0;

 		for (int z = 0; z < height; z++) 
 		{
 			for (int x = 0; x < width; x++) 
 			{
 				CreateCell(x, z, i);
 				i++;
 			}
 		}

		foreach (HexCell cell in cells)
			cell.Triangulate();
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
		if (height != curHeight || width != curWidth)
		{
			Debug.Log("build");
			BuildGrid(width, height);
		}

 		if (Input.GetMouseButtonDown(0)) 
 		{
 			HandleInput(true);
 		} 
		if (Input.GetMouseButtonDown(1)) 
 		{
 			HandleInput(false);
 		} 
 	}

	void HandleInput (bool left = true) 
	{
 		Ray inputRay = Camera.main.ScreenPointToRay(Input.mousePosition);
 		RaycastHit hitInfo;

		if (Physics.Raycast(inputRay, out hitInfo))
		{
			GameObject hitGO = hitInfo.transform.gameObject;
			HexCell hitCell = hitGO.GetComponent<HexCell>();
			if (hitCell != null)
			{
				hitCell.Clicked(left);
			}
		}
 	}

   	void TouchCell (Vector3 position) 
   	{
 		position = transform.InverseTransformPoint(position);
		HexCoordinates coordinates = HexCoordinates.FromPosition(position);
		Debug.Log(coordinates.ToString());
	}
}
