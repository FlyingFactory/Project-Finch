using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Proyecto26;
using System.Threading;

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
        [System.NonSerialized] public List<ActionUnit> otherPlayerUnits = new List<ActionUnit>();
        [System.NonSerialized] public ActionUnit selectedUnit = null;
        [System.NonSerialized] public Unit targetedUnit = null;

        // --- UI ---
#pragma warning disable 649
        [SerializeField] private GameObject unitSelectionIndicator;
        [SerializeField] private GameObject endTurnUI;
        [SerializeField] private TMPro.TextMeshProUGUI hitChanceText;
        [SerializeField] private TMPro.TextMeshProUGUI coverText;
        [SerializeField] private Color lowCoverTextColor = new Color32(255, 255, 0, 255);
        [SerializeField] private Color highCoverTextColor = new Color32(120, 180, 255, 255);
        [SerializeField] private Color victoryTextColor = new Color32(120, 120, 255, 255);
        [SerializeField] private Color defeatTextColor = new Color32(255, 60, 60, 255);
#pragma warning restore 649
        private GameObject moveCircle1;
        private GameObject moveCircle2;

        // networking
        [System.NonSerialized] public List<string> confirmedMoves = new List<string>();
        [System.NonSerialized] public Dictionary<string, ActionUnit> allUnits = new Dictionary<string, ActionUnit>();
        [System.NonSerialized] public int nextListMove = 0;
        [System.NonSerialized] public int nextExecMove = 0;
        [System.NonSerialized] public bool matchEnded = false;
        [System.NonSerialized] public float serverPingCounter = 0;
        public const float serverPingInterval = 0.2f;
        [System.NonSerialized] public bool waitingForServer = false;
        [System.NonSerialized] public string lastSentMove = "";
        [System.NonSerialized] public float timeWaitingForServer = 0;
        public const float serverResendInterval = 10f;
        [System.NonSerialized] public int numResends = 0;
        public const int maxResendCount = 3;


        private void Awake() {
            if (playerOrdersController != null) Destroy(playerOrdersController);
            playerOrdersController = this;
        }

        private void Start() {
            moveCircle1 = unitSelectionIndicator.transform.Find("SingleMove").gameObject;
            moveCircle2 = unitSelectionIndicator.transform.Find("DoubleMove").gameObject;
        }

        private void Update() {
            if (matchEnded) return;

            if (GameFlowController.gameFlowController.moveHistory.Count > nextListMove
                && GameFlowController.gameFlowController.moveHistory[nextListMove].Count > 0) {
                string move = GameFlowController.gameFlowController.moveHistory[nextListMove].Dequeue();
                if (move != null && move != "") {
                    confirmedMoves.Add(move);
                    nextListMove++;
                }
            }
            if (confirmedMoves.Count > nextExecMove) {
                Debug.Log("Execute move #" + nextExecMove + ": " + confirmedMoves[nextExecMove]);
                string[] moveSplit = confirmedMoves[nextExecMove].Split(',');
                waitingForServer = false;
                timeWaitingForServer = 0;
                numResends = 0;

                string[] coords;
                Tile t;
                int h;
                switch (moveSplit[0]) {
                    case "m":
                        coords = moveSplit[2].Split(':');
                        Debug.Log("move");
                        t = MapInfo.currentMapInfo.bottomLayer[int.Parse(coords[0]), int.Parse(coords[1])];
                        h = int.Parse(coords[2]);
                        for (int i = 0; i < h; i++) {
                            if (t.above != null) { t = t.above; }
                            else { Debug.Log("coordinate error!"); }
                        }
                        allUnits[moveSplit[1]].OrderMove(t);
                        break;
                    case "a":
                        targetedUnit = null;
                        CanvasRefs.canvasRefs.fireUI.SetActive(false);

                        coords = moveSplit[2].Split(':');
                        t = MapInfo.currentMapInfo.bottomLayer[int.Parse(coords[0]), int.Parse(coords[1])];
                        h = int.Parse(coords[2]);
                        for (int i = 0; i < h; i++) {
                            try { t = t.above; }
                            catch (System.ArgumentOutOfRangeException) { }
                        }
                        allUnits[moveSplit[1]].OrderAttack(t, moveSplit[3], int.Parse(moveSplit[4]));

                        playerControlState = PlayerControlState.UnitSelect;
                        break;
                    case "endturn":
                        if (GameFlowController.gameFlowController.isMyTurn) {
                            GameFlowController.gameFlowController.isMyTurn = false;
                            EndTurn();
                        }
                        else {
                            GameFlowController.gameFlowController.isMyTurn = true;
                            StartTurn();
                        }
                        break;
                    case "endmatch":
                        break;
                }

                nextExecMove++;
            }
            else if (controllableUnits.Count == 0) {
                EndMatch(false);
            }
            else if (otherPlayerUnits.Count == 0) {
                EndMatch(true);
            }
            else if (!waitingForServer && GameFlowController.gameFlowController.isMyTurn && GameFlowController.gameFlowController.turnState == GameFlowController.TurnState.PlayerInput) {
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
                                                CanvasRefs.canvasRefs.fireUI.SetActive(true);
                                                selectedUnit.FaceTowards(hitTile.tile);
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

                                                hitChanceText.text = string.Format("Hit: {0:p}", Mathf.Clamp01(hitChance));
                                                switch (highestUnflankedCover) {
                                                    case CoverType.None:
                                                        coverText.text = "";
                                                        break;
                                                    case CoverType.Half:
                                                        coverText.color = lowCoverTextColor;
                                                        coverText.text = "LOW COVER";
                                                        break;
                                                    case CoverType.Full:
                                                        coverText.color = highCoverTextColor;
                                                        coverText.text = "HIGH COVER";
                                                        break;
                                                }
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
                                    int movesTaken = selectedUnit.NumMovesToTile(hitTile.tile);
                                    bool move = movesTaken <= selectedUnit.numActions;
                                    if (selectedUnit.takesTile && hitTile.tile.ContainsBlockingAnything()) {
                                        move = false;
                                    }
                                    if (move) {
                                        string serverMove = "";
                                        serverMove += "m,";
                                        serverMove += selectedUnit.dict_id + ",";
                                        serverMove += hitTile.tile.x + ":" + hitTile.tile.z + ":" + hitTile.tile.h;
                                        PostMove(serverMove);
                                    }
                                }
                            }
                        }

                        if (selectedUnit != null && unitSelectionIndicator != null) {
                            unitSelectionIndicator.transform.position = selectedUnit.transform.position;
                            moveCircle1.SetActive(selectedUnit.numActions >= 1);
                            moveCircle2.SetActive(selectedUnit.numActions >= 2);
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

            if (serverPingCounter <= 0) {
                GameFlowController.gameFlowController.getMove(nextListMove);
                serverPingCounter = serverPingInterval;
            }
            else {
                serverPingCounter -= Time.deltaTime;
            }

            if (waitingForServer) {
                timeWaitingForServer += Time.deltaTime;
                if (timeWaitingForServer > serverResendInterval) {
                    if (numResends < maxResendCount) {
                        timeWaitingForServer = 0;
                        numResends++;
                        Debug.Log("No response. Resending...");
                        ResendMove();
                    }
                    else {
                        // TODO: display connection issues
                        Debug.LogError("Connection to server lost!");
                        this.enabled = false;
                    }
                }
            }
        }

        public void FireButton() {
            if (targetedUnit.status != Unit.Status.Dead
                && selectedUnit.numActions > 0) {

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
                serverMove += "a,";
                serverMove += selectedUnit.dict_id + ",";
                serverMove += targetedUnit.tile.x + ":" + targetedUnit.tile.z + ":" + targetedUnit.tile.h + ",";
                serverMove += (hit ? "h," : "m,");
                serverMove += damage.ToString();

                targetedUnit = null;
                CanvasRefs.canvasRefs.fireUI.SetActive(false);
                playerControlState = PlayerControlState.UnitSelect;

                PostMove(serverMove);
            }
        }

        public void EndTurnButton() {
            CanvasRefs.canvasRefs.EndTurnButton.SetActive(false);
            CanvasRefs.canvasRefs.EnemyTurnIndicator.SetActive(true);
            GameFlowController.gameFlowController.turnState = GameFlowController.TurnState.EnemyTurn;
            endTurnUI.SetActive(false);
            unitSelectionIndicator.transform.position = new Vector3(1000, 0, 0);
            PostMove("endturn,voluntary");
            CanvasRefs.canvasRefs.fireUI.SetActive(false);
        }

        public void EndTurn() {
            CanvasRefs.canvasRefs.EndTurnButton.SetActive(false);
            CanvasRefs.canvasRefs.EnemyTurnIndicator.SetActive(true);
            GameFlowController.gameFlowController.turnState = GameFlowController.TurnState.EnemyTurn;
            endTurnUI.SetActive(false);
            unitSelectionIndicator.transform.position = new Vector3(1000, 0, 0);
            selectedUnit = null;
            for (int i = 0; i < otherPlayerUnits.Count; i++) {
                otherPlayerUnits[i].numActions = otherPlayerUnits[i].actionsPerTurn;
            }
            for (int i = 0; i < controllableUnits.Count; i++) {
                controllableUnits[i].numActions = 0;
            }
        }

        public void StartTurn() {
            CanvasRefs.canvasRefs.EndTurnButton.SetActive(true);
            CanvasRefs.canvasRefs.EnemyTurnIndicator.SetActive(false);
            GameFlowController.gameFlowController.turnState = GameFlowController.TurnState.PlayerInput;
            endTurnUI.SetActive(true);
            for (int i = 0; i < controllableUnits.Count; i++) {
                controllableUnits[i].numActions = controllableUnits[i].actionsPerTurn;
            }
            for (int i = 0; i < otherPlayerUnits.Count; i++) {
                otherPlayerUnits[i].numActions = 0;
            }
        }

        public void PostMove(string moveInfo) {
            string rubbish = Hash128.Compute(System.DateTime.Now.ToString()).ToString();
            moveInfo += "," + rubbish;
            GameFlowController.gameFlowController.addMove(moveInfo);
            serverPingCounter = 0.5f * serverPingInterval;
            waitingForServer = true;
            lastSentMove = moveInfo;
        }

        public void ResendMove() {
            GameFlowController.gameFlowController.addMove(lastSentMove);
            serverPingCounter = 0.5f * serverPingInterval;
        }

        public void EndMatch(bool win = false) {
            matchEnded = true;
            GameFlowController.gameFlowController.addMove(win ? "victory" : "defeat");
            CanvasRefs.canvasRefs.ExitMatchPanel.SetActive(true);
            CanvasRefs.canvasRefs.ExitMatchPanel.GetComponentInChildren<TMPro.TextMeshProUGUI>().text = win ? "VICTORY!" : "DEFEAT!";
            CanvasRefs.canvasRefs.ExitMatchPanel.GetComponentInChildren<TMPro.TextMeshProUGUI>().color = win ? victoryTextColor : defeatTextColor;
        }

        public async void EndMatchSwitchScene() {
            MenuView.PlayerAccount.loadDataAndLoadSoldierInfo new_instance1 = new MenuView.PlayerAccount.loadDataAndLoadSoldierInfo();
            new_instance1.loadDataInfo.output = MenuView.PlayerAccount.currentPlayer;
            new_instance1.loadDataInfo.userID = MenuView.PlayerAccount.currentPlayer.userName;
            MenuView.PlayerAccount.currentPlayer.dataLoaded = false;
            Thread loadDataThread = new Thread(new ParameterizedThreadStart(MenuView.PlayerAccount.LoadData_Thread));
            loadDataThread.Start(new_instance1);
            System.Threading.CancellationToken cancel4 = new CancellationToken();
            for (int i = 0; i < 10; i++)
            {
                if (new_instance1.complete) break;

                await System.Threading.Tasks.Task.Delay(1000, cancel4);
                if (cancel4.IsCancellationRequested) break;
            };
            if (new_instance1.loadDataInfo.output == null)
            {
                Debug.Log("Load new data unsuccessful");
            }
            else
            {
                MenuView.PlayerAccount.currentPlayer = new_instance1.loadDataInfo.output;
            }
            UnityEngine.SceneManagement.SceneManager.LoadScene("Main Menu");
        }
    }
}
