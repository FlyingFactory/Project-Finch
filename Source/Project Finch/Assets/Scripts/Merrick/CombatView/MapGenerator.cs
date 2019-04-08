﻿using System.Collections;
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
            for (int i = 0; i < 5; i += 2) {
                ActionUnit newUnit = Instantiate(unitPrefab);
                RegisterObjectTile(newUnit, MapInfo.currentMapInfo.bottomLayer[0, i]);
                PlayerOrdersController.playerOrdersController.controllableUnits.Add(newUnit);
            }

            PassiveUnit unitPrefab2 = Resources.Load<PassiveUnit>("Prefabs/Testing/Testdummy");
            EnvObject halfCoverPrefab = Resources.Load<EnvObject>("Prefabs/HalfCoverCube");
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
                            newCover.tiles.Add(halfCoverTile);
                            halfCoverTile.occupyingObjects.Add(newCover);
                            newCover.transform.position = new Vector3(halfCoverTile.x, halfCoverTile.h, halfCoverTile.z);
                            // add cover information to surrounding tiles
                            Tile tileGivenCover;
                            if (halfCoverTile.x - 1 >= 0) {
                                tileGivenCover = MapInfo.currentMapInfo.bottomLayer[halfCoverTile.x - 1, halfCoverTile.z].top;
                                if (!tileGivenCover.ContainsBlockingEnvironment()) {
                                    tileGivenCover.Cover[(int)Direction.X].Add(CoverType.Half);
                                }
                            }
                            if (halfCoverTile.z - 1 >= 0) {
                                tileGivenCover = MapInfo.currentMapInfo.bottomLayer[halfCoverTile.x, halfCoverTile.z - 1].top;
                                if (!tileGivenCover.ContainsBlockingEnvironment()) {
                                    tileGivenCover.Cover[(int)Direction.Z].Add(CoverType.Half);
                                }
                            }
                            if (halfCoverTile.x + 1 < 32) {
                                tileGivenCover = MapInfo.currentMapInfo.bottomLayer[halfCoverTile.x + 1, halfCoverTile.z].top;
                                if (!tileGivenCover.ContainsBlockingEnvironment()) {
                                    tileGivenCover.Cover[(int)Direction.minusX].Add(CoverType.Half);
                                }
                            }
                            if (halfCoverTile.z + 1 < 32) {
                                tileGivenCover = MapInfo.currentMapInfo.bottomLayer[halfCoverTile.x, halfCoverTile.z + 1].top;
                                if (!tileGivenCover.ContainsBlockingEnvironment()) {
                                    tileGivenCover.Cover[(int)Direction.minusZ].Add(CoverType.Half);
                                }
                            }
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
        public static void RegisterObjectTile(EnvObject envObject, Tile tile) {
            if (envObject != null && tile != null) {
                tile.occupyingObjects.Add(envObject);
                envObject.tiles.Add(tile);
                if (!envObject.takesMultipleTiles) envObject.transform.position = new Vector3(tile.x, tile.h, tile.z);
            }
        }
    }
}