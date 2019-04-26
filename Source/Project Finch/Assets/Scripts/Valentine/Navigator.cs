using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Proyecto26;
using System.Threading;
using System.Linq;

public class Navigator : MonoBehaviour
{
    public static List<MenuView.Soldier> opponentSoldiers = new List<MenuView.Soldier>();
    public static List<MenuView.Soldier> mySoldiers = new List<MenuView.Soldier>();
    public static string opponentName;

    public async void SearchForGame()
    {
        if (!MenuView.PlayerAccount.currentPlayer.InMatch)
        {
            UserQueue new_queue = new UserQueue(MenuView.PlayerAccount.currentPlayer.userName);
            RestClient.Put("https://project-finch-database.firebaseio.com/queuingForMatch/" + MenuView.PlayerAccount.currentPlayer.userName + ".json", new_queue);

        }

        while (!MenuView.PlayerAccount.currentPlayer.InMatch)
        {
            MenuView.PlayerAccount.loadDataAndLoadSoldierInfo new_instance1 = new MenuView.PlayerAccount.loadDataAndLoadSoldierInfo();
            new_instance1.loadDataInfo.output = MenuView.PlayerAccount.currentPlayer;
            new_instance1.loadDataInfo.userID = MenuView.PlayerAccount.currentPlayer.userName;

            Thread loadDataThread = new Thread(new ParameterizedThreadStart(MenuView.PlayerAccount.LoadData_Thread));
            loadDataThread.Start(new_instance1);

            System.Threading.CancellationToken cancel4 = new CancellationToken();
            for (int i = 0; i < 10; i++)
            {
                if (new_instance1.complete) break;

                await System.Threading.Tasks.Task.Delay(1000, cancel4);
                if (cancel4.IsCancellationRequested) break;
            };

            if (new_instance1.loadDataInfo.output == null)
            {
                Debug.Log("Load data unsuccessful");
            }
            else
            {  
                MenuView.PlayerAccount.currentPlayer = new_instance1.loadDataInfo.output;
            }
        }
        Debug.Log("found Game!, MatchID: "+ MenuView.PlayerAccount.currentPlayer.matchID);

        MatchDetails found_match = new MatchDetails();
        found_match.matchID = MenuView.PlayerAccount.currentPlayer.matchID;
        Thread loadMatchDetailThread = new Thread(new ParameterizedThreadStart(MenuView.PlayerAccount.getMatchDetails_thread));
        loadMatchDetailThread.Start(found_match);
        System.Threading.CancellationToken cancel3 = new CancellationToken();
        for (int i = 0; i < 10; i++)
        {
            if (found_match.complete) break;

            await System.Threading.Tasks.Task.Delay(1000, cancel3);
            if (cancel3.IsCancellationRequested) break;
        };
        if (found_match.matchedPlayer1 != MenuView.PlayerAccount.currentPlayer.userName)
        {
            startMatch(found_match.matchedPlayer1);
        }
        else
        {
            startMatch(found_match.matchedPlayer2);
        }
    }

    public IEnumerator LoadOpponentData(string userName)
    {
        bool loadingOpponent = true;
        MenuView.PlayerAccount.loadDataAndLoadSoldierInfo opponentData = new MenuView.PlayerAccount.loadDataAndLoadSoldierInfo();
        MenuView.PlayerAccount.LoadDataInfo opponentDataInfo = new MenuView.PlayerAccount.LoadDataInfo(userName);
        opponentData.loadDataInfo = opponentDataInfo;
        Thread loadOpponentDataThread = new Thread(new ParameterizedThreadStart( MenuView.PlayerAccount.LoadData_Thread));
        loadOpponentDataThread.Start(opponentData);
        Debug.Log("Loading Opponent data...");
        while (loadingOpponent)
        {
            Debug.Log("opponent data loaded?" + opponentData.complete);
            if (opponentData.loadDataInfo.output != null && opponentData.complete == true)
            {
                Debug.Log("soldiers?" + opponentData.loadDataInfo.output.soldiers);
                loadingOpponent = false;
            }
            yield return new WaitForSeconds(0.25f);
        }

        Debug.Log("opp soldiers:" +opponentData.loadDataInfo.output.soldiers.Count);
        List<string> tempKeys = opponentData.loadDataInfo.output.soldiers.Keys.ToList<string>();
        tempKeys.Sort();

        foreach (string soldierName in tempKeys)
        {
            
            opponentSoldiers.Add(opponentData.loadDataInfo.output.soldiers[soldierName]);
        }
        CombatView.MapGenerator.soldiers.Clear();
        for (int i = 0; i < 4; i++)
        {
            if (mySoldiers.Count > i) CombatView.MapGenerator.soldiers.Add(mySoldiers[i]);
        }
        for (int i = 0; i < 4; i++)
        {
            Debug.Log("number of opponent soldiers:" + opponentSoldiers.Count);
            if (opponentSoldiers.Count > i) CombatView.MapGenerator.soldiers.Add(opponentSoldiers[i]);
        }
        Debug.Log("total number of soldiers:" + CombatView.MapGenerator.soldiers.Count);
        CombatView.GameFlowController.matchID = MenuView.PlayerAccount.currentPlayer.matchID;
        Runner_call.Coroutines.Add(loadMatchDetails(MenuView.PlayerAccount.currentPlayer.matchID));
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
        CombatView.MapGenerator.mapSeed = _matchDetails.mapSeed;
        Debug.Log(CombatView.GameFlowController.player1);
        SceneManager.LoadSceneAsync("Merrick");
        MenuView.PlayerAccount.currentPlayer.dataLoaded = false;
    }

    public void GoToRoster()
    {
        SceneManager.LoadScene("Roster");
    }

    public void RostertoMenu()
    {
        SceneManager.LoadScene("Main Menu");
    }

    public void Logout()
    {
        SceneManager.LoadSceneAsync("LoginMenu");
    }

    public void QuitGame()
    {
        Debug.Log("Game Quit!");
        Application.Quit();
        SceneManager.LoadScene("Game Quit"); // Might need to remove this after it's done.
    }

    public void LoadScene(string name)
    {
        // Use a coroutine to load the Scene in the background
        StartCoroutine(LoadAsyncScene(name));
    }

    IEnumerator LoadAsyncScene(string name)
    {
        // The Application loads the Scene in the background as the current Scene runs.
        // This is particularly good for creating loading screens.
        // You could also load the Scene by using sceneBuildIndex. In this case Scene2 has
        // a sceneBuildIndex of 1 as shown in Build Settings.

        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(name);

        // Wait until the asynchronous scene fully loads
        while (!asyncLoad.isDone)
        {
            yield return null;
        }
    }


    public void GoToStore()
    {
        Debug.Log("Going from Stats to Store...");
        SceneManager.LoadScene("Store");
    }

    public void BacktoStats()
    {
        Debug.Log("Going back to Stats...");
        SceneManager.LoadScene("Roster");
    }

}
