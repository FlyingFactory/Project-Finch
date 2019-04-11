﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CombatView {

    public class MapGenerator : MonoBehaviour {

        public enum Map {
            TestingRange,
            RandomMultiplayer,
        }

        public Map map = Map.TestingRange;
        public bool player1 = true;
        public int seed = -294;

        private static Color allyColor = new Color32(127, 255, 255, 255);
        private static Color enemyColor = new Color32(255, 165, 165, 255);

        private void Start() {
            switch (map) {
                case Map.TestingRange:
                    GenerateRandomTerrain();
                    break;
                case Map.RandomMultiplayer:
                    GenerateRandomCoordinated(player1, seed);
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

        private void GenerateRandomCoordinated(bool player1, int seed) {
            new MapInfo().tiles = new List<Tile>();
            MapInfo.currentMapInfo.bottomLayer = new Tile[32, 32];
            System.Random r = new System.Random(seed);
            
            for (int x = 0; x < 32; x++) {
                for (int z = 0; z < 32; z++) {
                    Tile newTile = new Tile(x, z);
                    MakePlaceholderTile(newTile);
                    MapInfo.currentMapInfo.tiles.Add(newTile);
                    MapInfo.currentMapInfo.bottomLayer[x, z] = newTile;
                }
            }

            ActionUnit unitPrefab = Resources.Load<ActionUnit>("Prefabs/Testing/Unit");
            ActionUnit unitEnemyPrefab = Resources.Load<ActionUnit>("Prefabs/Testing/UnitEnemy");
            UnitUI unitUIPrefab = Resources.Load<UnitUI>("Prefabs/UnitUI");
            List<System.Tuple<int, int>> p1spawns = new List<System.Tuple<int, int>>() {
                new System.Tuple<int, int>(0, 0),
                new System.Tuple<int, int>(2, 0),
                new System.Tuple<int, int>(2, 2),
                new System.Tuple<int, int>(0, 2),
            };
            List<System.Tuple<int, int>> p2spawns = new List<System.Tuple<int, int>>() {
                new System.Tuple<int, int>(31, 31),
                new System.Tuple<int, int>(29, 31),
                new System.Tuple<int, int>(29, 29),
                new System.Tuple<int, int>(31, 29),
            };
            foreach (System.Tuple<int, int> pos in player1 ? p1spawns : p2spawns) {
                ActionUnit newUnit = Instantiate(unitPrefab);
                RegisterObjectTile(newUnit, MapInfo.currentMapInfo.bottomLayer[pos.Item1, pos.Item2]);
                PlayerOrdersController.playerOrdersController.controllableUnits.Add(newUnit);
                UnitUI u = Instantiate(unitUIPrefab, CanvasRefs.canvasRefs.transform);
                u.target = newUnit;
                u.transform.Find("Healthbar").GetComponent<UnityEngine.UI.Image>().color = allyColor;
            }
            foreach (System.Tuple<int, int> pos in player1 ? p2spawns : p1spawns) {
                ActionUnit newUnit = Instantiate(unitEnemyPrefab);
                RegisterObjectTile(newUnit, MapInfo.currentMapInfo.bottomLayer[pos.Item1, pos.Item2]);
                UnitUI u = Instantiate(unitUIPrefab, CanvasRefs.canvasRefs.transform);
                u.target = newUnit;
                u.transform.Find("Healthbar").GetComponent<UnityEngine.UI.Image>().color = enemyColor;
            }
            
            PassiveUnit unitPrefab2 = Resources.Load<PassiveUnit>("Prefabs/Testing/Testdummy");
            EnvObject halfCoverPrefab = Resources.Load<EnvObject>("Prefabs/HalfCoverCube");
            EnvObject fullCoverPrefab = Resources.Load<EnvObject>("Prefabs/FullCoverCube");

            for (int i = 0; i < 20; i++) {
                int x = r.Next(32);
                int z = r.Next(32);
                if (MapInfo.currentMapInfo.bottomLayer[x, z].top.occupyingObjects.Count == 0) {
                    EnvObject newCover = Instantiate((r.Next(2) == 0) ? halfCoverPrefab : fullCoverPrefab);
                    RegisterObjectTile(newCover, MapInfo.currentMapInfo.bottomLayer[x, z].top);
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