using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class updateRoster : MonoBehaviour
{
    public int numberOfSoldiers;
    public List<MenuView.Soldier> soldiers;
    // Start is called before the first frame update
    void Start()
    {
        numberOfSoldiers = MenuView.PlayerAccount.currentPlayer.numberOfSoldiers;
        soldiers = MenuView.PlayerAccount.currentPlayer.soldiers;
        Debug.Log(MenuView.PlayerAccount.currentPlayer.soldierNameList);
        Debug.Log(soldiers.Count);
        foreach ( MenuView.Soldier soldier in soldiers)
        {
            Debug.Log(soldier);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
