using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CombatView {

    public class Testdummy : PassiveUnit {
        
        public override bool OnDeath() {
            transform.GetChild(0).Translate(new Vector3(0, -0.7f, 0));
            Invoke("Respawn", 2.5f);
            return true;
        }

        public void Respawn() {
            transform.GetChild(0).Translate(new Vector3(0, 0.7f, 0));
            currentHealth = maxHealth;
        }
    }
}