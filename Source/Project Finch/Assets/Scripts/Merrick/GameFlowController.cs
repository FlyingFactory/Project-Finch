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
        [System.NonSerialized] public int currentPlayer = 0;
        [System.NonSerialized] public int numPlayers = 2;

        private void Start() {
            if (gameFlowController != null) Destroy(gameFlowController);
            gameFlowController = this;
        }
    }
}
