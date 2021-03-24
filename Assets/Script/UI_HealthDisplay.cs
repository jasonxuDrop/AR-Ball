using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UI_HealthDisplay : MonoBehaviour
{
    [HideInInspector] public HitPoint hitPointToTrack;

	public TMP_Text healthDisplayText;

	public void Update() {
		if (hitPointToTrack) {
			healthDisplayText.text = "Player HP: " + hitPointToTrack.hitPoint;
		}
		else {
			Debug.LogWarning("hitPointToTrack never assigned to this UI_HealthDisplay");
		}
	}
}
