using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class FuelOrderManager : MonoBehaviour
{
    [SerializeField]private TMP_InputField quantityText;
    [SerializeField]private TMP_Dropdown substanceDropdown;
    public GameObject selectedDestination;

    public string quantity;
    public string substance;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void selectDestination()
    {
        buildingType[] tanks = FindObjectsOfType<buildingType>();
        foreach(buildingType tank in tanks)
        {
            if(tank.type == "GSEtank")
            {
                tank.selectUI.SetActive(true);
            }
        }

    }

    public void addFuel()
    {
        if(quantity != null)
        {
            if(substanceDropdown.value == 0)
            {
                //Kerosene
                float substanceMolarMass = 170f;
                float moles = float.Parse(quantityText.text.ToString())/substanceMolarMass * 1000;
                selectedDestination.GetComponent<outputInputManager>().substance = "kerosene";
                selectedDestination.GetComponent<outputInputManager>().moles = moles;
                selectedDestination.GetComponent<outputInputManager>().internalTemperature = selectedDestination.GetComponent<outputInputManager>().externalTemperature;
            }

            if(substanceDropdown.value == 1)
            {
                //Oxygen
                float substanceMolarMass = 32f;
                float moles = float.Parse(quantityText.text.ToString())/substanceMolarMass * 1000;
                selectedDestination.GetComponent<outputInputManager>().substance = "LOX";
                selectedDestination.GetComponent<outputInputManager>().moles = moles;
                selectedDestination.GetComponent<outputInputManager>().internalTemperature = selectedDestination.GetComponent<outputInputManager>().externalTemperature;
            }
        }
    }
}