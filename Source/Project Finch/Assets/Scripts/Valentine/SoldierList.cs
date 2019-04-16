using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SoldierList : MonoBehaviour
{
    public string[] imported;
    
    public Text SoldierName1 = null;
    public Text SoldierName2 = null;
    public Text SoldierName3 = null;
    public Text SoldierName4 = null;
    public Text SoldierName5 = null;

    void Awake()
    {
        // imported = Resources.LoadAll<Text>("...");
        // To implement an import from the server. For now, use a local array.
        // Possible to get an array of soldier IDs, then extract their names?

        imported = new string[] { "Catherine", "Markus", "Ryan", "Clarissa", "Janet" };

        SoldierName1.text = imported[0];
        SoldierName2.text = imported[1];
        SoldierName3.text = imported[2];
        SoldierName4.text = imported[3];
        SoldierName5.text = imported[4];

    }




}
