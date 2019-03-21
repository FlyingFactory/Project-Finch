using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CombatView {

    public class TileEffector : MonoBehaviour {

        public Tile tile = new Tile();
        public ActionUnit TEST_StartingUnit;

        private void Start() {
            MapGenerator.RegisterObjectTile(TEST_StartingUnit, tile);
            PlayerOrdersController.playerOrdersController.controllableUnits.Add(TEST_StartingUnit);
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

        public List<FieldObject> occupyingObjects = new List<FieldObject>();

        public Tile above = null;
        public Tile top {
            get {
                if (above == null) return this;
                else return above.top;
            }
        }
        public Tile below = null;
        public Tile bottom {
            get {
                if (below == null) return this;
                else return below.bottom;
            }
        }
        public List<TileAccess> Neighbours = new List<TileAccess>();

        public List<CoverType>[] Cover = new List<CoverType>[4];

        public CoverType getCover(Direction d) {
            CoverType r = CoverType.None;
            foreach (CoverType c in Cover[(int)d]) {
                if (r < c) r = c;
            }
            return r;
        }

        public Tile(int x = 0, int z = 0, int h = 0) {
            this.x = x;
            this.z = z;
            this.h = h;
        }

        public static float DistanceBetween(Tile a, Tile b) {
            return Mathf.Sqrt(Mathf.Pow(b.x - a.x, 2) + Mathf.Pow(b.z - a.z, 2));
        }
    }

    public struct TileAccess {
        Tile tile;
        MovementType access;
    }

    public enum MovementType {
        Inaccessible,
        // Abyss, // flight only
        Walk,
        ClimbLedge,
        DropLedge,
        ClimbLadder,
        DropLadder,
        ClimbRope,
        DropRope,
        DropCliff,
        JumpCliff, // requires upgrade
        // Teleport,
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
