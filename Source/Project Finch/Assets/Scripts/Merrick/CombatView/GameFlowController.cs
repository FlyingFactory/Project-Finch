using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Proyecto26;
using UnityEditor;
using Firebase;
using Firebase.Database;
using System.Runtime.Serialization.Json;
using System.IO;
using System.Text;

namespace CombatView
{

    public class GameFlowController : MonoBehaviour
    {

        public enum TurnState
        {
            Entry,
            PlayerInput,
            PlayerAction,
            Wrapup,
            EnemyTurn,
        }

        public static GameFlowController gameFlowController;
        [System.NonSerialized] public TurnState turnState = TurnState.Entry;
        [System.NonSerialized] public bool waitingForServer = false;
        [System.NonSerialized] public int currentPlayer = 0;
        [System.NonSerialized] public int numPlayers = 2;
        public int matchID;
        private bool player1;
        public  string moveInformation = null;
        public  string NCmoveInformation = null;
        public  int moveNumber = 0;


        public  int matchedPlayer1 = 1;
        public  string match_details1 = null;
        public  int matchedPlayer2 = 2;
        public  bool matchfound = true;
        //public int userId;
        //public string userName;
        public  bool InMatch = false;
        public  MoveInfo move_info_exists = null;
        public  string move_info = null;

        public UserInfo player_account_info;
        public  string soldierList;
        public  string Password;
        public  bool match_exists;
        private bool waitingforMatchStatus;
        public  int[] listTest;

        public  List<MenuView.Soldier> listOfSoldiers;
        public MenuView.Soldier Soldiers;
        public  int numberOfSoldiers;
        


        public Text moveInfoText;
        public InputField matchDetailsInput;
        public InputField moveInfoInput;
        public InputField userIDInput;
        public InputField matchIDInput;


        private void Awake()
        {
            if (gameFlowController != null) Destroy(gameFlowController);
            gameFlowController = this;
        }

        /// <summary>
        /// Adds this user to the database list of users which are looking for a match.
        /// </summary>
        /// <param name="userID">The user's unique ID.</param>
        /// <param name="cancel">If true, the user is cancelling the request instead.</param>
        public void requestMatchmaking(string userID, bool cancel = false)
        {
            //Debug.Log("entered request Matchmaking");
            StartCoroutine(requestMatchMakingCoroutine(userID, cancel));

        }

        public IEnumerator requestMatchMakingCoroutine(string userId, bool cancel = false)
        {
            if (cancel)
            {
                RemoveUserLFMtoDatabase(userId);
            }

            //check if user is not already in queue.

            if (!cancel)
            {
                Debug.Log(cancel);
                PostUserLFMtoDatabase(userId);
            }


            //check if user is in match.
            MenuView.PlayerAccount new_player = new MenuView.PlayerAccount();
            new_player.userId = userId;
            MenuView.PlayerAccount.checkForMatch(new_player);

            if (new_player.InMatch)
            {
                //what to do if in match already?
            }

            yield return null;
        }



        ///// <summary>
        ///// Checks if a match has been assigned to the user.
        ///// </summary>
        ///// <param name="userID">The user's unique ID.</param>
        ///// <returns>Returns the match ID, or null if no match has been found.</returns>
        //public int checkForMatch(int userID)
        //{
        //    //If there is a match, it edits the match so that the server knows the user has read the directions. (not sure what this is supposed to do)
        //    MenuView.PlayerAccount new_player = new MenuView.PlayerAccount();
        //    new_player.userId = Convert.ToString(userID);
        //    StartCoroutine(checkForMatchCoroutine(new_player));
        //    for (int i = 0; i <30; i++)
        //    {
                
        //    }

        //    ///////PROBLEM HERE NOT RETURNNG CORRECT NEW PLAYER MATCH ID.
        //    Debug.Log("match ID:" + new_player.matchID);
        //    return new_player.matchID;
        //}

