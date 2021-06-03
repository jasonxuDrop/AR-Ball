using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_ForceIndicator : MonoBehaviour
{
    public List<Sprite> frames;

    SpriteRenderer sr;
    Transform playerToFollow;
    
    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
    }

	private void Update()
	{

        // follow player object
		if (!playerToFollow)
		{
            playerToFollow = FindObjectOfType<PlayerMotor>().transform;
        }
        else
		{
            var pos = playerToFollow.position;
            pos.y -= 0.035f;
            transform.position = pos;
        }
	}


	public void ChangeForceDisplay(float m)
	{
        m = m * -1 + 1; // invert 01
        int frame = Mathf.CeilToInt(m * frames.Count);
        frame = Mathf.Clamp(frame, 1, frames.Count);
        frame --;

        sr.sprite = frames[frame];
    }
}
