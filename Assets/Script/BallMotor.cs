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
	[HideInInspector] public float maxSpeed;

	protected bool updateVelocity;
	protected Vector3 toVelocity = new Vector3();

	private void Awake() {
		if (!rb) {
			rb = GetComponent<Rigidbody>();
		}
	}
	private void Update() {
		if (!rb) {
			rb = GetComponent<Rigidbody>();
		}
	}

	public virtual void Move(Vector3 force) {
		rb.AddForce(force, ForceMode.VelocityChange);
		timeSinceMoved = 0;
	}
	public virtual void Move(Vector3 force, float _timeSinceMoved) {
		rb.AddForce(force, ForceMode.VelocityChange);
		timeSinceMoved = _timeSinceMoved;
	}
	public virtual void SetTimeSinceMoved(float toTime) {
		timeSinceMoved = toTime;
	}
	public float GetSpeedRatio() {
		return rb.velocity.magnitude / maxSpeed;
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
	}

	protected virtual void OnCollisionEnter(Collision collision) {
		if (collision.gameObject.tag == "Wall") {
			var contact = collision.GetContact(0);

			// try to change the velocity directly
			toVelocity = Vector3.Reflect(rb.velocity, contact.normal);
			rb.velocity = toVelocity;
		}
	}

	public void ApplyGravity() {
		Vector3 gravity = -9.81f * gravityScale * Vector3.up;
		rb.AddForce(gravity, ForceMode.Acceleration);
	}
}
