using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
	// TODO random a order of release
	public Vector2 turnOrderRange = new Vector2(1, 100);

	// is currently my turn
	[HideInInspector]
	public bool isTurn = false;


	bool waitForEndTurn = false;
	EnemyAction[] enemyActions;

	private void Start()
	{
		enemyActions = GetComponents<EnemyAction>();
	}

	private void Update()
	{
		if (isTurn && waitForEndTurn)
		{
			bool allDone = true;
			foreach (var ea in enemyActions)
			{
				if (!ea.done)
				{
					allDone = false;
					break;
				}
			}

			if (allDone)
				EndTurn();
		}
	}


	public void StartTurn()
	{
		isTurn = true;
		print(name + " is turn: " + isTurn);

		foreach (var ea in enemyActions)
		{
			ea.Action();
		}
		waitForEndTurn = true;
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
		waitForEndTurn = false;

		print(name + " is turn: " + isTurn);
	}
}
