using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CombatView {

    public class PlayerOrdersController : MonoBehaviour {

        public enum PlayerControlState {
            UnitSelect,
            ActionSelect,
            OnHold,
        }

        public static PlayerOrdersController playerOrdersController;
        public PlayerControlState playerControlState = PlayerControlState.UnitSelect;
        public List<ActionUnit> controllableUnits = new List<ActionUnit>();
        public ActionUnit selectedUnit = null;

    private void Awake() {
        if (playerOrdersController != null) Destroy(playerOrdersController);
            playerOrdersController = this;
        }

        private void Update() {
            switch (playerControlState) {
                case PlayerControlState.UnitSelect:
                    if (Input.GetButtonDown("select")) {
                        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                        RaycastHit hit;
                        if (Physics.Raycast(ray, out hit, 100, LayerMask.GetMask("Click"))) {

                            TileEffector hitTile = hit.collider.transform.parent.gameObject.GetComponent<TileEffector>();

                            if (hitTile != null) {
                                for (int i = 0; i < hitTile.tile.occupyingObjects.Count; i++) {
                                    Debug.Log(hitTile.gameObject.name + " clicked");
                                    if (hitTile.tile.occupyingObjects[i] is ActionUnit && controllableUnits.Contains(hitTile.tile.occupyingObjects[i] as ActionUnit)) {
                                        Debug.Log(gameObject.name + " selected");
                                        selectedUnit = hitTile.tile.occupyingObjects[i] as ActionUnit;
                                        break;
                                    }
                                }
                            }
                        }
                    }
                    else if (Input.GetButtonDown("select2")) {
                        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                        RaycastHit hit;
                        if (Physics.Raycast(ray, out hit, 100, LayerMask.GetMask("Click"))) {

                            TileEffector hitTile = hit.collider.transform.parent.gameObject.GetComponent<TileEffector>();
                            if (hitTile != null) {
                                bool move = true;
                                if (selectedUnit.takesTile) {
                                    for (int i = 0; i < hitTile.tile.occupyingObjects.Count; i++) {
                                        if (hitTile.tile.occupyingObjects[i].takesTile) {
                                            move = false;
                                            break;
                                        }
                                    }
                                }
                                if (move) {
                                    selectedUnit.transform.position = hitTile.transform.position;
                                    selectedUnit.tile.occupyingObjects.Remove(selectedUnit);
                                    selectedUnit.tile = hitTile.tile;
                                    selectedUnit.tile.occupyingObjects.Add(selectedUnit);
                                }
                            }
                        }
                    }
                    break;
            }
        }
    }
}
