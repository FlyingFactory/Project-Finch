﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Proyecto26;

namespace CombatView {

    public class GameFlowController : MonoBehaviour {

        public enum TurnState {
            Entry,
            PlayerInput,
            PlayerAction,
            Wrapup,
        }

        [System.NonSerialized] public static GameFlowController gameFlowController;
        [System.NonSerialized] public TurnState turnState = TurnState.Entry;
        [System.NonSerialized] public bool waitingForServer = false;
        [System.NonSerialized] public int currentPlayer = 0;
        [System.NonSerialized] public int numPlayers = 2;
        [System.NonSerialized] public static int matchID = 68;
        private bool player1;
        [System.NonSerialized] public static string moveInformation = null;
        [System.NonSerialized] public static string NCmoveInformation = null;
        [System.NonSerialized] public static int moveNumber = 0;


        [System.NonSerialized] public static string match_details1 = null;
        [System.NonSerialized] public static int matchedPlayer1 = 1;
        [System.NonSerialized] public static int matchedPlayer2 = 2;
        [System.NonSerialized] public static bool matchfound = true;
        [System.NonSerialized] public static int UserID = 0;
        [System.NonSerialized] public static bool InMatch = false;
        [System.NonSerialized] public static MoveInfo move_info_exists = null;
        [System.NonSerialized] public static string move_info = null;

        UserQueue user_queue = new UserQueue();


        public Text moveInfoText;
        public InputField matchDetailsInput;
        public InputField moveInfoInput;
        public InputField userIDInput;
        public InputField matchIDInput;
        

        private void Awake() {
            if (gameFlowController != null) Destroy(gameFlowController);
            gameFlowController = this;
        }

        /// <summary>
        /// Adds this user to the database list of users which are looking for a match.
        /// </summary>
        /// <param name="userID">The user's unique ID.</param>
        /// <param name="cancel">If true, the user is cancelling the request instead.</param>
        public void requestMatchmaking(int userID, bool cancel = false) {
            // The server should check if the user is already searching for or in a match.

            //check if user is not already in queue.
            if (!CheckIfUserInQueue(userID))
            {
                PostUserLFMtoDatabase(userID);
            }
            
            //check if user is in match.
            if (CheckIfUserInMatch(userID))
            {
                //user is in match already. do what?
            }

            if (cancel)
            {
                RemoveUserLFMtoDatabase(userID);
            }
        }

        

        /// <summary>
        /// Checks if a match has been assigned to the user.
        /// </summary>
        /// <param name="userID">The user's unique ID.</param>
        /// <returns>Returns the match ID, or null if no match has been found.</returns>
        public string checkForMatch(int userID) {
            //If there is a match, it edits the match so that the server knows the user has read the directions.
            return null;
        }

        /// <summary>
        /// Adds a unverified move to the match information. Information includes moveNumber as well as moveInfo.
        /// eg moveInfo: p1(playerNumber),mN(moveNumber):0,uN(unitNumber):0,x:000,z:000,h:0,0
        /// The game should wait until the move has been verified before proceeding.
        /// The server will verify that it is this player who can make a move.
        /// verification not implemented yet. can add moveinfo already.
        /// </summary>
        /// <param name="moveInfo"></param>
        public void addMove(string moveInfo) {
            moveInformation = moveInfo;
            PostMoveInfoToDatabase();
        }

        

        /// <summary>
        /// Checks if a new move has been accepted by the server.
        /// If there is a new move, moveNumber should be incremented.
        /// If there are multiple unread moves, returns the earliest unread.
        /// </summary>
        /// <param name="matchID">The match's unique ID</param>
        /// <returns>Returns the move information, or null if there is no new move.</returns>
        public string checkNextMove(int matchID) {
            //check if match exists before we grab moveInfo
            
            if (matchExists(matchID))
            {
                Debug.Log("match ID exists");
                RestClient.Get<MoveInfo>("https://project-finch-database.firebaseio.com/Match/" + matchID + "/moveInfo.json").Then(response =>
                {
                    move_info_exists = response;
                    //UpdateMoveInfo();
                });
            }
            //Debug.Log(move_info_exists.moveInfo);
            return move_info_exists.moveInfo;
        }


       


        public void InitialiseMatch(string matchDetails)
        {
            match_details1 = matchDetails;
            PostMatchDetails();
        }

        //START OF FUNCTIONS REQUIRED FOR checkNextMove()
        public bool matchExists(int matchID)
        {
            try
            {
                RestClient.Get<MoveInfo>("https://project-finch-database.firebaseio.com/Match/" + matchID + "/moveInfo.json").Then(response =>
                {
                    MoveInfo move_info_exists = response;

                });
                return true;
            }
            catch (NullReferenceException e)
            {
                return false;
            }
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
        public void RemoveUserLFMtoDatabase(int userID)
        {
            user_queue = new UserQueue();
            user_queue.UserID = null;
            RestClient.Put("https://project-finch-database.firebaseio.com/queuingForMatch/" + userID + "/.json", user_queue);
        }

        //works
        public void PostUserLFMtoDatabase(int userID)
        {
            user_queue = new UserQueue();
            user_queue.UserID = userID;
            RestClient.Put("https://project-finch-database.firebaseio.com/queuingForMatch/" + userID + ".json", user_queue);
        }

        //works
        public bool CheckIfUserInQueue(int userID)
        {
            RestClient.Get<UserQueue>("https://project-finch-database.firebaseio.com/queuingForMatch/" + ".json").Then(response =>
            {
                user_queue = response;
            });

            if (user_queue.UserID == userID)
            {
                return true;
            }
            else
            {
                return false;
            }

        }

        public int getUserIdFromInput()
        {
            int id = Convert.ToInt32(userIDInput.text);
            return id;
        }

        public void onPostUserIDtoQueue()
        {
            int x = getUserIdFromInput();
            requestMatchmaking(x);
        }

        public void onRemoveUserIDFromQueue()
        {
            int x = getUserIdFromInput();
            requestMatchmaking(x, true);
        }

        public bool CheckIfUserInMatch(int userID)
        {
            RestClient.Get<bool>("https://project-finch-database.firebaseio.com/User/" + userID + "InMatch/.json").Then(response =>
            {
                InMatch = response;
            });

            return InMatch;
        }
        //END OF REQUESTMATCHMAKING REQUIRED FUNCTIONS


        //START OF FUNCTIONS REQUIRED FOR ADDMOVE().
        public void PostMoveInfoToDatabase()
        {
            MoveInfo moveinfo = new MoveInfo();
            matchID = 68;
            RestClient.Put("https://project-finch-database.firebaseio.com/Match/" + matchID + "/moveInfo.json", moveinfo);
        }

        public string getMoveInfoFromInput()
        {
            string move_info = moveInfoInput.text;
            return move_info;
        }

        public void OnAddMove()
        {
            string moveInfo = getMoveInfoFromInput();
            addMove(moveInfo);
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
    }
}
