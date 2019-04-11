using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HeadSpriteChange : MonoBehaviour
{
    public Sprite[] s1;
    public Button b1;
    public Button b2;
    public Image CurrentHead;

    int count = 0;

    void Awake()
    {
        s1 = Resources.LoadAll<Sprite>("Head_Sprites");
        CurrentHead.sprite = s1[count];
    }

    public void OnPreviousClick()
    {
        if (count == 0)
        {
            count = s1.Length;
        }

        count--;

        CurrentHead.sprite = s1[count];
    }

    public void OnNextClick()
    {
        count++;

        if (count == s1.Length)
        {
            count = 0;
        }

        CurrentHead.sprite = s1[count];
    }
}
