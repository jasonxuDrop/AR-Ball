using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// enemy action parent class
/// All EAct class should drive from this class
/// Action perform in Action()
/// done is marked true when action finishes
public class EnemyAction : MonoBehaviour
{
    public bool done = true;
    public virtual void Action() {
        done = false;
    }

    // OVERRIDE CODE
    //public override void Action()
    //{
    //    base.Action();
    //     // Action
    //    MarkDone();
    //}


    // ANIMATOR CALLBACK
    public void MarkDone ()
	{
        done = true;
	}
}
