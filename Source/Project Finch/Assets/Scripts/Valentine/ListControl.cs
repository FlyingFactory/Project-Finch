using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ListControl : MonoBehaviour
{
    [SerializeField]
    private GameObject buttonTemplate;

    void Start()
    {
       for (int i = 1; i<= 20; i++)
        {
            GameObject button = Instantiate(buttonTemplate) as GameObject;
            button.SetActive(true);

            button.GetComponent<SoldierList>().SetText("Soldier #" + i);

            button.transform.SetParent(buttonTemplate.transform.parent, false);

        }
    }


}
