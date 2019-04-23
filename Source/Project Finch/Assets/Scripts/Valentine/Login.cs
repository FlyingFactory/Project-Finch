using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Linq;

public class Login : MonoBehaviour
{
    public static List<MenuView.Soldier> opponentSoldiers = new List<MenuView.Soldier>();
    public static List<MenuView.Soldier> mySoldiers = new List<MenuView.Soldier>();
    public static string opponentName;

    public InputField userName;
    public InputField password;

    public static MenuView.PlayerAccount me = null;

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
        me = MenuView.PlayerAccount.currentPlayer;
        SceneManager.LoadSceneAsync("Main Menu");
        yield return null;
    }

    public IEnumerator LoadOpponentData(string userName)
    {
        bool loadingOpponent = true;
        MenuView.PlayerAccount.loadDataAndLoadSoldierInfo opponentData = new MenuView.PlayerAccount.loadDataAndLoadSoldierInfo();
        MenuView.PlayerAccount.LoadDataInfo opponentDataInfo = new MenuView.PlayerAccount.LoadDataInfo(userName);
        opponentData.loadDataInfo = opponentDataInfo;

        MenuView.PlayerAccount.LoadData_Thread(opponentData.loadDataInfo);

        while (loadingOpponent)
        {
            if (opponentData.loadDataInfo.output != null && opponentData.loadDataInfo.output.dataLoaded)
            {
                loadingOpponent = false;
            }
        }
        yield return new WaitForSeconds(0.25f);
        List<string> tempKeys = opponentData.loadDataInfo.output.soldiers.Keys.ToList<string>();
        tempKeys.Sort();
        
        foreach(string soldierName in tempKeys)
        {
            opponentSoldiers.Add(opponentData.loadDataInfo.output.soldiers[soldierName]);
        }
    }

    public void startMatch(string opponentUser)
    {
        List<string> tempKeys = MenuView.PlayerAccount.currentPlayer.soldiers.Keys.ToList<string>();
        tempKeys.Sort();
        foreach (string soldierName in tempKeys)
        {
            mySoldiers.Add(MenuView.PlayerAccount.currentPlayer.soldiers[soldierName]);
        }
        Runner_call.Coroutines.Add(LoadOpponentData(opponentUser));
        for(int i=0; i <4; i++)
        {
            CombatView.MapGenerator.soldiers[i] = mySoldiers[i];
        }
        for (int i=4; i<8; i++)
        {
            CombatView.MapGenerator.soldiers[i] = opponentSoldiers[i-4];
        }
        CombatView.GameFlowController.matchID = MenuView.PlayerAccount.currentPlayer.matchID;
        Runner_call.Coroutines.Add(loadMatchDetails(MenuView.PlayerAccount.currentPlayer.matchID));
        SceneManager.LoadSceneAsync("Merrick");
    }

    public IEnumerator loadMatchDetails(int matchID)
    {
        bool waitingMatch = true;
        MatchDetails _matchDetails = new MatchDetails();
        _matchDetails.matchID = matchID;
        MenuView.PlayerAccount.getMatchDetails_thread(_matchDetails);

        while (waitingMatch)
        {
            if (_matchDetails.complete && _matchDetails != null)
            {
                waitingMatch = false;
            }
            yield return new WaitForSeconds(0.25f);
        }
        

        if (_matchDetails.matchedPlayer1 == MenuView.PlayerAccount.currentPlayer.userName)
        {
            CombatView.GameFlowController.player1 = true;
        }
        else CombatView.GameFlowController.player1 = false;
        Debug.Log(CombatView.GameFlowController.player1);
        SceneManager.LoadSceneAsync("Merrick");
    }

}
