using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ListControl : MonoBehaviour
{
#pragma warning disable 0649
    [SerializeField] private GameObject buttonTemplate;
    [SerializeField] private GameObject buttonParent;
#pragma warning restore 0649
    public int numberOfSoldiers;
    public static ListControl listControl = null;

    private void Awake()
    {
        if (listControl == null)
        {
            listControl = this;
        }
        else Destroy(this);
        
    }

    void Start()
    {
        buttonTemplate = Resources.Load<GameObject>("Prefabs/soldierDetailsButton");

        foreach (KeyValuePair<string, MenuView.Soldier> soldier in MenuView.PlayerAccount.currentPlayer.soldiers)
        {
            GameObject button = Instantiate(buttonTemplate, buttonParent.transform) as GameObject;
            button.SetActive(true);
            button.GetComponent<SoldierList>().SetText(soldier.Value.name);
            button.GetComponent<SoldierList>().SetSoldierId(soldier.Key);
        }
    }

    public void SoldierDetails(string soldier_id)
    {
        MenuView.Soldier new_soldier = MenuView.PlayerAccount.currentPlayer.soldiers[soldier_id];
    }


}