        //public IEnumerator checkForMatchCoroutine(MenuView.PlayerAccount new_player)
        //{
        //    Debug.Log("starting checkForMatchCoroutine");
        //    bool inProgress1 = true;
        //    //not waiting for this to end
        //    StartCoroutine(CheckIfUserInMatch(new_player));
            
        //    if (new_player.InMatch)
        //    {
                
        //        Debug.Log("matchID from database:" +new_player.matchID);
        //    }
        //    else
        //    {
        //        Debug.Log("new_player not in match");
        //    }
            
            
        //    while (inProgress1) yield return new WaitForSeconds(0.25f);
           
        //}


        //public void onGetMatchID()
        //{
        //    int x = Convert.ToInt32(userIDInput.text);
        //    Debug.Log(x);
        //    checkForMatch(x);

        //}





        /// <summary>
        /// Adds a unverified move to the match information. Information includes moveNumber as well as moveInfo.
        /// eg moveInfo: p1(playerNumber),mN(moveNumber):0,uN(unitNumber):0,x:000,z:000,h:0,0
        /// The game should wait until the move has been verified before proceeding.
        /// The server will verify that it is this player who can make a move.
        /// verification not implemented yet. can add moveinfo already.
        /// </summary>
        /// <param name="moveInfo"></param>
        public void addMove(string moveInfo, int matchID)
        {
            moveInformation = moveInfo;
            PostMoveInfoToDatabase(moveInfo, matchID);
        }



        /// <summary>
        /// Checks if a new move has been accepted by the server.
        /// If there is a new move, moveNumber should be incremented.
        /// If there are multiple unread moves, returns the earliest unread.
        /// </summary>
        /// <param name="matchID">The match's unique ID</param>
        /// <returns>Returns the move information, or null if there is no new move.</returns>
        public void checkNextMove(int matchID)
        {
            //check if match exists before we grab moveInfo

            StartCoroutine(checkNextMoveCoroutine(matchID));
        }

        public IEnumerator checkNextMoveCoroutine(int matchID)
        {
            yield return StartCoroutine(getMoveInfo(matchID));
            if (match_exists)
            {
                Debug.Log(move_info_exists.moveInfo);
            }
        }



        public void InitialiseMatch(string matchDetails)
        {
            match_details1 = matchDetails;
            PostMatchDetails();
        }

        //START OF FUNCTIONS REQUIRED FOR checkNextMove()
        public IEnumerator getMoveInfo(int matchID)
        {
            bool inProgress = true;
            try
            {

                RestClient.Get<MoveInfo>("https://project-finch-database.firebaseio.com/Match/" + matchID + "/moveInfo.json").Then(response =>
                {
                    move_info_exists = response;
                    inProgress = false;
                    if (move_info_exists.moveInfo != null)
                    {
                        match_exists = true;
                    }
                });

            }
            catch (NullReferenceException)
            {
                match_exists = false;
            }

            while (inProgress) yield return new WaitForSeconds(0.25f);

        }

        public void UpdateMoveInfo()
        {
            moveInfoText.text = move_info_exists.moveInfo;
        }

        public void OnCheckMoveInfo()
        {
            int x = Convert.ToInt32(matchIDInput.text);
            checkNextMove(x);
        }
        //END OF FUNCTIONS REQUIRED FOR checkNextMove()


        //START OF REQUESTMATCHMAKING REQUIRED FUNCTIONS
        //works
        public void RemoveUserLFMtoDatabase(string userID)
        {
            UserQueue user_queue = new UserQueue(userID);
            user_queue.UserID = null; //need to fix this issue.
            RestClient.Put("https://project-finch-database.firebaseio.com/queuingForMatch/" + userID + "/.json", user_queue);
        }

