﻿using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum ItemType { Weapon, Food, Ammo, Health, Armor };

public class GameItem
{
    public int Id { get; set; }
    public ItemType Type { get; set; }

    public string Name { get; set; }
    public string Description { get; set; }
    public string ImageName { get; set; }
    public int Cost { get; set; }
    public int InventoryCount { get; set; }

}

public class GameItemWeapon : GameItem
{
    public float DamageModifier { get; set; }
    public float AimModifier { get; set; }
    public float MobilityModifier { get; set; }
}

public class GameItemHealthKit : GameItem
{
    public float HealingDone { get; set; }
}

public class GameItemArmor : GameItem
{
    public float MobilityModifier { get; set; }
    public float DefenseModifier { get; set; }
}

