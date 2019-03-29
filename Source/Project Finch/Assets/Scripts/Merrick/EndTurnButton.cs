using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CombatView {

    public class EndTurnButton : MonoBehaviour {

        public void Click() {
            PlayerOrdersController.playerOrdersController.EndTurnButton();
        }
    }
}