using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class userDetails : MonoBehaviour
{
    public static GameObject StatsGroup = null;
    [SerializeField] private GameObject StatsGroupObject = null;

    public static Text currency = null;
    [SerializeField] private Text currencyObject = null;

    public static Text userName = null;
    [SerializeField] private Text userNameObject = null;


    private void Awake()
    {
        StatsGroup = StatsGroupObject;
        currency = currencyObject;
        userName = userNameObject;
        Destroy(this);

        GameObject statsGroup = userDetails.StatsGroup;
        userDetails.currency.text = MenuView.PlayerAccount.currentPlayer.currency.ToString();
        userDetails.userName.text = MenuView.PlayerAccount.currentPlayer.userName.ToString();
    }
}
