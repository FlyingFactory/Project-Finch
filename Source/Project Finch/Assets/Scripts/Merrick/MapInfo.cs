using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CombatView {

    public class MapInfo {

        public static MapInfo currentMapInfo = null;
        public List<Tile> tiles = new List<Tile>();

        public MapInfo() {
            currentMapInfo = this;
        }
    }
}