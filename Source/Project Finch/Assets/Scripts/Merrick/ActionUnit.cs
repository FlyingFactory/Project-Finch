using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CombatView {

    public class ActionUnit : Unit {

        public bool Debug = false;
        public int numActions = 2;
        public int actionsPerTurn = 2;
        public int mobility = 6;

        new public void Start() {
            base.Start();
        }

        public void OrderMove(Tile t) {
            //TODO
        }
    }
}