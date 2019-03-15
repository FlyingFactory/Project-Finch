using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CombatView {

    public class EnvObject : FieldObject {

        public List<Tile> tiles = new List<Tile>();

        new public void Start() {
            base.Start();
        }
    }
}