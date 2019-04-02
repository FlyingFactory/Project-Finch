using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MenuView {

    public class PlayerAccount {

        public string username;
        public byte[] passwordHash;
        public float unrankedMMR;
        public float rankedMMR;
        // future getonly property to get rank name from MMR
        public List<OwnableItem> items = new List<OwnableItem>();
        public List<Soldier> soldiers;
    }
}