using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StoreEquipmentManager : MonoBehaviour
{
    public GameObject ContentPanel;
#pragma warning disable 0649
    [SerializeField] private GameObject EquipmentListItemPrefab;
#pragma warning restore 0649

    private string[] itemnumber = { "1", "2", "3", "4", "5" };

    private List<GameItemWeapon> _weapons = new List<GameItemWeapon>
            {
                new GameItemWeapon{ Id = 0, Type = ItemType.Weapon, Name = "Rifle", Description = "A gun.", ImageName = "EquipmentGuns/Weapon1",
                                   Cost = 300, DamageModifier = 1.04f, AimModifier = 0.5f, MobilityModifier = 1.0f },
                new GameItemWeapon{ Id = 1, Type = ItemType.Weapon, Name = "Rifle 2", Description = "Another gun.", ImageName = "EquipmentGuns/Weapon2",
                                   Cost = 400, DamageModifier = 1.04f, AimModifier = 0.5f, MobilityModifier = 1.0f },
                new GameItemWeapon{ Id = 2, Type = ItemType.Weapon, Name = "Rifle 3", Description = "Another gun!", ImageName = "EquipmentGuns/Weapon3",
                                   Cost = 500, DamageModifier = 1.04f, AimModifier = 0.5f, MobilityModifier = 1.0f },
            };

    private void Awake()
    {
        //foreach (GameItemWeapon weapon in _weapons)
        //{
        //    GameObject storeItem = Instantiate(EquipmentListItemPrefab) as GameObject;
        //    EquipmentPlaceHolder placeHolder = storeItem.GetComponent<EquipmentPlaceHolder>();
        //    placeHolder.ItemImage.sprite = Resources.Load<Sprite>(weapon.ImageName);
        //    placeHolder.Name.text = weapon.Name;

        //    storeItem.transform.parent = ContentPanel.transform;
        //    storeItem.transform.localScale = Vector3.one;
        //    storeItem.SetActive(true);
        //}

        StartCoroutine(GenerateList());
    }

    private IEnumerator GenerateList()
    {
        yield return null;
        foreach (GameItemWeapon weapon in _weapons)
        {
            GameObject storeItem = Instantiate(EquipmentListItemPrefab) as GameObject;
            EquipmentPlaceHolder placeHolder = storeItem.GetComponent<EquipmentPlaceHolder>();
            placeHolder.Id = weapon.Id;
            placeHolder.ItemImage.sprite = Resources.Load<Sprite>(weapon.ImageName);
            placeHolder.Name.text = weapon.Name;

            storeItem.transform.parent = ContentPanel.transform;
            storeItem.transform.localScale = Vector3.one;
            storeItem.SetActive(true);
        }
    }
}
