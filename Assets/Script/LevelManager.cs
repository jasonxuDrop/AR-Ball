using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
	static LevelManager instance;

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

		ui = FindObjectOfType<UI>();
		gameSystemManager = FindObjectOfType<GameSystemManager>();
		// assign the level to the GameSystemManager 
		AssignCurentLevel();
	}

	private void Update()
	{
		if (gameSystemManager.IsAllEnemyDestroyed())
		{
			// TODO prompt end level UI
			Debug.Log("all enemy destroyed, showing the End screen UI");
			ui.winScreenAnimator.SetTrigger("In");
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
		Debug.Log("LoadNextLevel");
		instance.currentLevel++;
		SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
	}

	#endregion

}
