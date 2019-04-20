using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum ItemType { Food, Medicine, Drink, Weapon, Ammo, FirstAid };

public class GameItemGeneral
{
    public int id;
    public ItemType itemType;

    public string name;
    public string description;
    public string imageName;

    public float price;

    // Other modifiers need to be added, but should we separate them by type?
    // After all, each type will have different modifiers.
}

public class GameItemWeapon
{
    public int id;
    public ItemType Weapon;

    public string name;
    public string description;
    public string imageName;

    public float price;

    public float damageModifier;
    public float aimModifier;
    public float mobilityModifier;

}

public class GameItemFood
{
    public int id;
    public ItemType Food;

    public string name;
    public string description;
    public string imageName;

    public float price;

    public float HPIncrease;

}

public class Item : MonoBehaviour
{
    public Text Name;
    public Image ItemImage;

    public void SetName(string textString)
    {
        Name.text = textString;
    }

    public void SetImage(Sprite ImageName)
    {
        ItemImage.sprite = ImageName;
    }
    
}
