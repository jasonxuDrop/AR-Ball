using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMotor : BallMotor
{
	// for line renderer only
	[HideInInspector] public bool breakSimulation = false;
	


	//public void Move(Vector2 force) {
	//	Vector3 finalForce = forceStrength * new Vector3(force.x, 0, force.y);
	//	// using ForceMode.VelocityChange to ignore mass
	//	// for calculations: https://answers.unity.com/questions/696068/difference-between-forcemodeforceaccelerationimpul.html
	//	rb.AddForce(finalForce, ForceMode.VelocityChange);
	//}

	public override void FixedUpdate() {
		base.FixedUpdate();

		// reset similuation on OG object but not simulation object
		breakSimulation = false;
	}

	protected override void OnCollisionEnter(Collision collision) {
		base.OnCollisionEnter(collision);

		if (collision.gameObject.tag == "Wall" && frameCountBeforeSimulationBreak <= 0) {
			breakSimulation = true;
		}
		// if ramming into enemy
		else if (collision.gameObject.tag == "Enemy") {
			breakSimulation = true;
			//print("hit enemy");
			BallMotor otherMotor = collision.gameObject.GetComponent<BallMotor>();
			if (otherMotor) {
				otherMotor.SetTimeSinceMoved(timeSinceMoved);
			}
		}
	}
}
