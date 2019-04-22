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
                new GameItemWeapon{ Id = 0, Type = ItemType.Weapon, Name = "Rifle", Description = "A gun.", ImageName = "EquipmentGuns/Weapon1.png",
                                   Cost = 300, DamageModifier = 1.04f, AimModifier = 0.5f, MobilityModifier = 1.0f },
                new GameItemWeapon{ Id = 1, Type = ItemType.Weapon, Name = "Rifle 2", Description = "Another gun.", ImageName = "EquipmentGuns/Weapon2.png",
                                   Cost = 400, DamageModifier = 1.04f, AimModifier = 0.5f, MobilityModifier = 1.0f },
                new GameItemWeapon{ Id = 2, Type = ItemType.Weapon, Name = "Rifle 3", Description = "Another gun!", ImageName = "EquipmentGuns/Weapon3.png",
                                   Cost = 500, DamageModifier = 1.04f, AimModifier = 0.5f, MobilityModifier = 1.0f },
            };


    void Start()
    {
    }

    private void Awake()
    {
        StartCoroutine(GenerateList());
    }

    // every 2 seconds perform the print()
    private IEnumerator GenerateList()
    {
        yield return null;
        foreach (GameItemWeapon weapon in _weapons)
        {
            GameObject storeItem = Instantiate(EquipmentListItemPrefab) as GameObject;
            EquipmentPlaceHolder placeHolder = storeItem.GetComponent<EquipmentPlaceHolder>();
            placeHolder.ItemImage = Resources.Load<Image>(weapon.ImageName);
            placeHolder.Name.text = weapon.Name;

            storeItem.transform.parent = ContentPanel.transform;
            storeItem.transform.localScale = Vector3.one;
            storeItem.SetActive(true);
        }


        //// 2. Iterate through the data, 
        ////	  instantiate prefab, 
        ////	  set the data, 
        ////	  add it to panel
        //foreach (Animal animal in Anmimals)
        //{
        //    GameObject newAnimal = Instantiate(ListItemPrefab) as GameObject;
        //    ListItemController controller = newAnimal.GetComponent();
        //    controller.Icon.sprite = animal.Icon;
        //    controller.Name.text = animal.Name;
        //    controller.Description.text = animal.Description;
        //    newAnimal.transform.parent = ContentPanel.transform;
        //    newAnimal.transform.localScale = Vector3.one;
        //}
    }

    
        //for (int i = 0; i < itemnumber.Length; i++)
        //{
        //    GameObject StoreItem = Instantiate(EquipmentPanel) as GameObject;

        //    StoreItem.SetActive(true);

        //    StoreItem.GetComponent<EquipmentPlaceHolder>().SetName("equip_" + itemnumber[i]);
        //    StoreItem.GetComponent<EquipmentPlaceHolder>().SetImage(Resources.Load<Image>("..."));
        //    // TODO: Image SHOULD load, but it's not working as of now.

        //    StoreItem.transform.SetParent(EquipmentPanel.transform.parent, false);

        //}

    void Update()
    {

    }
}
