using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MenuView {

    public class Soldier {

        public string name;
        public int maxHealth;
        public int mobility;
        public int aim;
        public int level;
        public float experience;
        public float fatigue;
        public CharacterClass characterClass;
        public List<Training> trainings = new List<Training>();
        public List<Mutation> mutations = new List<Mutation>();
        public List<Equipment> equipments = new List<Equipment>();

        public Soldier() {
            this.name = "undefined";
            this.maxHealth = 6;
            this.mobility = 6;
            this.aim = 65;
            this.level = 1;
            this.experience = 0;
            this.fatigue = 0;
            this.characterClass = CharacterClass.Ranger;
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

        public static Soldier fromCSV(string csvData) {
            // TODO: read csv and fill values
            return new Soldier();
        }
        
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