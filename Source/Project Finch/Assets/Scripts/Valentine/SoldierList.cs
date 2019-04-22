using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SoldierList : MonoBehaviour
{
#pragma warning disable 0649
    [SerializeField] private Text myText;
    
#pragma warning restore 0649
    [System.NonSerialized] public string soldier_dict_id = "";

    public void SetText(string textString)
    {
        myText.text = textString;
    }

    public void SetSoldierId(string soldier_id)
    {
        soldier_dict_id = soldier_id;
    }

    public void onSoldierDetails()
    {
        ListControl.listControl.SoldierDetails(soldier_dict_id);
        transform.parent.parent.parent.parent.parent.gameObject.SetActive(false);
        GameObject statsGroup = MenuView.RosterSceneStaticRefs.StatsGroup;
        statsGroup.SetActive(true);
        MenuView.Soldier this_soldier = MenuView.PlayerAccount.currentPlayer.soldiers[soldier_dict_id];

        //CODE SMELL PREVENTED
        //MenuView.RosterSceneStaticRefs.StatsGroup.transform.Find("Stats").Find("AimLabel").Find("Aim").GetComponent<Text>().text = this_soldier.aim.ToString();

        MenuView.RosterSceneStaticRefs.Aim.text = this_soldier.aim.ToString();
        MenuView.RosterSceneStaticRefs.Level.text = this_soldier.level.ToString();
        MenuView.RosterSceneStaticRefs.Experience.text = this_soldier.experience.ToString();
        MenuView.RosterSceneStaticRefs.Fatigue.text = this_soldier.fatigue.ToString();
        MenuView.RosterSceneStaticRefs.HP.text = this_soldier.maxHealth.ToString();
        MenuView.RosterSceneStaticRefs.Mobility.text = this_soldier.mobility.ToString();
        
    }
}
