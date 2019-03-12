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

    public class Tile {

        public const float DIST_1_1 = 1.414213562373095048801688724209698078569671875376948073176f;
        public const float DIST_1_2 = 2.236067977499789696409173668731276235440618359611525724270f;
        public const float DIST_1_3 = 3.162277660168379331998893544432718533719555139325216826857f;
        public const float DIST_2_3 = 3.605551275463989293119221267470495946251296573845246212710f;

        public int x;
        public int z;
        public int h;

        public List<Tile> Neighbours = new List<Tile>();

        public List<CoverType>[] Cover = new List<CoverType>[4];

        public CoverType getCover(Direction d) {
            CoverType r = CoverType.None;
            foreach(CoverType c in Cover[(int)d]) {
                if (r < c) r = c;
            }
            return r;
        }
    }

    public enum CoverType {
        None,
        Half,
        Full
    }

    public enum Direction {
        X,
        Z,
        minusX,
        minusZ
    }
}