﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CombatView {

    public class Unit : FieldObject {

        public Tile tile;
        public Status status;
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
            currentHealth = Mathf.Max(currentHealth - amt, 0);
            return amt;
        }

        public int Heal(int amt) {
            // effects here
            currentHealth = Mathf.Min(currentHealth + amt, maxHealth);
            return amt;
        }

        /// <summary>
        /// Executes effects that happen when the unit it "supposed" to ide. Updates status if the unit really died.
        /// </summary>
        public virtual bool OnDeath() {
            // effects here
            status = Status.Dead;
            Invoke("DestroySelf", 0.2f);
            return true;
        }

        public void DestroySelf() {
            tile.occupyingObjects.Remove(this);
            Destroy(gameObject);
        }

        new public void Start() {
            base.Start();
        }

        public enum Status {
            Alive,
            Dead,
            // others
        }
    }
}