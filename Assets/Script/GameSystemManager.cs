using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameSystemManager : MonoBehaviour
{
	public ARObjectPlacementController objectPlacementController;

    public GameObject levelToInstantiate;

	[Header("Debug")]
	public OnScreenTextDebugger debugText;

	private void Start() {
		if (!debugText) {
			debugText = FindObjectOfType<OnScreenTextDebugger>();
		}
	}


	void PlaceLevel() {
		bool res = objectPlacementController.PlaceObjectScreenCenter(levelToInstantiate);
		debugText.Queue("button pressed | level placed: " + res);
	}


	#region Unity Event References

	public void PlaceLevelButtonPressed() {
		PlaceLevel();
		Debug.Log("hi");
	}

	#endregion
}