        //works
        public void PostUserLFMtoDatabase(string userID)
        {
            UserQueue user_queue = new UserQueue(userID);
            user_queue.UserID = userID;
            Debug.Log("userID:" + user_queue.UserID);
            RestClient.Put("https://project-finch-database.firebaseio.com/queuingForMatch/"+user_queue.UserID+".json", user_queue);
            Debug.Log("put passed");
        }

        public string getUserIdFromInput()
        {
            string id = userIDInput.text;
            return id;
        }

        public void onPostUserIDtoQueue()
        {
            string x = getUserIdFromInput();
            requestMatchmaking(x);
        }

        public void onRemoveUserIDFromQueue()
        {
            string x = getUserIdFromInput();
            requestMatchmaking(x, true);
        }

        //public IEnumerator CheckIfUserInMatch(MenuView.PlayerAccount new_player)
        //{
        //    bool inProgress = true;
        //    RestClient.Get("https://project-finch-database.firebaseio.com/User/" + new_player.userId + ".json").Then(response =>
        //    {
        //        new_player.InMatch = JsonUtility.FromJson<MenuView.PlayerAccount>(response.Text).InMatch;
        //        new_player.matchID = JsonUtility.FromJson<MenuView.PlayerAccount>(response.Text).matchID;
        //        Debug.Log("new_player.matchID:" + new_player.matchID);

        //        inProgress = false;
        //    });

        //    Debug.Log("checkIfUserInMatch still in progress: "+inProgress);
        //    while (inProgress) yield return new WaitForSeconds(0.25f);

        //}
        //END OF REQUESTMATCHMAKING REQUIRED FUNCTIONS


        //START OF FUNCTIONS REQUIRED FOR ADDMOVE().
        public void PostMoveInfoToDatabase(string moveInfo, int matchID)
        {
            MoveInfo moveinfo = new MoveInfo();
            moveinfo.moveInfo = moveInfo;
            moveinfo.matchID = matchID;
            RestClient.Put("https://project-finch-database.firebaseio.com/Match/" + moveinfo.matchID + "/moveInfo.json", moveinfo);
        }

        public string getMoveInfoFromInput()
        {
            string move_info = moveInfoInput.text;
            return move_info;
        }

        public void OnAddMove()
        {
            string moveInfo = getMoveInfoFromInput();
            int matchID = Convert.ToInt32(matchIDInput.text);
            addMove(moveInfo, matchID);
        }
        //END OF FUNCTIONS REQUIRED FOR ADDMOVE().



        //START OF FUNCTIONS REQUIRED FOR INITMATCH()
        public void PostMatchDetails()
        {
            MatchDetails match_details = new MatchDetails();
            matchID = 68;
            RestClient.Put("https://project-finch-database.firebaseio.com/Match/" + matchID + "/matchDetails.json", match_details);
        }

        public string getMatchDetailsFromInput()
        {
            return matchDetailsInput.text;
        }

        public void OnAddMatchDetail()
        {
            string match_d = getMatchDetailsFromInput();
            InitialiseMatch(match_d);
        }
        //END OF FUNCTIONS REQUIRED FOR INITMATCH().


        //public IEnumerator getPlayerAccountInfo(int userID)
        //{

        //    //RestClient.Get<UserInfo>("https://project-finch-database.firebaseio.com/User/" + userID + ".json").Then(response =>
        //    //{
        //    //    numberOfSoldiers = response.NumberOfSoldiers;
        //    //    soldiers = response.soldiers;
        //    //    Debug.Log("test");
        //    //    Debug.Log(soldiers["Soldier1"]);
        //    //    inProgress = false;
        //    //});

        //    //Get all attributes in User account info, except for Soldiers.

        //    UserInfo player_account_info = new UserInfo();
        //    player_account_info.userId = userID;
        //    //Debug.Log(player_account_info.userId);

