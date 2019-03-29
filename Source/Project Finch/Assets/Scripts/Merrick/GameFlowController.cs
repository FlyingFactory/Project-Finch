using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

        private string matchID = null;
        private bool player1;
        private string moveNumber = null;

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
        /// The game should wait until the move has been verified before proceeding.
        /// The server will verify that it is this player who can make a move.
        /// </summary>
        /// <param name="moveInfo"></param>
        public void addMove(string moveInfo) {
            
        }

        /// <summary>
        /// Checks if a new move has been accepted by the server.
        /// If there is a new move, moveNumber should be incremented.
        /// If there are multiple unread moves, returns the earliest unread.
        /// </summary>
        /// <returns>Returns the move information, or null if there is no new move.</returns>
        public string checkNextMove() {
            return null;
        }
    }
}
