using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MenuView {

    public class PurchasableItem : OwnableItem {

        public Acquisition acquisitionMethod;
        public float purchasePrice;
        public bool sellable;

        /// <param name="name">Place an empty string to draw the default name using the id.</param>
        public PurchasableItem(string          id,
                               System.DateTime dateAcquired,
                               string          name,
                               Acquisition     acquisitionMethod,
                               float           purchasePrice,
                               bool            sellable)
                               : base(id, dateAcquired, name) {
            this.acquisitionMethod = acquisitionMethod;
            this.purchasePrice = purchasePrice;
            this.sellable = sellable;
        }
    }

    public enum Acquisition {
        Purchase,
        PurchaseDiscount,
        Free,
        FreeLimited,
    }
}