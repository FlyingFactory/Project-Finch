using System;
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

        public static GameFlowController gameFlowController;
        [System.NonSerialized] public TurnState turnState = TurnState.Entry;
        [System.NonSerialized] public bool waitingForServer = false;
        [System.NonSerialized] public int currentPlayer = 0;
        [System.NonSerialized] public int numPlayers = 2;
        public static int matchID = 68;
        private bool player1;
        public static string moveInformation = null;
        public static string NCmoveInformation = null;
        public static int moveNumber = 0;


        public static int matchedPlayer1 = 1;
        public static string match_details1 = null;
        public static int matchedPlayer2 = 2;
        public static bool matchfound = true;
        public static int UserID = 0;
        public static string UserName = "";
        public static bool InMatch = false;
        public static MoveInfo move_info_exists = null;
        public static string move_info = null;

        public static bool match_exists;
        private bool waitingforMatchStatus;

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
            StartCoroutine(requestMatchMakingCoroutine(userID, cancel));
            
        }

        public IEnumerator requestMatchMakingCoroutine(int userId, bool cancel = false)
        {
            if (cancel)
            {
                RemoveUserLFMtoDatabase(userId);
            }

            //check if user is not already in queue.

            if (!cancel)
            {
                PostUserLFMtoDatabase(userId);
            }
            

            //check if user is in match.
            yield return StartCoroutine(CheckIfUserInMatch(userId));

            if (InMatch)
            {
                //what to do if in match already?
            }
        }



        /// <summary>
        /// Checks if a match has been assigned to the user.
        /// </summary>
        /// <param name="userID">The user's unique ID.</param>
        /// <returns>Returns the match ID, or null if no match has been found.</returns>
        public void checkForMatch(int userID)
        {
            //If there is a match, it edits the match so that the server knows the user has read the directions. (not sure what this is supposed to do)

            StartCoroutine(checkForMatchCoroutine(userID));
            
        }

        public IEnumerator checkForMatchCoroutine(int userID)
        {
            yield return StartCoroutine(CheckIfUserInMatch(userID));
            if (InMatch)
            {
                Debug.Log(matchID);
            }
            else
            {
                
            }
        }


        public void onGetMatchID()
        {
            int x = Convert.ToInt32(userIDInput.text);
            Debug.Log(x);
            checkForMatch(x);
        
        }





        /// <summary>
        /// Adds a unverified move to the match information. Information includes moveNumber as well as moveInfo.
        /// eg moveInfo: p1(playerNumber),mN(moveNumber):0,uN(unitNumber):0,x:000,z:000,h:0,0
        /// The game should wait until the move has been verified before proceeding.
        /// The server will verify that it is this player who can make a move.
        /// verification not implemented yet. can add moveinfo already.
        /// </summary>
        /// <param name="moveInfo"></param>
        public void addMove(string moveInfo)
        {
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
        public void checkNextMove(int matchID) {
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

        public IEnumerator CheckIfUserInMatch(int userID)
        {
            bool inProgress = true;
            RestClient.Get<UserInfo>("https://project-finch-database.firebaseio.com/User/" + userID + ".json").Then(response =>
            {
                InMatch = response.InMatch;
                matchID = response.matchID;
                inProgress = false;
            });

            while (inProgress) yield return new WaitForSeconds(0.25f);
            
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
