using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TankUIModule : MonoBehaviour
{
    [SerializeField]TMP_Text internalPressure;
    [SerializeField]TMP_Text internalTemperature;
    [SerializeField]TMP_Text volume;
    [SerializeField]TMP_Text quantity;
    public container tank;
    [SerializeField]private TMP_InputField targetTemperature;
    public float rate = 5000;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        updateValues();
    }

    void updateValues()
    {
        internalPressure.text = tank.internalPressure.ToString();
        internalTemperature.text = tank.internalTemperature.ToString();
        volume.text = tank.volume.ToString() + "/" + tank.tankVolume.ToString();
        quantity.text = tank.mass.ToString();
    }

    public void updateTemp()
    {
        float tryN = 0;
        if(float.TryParse(targetTemperature.text.ToString(), out tryN))
        {
            tank.GetComponent<cooler>().targetTemperature = tryN;
            return;
        }else{
            tank.GetComponent<cooler>().targetTemperature = tank.internalTemperature;
        }
    }

    public void valve()
    {
        if(tank.GetComponent<flowController>().opened == false)
        {
            tank.GetComponent<flowController>().opened = true;
            return;
        }

        if(tank.GetComponent<flowController>().opened == true)
        {
            tank.GetComponent<flowController>().opened = false;
            return;
        }
    }

    public void cooler()
    {
        if(tank.GetComponent<cooler>().active == false)
        {
            tank.GetComponent<cooler>().active = true;
            return;
        }

        if(tank.GetComponent<cooler>().active == true)
        {
            tank.GetComponent<cooler>().active = false;
            return;
        }
    }

    public void vent()
    {
        if(tank.GetComponent<gasVent>().open == false)
        {
            tank.GetComponent<gasVent>().open  = true;
            return;
        }

        if(tank.GetComponent<gasVent>().open  == true)
        {
            tank.GetComponent<gasVent>().open  = false;
            return;
        }
    }
}
