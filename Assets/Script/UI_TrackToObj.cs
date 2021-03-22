using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_TrackToObj : MonoBehaviour
{
	public Transform target;

	private void Update() {
		if (target) {
			var wantedPos = Camera.main.WorldToScreenPoint(target.position);
			transform.position = wantedPos;
		}
	}

	public void SetTrackTransform(Transform t) {
		target = t;
	}
}
