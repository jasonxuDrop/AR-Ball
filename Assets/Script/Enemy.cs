using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
	// TODO random a order of release
	public Vector2 turnOrderRange = new Vector2(1, 100);

	//[HideInInspector]
	public bool isTurn = false;
	// is currently my turn

    public void StartTurn()
	{
		isTurn = true;
		print(name + " is turn: " + isTurn);

		StartCoroutine(SudoTurnAction());
	}


	IEnumerator SudoTurnAction ()
	{
		yield return new WaitForSecondsRealtime(2);
		EndTurn();
	}


	public void EndTurn()
	{
		// TODO add modifiers

		isTurn = false;
		print(name + " is turn: " + isTurn);
	}
}
