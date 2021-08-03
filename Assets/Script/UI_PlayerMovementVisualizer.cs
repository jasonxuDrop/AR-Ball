using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_PlayerMovementVisualizer : MonoBehaviour
{
	public MeshFilter directionLineMeshFilter;
	public Transform hitIndicator;
	public int recursionCount = 20;
	public float rayStep = 0.0358f;                 // player scale / 2 ? 
	public float yOffsetFromGround = 0.0358f;		// player scale / 2
	public float yInclinePercent = 1;				// how much to go up for while top to bottom projection
	public float yDropPercent = 1;                  // max distance to drop to register
	public float lineWidth = 0.0358f;               // full width of vis line: player scale / 2
	public float wallAngleThreshold = 0.35f;		// how steep (0-1) until a forward hit is marked as wall

	[Header("Visualization")]
	public float gizmoSphereRadius = 0.0358f;

	MeshRenderer meshRenderer;
	SpriteRenderer hitSpriteRenderer;
	bool showOnThisFrame;
	float yIncline;
	float yDrop;
	Vector3 solidifyOffset;							// how far to move LEFT from the current point for a vert
	List<Vector3> vertices = new List<Vector3>();
	List<int> triangles = new List<int>();

	private void OnValidate()
	{
		yIncline = rayStep * yInclinePercent;
		yDrop = rayStep * yDropPercent;
	}

	private void Start()
	{
		if (directionLineMeshFilter.mesh == null)
			directionLineMeshFilter.mesh = new Mesh();
		meshRenderer = directionLineMeshFilter.GetComponent<MeshRenderer>();

		hitSpriteRenderer = hitIndicator.GetComponentInChildren<SpriteRenderer>();
	}

	public void Visualize(Vector3 playerPosition, Vector3 moveForce)
	{
		meshRenderer.enabled = true;
		hitSpriteRenderer.enabled = false;

		vertices.Clear();
		triangles.Clear();

		// create a layer mask of everything but ignore layer 8 (player)
		// TODO: optimized so that the level colls are in a enviorment layer so we don't have to worry about every other layer. 
		int layerMask = 1 << 8; // shift bit #8 to 1 ???
		layerMask = ~layerMask; // invert all bits

		// one time calculation
		solidifyOffset = Quaternion.Euler(0, -90, 0) * moveForce * 0.5f * lineWidth;

		// resrusive path finding
		// TODO: if a straight line is clear, don't do the resrusion. 
		CastRay(playerPosition, moveForce, layerMask, recursionCount);


		UpdateMesh();
	}

	public void Clear()
	{
		meshRenderer.enabled = false;
		hitSpriteRenderer.enabled = false;

		vertices.Clear();
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
		MakeVerts(pos);

		RaycastHit hit;
		if (Physics.Raycast(pos, posDelta, out hit, rayStep, layerMask))
		{
			if(Mathf.Abs(hit.normal.y) < wallAngleThreshold)
			//if (hit.collider.tag == "Wall")
			{
				// yes: draw a circle and return
				Debug.DrawRay(pos, posDelta * hit.distance, Color.red);
				DisplayMarker(hit.point, hit.normal);
				return;
			}
		}

		// top to bottom projection
		Vector3 projectionStartPos = pos + (posDelta * rayStep);
		projectionStartPos.y += yIncline;
		RaycastHit hit2;
		if (Physics.Raycast(projectionStartPos, Vector3.down, out hit2, yDrop + yIncline + yOffsetFromGround, layerMask))
		{
			finalPos = projectionStartPos + Vector3.down * (hit2.distance - yOffsetFromGround);
			// VIS projection drom top down
			Debug.DrawRay(projectionStartPos, Vector3.down * hit2.distance, Color.yellow);
			// VIS Connect the dots
			Debug.DrawRay(pos, finalPos - pos, Color.blue);

			// call the recursion again
			CastRay(finalPos, posDelta, layerMask, --remainingRecursion);
		}
		else
		{
			// VIS projection drom top down
			Debug.DrawRay(projectionStartPos, Vector3.down * (yDrop + yIncline + yOffsetFromGround), Color.red);
			return;
		}

	}

	
	private void MakeVerts(Vector3 pos)
	{
		// verts
		Vector3 posL = pos + solidifyOffset;
		Vector3 posR = pos - solidifyOffset;
		vertices.Add(posL);
		vertices.Add(posR);

		//tris
		int c = vertices.Count;
		if (c >= 4)
		{
			triangles.AddRange(new int[] {
				c-4, c-2, c-3, //0, 2, 1
				c-3, c-2, c-1, //1, 2, 3
			});
		}
	}

	private void UpdateMesh()
	{
		var m = directionLineMeshFilter.mesh;
		m.Clear();

		m.vertices = vertices.ToArray();
		m.triangles = triangles.ToArray();

		// uv
		if (vertices.Count >= 4)
		{
			Vector2[] uvs = new Vector2[vertices.Count];
			float v = 0;
			float vIncrement = 0.5f / ((float)vertices.Count / 2 - 1);
			for (int i = 0; i < vertices.Count; i++)
			{
				if (i >= vertices.Count - 2)
					v = 1;
				if (i % 2 == 0)
				{
					uvs[i] = new Vector2(0, v);
				}
				else
				{
					uvs[i] = new Vector2(0.5f, v);
					v += vIncrement;
				}
			}

			m.uv = uvs;
			//Debug.Log(uvs.Length + "|||||" + vertices.Count);
		}


		int biggestTriIdx = 0;
		foreach (int i in triangles)
		{
			if (i > biggestTriIdx)
				biggestTriIdx = i;
		}
		//print("vert length: " + m.vertices.Length + " | tri max index: " + biggestTriIdx);

		directionLineMeshFilter.mesh = m;
	}


	private void DisplayMarker(Vector3 pos, Vector3 normal)
	{
		//print(normal);
		hitSpriteRenderer.enabled = true;

		hitIndicator.position = pos;
		hitIndicator.rotation = Quaternion.FromToRotation(transform.forward, normal) * transform.rotation;
		// give me a pos and normal and I'll activate the marker obj and move it there.  
	}

	private void OnDrawGizmos()
	{
		foreach(var point in vertices) {
			Gizmos.DrawSphere(point, gizmoSphereRadius);
		}
	}
}
