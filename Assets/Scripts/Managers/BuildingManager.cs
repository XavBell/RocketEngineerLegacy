using System.Xml;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using DG.Tweening;

public class BuildingManager : MonoBehaviour
{
    public GameObject partToConstruct;
    public GameObject customCursor;
    public GameObject earth;
    public GameObject moon;
    public GameObject menu;

    public MasterManager MasterManager;
    public WorldSaveManager WorldSaveManager;
    public launchsiteManager launchsiteManager;
    public SolarSystemManager solarSystemManager = new SolarSystemManager();
    public FuelConnectorManager fuelConnectorManager;
    public GameObject[] panels;
    public GameObject[] mainBar;
    public GameObject[] flightUI;


    public float planetRadius;

    public string localMode = "none";

    public int IDMax = 0;
    public GameObject PauseUI;
    public GameObject TutorialPanel;
    public bool CanDestroy = false;

    public List<GameObject> DynamicParts = new List<GameObject>();    
    
    // Start is called before the first frame update
    void Start()
    {
        GameObject GMM = GameObject.FindGameObjectWithTag("MasterManager");
        MasterManager = GMM.GetComponent<MasterManager>();

        GameObject GWS = GameObject.FindGameObjectWithTag("WorldSaveManager");
        WorldSaveManager = GWS.GetComponent<WorldSaveManager>();

        customCursor.gameObject.SetActive(false);
        
        solarSystemManager = FindObjectOfType<SolarSystemManager>();
        planetRadius = solarSystemManager.earthRadius;
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetMouseButtonDown(0) && partToConstruct != null)
        {
            if (partToConstruct != null && Cursor.visible == false && customCursor.GetComponent<CustomCursor>().constructionAllowed == true)
            {
                Vector2 position = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                Vector2 v = new Vector2(earth.transform.position.x, earth.transform.position.y) - position;
                float lookAngle = 90 + Mathf.Atan2(v.y, v.x) * Mathf.Rad2Deg;
                position = (v.normalized*-(planetRadius + partToConstruct.GetComponent<BoxCollider2D>().size.y/2));
                position+= new Vector2(earth.transform.position.x, earth.transform.position.y);
                GameObject current = Instantiate(partToConstruct, position, Quaternion.Euler(0f, 0f, lookAngle));
                ObjectFall(current);
                current.transform.SetParent(earth.transform);
                current.GetComponent<buildingType>().buildingID = IDMax+1;
                IDMax += 1;

                if(partToConstruct.GetComponent<buildingType>().type == "designer")
                {
                    launchsiteManager.designer = current;
                }

                if(partToConstruct.GetComponent<buildingType>().type == "commandCenter")
                {
                    launchsiteManager.commandCenter = current;
                }

                MasterManager.GetComponent<pointManager>().nPoints -= partToConstruct.GetComponent<buildingType>().cost;


            }
            launchsiteManager.updateVisibleButtons();
            partToConstruct = null;
            Cursor.visible = true;
            customCursor.gameObject.SetActive(false);
        }

        if(Input.GetKey(KeyCode.Escape))
        {
            if(PauseUI.activeSelf == false)
            {
                PauseUI.SetActive(true);
            }
        }

