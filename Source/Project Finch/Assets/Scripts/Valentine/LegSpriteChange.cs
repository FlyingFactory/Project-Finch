using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LegSpriteChange : MonoBehaviour
{
    public Sprite[] s1;
    public Button b1;
    public Button b2;
    public Image CurrentLeg;

    int count = 0;

    void Awake()
    {
        s1 = Resources.LoadAll<Sprite>("Leg_Sprites");
        CurrentLeg.sprite = s1[count];
    }

    public void OnPreviousClick()
    {
        if (count == 0)
        {
            count = s1.Length;
        }

        count--;

        CurrentLeg.sprite = s1[count];
    }

    public void OnNextClick()
    {
        count++;

        if (count == s1.Length)
        {
            count = 0;
        }

        CurrentLeg.sprite = s1[count];
    }

}
