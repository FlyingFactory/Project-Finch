using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class FuzzerTester : MonoBehaviour
{
    const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
    const string skill = "am";
    const string numbers = "0123456789";
    const string player = "12";
    const string symbols = ",.!@#$%^&*():'";
    const string hit = "hm";

    public string fuzzertester(int length, string category)
    {
        //moveinfo protocol: "skill, playerid_soldierid, x:z:h, h/m, dmg"
        return new string(Enumerable.Repeat(category, length).Select(s => s[UnityEngine.Random.Range(0,s.Length)]).ToArray());
    }

    public string fuzzertesterxzh(int cap)
    {
        int x = UnityEngine.Random.Range(0, cap);
        return x.ToString();
    }

    public void onFuzzerTest()
    {
        string player1or2 = fuzzertester(1,player);
        string skill_move = fuzzertester(1, skill);
        string playerid_soldierid = "p" + player1or2 + "_" + fuzzertester(2, numbers);
        string x_z_h = fuzzertesterxzh(35)+ ":" + fuzzertesterxzh(35) + ":" + fuzzertesterxzh(35);
        string h_m = fuzzertester(1, hit);
        string dmg = fuzzertester(2, numbers);

        string result = skill_move + "," + playerid_soldierid + "," + x_z_h + "," + h_m + "," + dmg;
        Debug.Log(result);
        //CombatView.GameFlowController.gameFlowController.addMove(x);

    }
}
