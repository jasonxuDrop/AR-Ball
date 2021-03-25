using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class HitPoint : MonoBehaviour
{
	public bool isPlayerBall = false;
    public int hitPoint;
	public float respawnDelay;

	[HideInInspector] public int hitPointMax;
	protected float timeSinceDamageTaken = -1f;

    Vector3 respawnLocation;

	// change here to alter the damage taken when hit
	static int trapDamage = 10;
	static int playerDamage = 5; // damage on max speed, scaled down linearly to the current player speed

	public float GetHealthRatio() {
		return (float) hitPoint / (float) hitPointMax;
	}
	public bool IsDead() {
		return (hitPoint <= 0);
	}

	private void Update() {
		if (timeSinceDamageTaken >= respawnDelay) {
			transform.localPosition = respawnLocation;
			GetComponent<Rigidbody>().velocity = Vector3.zero;


			timeSinceDamageTaken = -1f;
		}
		else if (timeSinceDamageTaken >= 0) {
			timeSinceDamageTaken += Time.deltaTime;
		}
		// disable player when hp reaches below zero
		if (IsDead()) {
			// TODO: Use animation to disable instead
			gameObject.SetActive(false);
		}

		// TODO: Add restart function 
		// TODO: Clear level when all enemy die. 
	}

	private void OnCollisionEnter(Collision collision) {
		if (collision.gameObject.tag == "Trap") {
			if (timeSinceDamageTaken < 0) {
				hitPoint-=trapDamage;
				timeSinceDamageTaken = 0;
			}
		}
		if (collision.gameObject.tag == "Player") {
			PlayerMotor motor = collision.gameObject.GetComponent<PlayerMotor>();

			int dmg = Mathf.CeilToInt(playerDamage * motor.GetSpeedRatio());

			Debug.Log("Taken damage " + dmg);

			hitPoint-= dmg;
		}
	}

	public void Awake() {
		SetSpawnLocation(transform.localPosition);
		hitPointMax = hitPoint;
	}

	public void SetSpawnLocation(Vector3 newLocation) {
		respawnLocation = newLocation;
	}
}
