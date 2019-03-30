using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]

//class to store variables and data that will be sent to firebase with PostToDatabase() function call
public class MoveInfo
{
    //add more data that is going to be stored on database here
    public String moveInfo;
    public bool verified = false;


    public MoveInfo()
    {
        moveInfo = CombatView.GameFlowController.moveInformation;
        verified = false;
    }
}
