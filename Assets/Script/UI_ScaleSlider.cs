using UnityEngine;
using UnityEngine.UI;

public class UI_ScaleSlider : MonoBehaviour
{
    GameSystemManager gameSystemManager;
	Slider slider;

	private void Awake()
	{
		gameSystemManager = FindObjectOfType<GameSystemManager>();
		slider = GetComponent<Slider>();
		slider.value = gameSystemManager.contentScale;
		slider.minValue = gameSystemManager.minScale;
		slider.maxValue = gameSystemManager.maxScale;
	}

	#region Unity Event References

	public void UpdateValue (float val)
	{
		gameSystemManager.contentScale = val;
		gameSystemManager.ChangeContentScale();
	}

	#endregion
}
