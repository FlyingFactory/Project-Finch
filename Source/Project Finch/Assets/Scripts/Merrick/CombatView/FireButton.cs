using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CombatView {

    public class FireButton : MonoBehaviour {
        
        public void Click() {
            PlayerOrdersController.playerOrdersController.FireButton();
        }
    }
}