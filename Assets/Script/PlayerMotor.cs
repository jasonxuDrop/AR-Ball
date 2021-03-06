using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMotor : MonoBehaviour
{
	public float forceStrength = 50f;

	Rigidbody rb;

	private void Start() {
		rb = GetComponent<Rigidbody>();
	}

	public void Move(Vector2 force) {
		Vector3 finalForce = forceStrength * new Vector3(force.x, 0, force.y);
		// using ForceMode.VelocityChange to ignore mass
		// for calculations: https://answers.unity.com/questions/696068/difference-between-forcemodeforceaccelerationimpul.html
		rb.AddForce(finalForce, ForceMode.VelocityChange);
	}
}
