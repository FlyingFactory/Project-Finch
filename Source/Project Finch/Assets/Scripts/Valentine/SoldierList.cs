using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SoldierList : MonoBehaviour
{
#pragma warning disable 0649
    [SerializeField] private Text myText;
#pragma warning restore 0649

    public void SetText(string textString)
    {
        myText.text = textString;
    }





}