        if(Input.GetMouseButton(1))
        {
            Cursor.visible = true;
            customCursor.gameObject.SetActive(false);
            partToConstruct = null;
            unAllowDestroy();
        }
    }

    private void unAllowDestroy()
    {
        if (CanDestroy == true)
        {
            CanDestroy = false;
            fuelConnectorManager.ShowConnection();
        }
    }

    public void allowDestroy()
    {
        CanDestroy = true;
        fuelConnectorManager.ShowConnection();
    }

    public void Close()
    {
        PauseUI.SetActive(false);
    }

    public void Quit()
    {
        Application.Quit();
    }

    public void ReturnToMenu()
    {
        Destroy(MasterManager.gameObject);
        SceneManager.LoadScene("Menu");
    }


    public void EnterEngineDesign()
    {
        WorldSaveManager.saveTheWorld();
        SceneManager.LoadScene("EngineDesign");
    }
    public void EnterTankDesign()
    {
        WorldSaveManager.saveTheWorld();
        SceneManager.LoadScene("TankDesign");
    }
    public void EnterRocketDesign()
    {
        WorldSaveManager.saveTheWorld();
        SceneManager.LoadScene("Building");
    }

    public void EnterResearch()
    {
        WorldSaveManager.saveTheWorld();
        SceneManager.LoadScene("Research");
    }

    public void Tutorial()
    {
        if(TutorialPanel.activeSelf == false)
        {
            TutorialPanel.SetActive(true);
            return;
        }

        if(TutorialPanel.activeSelf == true)
        {
            TutorialPanel.SetActive(false);
            return;
        }
    }

    public void ConstructPart(GameObject part)
    {
        if(Cursor.visible == true)
        {
            customCursor.gameObject.SetActive(true);
            customCursor.GetComponent<SpriteRenderer>().sprite = part.GetComponent<SpriteRenderer>().sprite;
            customCursor.GetComponent<SpriteRenderer>().size = part.GetComponent<SpriteRenderer>().size;
            customCursor.GetComponent<CustomCursor>().defaultColor = part.GetComponent<SpriteRenderer>().color;
            customCursor.GetComponent<SpriteRenderer>().color = customCursor.GetComponent<CustomCursor>().defaultColor;
            Cursor.visible = false;
            customCursor.GetComponent<CustomCursor>().type = part.GetComponent<buildingType>().type;
            partToConstruct = part;
        }
    }

    public void toggleBuildMenu()
    {
        if(menu.activeSelf == true)
        {
            menu.SetActive(false);
            return;
        }

        if(menu.activeSelf == false)
        {
            menu.SetActive(true);
            return;
        }
    }

    public void activateDeactivate(GameObject button)
    {
        hidePanels(button);

        if(button.activeSelf == false)
        {
            button.SetActive(true);
            PanelFadeIn(button);
            StartCoroutine(ActiveDeactive(0.1f, button, true));
            return;
        }

        if(button.activeSelf == true)
        {
            PanelFadeOut(button);
            StartCoroutine(ActiveDeactive(0.1f, button, false));
            return;
        }
    }

    private IEnumerator ActiveDeactive(float waitTime, GameObject panel, bool activated)
    {
        yield return new WaitForSeconds(waitTime);
        panel.SetActive(activated);
    }

    public void PanelFadeIn(GameObject panel)
    {
        panel.transform.localScale = new Vector3(0, 0, 0);
        panel.transform.DOScale(1, 0.1f);
    }

    public void PanelFadeOut(GameObject panel)
    {
        panel.transform.DOScale(0, 0.1f);
        panel.transform.localScale = new Vector3(1, 1, 1);
    }

    public void ObjectFall(GameObject current)
    {
        current.transform.localScale = new Vector3(0, 0, 0);
        current.transform.DOScale(0.1f, 0.1f);
    }

    public void hidePanels(GameObject excludedPanel)
    {
        foreach(GameObject panel in panels)
        {
            if(panel != excludedPanel)
            {
                PanelFadeOut(panel);
                panel.SetActive(false);
            }
        }
    }

    public void enterFlightMode()
    {
        foreach(GameObject panel in panels)
        {
            panel.SetActive(false);   
        }

        foreach(GameObject panel in mainBar)
        {
            panel.SetActive(false);   
        }

        foreach(GameObject panel in flightUI)
        {
            panel.SetActive(true);   
        }

    }

    public void exitFlightMode()
    {
        foreach(GameObject panel in panels)
        {
            panel.SetActive(true);   
        }

        foreach(GameObject panel in mainBar)
        {
            panel.SetActive(true);   
        }

        foreach(GameObject panel in flightUI)
        {
            panel.SetActive(false);   
        }

    }
}
