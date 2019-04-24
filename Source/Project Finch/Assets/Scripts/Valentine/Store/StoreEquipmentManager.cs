using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StoreEquipmentManager : MonoBehaviour
{
    public GameObject ContentPanel;
    public Text CreditsLeftText;


#pragma warning disable 0649
    [SerializeField] private GameObject EquipmentItemPrefab;
#pragma warning restore 0649

    private double _creditsLeft = 5000; // To be replaced with user's own credit values soon.


    private List<GameItemWeapon> _weapons = new List<GameItemWeapon>
            {
                new GameItemWeapon{ Id = 0, Type = ItemType.Weapon, Name = "Rifle 1", Description = "A gun.", ImageName = "EquipmentGuns/Weapon1",
                                   Cost = 300, DamageModifier = 1.05f, AimModifier = 0.8f, MobilityModifier = 1.0f , InventoryCount = 4},
                new GameItemWeapon{ Id = 1, Type = ItemType.Weapon, Name = "Rifle 2", Description = "Entry-level weapon.", ImageName = "EquipmentGuns/Weapon2",
                                   Cost = 400, DamageModifier = 1.10f, AimModifier = 0.9f, MobilityModifier = 0.95f , InventoryCount = 0 },
                new GameItemWeapon{ Id = 2, Type = ItemType.Weapon, Name = "Rifle 3", Description = "Well, it's a gun!", ImageName = "EquipmentGuns/Weapon3",
                                   Cost = 500, DamageModifier = 1.06f, AimModifier = 1.0f, MobilityModifier = 0.90f , InventoryCount = 0 },
                new GameItemWeapon{ Id = 2, Type = ItemType.Weapon, Name = "Rifle 4", Description = "Another one!", ImageName = "EquipmentGuns/Weapon4",
                                   Cost = 550, DamageModifier = 1.04f, AimModifier = 1.2f, MobilityModifier = 1.05f , InventoryCount = 0 },
                new GameItemWeapon{ Id = 2, Type = ItemType.Weapon, Name = "Rifle 5", Description = "Might be the most expensive one!", ImageName = "EquipmentGuns/Weapon5",
                                   Cost = 600, DamageModifier = 0.80f, AimModifier = 1.1f, MobilityModifier = 0.80f , InventoryCount = 0 },
            };

    private List<GameItemHealthKit> _healthkits = new List<GameItemHealthKit>
            {
                new GameItemHealthKit {Id = 0, Type = ItemType.Health, Name = "Small Health Kit", Description = "Heals a small amount.",ImageName = "EquipmentHealth/Health-Pack-1",
                                   Cost = 100, HealingDone = 50 , InventoryCount = 2},
                new GameItemHealthKit {Id = 1, Type = ItemType.Health, Name = "Medium Health Kit", Description = "Heals a moderate amount.",ImageName = "EquipmentHealth/Health-Pack-2",
                                   Cost = 150, HealingDone = 100 , InventoryCount = 1},
                new GameItemHealthKit {Id = 2, Type = ItemType.Health, Name = "Large Health Kit", Description = "Heals a large amount.",ImageName = "EquipmentHealth/Health-Pack-3",
                                   Cost = 200, HealingDone = 150 , InventoryCount = 0},
            };

    private List<GameItemArmor> _armors = new List<GameItemArmor>
            {
                new GameItemArmor {Id = 0, Type = ItemType.Armor, Name = "Light Armor", Description = "Not very protective, but gets the job done.",ImageName = "EquipmentArmor/Light_Armor",
                                   Cost = 250, MobilityModifier = 1.00f, DefenseModifier = 1.05f , InventoryCount = 10},
                new GameItemArmor {Id = 0, Type = ItemType.Armor, Name = "Medium Armor", Description = "Your normal armor.",ImageName = "EquipmentArmor/Medium_Armor",
                                   Cost = 300, MobilityModifier = 0.95f, DefenseModifier = 1.15f , InventoryCount = 0},
                new GameItemArmor {Id = 0, Type = ItemType.Armor, Name = "Heavy Armor", Description = "A little heavy but it protects you well.",ImageName = "EquipmentArmor/Heavy_Armor",
                                   Cost = 400, MobilityModifier = 0.90f, DefenseModifier = 1.30f , InventoryCount = 0},
                new GameItemArmor {Id = 0, Type = ItemType.Armor, Name = "Titan Armor", Description = "Slows you down a lot more, but you're as tough as nails.",ImageName = "EquipmentArmor/Titan_Armor",
                                   Cost = 500, MobilityModifier = 0.80f, DefenseModifier = 1.50f , InventoryCount = 0},

            };

    // Inventory Count is 0 for all as initialised/hard-coded, but we will need to use the database if we want it for each soldier.

    private void Awake()
    {
        CreditsLeftText.text = _creditsLeft.ToString("#,##0");
        StartCoroutine(GenerateList());
    }

    private IEnumerator GenerateList()
    {
        yield return null;
        foreach (GameItemWeapon weapon in _weapons)
        {
            GameObject storeItem = Instantiate(EquipmentItemPrefab) as GameObject;
            EquipmentPlaceHolder placeHolder = storeItem.GetComponent<EquipmentPlaceHolder>();
            placeHolder.Id = weapon.Id;
            placeHolder.ItemImage.sprite = Resources.Load<Sprite>(weapon.ImageName);

            placeHolder.Name.text = weapon.Name;
            placeHolder.Price.text = weapon.Cost.ToString();
            placeHolder.Type.text = weapon.Type.ToString();
            placeHolder.Description.text = weapon.Description;

            placeHolder.Trait1Text.text = "Damage Modifier:";
            placeHolder.Trait1Value.text = (weapon.DamageModifier*100).ToString() + "%";

            placeHolder.Trait2Text.text = "Aim Modifier:";
            placeHolder.Trait2Value.text = (weapon.AimModifier*100).ToString() + "%";

            placeHolder.Trait3Text.text = "Mobility Modifier:";
            placeHolder.Trait3Value.text = (weapon.MobilityModifier*100).ToString() + "%";

            placeHolder.InventoryCountValue.text = weapon.InventoryCount.ToString();
            // Should be on how much item each soldier have of each equipment.

            storeItem.transform.parent = ContentPanel.transform;
            storeItem.transform.localScale = Vector3.one;
            storeItem.SetActive(true);

            // Attach the listener, but check if you have the equipment already.
            if (weapon.InventoryCount == 0) {
                placeHolder.BuyButton.onClick.AddListener(() =>
                SinglePurchase(placeHolder, weapon.Cost, weapon.InventoryCount));
            }

            else
            {
                placeHolder.BuyButton.interactable = false;
            }
            
        }

        foreach (GameItemHealthKit healthkit in _healthkits)
        {
            GameObject storeItem = Instantiate(EquipmentItemPrefab) as GameObject;
            EquipmentPlaceHolder placeHolder = storeItem.GetComponent<EquipmentPlaceHolder>();
            placeHolder.Id = healthkit.Id;
            placeHolder.ItemImage.sprite = Resources.Load<Sprite>(healthkit.ImageName);

            placeHolder.Name.text = healthkit.Name;
            placeHolder.Price.text = healthkit.Cost.ToString();
            placeHolder.Type.text = healthkit.Type.ToString();
            placeHolder.Description.text = healthkit.Description;

            placeHolder.Trait1Text.text = "Healing Done:";
            placeHolder.Trait1Value.text = healthkit.HealingDone.ToString();

            placeHolder.InventoryCountValue.text = healthkit.InventoryCount.ToString();

            storeItem.transform.parent = ContentPanel.transform;
            storeItem.transform.localScale = Vector3.one;
            storeItem.SetActive(true);

            // Attach the listener. Healthkit is unique because you can have up to 3.
            if (healthkit.InventoryCount <= 3)
            {
                placeHolder.BuyButton.onClick.AddListener(() =>
                PurchaseHealthKit(placeHolder, healthkit.Cost, healthkit.InventoryCount));
            }

            else
            {
                placeHolder.BuyButton.interactable = false;
            }

        }

        foreach (GameItemArmor armor in _armors)
        {
            GameObject storeItem = Instantiate(EquipmentItemPrefab) as GameObject;
            EquipmentPlaceHolder placeHolder = storeItem.GetComponent<EquipmentPlaceHolder>();

            placeHolder.Id = armor.Id;
            placeHolder.ItemImage.sprite = Resources.Load<Sprite>(armor.ImageName);


            placeHolder.Name.text = armor.Name;
            placeHolder.Price.text = armor.Cost.ToString();
            placeHolder.Type.text = armor.Type.ToString();
            placeHolder.Description.text = armor.Description;

            placeHolder.Trait1Text.text = "Mobility Modifier:";
            placeHolder.Trait1Value.text = (armor.MobilityModifier * 100).ToString() + "%";

            placeHolder.Trait2Text.text = "Defense Modifier:";
            placeHolder.Trait2Value.text = (armor.DefenseModifier * 100).ToString() + "%";

            placeHolder.InventoryCountValue.text = armor.InventoryCount.ToString();

            // Attach the listener, but check if you have the equipment already.
            if (armor.InventoryCount == 0)
            {
                placeHolder.BuyButton.onClick.AddListener(() =>
                SinglePurchase(placeHolder, armor.Cost, armor.InventoryCount));
            }

            else
            {
                placeHolder.BuyButton.interactable = false;
            }

            storeItem.transform.parent = ContentPanel.transform;
            storeItem.transform.localScale = Vector3.one;
            storeItem.SetActive(true);
        }
    }



    // This is ONLY for those equipments that you can only buy once.
    public void SinglePurchase(EquipmentPlaceHolder placeHolder, float cost, int InventoryCount)
    {
        if (_creditsLeft >= cost && InventoryCount == 0)
        {
            _creditsLeft -= cost;
            InventoryCount += 1;
            CreditsLeftText.text = _creditsLeft.ToString("#,##0");
            placeHolder.InventoryCountValue.text = InventoryCount.ToString();
            placeHolder.BuyButton.interactable = false;
        }

        else
        {
            Debug.Log("Unable to buy! Insufficient funds.");
        }
    }

    // This is ONLY for Healthpacks. You can buy up to 3.
    public void PurchaseHealthKit(EquipmentPlaceHolder placeHolder, float cost, int InventoryCount)
    {
        if (_creditsLeft >= cost && InventoryCount == 2)
        {
            _creditsLeft -= cost;
            InventoryCount += 1;
            CreditsLeftText.text = _creditsLeft.ToString("#,##0");
            placeHolder.InventoryCountValue.text = InventoryCount.ToString();
            placeHolder.BuyButton.interactable = false;
        }

        else if (_creditsLeft >= cost && InventoryCount < 3)
        {
            _creditsLeft -= cost;
            InventoryCount += 1;
            CreditsLeftText.text = _creditsLeft.ToString("#,##0");
            placeHolder.InventoryCountValue.text = InventoryCount.ToString();
        }

        else
        {
            Debug.Log("Unable to buy! Insufficient funds.");
        }
    }

}
