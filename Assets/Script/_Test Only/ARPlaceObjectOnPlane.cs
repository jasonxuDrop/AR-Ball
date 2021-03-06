using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;

public class ARPlaceObjectOnPlane : MonoBehaviour
{
    [SerializeField]
    ARRaycastManager m_RaycastManager;

    static List<ARRaycastHit> s_Hits = new List<ARRaycastHit>();

    [SerializeField]
    GameObject m_ObjectToPlace;

    void Update()
    {
        if (Input.touchCount > 0) {
            Touch touch = Input.GetTouch(0);

            if (touch.phase == TouchPhase.Began) {
                if (m_RaycastManager.Raycast(touch.position, s_Hits)) {
                    Pose hitPose = s_Hits[0].pose;

                    Instantiate(m_ObjectToPlace, hitPose.position, hitPose.rotation);
				}
			}
		}
    }

}