        //    bool inProgress = true;
        //    RestClient.Get("https://project-finch-database.firebaseio.com/User/" + userID + ".json").Then(response =>
        //    {
        //        Debug.Log(player_account_info.userId);
        //        //EditorUtility.DisplayDialog("Response", response.Text, "Ok");
        //        //Debug.Log(response.Text);
        //        //string json = JsonUtility.ToJson(response.Text);
        //        player_account_info = JsonUtility.FromJson<UserInfo>(response.Text);
        //        //player_account_info.userId = player_account_info_buffer.userId;
        //        //Debug.Log(player_account_info_buffer.userName);
        //        //Debug.Log(player_account_info_buffer.matchID);
        //        Debug.Log("userid:" + player_account_info.userId);
        //        try
        //        {
        //            Debug.Log("here:" + player_account_info.soldierList);
        //            numberOfSoldiers = player_account_info.soldierList.Split(',').Length;
        //        }
        //        catch (Exception)
        //        {
        //            Debug.Log("entered catch");
        //            if (player_account_info.soldierList != null)
        //            {
        //                Debug.Log("entered catch if");
        //                numberOfSoldiers = 1;
        //            }
        //            else numberOfSoldiers = 0;
        //        }

        //        inProgress = true;
        //        Debug.Log("numberOfSoldiers: " + numberOfSoldiers);
        //        for (int i = 1; i < numberOfSoldiers + 1; i++)
        //        {

        //            RestClient.Get("https://project-finch-database.firebaseio.com/User/" + userID + "/Soldiers/Soldier" + i + ".json").Then(response2 =>
        //            {
        //                MenuView.Soldier soldier = JsonUtility.FromJson<MenuView.Soldier>(response2.Text);
        //                Debug.Log(soldier.aim);
        //                //Debug.Log("iterating.. :" + i);
        //                player_account_info.listOfSoldiers.Add(soldier);
        //                Debug.Log("number of soldiers in list currently:" + player_account_info.listOfSoldiers.Count);


        //            });
                    
        //            inProgress = false;

        //        };


                //Debug.Log(player_account_info.soldierList.Split(','));








                //bool inProgress2 = true;
                //RestClient.Get("https://project-finch-database.firebaseio.com/User/" + userID + "/Soldiers.json").Then(response => 
                //{
                //    Debug.Log(response.Text);


                //    List<MenuView.Soldier> soldier_info_list = JsonUtility.FromJson<List<MenuView.Soldier>>(response.Text);
                //    //Debug.Log(soldier_info_list.Count);
                //    //Debug.Log("aim: "+soldier_info.aim);
                //    //Debug.Log("maxhp: "+soldier_info.maxHealth);
                //    //Debug.Log("equips: " +soldier_info.equipments);
                //    byte[] bytes = Encoding.UTF8.GetBytes(response.Text);
                //    List<KeyValuePair<string, string>> soldier_info =JsonToDictionary(Encoding.UTF8.GetString(bytes, 0, response.Text.Length));
                //    Debug.Log("exited function");
                //    Debug.Log(soldier_info.Count);

                //   // Debug.Log(soldier_info.Values);




                //    inProgress2 = false;

                //});

                

            //});

        //    while (inProgress) yield return new WaitForSeconds(0.25f);
        //}

        //public void OnGetPlayerAccountInfo()
        //{

        //    int x = Convert.ToInt32(userIDInput.text);
        //    StartCoroutine(getPlayerAccountInfo(x));
        //    //Debug.Log(player_account_info.listOfSoldiers[0].aim);
        //}

        //public static List<KeyValuePair<string, string>> JsonToDictionary(string json)
        //{
        //    Debug.Log("entered jsontoDic");
        //    var ser = new DataContractJsonSerializer(typeof(List<KeyValuePair<string, string>>));
        //    var stream = new MemoryStream(Encoding.UTF8.GetBytes(json));
        //    stream.Position = 0;
        //    Debug.Log("finish jsontoDic");
        //    return (List<KeyValuePair<string, string>>)ser.ReadObject(stream);
        //}
    }

}

