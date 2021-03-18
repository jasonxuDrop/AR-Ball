using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;

public class ARObjectPlacementController : MonoBehaviour
{
	public Camera cam;
	[SerializeField]
	ARRaycastManager raycastManager;
	ARPlaneManager planeManager;

	static List<ARRaycastHit> Hits = new List<ARRaycastHit>();

	public GameObject PlaceObjectScreenCenter(GameObject objectToPlace) {
		var screenCenter = cam.ViewportToScreenPoint(new Vector3(.5f, .5f));

		raycastManager = GetComponent<ARRaycastManager>();
		if (raycastManager.Raycast(screenCenter, Hits)) {
			Pose hitPose = Hits[0].pose;

			print("getting plane manager");

			// disable the arplanes from displaying and generating
			planeManager = GetComponent<ARPlaneManager>();
			foreach (var plane in planeManager.trackables) {
				plane.gameObject.SetActive(false);
			}
			planeManager.enabled = false;
			// TODO: Make the level placeable for a second time. 

			GameObject levelInstance =  Instantiate(objectToPlace, hitPose.position, hitPose.rotation);

			return levelInstance;
		}
		else {
			return null;
		}
	}
}
