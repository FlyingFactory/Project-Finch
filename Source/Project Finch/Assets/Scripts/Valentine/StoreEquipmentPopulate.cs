using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class StoreEquipmentPopulate : MonoBehaviour
{
#pragma warning disable 0649
    [SerializeField] private GameObject EquipmentPanel;
#pragma warning restore 0649

    private string[] itemnumber = { "1", "2", "4", "8", "20", "32", "50", "80" };
    void GenerateList()
    { 
        for (int i = 0; i < itemnumber.Length; i++)
        {
            GameObject StoreItem = Instantiate(EquipmentPanel) as GameObject;

            StoreItem.SetActive(true);

            StoreItem.GetComponent<Item>().SetName("equip_" + itemnumber[i]);

            StoreItem.transform.SetParent(EquipmentPanel.transform.parent, false);

        }
    }

    void Start()
    {
        GenerateList();
    }
    
    void Update()
    {
        
    }
}
