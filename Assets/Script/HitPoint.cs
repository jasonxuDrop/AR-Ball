using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class HitPoint : MonoBehaviour
{
	public bool isPlayerBall = false;
    public int hitPoint;
	public float respawnDelay;

	protected float timeSinceDamageTaken = -1f;

    Vector3 respawnLocation;

	// change here to alter the damage taken by thing when hit
	static int trapDamage = 1;
	static int playerDamage = 0;

	public bool IsDead() {
		return (hitPoint <= 0);
	}

	private void Update() {
		if (timeSinceDamageTaken >= respawnDelay) {
			transform.position = respawnLocation;
			GetComponent<Rigidbody>().velocity = Vector3.zero;

			// disable player when hp reaches below zero
			// TODO: Add restart function 
			// TODO: Clear level when all enemy die. 
			if (IsDead()) {
				gameObject.SetActive(false);
			}

			timeSinceDamageTaken = -1f;
		}
		else if (timeSinceDamageTaken >= 0) {
			timeSinceDamageTaken += Time.deltaTime;
		}
	}

	private void OnCollisionEnter(Collision collision) {
		if (collision.gameObject.tag == "Trap") {
			if (timeSinceDamageTaken < 0) {
				hitPoint-=trapDamage;
				timeSinceDamageTaken = 0;
			}
		}
		if (collision.gameObject.tag == "Player") {
			hitPoint-=playerDamage;
		}

		
	}

	public void Awake() {
		respawnLocation = transform.position;
	}

	public void SetSpawnLocation(Vector3 newLocation) {
		respawnLocation = newLocation;
	}
}
