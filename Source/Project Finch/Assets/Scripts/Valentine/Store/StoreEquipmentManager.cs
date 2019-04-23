using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StoreEquipmentManager : MonoBehaviour
{
    public GameObject ContentPanel;
#pragma warning disable 0649
    [SerializeField] private GameObject EquipmentItemPrefab;
#pragma warning restore 0649

    private List<GameItemWeapon> _weapons = new List<GameItemWeapon>
            {
                new GameItemWeapon{ Id = 0, Type = ItemType.Weapon, Name = "Rifle 1", Description = "A gun.", ImageName = "EquipmentGuns/Weapon1",
                                   Cost = 300, DamageModifier = 1.05f, AimModifier = 0.8f, MobilityModifier = 1.0f },
                new GameItemWeapon{ Id = 1, Type = ItemType.Weapon, Name = "Rifle 2", Description = "Entry-level weapon.", ImageName = "EquipmentGuns/Weapon2",
                                   Cost = 400, DamageModifier = 1.10f, AimModifier = 0.9f, MobilityModifier = 0.95f },
                new GameItemWeapon{ Id = 2, Type = ItemType.Weapon, Name = "Rifle 3", Description = "Well, it's a gun!", ImageName = "EquipmentGuns/Weapon3",
                                   Cost = 500, DamageModifier = 1.06f, AimModifier = 1.0f, MobilityModifier = 0.90f },
                new GameItemWeapon{ Id = 2, Type = ItemType.Weapon, Name = "Rifle 4", Description = "Another one!", ImageName = "EquipmentGuns/Weapon4",
                                   Cost = 550, DamageModifier = 1.04f, AimModifier = 1.2f, MobilityModifier = 1.05f },
                new GameItemWeapon{ Id = 2, Type = ItemType.Weapon, Name = "Rifle 5", Description = "Might be the most expensive one!", ImageName = "EquipmentGuns/Weapon5",
                                   Cost = 600, DamageModifier = 0.80f, AimModifier = 1.1f, MobilityModifier = 0.80f },
            };

    private List<GameItemHealthKit> _healthkits = new List<GameItemHealthKit>
            {
                new GameItemHealthKit {Id = 0, Type = ItemType.Health, Name = "Small Health Kit", Description = "Heals a small amount.",ImageName = "EquipmentHealth/Health-Pack-1",
                                   Cost = 100, HealingDone = 50},
                new GameItemHealthKit {Id = 1, Type = ItemType.Health, Name = "Medium Health Kit", Description = "Heals a moderate amount.",ImageName = "EquipmentHealth/Health-Pack-2",
                                   Cost = 150, HealingDone = 100},
                new GameItemHealthKit {Id = 2, Type = ItemType.Health, Name = "Large Health Kit", Description = "Heals a large amount.",ImageName = "EquipmentHealth/Health-Pack-3",
                                   Cost = 200, HealingDone = 150},
            };

    private void Awake()
    {

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
            placeHolder.DamageModifier.text = (weapon.DamageModifier*100).ToString() + "%";
            placeHolder.AimModifier.text = (weapon.AimModifier*100).ToString() + "%";
            placeHolder.MobilityModifier.text = (weapon.MobilityModifier*100).ToString() + "%";

            storeItem.transform.parent = ContentPanel.transform;
            storeItem.transform.localScale = Vector3.one;
            storeItem.SetActive(true);
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

            storeItem.transform.parent = ContentPanel.transform;
            storeItem.transform.localScale = Vector3.one;
            storeItem.SetActive(true);
            
        }
    }

    private IEnumerator OnHealthPackInfoClick()
    {
        yield return null;
        //GameObject HealthPackPanel = Instantiate(HealthPack) as GameObject;
        //HealthPackInfo healthPackInfo = HealthPack.GetComponent<HealthPackInfo>();
        //healthPackInfo.HealingDone = healthkit.HealingDone;
    }
}
