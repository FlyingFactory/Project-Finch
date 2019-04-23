using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CombatView {

    public class ActionUnit : Unit {
        
        [System.NonSerialized] public int numActions = 2;
        [System.NonSerialized] public int actionsPerTurn = 2;
        [System.NonSerialized] public int mobility = 7;
        [System.NonSerialized] public int id = 0;
        [System.NonSerialized] public string dict_id = "";

        new public void Start() {
            base.Start();
        }

        public void OrderMove(Tile destination) {
            if (destination != null && numActions > 0) {
                bool move = true;
                if (takesTile && destination.ContainsBlockingAnything()) {
                    Debug.LogWarning("Invalid move location!");
                    move = false;
                }
                if (move) {
                    numActions -= NumMovesToTile(destination);
                    transform.position = new Vector3(destination.x, destination.h, destination.z);
                    tile.occupyingObjects.Remove(this);
                    tile = destination;
                    tile.occupyingObjects.Add(this);
                }
            }
        }

        public int NumMovesToTile(Tile destination) {
            return Mathf.CeilToInt((Tile.DistanceBetween(destination, tile) - 0.1f) / mobility);
        }

        public void OrderAttack(Tile destination, string outcome, int rawdamage) {

            Unit target = null;
            foreach (FieldObject f in destination.occupyingObjects) {
                if (f is ActionUnit) {
                    target = f as ActionUnit;
                    break;
                }
            }
            if (target != null) {
                if (outcome == "h") {
                    Debug.Log("hit!");
                    target.Damage(rawdamage);
                }
                else {
                    Debug.Log("missed!");
                }
            }
            else {
                Debug.LogWarning("No target!");
            }
        }

        public void DrawMovableLocations() {
            //TODO
        }

        public override bool OnDeath() {
            if (PlayerOrdersController.playerOrdersController.controllableUnits.Contains(this)) PlayerOrdersController.playerOrdersController.controllableUnits.Remove(this);
            else if (PlayerOrdersController.playerOrdersController.otherPlayerUnits.Contains(this)) PlayerOrdersController.playerOrdersController.otherPlayerUnits.Remove(this);
            return base.OnDeath();
        }
    }
}