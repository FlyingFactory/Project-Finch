using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TorsoSpriteChange : MonoBehaviour
{
    public Sprite[] s1;
    public Button b1;
    public Button b2;
    public Image CurrentTorso;

    int count = 0;

    void Awake()
    {
        s1 = Resources.LoadAll<Sprite>("Torso_Sprites");
        CurrentTorso.sprite = s1[count];
    }

    public void OnPreviousClick()
    {
        if (count == 0)
        {
            count = s1.Length;
        }

        count--;

        CurrentTorso.sprite = s1[count];
    }

    public void OnNextClick()
    {
        count++;

        if (count == s1.Length)
        {
            count = 0;
        }

        CurrentTorso.sprite = s1[count];
    }
}
