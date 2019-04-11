using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CombatView {

    public class MapGenerator : MonoBehaviour {

        public enum Map {
            Random,
            TestingRange,
        }

        public Map map = Map.Random;

        private void Start() {
            switch (map) {
                case Map.Random:
                    GenerateRandomTerrain();
                    break;
            }
        }

        private void GenerateRandomTerrain() {

            new MapInfo().tiles = new List<Tile>();
            MapInfo.currentMapInfo.bottomLayer = new Tile[32, 32];

            for (int x = 0; x < 32; x++) {
                for (int z = 0; z < 32; z++) {
                    Tile newTile = new Tile(x, z);
                    MakePlaceholderTile(newTile);
                    MapInfo.currentMapInfo.tiles.Add(newTile);
                    MapInfo.currentMapInfo.bottomLayer[x, z] = newTile;
                }
            }

            ActionUnit unitPrefab = Resources.Load<ActionUnit>("Prefabs/Testing/Unit");
            UnitUI unitUIPrefab = Resources.Load<UnitUI>("Prefabs/UnitUI");
            for (int i = 0; i < 5; i += 2) {
                ActionUnit newUnit = Instantiate(unitPrefab);
                RegisterObjectTile(newUnit, MapInfo.currentMapInfo.bottomLayer[0, i]);
                PlayerOrdersController.playerOrdersController.controllableUnits.Add(newUnit);
                Instantiate(unitUIPrefab, CanvasRefs.canvasRefs.transform).target = newUnit;
            }

            PassiveUnit unitPrefab2 = Resources.Load<PassiveUnit>("Prefabs/Testing/Testdummy");
            EnvObject halfCoverPrefab = Resources.Load<EnvObject>("Prefabs/HalfCoverCube");
            EnvObject fullCoverPrefab = Resources.Load<EnvObject>("Prefabs/FullCoverCube");
            for (int i = 0; i < 5; i ++) {
                int x = Random.Range(0, 32);
                int z = Random.Range(0, 32);
                if (MapInfo.currentMapInfo.bottomLayer[x, z].top.occupyingObjects.Count == 0) {
                    PassiveUnit newUnit = Instantiate(unitPrefab2);
                    RegisterObjectTile(newUnit, MapInfo.currentMapInfo.bottomLayer[x, z].top);
                    if (x - 1 >= 0) {
                        Tile halfCoverTile = MapInfo.currentMapInfo.bottomLayer[x - 1, z].top;
                        if (halfCoverTile.occupyingObjects.Count == 0 && halfCoverTile.h == MapInfo.currentMapInfo.bottomLayer[x, z].top.h) {
                            EnvObject newCover = Instantiate(halfCoverPrefab);
                            RegisterObjectTile(newCover, halfCoverTile);
                        }
                    }
                    if (z - 1 >= 0) {
                        Tile fullCoverTile = MapInfo.currentMapInfo.bottomLayer[x, z - 1].top;
                        if (fullCoverTile.occupyingObjects.Count == 0 && fullCoverTile.h == MapInfo.currentMapInfo.bottomLayer[x, z].top.h) {
                            EnvObject newCover = Instantiate(fullCoverPrefab);
                            RegisterObjectTile(newCover, fullCoverTile);
                        }
                    }
                }
            }
        }

        private void MakePlaceholderTile(Tile tile) {
            TileEffector te = Instantiate(Resources.Load<TileEffector>("Prefabs/Testing/Tile"));
            te.tile = tile;
            te.transform.position = new Vector3(tile.x, tile.h, tile.z);
            te.gameObject.name = "Tile " + tile.x + ", " + tile.z;
        }

        public static void RegisterObjectTile(FieldObject fieldObject, Tile tile) {
            if (fieldObject is Unit) RegisterObjectTile(fieldObject as Unit, tile);
            else if (fieldObject is EnvObject) RegisterObjectTile(fieldObject as EnvObject, tile);
        }
        public static void RegisterObjectTile(Unit unit, Tile tile) {
            if (unit != null && tile != null) {
                tile.occupyingObjects.Add(unit);
                unit.tile = tile;
                unit.transform.position = new Vector3(tile.x, tile.h, tile.z);
            }
        }
        // This function for single-tile objects only
        public static void RegisterObjectTile(EnvObject envObject, Tile tile) {
            if (envObject != null && tile != null) {
                tile.occupyingObjects.Add(envObject);
                envObject.tiles.Add(tile);
                envObject.transform.position = new Vector3(tile.x, tile.h, tile.z);
                if (envObject.coverProvided != CoverType.None) {
                    // add cover information to surrounding tiles
                    Tile tileGivenCover;
                    if (tile.x - 1 >= 0) {
                        tileGivenCover = MapInfo.currentMapInfo.bottomLayer[tile.x - 1, tile.z].top;
                        if (!tileGivenCover.ContainsBlockingEnvironment()) {
                            tileGivenCover.Cover[(int)Direction.X].Add(envObject.coverProvided);
                        }
                    }
                    if (tile.z - 1 >= 0) {
                        tileGivenCover = MapInfo.currentMapInfo.bottomLayer[tile.x, tile.z - 1].top;
                        if (!tileGivenCover.ContainsBlockingEnvironment()) {
                            tileGivenCover.Cover[(int)Direction.Z].Add(envObject.coverProvided);
                        }
                    }
                    if (tile.x + 1 < 32) {
                        tileGivenCover = MapInfo.currentMapInfo.bottomLayer[tile.x + 1, tile.z].top;
                        if (!tileGivenCover.ContainsBlockingEnvironment()) {
                            tileGivenCover.Cover[(int)Direction.minusX].Add(envObject.coverProvided);
                        }
                    }
                    if (tile.z + 1 < 32) {
                        tileGivenCover = MapInfo.currentMapInfo.bottomLayer[tile.x, tile.z + 1].top;
                        if (!tileGivenCover.ContainsBlockingEnvironment()) {
                            tileGivenCover.Cover[(int)Direction.minusZ].Add(envObject.coverProvided);
                        }
                    }
                }
            }
        }
    }
}