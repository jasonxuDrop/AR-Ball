using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PlayerController : MonoBehaviour
{
	public float forceStrength = 1f;

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
	Vector3 lastCameraPosition;

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

		bool canMovePlayer = (playerMotor && playerMotor.HasStoppedMoving());
		if (!canMovePlayer) {
			print(playerMotor.HasStoppedMoving());
			return;
		}

		Vector3 moveForce3d;

		if (playerMotor) {

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
			moveForce3d = new Vector3(-lastInput.x, highAngleProtection, -lastInput.y);

			// make the direction of movement relative to the camera 
			moveForce3d = cameraTransform.TransformDirection(moveForce3d);

			// flatten the movement to x and z axis only
			moveForce3d.y = 0;

			// normalize movement
			moveForce3d.Normalize();

			// scale movement
			moveForce3d *= lastInput.SqrMagnitude();
			//Debug.Log("Move Force (flat and scaled): " + moveForce3d);

			// scale movement to force amount
			moveForce3d *= forceStrength;

			if (doRelease) {
				Debug.Log("doRelease");

				playerMotor.Move(moveForce3d);
			}


			// if input change, render line
			// else hide renderer
			if (predictionManager
				&& (inputHorizontal != 0 || inputVertical != 0)
				&& ((inputHorizontal != lastInput.x || inputVertical != lastInput.y)
					|| (Vector3.SqrMagnitude(lastCameraPosition - cameraTransform.position) < 0.001f)) ) {
				predictionManager.Predict(playerMotor.gameObject, playerMotor.transform.position, moveForce3d);
			}
			if (doRelease) {
				predictionManager.ClearPrediction();
			}
			Debug.DrawRay(playerMotor.transform.position, moveForce3d * 20, Color.red);


			lastInput = new Vector2(inputHorizontal, inputVertical);
			lastCameraPosition = cameraTransform.position;
		}
	}
}
