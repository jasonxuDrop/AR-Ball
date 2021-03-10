using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameSystemManager : MonoBehaviour
{
	public ARObjectPlacementController objectPlacementController;

    public GameObject levelToInstantiate;

	public PlayerController playerController;

	[Header("Debug")]
	public OnScreenTextDebugger debugText;
	public Level computerTestLevel;

	bool levelPlaced = false;

	private void Start() {
		if (!debugText) {
			debugText = FindObjectOfType<OnScreenTextDebugger>();
		}
		if (computerTestLevel) {
			playerController.playerMotor = computerTestLevel.playerMotor;
		}
	}


	void PlaceLevel() {
		if (levelPlaced) {
			// TODO Remove and Replace level functions

			debugText.Queue("Level already placed");
			return;
		}

		GameObject levelInstance =  objectPlacementController.PlaceObjectScreenCenter(levelToInstantiate);
		
		if (levelInstance) {
			levelPlaced = true;

			Level levelManager = levelInstance.GetComponent<Level>();
			playerController.playerMotor = levelManager.playerMotor;
		}
	}

	#region Unity Event References

	public void PlaceLevelButtonPressed() {
		if (objectPlacementController) {
			PlaceLevel();
		}
		else {
			Debug.Log("objectPlacementController not found");
		}
	}

	#endregion
}
