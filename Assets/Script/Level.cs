using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Level : MonoBehaviour
{
    public PlayerMotor playerMotor;
    public Transform placementIndicator;

	[Header("Player Attack Damage")]
	public bool doOverridePlayerAttackDamage = false;
	public int newPlayerDamage = 5;

	[Header("Debug")]
	public bool isComputerOnlyTestLevel = false;

    List<Transform> levelObjectTransforms = new List<Transform>();

	private void Awake() {
		foreach (Transform t in transform) {
			if (t != placementIndicator && t.gameObject.activeSelf) {
				levelObjectTransforms.Add(t);
			}
		}
		if (isComputerOnlyTestLevel) {
			ShowLevel();
		}
		else {
			ShowIndicator();
		}

		if (doOverridePlayerAttackDamage)
		{
			HitPoint.SetPlayerDamage(newPlayerDamage);
		}
	}

	public void ShowIndicator() {
		foreach (Transform t in levelObjectTransforms) {
			t.gameObject.SetActive(false);
		}
		placementIndicator.gameObject.SetActive(true);
	}
	public void ShowLevel() {
		foreach (Transform t in levelObjectTransforms) {
			t.gameObject.SetActive(true);
		}
		placementIndicator.gameObject.SetActive(false);
	}
}
