using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CombatView {

    public class ActionUnit : Unit {
        
        public int numActions = 2;
        public int actionsPerTurn = 2;
        public int mobility = 6;
        public int id = 0;
        public string dict_id = "";

        new public void Start() {
            base.Start();
        }

        public void OrderMove(Tile t) {
            //TODO
        }

        public void DrawMovableLocations() {
            //TODO
        }
    }
}