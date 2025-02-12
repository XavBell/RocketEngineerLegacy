using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class launchsiteManager : MonoBehaviour
{
    public GameObject commandCenter;
    public GameObject designer;


    [SerializeField]private GameObject commandCenterBtn;
    [SerializeField]private GameObject designerBtn;
    [SerializeField]private GameObject operationBtn;
    [SerializeField]private GameObject designBtn;
    [SerializeField]private GameObject connectBtn;
    [SerializeField]private GameObject fuelOrderBtn;
    [SerializeField]private List<GameObject> commonButton;
    // Start is called before the first frame update
    void Start()
    {
        updateVisibleButtons();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void updateVisibleButtons()
    {
        if(commandCenter != null)
        {
            commandCenterBtn.SetActive(false);
            operationBtn.SetActive(true);
            connectBtn.SetActive(true);
            //fuelOrderBtn.SetActive(true);
            foreach(GameObject button in commonButton)
            {
                button.SetActive(true);
            }
        }else if(commandCenter == null)
        {
            commandCenterBtn.SetActive(true);
            operationBtn.SetActive(false);
            //fuelOrderBtn.SetActive(false);
            connectBtn.SetActive(false);
            foreach(GameObject button in commonButton)
            {
                button.SetActive(false);
            }
        }

        if(designer == null)
        {
            designBtn.SetActive(false);
        }else{
            designBtn.SetActive(true);
        }

        if(designer != null)
        {
            designerBtn.SetActive(false);
        }
    }
}
