using System.Collections;
using System.Collections.Generic;
using MenuView;
using UnityEngine;
using System;


public class PopulateList : MonoBehaviour
{
#pragma warning disable 0649
    [SerializeField] private GameObject buttonTemplate;
#pragma warning restore 0649

    private string[] imported = {"0","2","5","9","11", "18", "50", "51", "80", "101", "200"};
    //public List<string> imported = new List<string> Login.importedinfo;

    // TODO: Once someone has logged in, import a list of soldier names from Firebase upon a successful login, based on the user's account.
    // This should then be reflected in populateRosterList().

    void populateRosterList()
    {
        try
        {
            for (int i = 0; i < imported.Length; i++)
            {
                GameObject button = Instantiate(buttonTemplate) as GameObject;
                button.SetActive(true);

                button.GetComponent<SoldierList>().SetText("Soldier #" + imported[i]);

                button.transform.SetParent(buttonTemplate.transform.parent, false);

            }
        }

        catch (Exception noSoldiers){
            print("No soldier in the list!" + noSoldiers);
        }
    }

    void Start()
    {
        populateRosterList();
    }


}
