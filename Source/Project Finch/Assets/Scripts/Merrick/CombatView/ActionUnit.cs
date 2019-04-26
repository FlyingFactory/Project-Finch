using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CharacterClass = MenuView.Soldier.CharacterClass;

namespace CombatView {

    public class ActionUnit : Unit {

        [System.NonSerialized] public float aim = 0.65f;
        [System.NonSerialized] public int numActions = 2;
        [System.NonSerialized] public int actionsPerTurn = 2;
        [System.NonSerialized] public int mobility = 7;
        [System.NonSerialized] public int id = 0;
        [System.NonSerialized] public string dict_id = "";
        [System.NonSerialized] public CharacterClass characterClass = CharacterClass.Standard;

        // visuals
        [SerializeField] private GameObject standardWeapon = null;
        [SerializeField] private GameObject sniperWeapon = null;
        [SerializeField] private GameObject meleeWeapon = null;

        new public void Start() {
            base.Start();
            switch (characterClass) {
                case CharacterClass.Standard:
                    standardWeapon.SetActive(true);
                    break;
                case CharacterClass.Sniper:
                    sniperWeapon.SetActive(true);
                    aim += 0.1f;
                    break;
                case CharacterClass.Melee:
                    meleeWeapon.SetActive(true);
                    actionsPerTurn++;
                    if (numActions != 0) numActions++;
                    break;
            }
        }

        public void OrderMove(Tile destination) {
            if (destination != null && numActions > 0) {
                bool move = true;
                if (takesTile && destination.ContainsBlockingAnything()) {
                    Debug.LogWarning("Invalid move location!");
                    move = false;
                }
                if (move) {
                    FaceTowards(destination);
                    numActions -= NumMovesToTile(destination);
                    transform.position = new Vector3(destination.x, destination.h, destination.z);
                    tile.occupyingObjects.Remove(this);
                    tile = destination;
                    tile.occupyingObjects.Add(this);
                }
            }
        }

        public int NumMovesToTile(Tile destination) {
            return Mathf.CeilToInt((Tile.DistanceBetween(destination, tile) - 0.1f) / mobility);
        }

        public void OrderAttack(Tile destination, string outcome, int rawdamage) {

            Unit target = null;
            foreach (FieldObject f in destination.occupyingObjects) {
                if (f is ActionUnit) {
                    target = f as ActionUnit;
                    break;
                }
            }
            if (target != null) {
                FaceTowards(destination);
                numActions = 0;
                if (outcome == "h") {
                    Debug.Log("hit!");
                    target.Damage(rawdamage);
                }
                else {
                    Debug.Log("missed!");
                }
            }
            else {
                Debug.LogWarning("No target!");
            }
        }

        public struct HitChanceInfo {
            public float hitChance;
            public CoverType highestUnflankedCover;
            public HitChanceInfo(float hitChance, CoverType highestUnflankedCover) {
                this.hitChance = Mathf.Clamp01(hitChance);
                this.highestUnflankedCover = highestUnflankedCover;
            }
        }
        public HitChanceInfo CalculateHitChance(Unit target) {
            float d = Tile.DistanceBetween(tile, target.tile);
            float hitChance = aim;
            switch (characterClass) {
                case CharacterClass.Standard:
                    if (d > 10) hitChance = aim - (d - 10) / 12.5f;
                    else if (d < 6) hitChance = aim + (6 - d) / 12.5f;
                    break;
                case CharacterClass.Sniper:
                    if (d > 16) hitChance = aim - (d - 16) / 12.5f;
                    else if (d < 8) hitChance = aim - (8 - d) / 16f;
                    break;
                case CharacterClass.Melee:
                    if (d < 1.6f) hitChance = 0.95f;
                    else hitChance = 0;
                    break;
            }

            CoverType highestUnflankedCover = CoverType.None;
            if (tile.x < target.tile.x) {
                CoverType directionCover = target.tile.getCover(Direction.minusX);
                if (directionCover > highestUnflankedCover) highestUnflankedCover = directionCover;
            }
            else if (tile.x > target.tile.x) {
                CoverType directionCover = target.tile.getCover(Direction.X);
                if (directionCover > highestUnflankedCover) highestUnflankedCover = directionCover;
            }
            if (tile.z < target.tile.z) {
                CoverType directionCover = target.tile.getCover(Direction.minusZ);
                if (directionCover > highestUnflankedCover) highestUnflankedCover = directionCover;
            }
            else if (tile.z > target.tile.z) {
                CoverType directionCover = target.tile.getCover(Direction.Z);
                if (directionCover > highestUnflankedCover) highestUnflankedCover = directionCover;
            }
            hitChance -= (int)highestUnflankedCover * PlayerOrdersController.HALFCOVER_PENALTY;

            return new HitChanceInfo(hitChance, highestUnflankedCover);
        }

        public void DrawMovableLocations() {
            //TODO
        }

        public override bool OnDeath() {
            if (PlayerOrdersController.playerOrdersController.controllableUnits.Contains(this)) PlayerOrdersController.playerOrdersController.controllableUnits.Remove(this);
            else if (PlayerOrdersController.playerOrdersController.otherPlayerUnits.Contains(this)) PlayerOrdersController.playerOrdersController.otherPlayerUnits.Remove(this);
            return base.OnDeath();
        }

        public void FaceTowards(Tile t) {
            float d = Tile.DistanceBetween(t, tile);
            if (d < 0.5) return;

            if (t.x - tile.x >= 0) transform.eulerAngles = new Vector3(0, Mathf.Acos((t.z - tile.z) / d) * Mathf.Rad2Deg, 0);
            else transform.eulerAngles = new Vector3(0, -Mathf.Acos((t.z - tile.z) / d) * Mathf.Rad2Deg, 0);
        }
    }
}