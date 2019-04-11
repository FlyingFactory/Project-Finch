using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]

//class to store variables and data that will be sent to firebase with PostToDatabase() function call
public class UserQueue
{
    //add more data that is going to be stored on database here
    public string UserID;

    public UserQueue(string userId)
    {
        this.UserID = userId;
    }
}
