using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MenuView {

    public class Mutation : PurchasableItem {

        public Mutation(string id,
                        System.DateTime dateAcquired,
                        string name,
                        Acquisition acquisitionMethod,
                        float purchasePrice,
                        bool sellable)
                        : base(id, dateAcquired, name, acquisitionMethod, purchasePrice, sellable) {
            // TODO
        }
    }
}