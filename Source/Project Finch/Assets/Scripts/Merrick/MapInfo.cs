using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CombatView {

    public class MapInfo {

        public static MapInfo currentMapInfo = null;
        public List<Tile> Tiles;

        public MapInfo() {
            currentMapInfo = this;
        }
    }
}