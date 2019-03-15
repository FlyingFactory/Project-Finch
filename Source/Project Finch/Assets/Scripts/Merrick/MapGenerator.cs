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

            for (int i = -15; i <= 15; i++) {
                for (int j = -15; j <= 15; j++) {
                    Tile t = new Tile(i, j);
                    MakePlaceholderTile(t);
                    MapInfo.currentMapInfo.tiles.Add(t);
                }
            }
        }

        private void MakePlaceholderTile(Tile tile) {
            TileEffector te = Instantiate(Resources.Load<TileEffector>("Prefabs/Testing/Tile"));
            te.tile = tile;
            te.transform.position = new Vector3(tile.x, tile.h, tile.z);
        }

        public static void RegisterObjectTile(FieldObject fieldObject, Tile tile) {
            if (fieldObject is Unit) RegisterObjectTile(fieldObject as Unit, tile);
            else if (fieldObject is EnvObject) RegisterObjectTile(fieldObject as EnvObject, tile);
        }
        public static void RegisterObjectTile(EnvObject envObject, Tile tile) {
            if (envObject != null && tile != null) {
                tile.occupyingObjects.Add(envObject);
                envObject.tiles.Add(tile);
            }
        }
        public static void RegisterObjectTile(Unit unit, Tile tile) {
            if (unit != null && tile != null) {
                tile.occupyingObjects.Add(unit);
                unit.tile = tile;
            }
        }
    }
}