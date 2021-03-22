using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameSystemManager : MonoBehaviour
{
	public ARObjectPlacementController objectPlacementController;

    public GameObject levelToInstantiate;

	public PlayerController playerController;
	public UI_HealthDisplay healthDisplay;

	[Header("Prefabs")]
	public GameObject enemyHitPointUi;

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
			if (healthDisplay)
				healthDisplay.hitPointToTrack = levelManager.playerMotor.GetComponent<HitPoint>();
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

		// setup enemy health display
		Canvas canvas = FindObjectOfType<Canvas>();
		var ballMotors = levelManager.GetComponentsInChildren<EnemyMotor>();
		foreach (var ballMotor in ballMotors) {
			var ballHp = ballMotor.GetComponent<HitPoint>();
			var ui = Instantiate(enemyHitPointUi, canvas.transform);
			ui.GetComponent<UI_HealthDisplay_Enemy>().Init(ballMotor.gameObject);
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
