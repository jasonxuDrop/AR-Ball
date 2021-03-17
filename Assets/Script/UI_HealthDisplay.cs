using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UI_HealthDisplay : MonoBehaviour
{
    public HitPoint hitPointToTrack;

	public TMP_Text healthDisplayText;

	public void Update() {
		healthDisplayText.text = "Player HP: " + hitPointToTrack.hitPoint;
	}
}
