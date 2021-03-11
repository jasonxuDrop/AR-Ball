using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMotor : MonoBehaviour
{
	Rigidbody rb;
	public AnimationCurve speedDownCurve;
	public float speedDownDuration;


	float timeSinceMoved = -1f;

	bool updateVelocity;
	Vector3 toVelocity =new Vector3();

	// for line renderer only
	[HideInInspector] public bool breakSimulation = false;

	private void Awake() {
		if (!rb) {
			rb = GetComponent<Rigidbody>();
		}
	}

	//public void Move(Vector2 force) {
	//	Vector3 finalForce = forceStrength * new Vector3(force.x, 0, force.y);
	//	// using ForceMode.VelocityChange to ignore mass
	//	// for calculations: https://answers.unity.com/questions/696068/difference-between-forcemodeforceaccelerationimpul.html
	//	rb.AddForce(finalForce, ForceMode.VelocityChange);
	//}


	public void FixedUpdate() {
		if (timeSinceMoved > speedDownDuration) {
			timeSinceMoved = -1f;
			rb.velocity *= 0;
		}
		else {
			float dampFactor = speedDownCurve.Evaluate(timeSinceMoved / speedDownDuration);
			rb.velocity *= dampFactor;
			timeSinceMoved += Time.fixedDeltaTime;
		}

		// reset similuation on OG object but not simulation object
		breakSimulation = false;

		// *late* update to change velocity
		if (updateVelocity) {
			print("bouncing velocity from "+rb.velocity.ToString("F2") + " to " + toVelocity.ToString("F2"));
			rb.velocity = toVelocity;
			updateVelocity = false;
		}
	}

	private void Update() {
		
	}

	public void Move(Vector3 force) {
		rb.AddForce(force, ForceMode.VelocityChange);
		timeSinceMoved = 0;
	}

	private void OnCollisionEnter(Collision collision) {
		
		if (collision.gameObject.tag == "Wall") {
			var contact = collision.GetContact(0);

			breakSimulation = true;

			// try to change the velocity directly
			toVelocity = Vector3.Reflect(rb.velocity, contact.normal);
			updateVelocity = true;
		}
	}
}
