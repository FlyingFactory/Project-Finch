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

            for (int i = 0; i < 5; i += 2) {
                ActionUnit newUnit = Instantiate(Resources.Load<ActionUnit>("Prefabs/Testing/Unit"));
                RegisterObjectTile(newUnit, MapInfo.currentMapInfo.bottomLayer[0, i]);
                PlayerOrdersController.playerOrdersController.controllableUnits.Add(newUnit);
            }

            for (int i = 0; i < 5; i ++) {
                int x = Random.Range(0, 32);
                int z = Random.Range(0, 32);
                if (MapInfo.currentMapInfo.bottomLayer[x, z].top.occupyingObjects.Count == 0) {
                    ActionUnit newUnit = Instantiate(Resources.Load<ActionUnit>("Prefabs/Testing/Testdummy"));
                    RegisterObjectTile(newUnit, MapInfo.currentMapInfo.bottomLayer[x, z].top);
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