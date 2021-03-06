using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PlayerController : MonoBehaviour
{
    [SerializeField]
    Joystick joystick;

	public Transform cameraTransform;

	[HideInInspector]
	public PlayerMotor playerMotor;

	[Range(0f, 1f)]
	public float cancelMaxDistanceSqr = 0.25f;

	public OnScreenTextDebugger debugText;

	Vector2 lastInput;

	private void Start() {
		if (!debugText) {
			debugText = FindObjectOfType<OnScreenTextDebugger>();
		}
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

			Debug.Log("player motor found");

			float inputHorizontal = joystick.Horizontal;
			float inputVertical = joystick.Vertical;

			// true: fire the player this frame
			// false: do not fire the player
			// true if 0 input this frame and was not all zero last frame
			// false if last frame is in cancel zone
			bool doRelease = (inputHorizontal == 0 && inputVertical == 0 &&
				!(Mathf.Abs(lastInput.SqrMagnitude()) < cancelMaxDistanceSqr));


			Vector2 normalMoveForce = new Vector2(lastInput.x, lastInput.y);

			// Find camera Angle forward from camera
			float deltaX = cameraTransform.position.x - playerMotor.transform.position.x;
			float deltaZ = cameraTransform.position.z - playerMotor.transform.position.z;
			float cameraAngleRad = Mathf.Atan2(deltaZ, deltaX) + 1.571f;

			//adjust input to camera angle
			float sin = Mathf.Sin(cameraAngleRad);
			float cos = Mathf.Cos(cameraAngleRad);

			float tx = normalMoveForce.x;
			float tz = normalMoveForce.y;
			normalMoveForce.x = (cos * tx) - (sin * tz);
			normalMoveForce.y = (sin * tx) + (cos * tz);

			normalMoveForce = -normalMoveForce;

			Debug.DrawRay(playerMotor.transform.position, normalMoveForce * 20, Color.red);



			if (doRelease) {
				Debug.Log("doRelease");

				playerMotor.Move(normalMoveForce);

				doRelease = false;
			}


			lastInput = new Vector2(inputHorizontal, inputVertical);

		}
	}
}
