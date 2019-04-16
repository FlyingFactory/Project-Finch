using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SoldierData :MonoBehaviour

{
    public Text title;
    public Text ID;
    public Text Level;
    public Text Experience;
    public Text Bio;
    public Text HP;
    public Text Fatigue;
    public Text Mobility;
    public Text Aim;

    public string[] imported;
    // This time, it's for that particular soldier.
    // I am to assume that these strings of data are in array form.

    
    // void onClick(int soldierID)
    // The code below should only work when the name is clicked on the previous screen.
    // For testing purposes, Awake() is used.

    void Awake()
    {
        // imported = Resources.LoadAll<Text>("...");
        // To implement an import from the server. For now, use a local array.
        // On click, match with the Soldier's unique ID, and pull their array of results.

        imported = new string[] { "Catherine", "10005", "1","500","Your typical soldier. Will get job done.", "200","10","5","3" };

        title.text = imported[0];
        ID.text = imported[1];
        Level.text = imported[2];
        Experience.text = imported[3];
        Bio.text = imported[4];
        HP.text = imported[5];
        Fatigue.text = imported[6];
        Mobility.text = imported[7];
        Aim.text = imported[8];

    }



}
