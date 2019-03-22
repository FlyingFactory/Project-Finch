using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CombatView {

    public class PlayerOrdersController : MonoBehaviour {

        public const float BASE_AIM = 0.65f;

        public enum PlayerControlState {
            UnitSelect,
            ActionSelect,
            OnHold,
        }

        public static PlayerOrdersController playerOrdersController;
        public PlayerControlState playerControlState = PlayerControlState.UnitSelect;
        public List<ActionUnit> controllableUnits = new List<ActionUnit>();
        [System.NonSerialized] public ActionUnit selectedUnit = null;
        [System.NonSerialized] public Unit targetedUnit = null;

        // --- UI ---
        [SerializeField] private GameObject unitSelectionIndicator;
        [SerializeField] private GameObject fireUI;
        [SerializeField] private TMPro.TextMeshProUGUI hitChanceText;


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
                                    if (hitTile.tile.occupyingObjects[i] is Unit) {
                                        if (hitTile.tile.occupyingObjects[i] is ActionUnit 
                                            && controllableUnits.Contains(hitTile.tile.occupyingObjects[i] as ActionUnit) 
                                            && (hitTile.tile.occupyingObjects[i] as Unit).status != Unit.Status.Dead) {
                                            selectedUnit = hitTile.tile.occupyingObjects[i] as ActionUnit;
                                        }
                                        else {
                                            targetedUnit = hitTile.tile.occupyingObjects[i] as Unit;
                                            fireUI.SetActive(true);
                                            playerControlState = PlayerControlState.ActionSelect;
                                            float d = Tile.DistanceBetween(selectedUnit.tile, targetedUnit.tile);
                                            float hitChance;
                                            if (d > 8) hitChance = Mathf.Clamp01(BASE_AIM - (d - 8) / 10f);
                                            else if (d < 5) hitChance = Mathf.Clamp01(BASE_AIM + (5 - d) / 10f);
                                            else hitChance = BASE_AIM;
                                            hitChanceText.text = string.Format("Hit: {0:p}", hitChance);
                                        }
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
                            if (hitTile != null && selectedUnit != null) {
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

                    if (selectedUnit != null && unitSelectionIndicator != null) {
                        unitSelectionIndicator.transform.position = selectedUnit.transform.position;
                    }
                    break;

                case PlayerControlState.ActionSelect:

                    if (Input.GetButtonDown("escape")) {
                        targetedUnit = null;
                        fireUI.SetActive(false);
                        playerControlState = PlayerControlState.UnitSelect;
                    }
                    break;
            }
        }

        public void FireButton() {
            float d = Tile.DistanceBetween(selectedUnit.tile, targetedUnit.tile);
            float hitChance;
            if (d > 8) hitChance = Mathf.Clamp01(BASE_AIM - (d - 8) / 10f);
            else if (d < 5) hitChance = Mathf.Clamp01(BASE_AIM + (5 - d) / 10f);
            else hitChance = BASE_AIM;
            if (Random.Range(0f, 1) < hitChance) {
                Debug.Log("Hit!");
                targetedUnit.Damage(5); // TODO: fill with real damage calculation
            }
            else {
                Debug.Log("Missed!");
            }
            targetedUnit = null;
            fireUI.SetActive(false);
            playerControlState = PlayerControlState.UnitSelect;
        }
    }
}
