using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//PRETTY SURE IT'S OBSOLETE
public class BodySwitcher : MonoBehaviour
{
    private GameObject moon;
    private GameObject sun;
    private GameObject earth;
    public List<GameObject> planets = new List<GameObject>();
    public GameObject referenceBody;
    private RocketStateManager RSM;
    private SolarSystemManager solarSystemManager;
    void Start()
    {
        TypeScript[] planetsToAssign = FindObjectsOfType<TypeScript>();
        foreach(TypeScript planetToAssign in planetsToAssign)
        {
            if(planetToAssign.type == "earth")
            {
                earth = planetToAssign.gameObject;
            }

            if(planetToAssign.type == "moon")
            {
                moon = planetToAssign.gameObject;
            }

        }

        if(RSM == null)
        {
            RSM = this.GetComponent<RocketStateManager>();
        }

        if(solarSystemManager == null)
        {
            solarSystemManager = FindObjectOfType<SolarSystemManager>();
        }
    }

    void Update()
    {
        if(RSM == null)
        {
            RSM = this.GetComponent<RocketStateManager>();
        }
        //updateReferenceBody();
    }

    public void updateReferenceBody()
    {
        
    }
    
}
