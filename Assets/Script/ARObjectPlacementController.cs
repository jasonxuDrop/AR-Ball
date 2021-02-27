using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;

public class ARObjectPlacementController : MonoBehaviour
{
    [SerializeField]
    ARRaycastManager RaycastManager;

    static List<ARRaycastHit> Hits = new List<ARRaycastHit>();

    public bool PlaceObjectScreenCenter(GameObject objectToPlace) {
        var screenCenter = Camera.current.ViewportToScreenPoint(new Vector3(.5f, .5f));

        if (RaycastManager.Raycast(screenCenter, Hits)) {
            Pose hitPose = Hits[0].pose;

            Instantiate(objectToPlace, hitPose.position, hitPose.rotation);
        }
        else {
            return false;
		}

        return true;
    }
}
