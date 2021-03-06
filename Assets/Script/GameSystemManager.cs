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

	bool levelPlaced = false;

	private void Start() {
		if (!debugText) {
			debugText = FindObjectOfType<OnScreenTextDebugger>();
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
		PlaceLevel();
	}

	#endregion
}
