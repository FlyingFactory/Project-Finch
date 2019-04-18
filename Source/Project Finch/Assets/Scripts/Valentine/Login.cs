using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Login : MonoBehaviour
{

    public InputField userName;
    public InputField password;

    public void onSignIn()
    {
        Debug.Log("signing in..");
        string user_name = userName.text;
        string pass_word = password.text;

        StartCoroutine(login(user_name, pass_word));
    }

    public IEnumerator login(string user, string password)
    {
        bool waitingForLogin = true;
        MenuView.PlayerAccount.LoginInfo loginInfo = new MenuView.PlayerAccount.LoginInfo(user,password);

        MenuView.PlayerAccount.LoginAndLoadAllData_Thread(loginInfo);

        while (waitingForLogin) {
            if (MenuView.PlayerAccount.currentPlayer != null
                && MenuView.PlayerAccount.currentPlayer.dataLoaded)
            {
                waitingForLogin = false;
            }
            yield return new WaitForSeconds(0.25f);
        } 

        Debug.Log("asdasdasdnumOfSoldiers: " + MenuView.PlayerAccount.currentPlayer.numberOfSoldiers);
        yield return null;
    }


}
