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

	[Header("Debug")]
	public OnScreenTextDebugger debugText;
	public Level computerTestLevel;

	PredictionManager predictionManager;
	List<GameObject> enemyHpDisplays = new List<GameObject>();
	bool isLevelInitialized = false;

	private void Start() {
		if (!debugText) {
			debugText = FindObjectOfType<OnScreenTextDebugger>();
		}
		if (computerTestLevel) {
			InitLevel(computerTestLevel);
		}
	}

	public void Awake() {
		if (objectPlacementController && !computerTestLevel) {
			objectPlacementController.objectToPlace = levelToInstantiate;
		}
	}


	void PlaceLevel() {

		GameObject levelInstance =  objectPlacementController.PlaceObjectScreenCenter();
		
		if (levelInstance) {
			Level levelManager = levelInstance.GetComponent<Level>();

			if (!isLevelInitialized) {
				InitLevel(levelManager);
				Debug.Log("init level called");
			}
			else
				ActiveLevel(true);
		}
		else {
			ActiveLevel(false);
		}
	}


	private void InitLevel(Level levelManager) {

		isLevelInitialized = true;

		// connect player controller to player motor
		if (levelManager) {
			playerController.playerMotor = levelManager.playerMotor;
			playerController.Init();
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
			enemyHpDisplays.Add(ui);
		}
	}

	// set an existing level active to state (must be called after init level is called
	private void ActiveLevel(bool state) {
		// set the script active state
		playerController.enabled = state;
		predictionManager.enabled = state;
		enemyHpDisplays.ForEach(display => {
			display.SetActive(state);
		});

		predictionManager.doPredict = state;
		if (state == true) {
			predictionManager.UpdateLevel();
		}

		// TODO: set UI animation
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
