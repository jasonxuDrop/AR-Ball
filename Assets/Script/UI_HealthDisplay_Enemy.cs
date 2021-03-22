using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_HealthDisplay_Enemy : MonoBehaviour
{
    UI_TrackToObj objectTracker;
    Animator animator;

	HitPoint targetHitPoint;

    public void Init(GameObject targetEnemyBall) {
		Start();
		targetHitPoint = targetEnemyBall.GetComponent<HitPoint>();
		objectTracker.SetTrackTransform(targetEnemyBall.transform);
	}

	private void Start() {
		objectTracker = GetComponent<UI_TrackToObj>();
		animator = GetComponent<Animator>();
	}

	public void Update() {
		float healthRatio = targetHitPoint.GetHealthRatio();
		animator.SetFloat("fillAmount", healthRatio);
		if (targetHitPoint.IsDead()) {
			gameObject.SetActive(false);
		}
	}

}
