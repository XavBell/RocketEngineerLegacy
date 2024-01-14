using System.Collections;
using System.Collections.Generic;
using System.Data;
using UnityEngine;

public class MapManager : MonoBehaviour
{
    public GameObject EarthIcon;
    public GameObject Earth;

    public GameObject SunIcon;
    public GameObject Sun;

    public GameObject MoonIcon;
    public GameObject Moon;

    public Camera mapCam;
    public List<GameObject> icons = new List<GameObject>();
    public List<PlanetGravity> rockets = new List<PlanetGravity>();
    public List<GameObject> prediction = new List<GameObject>();
    public bool MapOn = false;
    public GameObject cursor;

    public GameObject predictionPrefab;
    private float lineFactor = 0.01f;

    // Start is called before the first frame update
    void Start()
    {
        mapCam.GetComponent<Camera>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (MapOn == true)
        {
            EarthIcon.transform.position = mapCam.WorldToScreenPoint(Earth.transform.position);
            SunIcon.transform.position = mapCam.WorldToScreenPoint(Sun.transform.position);
            MoonIcon.transform.position = mapCam.WorldToScreenPoint(Moon.transform.position);
            foreach(GameObject icon in icons)
            {
                if(icon != null)
                {

                    icon.transform.localScale = new Vector2(mapCam.orthographicSize,mapCam.orthographicSize);
                }
                
            }

            foreach(GameObject pred in prediction)
            {
                pred.GetComponent<LineRenderer>().widthMultiplier = mapCam.orthographicSize * lineFactor;
            }

            

        }

    }

    public void mapOn()
    {
        if (MapOn == false)
        {
            EarthIcon.SetActive(true);
            SunIcon.SetActive(true);
            MoonIcon.SetActive(true);
            PlanetGravity[] planetGravity = FindObjectsOfType<PlanetGravity>();
            foreach (PlanetGravity planetGravity1 in planetGravity)
            {
                GameObject arrow = Instantiate(cursor, planetGravity1.transform);
                arrow.transform.position = planetGravity1.gameObject.transform.position;
                icons.Add(arrow);
                rockets.Add(planetGravity1);

                GameObject prediction1 = Instantiate(predictionPrefab);
                prediction1.GetComponent<Prediction>().planetGravity = planetGravity1;
                prediction.Add(prediction1);

            }
            MapOn = true;
            return;

        }

        if (MapOn == true)
        {
            EarthIcon.SetActive(false);
            SunIcon.SetActive(false);
            MoonIcon.SetActive(false);
            foreach(GameObject icon in icons)
            {
                Destroy(icon);
            }
            icons.Clear();

            foreach(GameObject pred in prediction)
            {
                Destroy(pred);
            }
            prediction.Clear();
            MapOn = false;
            return;
        }

    }
}
