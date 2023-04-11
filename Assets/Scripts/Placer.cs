using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using TMPro;
using Auxiliars;
using UnityEngine.UI;

[RequireComponent(typeof(ARRaycastManager))]
[RequireComponent(typeof(ARPlaneManager))]
public class Placer : MonoBehaviour {

	private const int TOUCH_INDEX = 0;
	private const int MAX_MARKER_COUNT = 4;
	private const float PLACING_COOLDOWN_SECONDS = 1f;

	private List<ARRaycastHit> hits = new List<ARRaycastHit>();
	private List<GameObject> activeMarkers;
	
	[SerializeField]
	private GameObject markerPrefab;
	[SerializeField]
	private GameObject guideMarker;
	[SerializeField]
	private Button addButton;
	[SerializeField]
	private Button removeButton;
	[SerializeField]
	private MeshGenerator meshGenerator;
	[SerializeField]
	private TextMeshProUGUI debugText;
	private ARRaycastManager raycastManager;
	private ARPlaneManager planeManager;
	private Camera mainCam;

	//Makes sure we don't place like 3 markers at the same time with a small delay
	private SpartanTimer cooldownTimer;

	private void Start() {
		//Instantiate input events
		raycastManager = GetComponent<ARRaycastManager>();
		planeManager = GetComponent<ARPlaneManager>();
		this.activeMarkers = new List<GameObject>(MAX_MARKER_COUNT);
		this.cooldownTimer = new SpartanTimer(TimeMode.Framed);
		this.mainCam = Camera.main;
		this.addButton.onClick.AddListener(PlaceMarker);
	}

	private void Update() {
		this.HandleCameraRaycast();
	}

	private void HandleCameraRaycast() {
		//Get the camera's raycast
		//First, we need the center of the screen
		Ray centerRay = GetCameraForwardRay();
		bool canMark = RayCastToPlane(centerRay);
		this.guideMarker.SetActive(canMark);
		if (!canMark || hits.Count < 1) return;
		//Keep a cone attached to the hit.point of the camera's raycast
		this.guideMarker.transform.position = hits[0].pose.position;
	}

	public void PlaceMarker() {
		if (!CanPlaceMarker(GetCameraForwardRay())) return;
		//Guaranteed to have more than 1 hit here, so let's get the first one
		ARRaycastHit hit = hits[0];
		debugText.text = $"The hit is: {hit.pose.position}";
		//Create the marker
		GameObject instance = Instantiate(markerPrefab, hit.pose.position, markerPrefab.transform.rotation);
		//Add it to the list to keep track of it
		this.activeMarkers.Add(instance);
		//Start the cooldown timer
		this.cooldownTimer.Reset();
		//If there are 4 active markers, call upon the mesh generator
		if (this.activeMarkers.Count >= MAX_MARKER_COUNT) {
			this.meshGenerator.BeginFloorBuilding(activeMarkers);
			this.activeMarkers.Clear();
		}
	}

	public void RemoveLastMarker() {
		if (this.activeMarkers.Count < 1) return;
		//There are markers in the list
		//Get the referenced object
		GameObject instance = this.activeMarkers[this.activeMarkers.Count - 1];
		//Remove the reference from the list
		this.activeMarkers.RemoveAt(this.activeMarkers.Count - 1);
		//Delete the game object
		Destroy(instance);
	}

	private bool RayCastToPlane(Ray ray) {
		return raycastManager.Raycast(ray, hits, UnityEngine.XR.ARSubsystems.TrackableType.PlaneWithinPolygon);
	}

	private Ray GetCameraForwardRay() {
		Vector3 viewportCenter = new Vector3(0.5f, 0.5f, 0);
		return mainCam.ViewportPointToRay(viewportCenter);
	}

	private bool IsInCooldown() {
		float currSeconds = this.cooldownTimer.GetCurrentTime(TimeScaleMode.Seconds);
		return this.cooldownTimer.Started && currSeconds < PLACING_COOLDOWN_SECONDS;
	}

	private bool CanPlaceMarker(Ray ray) {
		return this.activeMarkers.Count < MAX_MARKER_COUNT && RayCastToPlane(ray) && hits.Count > 0 && !IsInCooldown();
	}
}
