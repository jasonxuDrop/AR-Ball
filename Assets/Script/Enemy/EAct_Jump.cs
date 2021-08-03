using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EAct_Jump : EnemyAction
{
	private void Start()
	{
		Action();
	}

	public override void Action()
	{
		print(name + "doing Action");

		base.Action();

		Rigidbody rb = GetComponent<Rigidbody>();
		rb.AddForce(Vector3.up * .02f, ForceMode.Impulse);

		StartCoroutine("DisableIn2Sec");
	}


	IEnumerator DisableIn2Sec ()
	{
		yield return new WaitForSecondsRealtime(1.2f);
		MarkDone();
	}
}
