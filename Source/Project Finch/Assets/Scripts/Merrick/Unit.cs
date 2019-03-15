using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CombatView {

    public class Unit : FieldObject {

        public Tile tile;
        private int _health = 10;
        /// <summary>
        /// Do NOT change this directly unless you want to bypass normal effects.
        /// </summary>
        public int currentHealth {
            get { return _health; }
            set {
                _health = Mathf.Clamp(value, 0, maxHealth);
                if (_health <= 0) OnDeath();
            }
        }
        public int maxHealth = 10;

        public int Damage(int amt) {
            // effects here
            currentHealth = Mathf.Min(currentHealth + amt, maxHealth);
            return amt;
        }

        public int Heal(int amt) {
            // effects here
            currentHealth = Mathf.Max(currentHealth - amt, 0);
            return amt;
        }

        /// <summary>
        /// Executes effects that happen when the unit it "supposed" to ide. Returns true if the unit really died.
        /// </summary>
        public bool OnDeath() {
            // effects here
            Invoke("DestroySelf", 0.2f);
            return true;
        }

        public void DestroySelf() {
            Destroy(gameObject);
        }

        new public void Start() {
            base.Start();
        }
    }
}