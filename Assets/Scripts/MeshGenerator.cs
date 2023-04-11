using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(MeshFilter))]
public class MeshGenerator : MonoBehaviour {

	[SerializeField]
	private Material baseMaterial;
	private ItemLimits currLimits;
	[SerializeField]
	private Button patternButton;

	[SerializeField]
	private Vector3[] vertices;

	public bool BuildingFloor => this.currentFloor != null;
	public Texture2D[] availableTextures;
	private int textureIndex = 0;
	public Texture2D CurrentTexture => availableTextures[this.textureIndex];
	private GameObject currentFloor;

	[SerializeField]
	private TextMeshProUGUI debugText;

	private void Start() {
		//Initialize the mesh data
		this.currentFloor = null;
		this.textureIndex = 2;
		this.patternButton.onClick.AddListener(CycleTexture);
	}

	private void ShowTextures() {
		//Set the UI to active, and manage the rest with the buttons
		this.currentFloor = MakeFloor(currLimits);

	}

	public void BeginFloorBuilding(List<GameObject> markers) {
		//Get all the position info into a vec3 array
		const int CAPACITY = 4;
		Vector3[] positions = new Vector3[CAPACITY];
		for (int i = 0; i < CAPACITY; i++) {
			positions[i] = markers[i].transform.position;
		}
		//Create a delimiter for that
		currLimits = new ItemLimits(positions[0].y, positions);
		this.ShowTextures();
	}

	private void EndFloorBuilding(bool keepFloor) {
		//Destroy the floor if we don't want to keep it
		if (!keepFloor) {
			Destroy(this.currentFloor);
		}
		//Remove the reference we kept in our globals
		this.currentFloor = null;
	}

	private GameObject MakeFloor(ItemLimits limits) {
		//Create the object
		GameObject result = SpawnFloor(limits, CurrentTexture);
		//Update the mesh, so that all materials are incorporated properly and affected by lighting
		return result;
	}

	private void UpdateMesh(Mesh meshData, ItemLimits limits) {
		meshData.Clear();
		meshData.vertices = limits.Vertices;
		meshData.triangles = ItemLimits.TRIANGLES_ARRAY;
		meshData.uv = ItemLimits.UV_ARRAY;
		meshData.RecalculateBounds(); //Could be used for collision purposes
	}

	private GameObject SpawnFloor(ItemLimits limits, Texture2D texture) {
		//Initialize all the components that the mesh will need
		Mesh mesh = new Mesh();
		Material material = new Material(baseMaterial);

		GameObject floor = new GameObject("Floor instance");
		floor.AddComponent<MeshFilter>().mesh = mesh;
		var renderer = floor.AddComponent<MeshRenderer>();
		renderer.material = material;
		renderer.material.SetTexture("_MainTex", texture);
		//Get the average of these two (I know, it's rough)
		renderer.material.mainTextureScale = GetAverageTiling(limits.Vertices);
		this.UpdateMesh(mesh, limits);
		return floor;
	}

	//IMPORTANT NOTE:
	//The best way to implement this would be with a dynamic for loop that multiplies the value of the desired width and height and adjusted UVs, Triangles, and vertices accordingly
	//Rudementary way of avoiding a bit of tiling implemented like this because of time
	private Vector2 GetAverageTiling(Vector3[] vertices) {
		//Get the length of vertex 0 to vertex 2 for x, of vector 2 to 3 for y
		Vector3 firstSideX = vertices[0] - vertices[2];
		Vector3 secondSideX = vertices[1] - vertices[3];
		Vector3 firstSideY = vertices[2] - vertices[3];
		Vector3 secondSideY = vertices[0] -vertices[1];

		float magnitudeAvgX = (firstSideX.magnitude + secondSideX.magnitude) / 2f;
		float magnitudeAvgY = (firstSideY.magnitude + secondSideY.magnitude) / 2f;

		return new Vector2(magnitudeAvgX, magnitudeAvgY);
	}

	private void ChangeTexture(GameObject preview, Texture2D texture) {
		//Change the preview object's texture
		var renderer = preview.GetComponent<MeshRenderer>();
		renderer.material.mainTexture = texture;
	}

	private void CycleTexture() {
		if (currentFloor == null) {
			debugText.text = "Please generate a floor mesh first!";
			return;
		}
		this.textureIndex++;
		//Resets the texture index
		if (textureIndex >= this.availableTextures.Length) {
			textureIndex = 0;
		}
		//Otherwise, cycle through
		this.debugText.text = $"The current texture index is: {this.textureIndex}";
		ChangeTexture(this.currentFloor, this.CurrentTexture);
	}


}
