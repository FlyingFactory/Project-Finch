using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MenuView {

    public class Soldier {

        public string name;
        public string owner;
        public string uniqueId; //index is unique id
        public int maxHealth;
        public int mobility;
        public int aim;
        public int level;
        public float experience;
        public float fatigue;
        public bool complete;
        //public CharacterClass characterClass;
        //public Dictionary<string, Training> trainings = new Dictionary<string, Training>();
        //public Dictionary<string, Mutation> mutations = new Dictionary<string, Mutation>();
        //public Dictionary<string, Equipment> equipments = new Dictionary<string, Equipment>();

        public Soldier() {
            
            this.uniqueId = "";
            this.name = "undefined";
            this.maxHealth = 6;
            this.mobility = 6;
            this.aim = 65;
            this.level = 1;
            this.experience = 0;
            this.fatigue = 0;
            
        }

        // Mutator methods
        public Soldier setName(string value) {
            // TODO: input sanitisation
            this.name = value;
            return this;
        }
        public Soldier setMaxHealth(int value) {
            // TODO: input sanitisation
            this.maxHealth = value;
            return this;
        }
        public Soldier setMobility(int value) {
            // TODO: input sanitisation
            this.mobility = value;
            return this;
        }
        public Soldier setAim(int value) {
            // TODO: input sanitisation
            this.aim = value;
            return this;
        }
        public Soldier setLevel(int value) {
            // TODO: input sanitisation
            this.level = value;
            return this;
        }
        public Soldier setExperience(float value) {
            // TODO: input sanitisation
            this.experience = value;
            return this;
        }
        public Soldier setFatigue(float value) {
            // TODO: input sanitisation
            this.fatigue = value;
            return this;
        }

        //public static Soldier fromCSV(string csvData) {
        //    // TODO: read csv and fill values
        //    return new Soldier();
        //}
        
        public CombatView.ActionUnit ToActionUnit() {
            // TODO
            // Like ToString hur hur don't laugh at my naming
            return null;
        }

        public enum CharacterClass {
            Ranger,
            Vanguard,
            Gunslinger,
            Technician,
        }
    }
}