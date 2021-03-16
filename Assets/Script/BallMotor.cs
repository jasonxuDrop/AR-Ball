using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallMotor : MonoBehaviour
{
	protected Rigidbody rb;
	public float gravityScale = 0.1f;
	public AnimationCurve speedDownCurve;
	public float speedDownDuration;

	protected float timeSinceMoved = -1f;

	protected bool updateVelocity;
	protected Vector3 toVelocity = new Vector3();

	private void Awake() {
		if (!rb) {
			rb = GetComponent<Rigidbody>();
		}
	}

	public virtual void Move(Vector3 force) {
		rb.AddForce(force, ForceMode.VelocityChange);
		timeSinceMoved = 0;
	}

	public virtual bool HasStoppedMoving() {
		return (timeSinceMoved < 0
			|| rb.velocity.sqrMagnitude < 0.003f);
	}

	public virtual void FixedUpdate() {
		// custome gravity
		ApplyGravity();

		// eventrual full stop
		if (timeSinceMoved > speedDownDuration) {
			timeSinceMoved = -1f;
			rb.velocity *= 0;
		}
		else if (timeSinceMoved >= 0) {
			float dampFactor = speedDownCurve.Evaluate(timeSinceMoved / speedDownDuration);
			rb.velocity *= dampFactor;
			timeSinceMoved += Time.fixedDeltaTime;
		}

		// *late* update to change velocity
		if (updateVelocity) {
			//print("bouncing velocity from "+rb.velocity.ToString("F2") + " to " + toVelocity.ToString("F2"));
			rb.velocity = toVelocity;
			updateVelocity = false;
		}
	}

	protected virtual void OnCollisionEnter(Collision collision) {
		if (collision.gameObject.tag == "Wall") {
			var contact = collision.GetContact(0);

			// try to change the velocity directly
			toVelocity = Vector3.Reflect(rb.velocity, contact.normal);
			updateVelocity = true;
		}
	}

	public void ApplyGravity() {
		Vector3 gravity = -9.81f * gravityScale * Vector3.up;
		rb.AddForce(gravity, ForceMode.Acceleration);
	}
}
