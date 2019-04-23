using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EquipmentPlaceHolder : MonoBehaviour
{
    public int Id { get;  set; }

    public Text Name;
    public Image ItemImage;

    public Text Description;
    public Text Price;
    public Text Type;

    public Button BuyButton;

    public Text DamageModifier;
    public Text AimModifier;
    public Text MobilityModifier;

    public Text HealingDone;
}

