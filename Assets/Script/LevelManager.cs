using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
	static LevelManager instance;

	public string gameSceneName;

	public List<GameObject> levels;
	// TODO: change to using addressables so not all levels are loaded at once. 
	public int currentLevel;

	UI ui;
	GameSystemManager gameSystemManager;


	// does not get called again when scene reloads because object is not destroyed
	private void Awake()
	{
		// makes sure that there is always only 1 level manager in the scene
		if (instance == null)
		{
			DontDestroyOnLoad(gameObject);
			instance = this;
			SceneManager.sceneLoaded += OnSceneLoaded;
		}
		else if (instance != this)
		{
			DestroyImmediate(gameObject);
			return;
		}

		Reset(); 
	}

	void Reset()
	{
		Debug.Log("Level Manager has reset");

		gameSystemManager = FindObjectOfType<GameSystemManager>();
		if (gameSystemManager)
		{
			ui = FindObjectOfType<UI>();
			// assign the level to the GameSystemManager 
			AssignCurentLevel();
		}
		else
		{
			Debug.LogWarning("not a game scene / GameSystemManager does not exist");
		}
		
	}

	private void Update()
	{
		if (!gameSystemManager)
			return; 

		if (gameSystemManager.IsAllEnemyDestroyed())
		{
			// TODO prompt end level UI
			Debug.Log("all enemy destroyed, showing the End screen UI");
			ui.winScreenAnimator.SetTrigger("In");
		}
		else if (gameSystemManager.IsPlayerDestroyed())
		{
			// TODO prompt end level UI
			Debug.Log("all enemy destroyed, showing the End screen UI");
			// ui.deathScreenAnimator.SetTrigger("In");
		}
	}

	void AssignCurentLevel()
	{
		if (currentLevel < levels.Count)
		{
			Debug.Log("loading level #" + currentLevel);
			gameSystemManager.ChangeLevel(levels[currentLevel]);
		}
		else
		{
			// TODO load credit scene
		}
	}

	void AssignNextLevel()
	{
		Debug.Log("Assigned next level");
		AssignCurentLevel();
	}

	private void OnSceneLoaded(Scene aScene, LoadSceneMode aMode)
	{
		Reset();
	}

	#region Unity Event References

	// Trigged when the next level button event is pressed. 
	public static void LoadNextLevel()
	{
		Debug.Log("Load Next Level");
		instance.currentLevel++;
		SceneManager.LoadScene(instance.gameSceneName);
	}
	public static void LoadThisLevel()
	{
		Debug.Log("Load This Level");
		SceneManager.LoadScene(instance.gameSceneName);
	}
	public static void LoadLevelN(int n)
	{
		Debug.Log("Load This Level");
		instance.currentLevel = n;
		SceneManager.LoadScene(instance.gameSceneName);
	}


	#endregion
}
