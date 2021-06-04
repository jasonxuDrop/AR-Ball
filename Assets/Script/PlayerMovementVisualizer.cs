using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovementVisualizer : MonoBehaviour
{
	public int recursionCount = 20;
	public float stepSize = 1;
	public float yOffsetFromGround = 0.0358f;	// player scale / 2
	public float yIncline = 0.07f;				// how much to go up for while top to bottom projection
	public float maxYDropDist = 0.1f;           // max distance to drop to register
	// TODO: make incline and drop as percent to step size

	[Header("Visualization")]
	public float gizmoSphereRadius = 0.0358f;

	List<Vector3> points = new List<Vector3>();


	public void Visualize(Vector3 playerPosition, Vector3 moveForce)
	{
		points.Clear();

		// create a layer mask of everything but ignore layer 8 (player)
		// TODO: optimized so that the level colls are in a enviorment layer so we don't have to worry about every other layer. 
		int layerMask = 1 << 8; // shift bit #8 to 1 ???
		layerMask = ~layerMask; // invert all bits


		// TODO: if a straight line is clear, don't do the resrusion. 

		// resrusive path finding
		CastRay(playerPosition, moveForce, layerMask, recursionCount);


		//RaycastHit hit;

		//if (Physics.Raycast(playerPosition, moveForce, out hit, stepSize, layerMask))
		//{
		//	Debug.DrawRay(playerPosition, moveForce * hit.distance, Color.yellow);
		//}
		//else
		//{
		//	Debug.DrawRay(playerPosition, moveForce * stepSize, Color.white);
		//}
	}

	public void Clear()
	{
		points.Clear();
	}

	// a recursive raycast
		// cast ray 
		// if hit something, test if it is a wall
		//   yes: draw a circle and return
		// draw a ray from higher up and calc the position for the next ray orign 
		// cast again.

	private void CastRay(Vector3 pos, Vector3 posDelta, int layerMask, int remainingRecursion)
	{
		// return value
		Vector3 finalPos;

		// terminator
		if (remainingRecursion <= 0)
			return;

		// save current position (including first iteration, excluding last)
		points.Add(pos);

		RaycastHit hit;
		if (Physics.Raycast(pos, posDelta, out hit, stepSize, layerMask))
		{
			// TODO: mark all vertical things as wall
			if (hit.collider.tag == "Wall")
			{
				// yes: draw a circle and return
				Debug.DrawRay(pos, posDelta * hit.distance, Color.red);
				return;
			}
		}

		// top to bottom projection
		Vector3 projectionStartPos = pos + (posDelta * stepSize);
		projectionStartPos.y += yIncline;
		RaycastHit hit2;
		if (Physics.Raycast(projectionStartPos, Vector3.down, out hit2, maxYDropDist + yIncline, layerMask))
		{
			finalPos = projectionStartPos + Vector3.down * (hit2.distance - yOffsetFromGround);
			Debug.DrawRay(projectionStartPos, Vector3.down * hit2.distance, Color.yellow);

			//Debug.DrawRay(firstObjPos, secondObjPos - firstObjPos)
			Debug.DrawRay(pos, finalPos - pos, Color.blue);


			// call the recursion again
			CastRay(finalPos, posDelta, layerMask, --remainingRecursion);
		}
		else
			return;
	}

	private void OnDrawGizmos()
	{
		foreach(var point in points) {
			Gizmos.DrawSphere(point, gizmoSphereRadius);
		}
		
	}

	private void DisplayMarker()
	{
		// give me a pos and normal and I'll activate the marker obj and move it there.  
	}
}
