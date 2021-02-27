using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PlayerController : MonoBehaviour
{
    [SerializeField]
    FixedJoystick joystick;
	public OnScreenTextDebugger debugText;

	private void Start() {
		if (!debugText) {
			debugText = FindObjectOfType<OnScreenTextDebugger>();
		}
	}

	private void Update() {
		DebugWithText();
	}

	void DebugWithText() {
		debugText.Queue("x: " + joystick.Vertical + ",  y: " + joystick.Horizontal);
	}
}
