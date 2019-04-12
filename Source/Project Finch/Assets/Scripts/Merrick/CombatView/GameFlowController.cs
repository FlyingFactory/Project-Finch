using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Proyecto26;
using UnityEditor;
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
        [System.NonSerialized] public bool player1;

        //set to 68 first for testing
        public int matchID = 68;

        public List<Queue<string>> moveHistory = new List<Queue<string>>();
        public string moveInformation = null;
        public string NCmoveInformation = null;
        public string match_details1 = null;
        public bool InMatch = false;
        public MoveInfo move_info_exists = null;
        public string move_info = null;
        public bool match_exists;

 
        

        /// <summary>
        /// The UI stuff below are for debugging.
        /// </summary>
        public Text moveInfoText;
        public InputField matchDetailsInput;
        public InputField moveInfoInput;
        public InputField userIDInput;
        public InputField matchIDInput;
        public InputField moveNumberInput;
        /// end of UI stuff for debugging.

        private void Awake()
        {
            if (gameFlowController != null) Destroy(gameFlowController);
            gameFlowController = this;
        }

        private void Start() {
            //Placeholder
            turnState = TurnState.PlayerInput;
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
                //Debug.Log(cancel);
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

        public void RemoveUserLFMtoDatabase(string userID)
        {
            UserQueue user_queue = new UserQueue(userID);
            user_queue.UserID = null;
            RestClient.Put("https://project-finch-database.firebaseio.com/queuingForMatch/" + userID + "/.json", user_queue);
            Debug.Log("User ID: " + userID + " has been removed from the queue..");
        }
        public void PostUserLFMtoDatabase(string userID)
        {
            UserQueue user_queue = new UserQueue(userID);
            user_queue.UserID = userID;
            RestClient.Put("https://project-finch-database.firebaseio.com/queuingForMatch/" + user_queue.UserID + ".json", user_queue);
            Debug.Log("User ID: " + user_queue.UserID + " has been added to queue..");
        }




        /// <summary>
        /// Adds a unverified move to the match information. Information includes moveNumber as well as moveInfo.
        /// eg u_player1Move: "pid_soldierid, x:z:h, skill, hit/miss,dmg dealt"
        /// The game should wait until the move has been verified before proceeding.
        /// The server will verify that it is this player who can make a move.
        /// verification not implemented yet. can add moveinfo already.
        /// </summary>
        /// <param name="moveInfo"></param>
        public void addMove(string moveInfo)
        {
            moveInformation = moveInfo;
            PostMoveInfoToDatabase(moveInfo, matchID, player1);
        }

        public void PostMoveInfoToDatabase(string moveInfo, int matchID, bool player1)
        {
            MoveInfo moveinfo = new MoveInfo();
            if (player1)
            {
                moveinfo.u_player1Move = moveInfo;
                RestClient.Put("https://project-finch-database.firebaseio.com/Match/" + matchID + "/moveInfo.json", moveinfo);
            }
            else
            {
                moveinfo.u_player2Move = moveInfo;
                RestClient.Put("https://project-finch-database.firebaseio.com/Match/" + matchID + "/moveInfo.json", moveinfo);
            }
            
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
                Debug.Log(move_info_exists.a_playersMoves);
            }
        }

        public IEnumerator getMoveInfo(int matchID)
        {
            bool inProgress = true;
            try
            {

                RestClient.Get<MoveInfo>("https://project-finch-database.firebaseio.com/Match/" + matchID + "/moveInfo.json").Then(response =>
                {
                    move_info_exists = response;
                    inProgress = false;
                    if (move_info_exists.a_playersMoves != null)
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

        public void getMove(int moveNumber)
        {
            matchID = 68;

            while (moveHistory.Count <= moveNumber)
            {
                moveHistory.Add(new Queue<string>());
            }

            RestClient.Get<MoveInfo>("https://project-finch-database.firebaseio.com/Match/" + matchID + "/moveInfo.json").Then(response =>
            {
                if (response.a_playersMoves.Count > moveNumber) moveHistory[moveNumber].Enqueue(response.a_playersMoves[moveNumber]);
            });

        }

        public void onGetMove()
        {
            int x = Convert.ToInt32(moveNumberInput.text);
            getMove(x);
        }


        /// <summary>
        /// Puts first match details on database to allow both players to draw information of match to start match.
        /// </summary>
        /// <param name="matchDetails">havent decided what match details</param>
        public void InitialiseMatch(string matchDetails)
        {
            match_details1 = matchDetails;
            PostMatchDetails();
        }

        public void PostMatchDetails()
        {
            MatchDetails match_details = new MatchDetails();
            matchID = 68;
            RestClient.Put("https://project-finch-database.firebaseio.com/Match/" + matchID + "/matchDetails.json", match_details);
        }

        



/// FROM THIS POINT ON, ALL IMPLEMENTED FUNCTIONS ARE USED TO TEST ABOVE FUNCTIONS.//////////////////////////////////////////
/// 

        //START OF FUNCTIONS REQUIRED FOR TESTING checkNextMove()


        public void UpdateMoveInfo()
        {
            //this is wrong pls fix later
            moveInfoText.text = move_info_exists.u_player1Move;
        }

        public void OnCheckMoveInfo()
        {
            int x = Convert.ToInt32(matchIDInput.text);
            getMove(x);
        }
        //END OF FUNCTIONS REQUIRED FOR TESTING checkNextMove()


        //START OF REQUESTMATCHMAKING FUNCTIONS FOR TESTING requestMatchmaking().
        
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
        //END OF REQUESTMATCHMAKING FUNCTIONS FOR TESTING requestMatchmaking().


        //START OF FUNCTIONS REQUIRED FOR TESTING ADDMOVE().


        public string getMoveInfoFromInput()
        {
            string move_info = moveInfoInput.text;
            return move_info;
        }

        public void OnAddMove()
        {
            string moveInfo = getMoveInfoFromInput();
            matchID = 68;
            Debug.Log("default matchID:" +matchID);
            addMove(moveInfo);
        }
        //END OF FUNCTIONS REQUIRED FOR TESTING ADDMOVE().



        //START OF FUNCTIONS REQUIRED FOR TESTING INITMATCH()
        

        public string getMatchDetailsFromInput()
        {
            return matchDetailsInput.text;
        }

        public void OnAddMatchDetail()
        {
            string match_d = getMatchDetailsFromInput();
            InitialiseMatch(match_d);
        }
        //END OF FUNCTIONS REQUIRED FOR TESTING INITMATCH().


       
    }

}

