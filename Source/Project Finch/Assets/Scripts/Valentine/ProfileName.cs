using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ProfileName : MonoBehaviour
{
    public Text username;


    void Awake()
    {
        username.text = MenuView.PlayerAccount.currentPlayer.userName;
    }

    
}
