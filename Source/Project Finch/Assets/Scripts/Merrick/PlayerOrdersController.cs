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

    private void Start() {
        if (playerOrdersController != null) Destroy(playerOrdersController);
            playerOrdersController = this;
        }

        private void Update() {
            if (Input.GetButtonDown("select")) {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;
                if (Physics.Raycast(ray, out hit, 100, LayerMask.GetMask("Click"))) {

                    GameObject hitobj = hit.collider.transform.parent.gameObject;

                    if (playerControlState == PlayerControlState.UnitSelect) {
                        if (hitobj.GetComponent<ActionUnit>() != null) {
                            ActionUnit clickedUnit = hitobj.GetComponent<ActionUnit>();
                            //if (controllableUnits.Contains(clickedUnit)) {
                                selectedUnit = clickedUnit;
                            //}
                        }
                        else if (selectedUnit != null && hitobj.GetComponent<TileEffector>() != null) {
                            // PLACEHOLDER
                            selectedUnit.transform.position = hitobj.transform.position;
                        }
                    }
                }
            }
        }
    }
}
