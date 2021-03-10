using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PlayerController : MonoBehaviour
{
	[Range(0f, 1f)]
	public float cancelMaxDistanceSqr = 0.25f;
	public float highAngleProtection = 0.1f;

	public Transform cameraTransform;

	[SerializeField]
    Joystick joystick;

	[HideInInspector]
	public PlayerMotor playerMotor;

	PredictionManager predictionManager;

	[Header("Debug")]
	public OnScreenTextDebugger debugText;

	Vector2 lastInput;

	private void Start() {
		if (!debugText) {
			debugText = FindObjectOfType<OnScreenTextDebugger>();
		}
		
		predictionManager = GetComponent<PredictionManager>();
	}

	private void Update() {
		DebugWithText();
		InputUpdate();
	}

	void DebugWithText() {
		debugText.Queue("x: " + joystick.Vertical + ",  y: " + joystick.Horizontal);
	}


	public void InputUpdate() {

		if (playerMotor) {

			//Debug.Log("player motor found");

			float inputHorizontal = joystick.Horizontal;
			float inputVertical = joystick.Vertical;

			// true: fire the player this frame
			// false: do not fire the player
			// true if 0 input this frame and was not all zero last frame
			// false if last frame is in cancel zone
			bool doRelease = (inputHorizontal == 0 && inputVertical == 0 &&
				!(Mathf.Abs(lastInput.SqrMagnitude()) < cancelMaxDistanceSqr));


			// NEW WAY OF MOVING PLAYER
			// generate a vector 2 from input. 
			Vector3 moveForce3d = new Vector3(lastInput.x, highAngleProtection, lastInput.y);

			// make the direction of movement relative to the camera 
			moveForce3d = Camera.main.transform.TransformDirection(moveForce3d);
			Debug.Log("Move Force (relative to camera): " + moveForce3d);


			// flatten the movement to x and z axis only
			moveForce3d.y = 0;

			// normalize movement
			moveForce3d.Normalize();
			Debug.Log("Move Force (flat and normalized): " + moveForce3d);


			if (doRelease) {
				Debug.Log("doRelease");

				playerMotor.Move(moveForce3d);

				doRelease = false;
			}

			if (predictionManager) {
				predictionManager.predict(playerMotor.gameObject, playerMotor.transform.position, moveForce3d);
			}
			Debug.DrawRay(playerMotor.transform.position, moveForce3d * 20, Color.red);


			lastInput = new Vector2(inputHorizontal, inputVertical);

		}
	}
}
