using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CombatView {

    public class PlayerOrdersController : MonoBehaviour {

        public const float BASE_AIM = 0.65f;
        public const float HALFCOVER_PENALTY = 0.20f;

        public enum PlayerControlState {
            UnitSelect,
            ActionSelect,
            OnHold,
        }

        public static PlayerOrdersController playerOrdersController;
        [System.NonSerialized] public PlayerControlState playerControlState = PlayerControlState.UnitSelect;
        [System.NonSerialized] public List<ActionUnit> controllableUnits = new List<ActionUnit>();
        [System.NonSerialized] public List<ActionUnit> otherUnits = new List<ActionUnit>();
        [System.NonSerialized] public ActionUnit selectedUnit = null;
        [System.NonSerialized] public Unit targetedUnit = null;

        // --- UI ---
#pragma warning disable 649
        [SerializeField] private GameObject unitSelectionIndicator;
        [SerializeField] private GameObject fireUI;
        [SerializeField] private TMPro.TextMeshProUGUI hitChanceText;
        [SerializeField] private TMPro.TextMeshProUGUI coverText;
        [SerializeField] private Color lowCoverTextColor = new Color32(255, 255, 0, 255);
        [SerializeField] private Color highCoverTextColor = new Color32(100, 100, 255, 255);
#pragma warning restore 649


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
                                    if (hitTile.tile.occupyingObjects[i] is Unit
                                        && (hitTile.tile.occupyingObjects[i] as Unit).status != Unit.Status.Dead) {
                                        if (hitTile.tile.occupyingObjects[i] is ActionUnit
                                            && controllableUnits.Contains(hitTile.tile.occupyingObjects[i] as ActionUnit)) {
                                            selectedUnit = hitTile.tile.occupyingObjects[i] as ActionUnit;
                                        }
                                        else if (selectedUnit != null) {
                                            targetedUnit = hitTile.tile.occupyingObjects[i] as Unit;
                                            fireUI.SetActive(true);
                                            playerControlState = PlayerControlState.ActionSelect;

                                            float d = Tile.DistanceBetween(selectedUnit.tile, targetedUnit.tile);
                                            float hitChance;
                                            if (d > 8) hitChance = Mathf.Clamp01(BASE_AIM - (d - 8) / 10f);
                                            else if (d < 5) hitChance = Mathf.Clamp01(BASE_AIM + (5 - d) / 10f);
                                            else hitChance = BASE_AIM;

                                            CoverType highestUnflankedCover = CoverType.None;
                                            if (selectedUnit.tile.x < targetedUnit.tile.x) {
                                                CoverType directionCover = targetedUnit.tile.getCover(Direction.minusX);
                                                if (directionCover > highestUnflankedCover) highestUnflankedCover = directionCover;
                                            }
                                            else if (selectedUnit.tile.x > targetedUnit.tile.x) {
                                                CoverType directionCover = targetedUnit.tile.getCover(Direction.X);
                                                if (directionCover > highestUnflankedCover) highestUnflankedCover = directionCover;
                                            }
                                            if (selectedUnit.tile.z < targetedUnit.tile.z) {
                                                CoverType directionCover = targetedUnit.tile.getCover(Direction.minusZ);
                                                if (directionCover > highestUnflankedCover) highestUnflankedCover = directionCover;
                                            }
                                            else if (selectedUnit.tile.z > targetedUnit.tile.z) {
                                                CoverType directionCover = targetedUnit.tile.getCover(Direction.Z);
                                                if (directionCover > highestUnflankedCover) highestUnflankedCover = directionCover;
                                            }
                                            hitChance -= (int)highestUnflankedCover * HALFCOVER_PENALTY;

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
                            if (hitTile != null && selectedUnit != null && selectedUnit.numActions > 0) {
                                bool move = true;
                                if (selectedUnit.takesTile && hitTile.tile.ContainsBlockingAnything()) {
                                    move = false;
                                }
                                if (move) {
                                    string serverMove = "";
                                    serverMove += selectedUnit.dict_id + ",";
                                    serverMove += hitTile.tile.x + ":" + hitTile.tile.z + ":" + hitTile.tile.h + ",";
                                    serverMove += "m";

                                    GameFlowController.gameFlowController.addMove(serverMove);

                                    // TO REMOVE
                                    selectedUnit.numActions -= 1;
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
                        CanvasRefs.canvasRefs.fireUI.SetActive(false);
                        playerControlState = PlayerControlState.UnitSelect;
                    }
                    break;
            }
        }

        public void FireButton() {
            if (targetedUnit.status != Unit.Status.Dead
                && selectedUnit.numActions > 0) {
                selectedUnit.numActions = 0;

                float d = Tile.DistanceBetween(selectedUnit.tile, targetedUnit.tile);
                float hitChance;
                if (d > 8) hitChance = Mathf.Clamp01(BASE_AIM - (d - 8) / 10f);
                else if (d < 5) hitChance = Mathf.Clamp01(BASE_AIM + (5 - d) / 10f);
                else hitChance = BASE_AIM;

                CoverType highestUnflankedCover = CoverType.None;
                if (selectedUnit.tile.x < targetedUnit.tile.x) {
                    CoverType directionCover = targetedUnit.tile.getCover(Direction.minusX);
                    if (directionCover > highestUnflankedCover) highestUnflankedCover = directionCover;
                }
                else if (selectedUnit.tile.x > targetedUnit.tile.x) {
                    CoverType directionCover = targetedUnit.tile.getCover(Direction.X);
                    if (directionCover > highestUnflankedCover) highestUnflankedCover = directionCover;
                }
                if (selectedUnit.tile.z < targetedUnit.tile.z) {
                    CoverType directionCover = targetedUnit.tile.getCover(Direction.minusZ);
                    if (directionCover > highestUnflankedCover) highestUnflankedCover = directionCover;
                }
                else if (selectedUnit.tile.z > targetedUnit.tile.z) {
                    CoverType directionCover = targetedUnit.tile.getCover(Direction.Z);
                    if (directionCover > highestUnflankedCover) highestUnflankedCover = directionCover;
                }
                hitChance -= (int)highestUnflankedCover * HALFCOVER_PENALTY;

                bool hit = Random.Range(0f, 1) < hitChance;
                int damage = 0;
                if (hit) {
                    Debug.Log("Hit!");
                    damage = 5; // TODO: fill with real damage calculation
                    // targetedUnit.Damage(5);
                }
                else {
                    Debug.Log("Missed!");
                }

                // === SERVER ===
                string serverMove = "";
                serverMove += selectedUnit.dict_id + ",";
                serverMove += targetedUnit.tile.x + ":" + targetedUnit.tile.z + ":" + targetedUnit.tile.h + ",";
                serverMove += "a,";
                serverMove += (hit ? "h" : "m") + ",";
                serverMove += damage;

                GameFlowController.gameFlowController.addMove(serverMove);

                // TO REMOVE
                targetedUnit = null;
                CanvasRefs.canvasRefs.fireUI.SetActive(false);
                playerControlState = PlayerControlState.UnitSelect;
            }
        }

        public void EndTurnButton() {
            GameFlowController.gameFlowController.addMove("endturn,voluntary");

            // TO REMOVE
            // need to handle wrapup effects
            CanvasRefs.canvasRefs.EndTurnButton.SetActive(false);
            CanvasRefs.canvasRefs.EnemyTurnIndicator.SetActive(true);
            GameFlowController.gameFlowController.turnState = GameFlowController.TurnState.EnemyTurn;
            selectedUnit = null;
            unitSelectionIndicator.transform.position = new Vector3(1000, 0, 0);

            // placeholder
            Invoke("StartTurn", 1.5f);
        }

        public void StartTurn() {
            // need to handle turn entry effects
            CanvasRefs.canvasRefs.EndTurnButton.SetActive(true);
            CanvasRefs.canvasRefs.EnemyTurnIndicator.SetActive(false);
            GameFlowController.gameFlowController.turnState = GameFlowController.TurnState.Entry;
            for (int i = 0; i < controllableUnits.Count; i++) {
                controllableUnits[i].numActions = controllableUnits[i].actionsPerTurn;
            }
        }
    }
}
