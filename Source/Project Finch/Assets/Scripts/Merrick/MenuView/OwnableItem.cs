using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MenuView {

    public class OwnableItem {

        public string id;
        public System.DateTime dateAcquired;
        public string name;
        public bool consumable;
        
        /// <param name="name">Place an empty string to draw the default name using the id.</param>
        public OwnableItem(string id, 
                           System.DateTime dateAcquired, 
                           string name) {
            this.id = id;
            this.dateAcquired = dateAcquired;
            if (name == "") this.name = "defaultname"; // TODO: to pull stock name from item id
            else this.name = name;
            this.consumable = false; // TODO: to pull information from item id
        }
    }
}