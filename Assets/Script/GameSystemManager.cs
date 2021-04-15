using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;

public class GameSystemManager : MonoBehaviour
{
	public PlayerController playerController;
	public UI_HealthDisplay healthDisplay;
	// the inverted scale the the game object appears to be at
	public float contentScale = 1f; 
	public float minScale = 1;
	public float maxScale = 5;

	[Header("Set on Awake")]
	public GameObject levelToInstantiate;

	[Header("Prefabs")]
	public GameObject enemyHitPointUi;

	[Header("AR only")]
	public ARObjectPlacementController objectPlacementController;
	public Transform arRootObject;
	public ARSessionOrigin arSessionOrigin; 

	[Header("Debug only")]
	public OnScreenTextDebugger debugText;
	public Level computerTestLevel;

	// private variables
	PredictionManager predictionManager;
	List<HitPoint> enemyHitPoints = new List<HitPoint>();
	List<GameObject> enemyHpDisplays = new List<GameObject>();
	HitPoint playerHp;
	bool isLevelAssignedByLevelManager = false;
	bool isLevelInitialized = false;

	private void Start() {
		if (!debugText) {
			debugText = FindObjectOfType<OnScreenTextDebugger>();
		}
		if (computerTestLevel) {
			InitLevel(computerTestLevel);
		}
		ChangeContentScale();
	}

	public void Awake() {
		if (isLevelAssignedByLevelManager)
			return;

		if (objectPlacementController && !computerTestLevel)
		{
			objectPlacementController.objectToPlace = levelToInstantiate;
		}
	}

	// called by level manager when scene reloads to restart the level. 
	public void ChangeLevel(GameObject newLevelToInstantiate)
	{
		isLevelAssignedByLevelManager = true;

		levelToInstantiate = newLevelToInstantiate;

		if (objectPlacementController && !computerTestLevel)
		{
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
			playerHp = levelManager.playerMotor.GetComponent<HitPoint>();
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
			var enemyHpUi = Instantiate(enemyHitPointUi, canvas.transform);
			enemyHpUi.GetComponent<UI_HealthDisplay_Enemy>().Init(ballMotor.gameObject);
			enemyHpDisplays.Add(enemyHpUi);

			var ballHp = ballMotor.GetComponent<HitPoint>();
			enemyHitPoints.Add(ballHp);
		}

		// UI animation
		var ui = FindObjectOfType<UI>();
		ui.hudAnimator.SetBool("isPlayMode", true);
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

		// UI animation
		var ui = FindObjectOfType<UI>();
		ui.hudAnimator.SetBool("isPlayMode", state);
	}

	// TODO make function to funk with the scale of AR Session Origin
	public void ChangeContentScale()
	{
		var level = FindObjectOfType<Level>();
		if (!arRootObject || !arSessionOrigin)
		{
			Debug.LogWarning("No ARRootObject Assigned");
		}
		else if (!level)
		{
			Debug.LogWarning("No level in the scene");
		}
		else
		{
			var levelObject = level.transform;
			arSessionOrigin.MakeContentAppearAt(
				levelObject, levelObject.transform.position);
			arRootObject.transform.localScale = new Vector3(contentScale, contentScale, contentScale);
		}
	}

	// returns true if all enemies are destroyed
	public bool IsAllEnemyDestroyed()
	{
		if (!isLevelInitialized)
			return false;

		foreach (var enemyHp in enemyHitPoints)
		{
			if (!enemyHp.IsDead())
				return false;
		}
		return true;
	}
	public bool IsPlayerDestroyed()
	{
		if (!isLevelInitialized)
			return false;

		return playerHp.GetComponent<HitPoint>().IsDead();
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
