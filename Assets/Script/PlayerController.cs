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
	public UI_PlayerMovementVisualizer movementVisualizer;

	public UI_PlayerForceVisualizer forceIndicator;

	Vector2 lastInput;
	Vector3 lastCameraPosition;

	[HideInInspector] public bool isTurn = false;
	bool watchForMotorStop = false;

	// init is called by Game System manager as a late Start function
	public void Init() {
		if (!forceIndicator) 
			forceIndicator = FindObjectOfType<UI_PlayerForceVisualizer>();
		if (!movementVisualizer)
			movementVisualizer = GetComponent<UI_PlayerMovementVisualizer>();

		playerMotor.maxSpeed = forceStrength;
	}

	private void Update() {
		InputUpdate();
	}



	public void InputUpdate() {

		// WATCH FOR END TURN (has fired and has stopped)
		if (watchForMotorStop && playerMotor.HasStoppedMoving())
		{
			print("player stopped moving????");
			// END TURN
			isTurn = false;
			watchForMotorStop = false;
		}
		
		// if motor still moving, skip code
		bool canMovePlayer = (playerMotor && playerMotor.HasStoppedMoving());
		if (!canMovePlayer || !isTurn) {
			//print(playerMotor.HasStoppedMoving());
			return;
		}

		

		Vector3 moveForce3d;

		if (playerMotor) {

			// VARIABLES from inputs
			float inputHorizontal = joystick.Horizontal;
			float inputVertical = joystick.Vertical;
			Vector2 input = new Vector2(inputHorizontal, inputVertical);

			// if the player didn't pull the thumb far enough
			bool isInCancelZone = Mathf.Abs(input.SqrMagnitude()) < cancelMaxDistanceSqr;
			bool isInCancelZone_Last = Mathf.Abs(lastInput.SqrMagnitude()) < cancelMaxDistanceSqr;

			// true: fire the player this frame
			// false: do not fire the player
			// true if 0 input this frame and was not all zero last frame
			// false if last frame is in cancel zone
			bool doRelease = (inputHorizontal == 0 && inputVertical == 0 && !isInCancelZone_Last);


			// UPDATE OTHER
			if (input != lastInput)
			{
				forceIndicator.ChangeForceDisplay(input.magnitude);
			}

			
			// MOVE PLAYER
			if (!isInCancelZone_Last) { 
				// generate a vector 2 from input. 
				moveForce3d = new Vector3(-lastInput.x, highAngleProtection, -lastInput.y);
				// make the direction of movement relative to the camera 
				moveForce3d = cameraTransform.TransformDirection(moveForce3d);
				// flatten the movement to x and z axis only
				moveForce3d.y = 0;
				// normalize movement
				moveForce3d.Normalize();

				Vector3 moveForceNormalized = moveForce3d;

				// VISUALIZE movement
				movementVisualizer.Visualize(playerMotor.transform.position, moveForceNormalized);

				// PLAYER FIRES
				if (doRelease) {
					// scale movement
					moveForce3d *= lastInput.magnitude;
					// scale movement to force amount
					moveForce3d *= forceStrength;

					playerMotor.Move(moveForce3d);
					watchForMotorStop = true;
					movementVisualizer.Clear();
				}


				
				//Debug.DrawRay(playerMotor.transform.position, moveForceNormalized * 10, Color.green);
			}
			else
			{
				// VISUALIZE CLEAR
				movementVisualizer.Clear();
			}

			lastInput = new Vector2(inputHorizontal, inputVertical);
			lastCameraPosition = cameraTransform.position;
		}

	}

	public void StartTurn()
	{
		isTurn = true;
	}
}
