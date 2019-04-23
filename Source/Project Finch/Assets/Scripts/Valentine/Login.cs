using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Linq;

public class Login : MonoBehaviour
{

    public InputField userName;
    public InputField password;
    public void onSignIn()
    {
        string user_name = userName.text;
        string pass_word = password.text;
        Runner_call.Coroutines.Add(login(user_name, pass_word));
    }
    public IEnumerator login(string user, string password)
    {
        gameObject.SetActive(false);
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
        SceneManager.LoadSceneAsync("Main Menu");
        yield return null;
    }
}
