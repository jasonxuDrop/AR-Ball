using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleTurnManager : MonoBehaviour
{
    //TODO wait for level load and get enemies and player from there. 
    public PlayerController player;
    public List<Enemy> enemies = new List<Enemy>();


    // detremines who's turn it is
    // 0: player | n: enemy #n
    [SerializeField] int turnIndex = -1;

    bool waitForTurnEnd = false;


	private void Start()
	{
        NextTurn();
    }

	private void Update()
	{
        // player's turn
        if (turnIndex == 0)
        {
            //player ...
            if (!player.isTurn)
            {
                waitForTurnEnd = false;
            }
        }

        // enemy's turn
        else
        {
            var e = enemies[turnIndex - 1];
            if (!e.isTurn)
			{
                waitForTurnEnd = false;
            }
        }

        // Start Next Turn
        // TODO Test if shit is ready
        if (!waitForTurnEnd)
		{
            NextTurn();
        }
    }

    public void NextTurn()
	{
        // overflow the turn index back to 0 (player
        turnIndex++;
        if (turnIndex > enemies.Count)
		{
            turnIndex = 0;
		}

        // player's turn
        if(turnIndex == 0)
		{
            //player ...
            player.StartTurn();
            waitForTurnEnd = true;
        }

        // enemy's turn
        else
		{
            enemies[turnIndex - 1].StartTurn();
            waitForTurnEnd = true;
        }
	}
}
