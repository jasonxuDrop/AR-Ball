using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameSystemManager : MonoBehaviour
{
	public ARObjectPlacementController objectPlacementController;

    public GameObject levelToInstantiate;

	public PlayerController playerController;

	PredictionManager predictionManager;

	[Header("Debug")]
	public OnScreenTextDebugger debugText;
	public Level computerTestLevel;

	bool levelPlaced = false;

	private void Start() {
		if (!debugText) {
			debugText = FindObjectOfType<OnScreenTextDebugger>();
		}
		if (computerTestLevel) {
			InitLevel(computerTestLevel);
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
			InitLevel(levelManager);
		}
	}


	private void InitLevel(Level levelManager) {

		if (levelManager) {
			playerController.playerMotor = levelManager.playerMotor;
		}

		// setup prediction manager
		if (!predictionManager) {
			predictionManager = GetComponent<PredictionManager>();
			if (predictionManager) {
				predictionManager.level = levelManager.transform;
				predictionManager.UpdateLevel();
			}
			else {
				Debug.LogError("no prediction manager found");
			}
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
