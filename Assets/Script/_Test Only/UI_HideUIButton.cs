using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_HideUIButton : MonoBehaviour
{
	public List<GameObject> objectsToHide;
	bool isHidden = false;
    public void HideUIToggle()
	{
		objectsToHide.ForEach(x =>
		{
			x.SetActive(isHidden);
		});
		isHidden = !isHidden;
	}
}
