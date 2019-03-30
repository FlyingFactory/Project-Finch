using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CombatView {

    public class EnvObject : FieldObject {

        public List<Tile> tiles = new List<Tile>();
        [SerializeField] public CoverType coverProvided = CoverType.None;

        new public void Start() {
            base.Start();
        }
    }
}