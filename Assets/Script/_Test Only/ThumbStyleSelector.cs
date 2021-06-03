using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ThumbStyleSelector : MonoBehaviour
{
    public Image thumbImg;
    public Image baseImg;

    public Sprite s1Thumb;
    public Sprite s1Base;
    public Sprite s2Thumb;
    public Sprite s2Base;
    public Sprite s3Thumb;
    public Sprite s3Base;

    public Color transpColor;

    public void ChangeStyle(int n)
	{
        if (n == 1)
		{
            thumbImg.sprite = s1Thumb;
            baseImg.sprite = s1Base;
            baseImg.color = (s1Base == null) ? transpColor : Color.white;
        }
        else if (n == 2)
		{
            thumbImg.sprite = s2Thumb;
            baseImg.sprite = s2Base;
            baseImg.color = (s2Base == null) ? transpColor : Color.white;
        }
        else if (n == 3)
		{
            thumbImg.sprite = s3Thumb;
            baseImg.sprite = s3Base;
            baseImg.color = (s3Base == null) ? transpColor : Color.white;
        }
	}
}
