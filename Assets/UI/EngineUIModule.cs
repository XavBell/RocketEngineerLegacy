using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EngineUIModule : MonoBehaviour
{
    public Engine engine;
    public TMP_Text buttonText;
    public Color color1;
    public Color color2;
    public Color color3;

    public Color colorWorking;
    public Color colorNotWorking;
    public GameObject imageStatus;

    // Start is called before the first frame update
    void Start()
    {
        color1 = engine.transform.GetChild(3).GetComponent<SpriteRenderer>().color;
        color2 = engine.transform.GetChild(4).GetComponent<SpriteRenderer>().color;
        color3 = engine.transform.GetChild(5).GetComponent<SpriteRenderer>().color;
    }

    // Update is called once per frame
    void Update()
    {
        updateStatus();
    }

    void updateStatus()
    {
        if(engine.operational == true)
        {
            imageStatus.GetComponent<Image>().color = colorWorking;
        }

        if(engine.operational == false)
        {
            imageStatus.GetComponent<Image>().color = colorNotWorking;
        }
    }

    public void activate()
    {
        if(engine.active == true)
        {
            engine.active = false;
            buttonText.text = "Activate";
            return;
        }

        if(engine.active == false)
        {
            engine.active = true;
            buttonText.text = "Deactivate";
            return;
        }
    }

    public void changeColorGreen()
    {
        engine.transform.GetChild(3).GetComponent<SpriteRenderer>().color = Color.green;
        engine.transform.GetChild(4).GetComponent<SpriteRenderer>().color = Color.green;
        engine.transform.GetChild(5).GetComponent<SpriteRenderer>().color = Color.green;
    }

    public void changeColorNormal()
    {
        engine.transform.GetChild(3).GetComponent<SpriteRenderer>().color = color1;
        engine.transform.GetChild(4).GetComponent<SpriteRenderer>().color = color2;
        engine.transform.GetChild(5).GetComponent<SpriteRenderer>().color = color3;
    }
}
