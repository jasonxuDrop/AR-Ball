using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;

public class ARObjectPlacementController : MonoBehaviour
{
	public Camera cam;
	// the level instance to be placed
	[HideInInspector] public GameObject objectToPlace;
	// false: object has not been attatched to the AR tracking yet, place level cannot be pressed
	[HideInInspector] public bool isObjectOnArPlane = false;

	[SerializeField]
	ARRaycastManager raycastManager;
	ARPlaneManager planeManager;

	// DYNAMIC OBJECTS
	GameObject levelInstance;
	bool isPlacementMode;

	static List<ARRaycastHit> Hits = new List<ARRaycastHit>();

	// TODO: Instantiate a level from the start and keep updating it when center raycast hits targets.
	//		PlaceObjectScreenCenter only toggles the active state of the level

	public void Start() {
		levelInstance = Instantiate(objectToPlace);
		isPlacementMode = true;
	}
	public void Update() {

		// obejct placement mode: the level instance will move to the center of the screen and find a plane to stay on
		//		only the indicator will be visible
		if (isPlacementMode) {

			// cast ray from screen center to see if there is a plane in the way
			var screenCenter = cam.ViewportToScreenPoint(new Vector3(.5f, .5f));
			raycastManager = GetComponent<ARRaycastManager>();
			if (raycastManager.Raycast(screenCenter, Hits)) {

				Pose hitPose = Hits[0].pose;

				levelInstance.transform.position = hitPose.position;
				levelInstance.transform.rotation = hitPose.rotation;

				isObjectOnArPlane = true;
			}
		}

	}

	public GameObject PlaceObjectScreenCenter() {
		if (isPlacementMode) {
			isPlacementMode = false;

			levelInstance.GetComponent<Level>().ShowLevel();

			// destory existing planes and disable ar plane tracking script
			if (!planeManager)
				planeManager = GetComponent<ARPlaneManager>();
			foreach (var plane in planeManager.trackables) {
				Destroy(plane.gameObject);
			}
			planeManager.enabled = false;

			return levelInstance;
		}
		else {
			isPlacementMode = true;

			levelInstance.GetComponent<Level>().ShowIndicator();

			// re enable the ar plane tracking script
			if (!planeManager)
				planeManager = GetComponent<ARPlaneManager>();
			planeManager.enabled = true;

			return null;
		}
	}
}
