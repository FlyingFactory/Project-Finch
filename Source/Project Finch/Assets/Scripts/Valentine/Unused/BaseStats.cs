using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class BaseName // Name
{
    [SerializeField]
    private string SoldierName;

    public string GetName()
    {
        return SoldierName;
    }

}

[System.Serializable]
public class BaseStats { // These are for integer stats.

    [SerializeField]
    private int BaseValue;

    public int GetValue()
    {
        return BaseValue;
    }

}


