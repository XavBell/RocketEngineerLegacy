using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControl : MonoBehaviour
{
    //Camera
    public Camera cam;
    public Rigidbody2D camBox;
    private float targetZoom;
    Vector3 dragOrigin;
    private float zoomFactor = 10f;
    private float zoomLerp = 10f;
    Vector3 position;
    public GameObject sun;
    public GameObject earth;
    public GameObject rocket;
    public Vector3 previousPos = new Vector3(0, 0, 0);

    private float sizeX, sizeY, ratio;

    public GameObject CamRef;

    public GameObject MasterManager;
    // Start is called before the first frame update
    void Start()
    {
        targetZoom = cam.orthographicSize;
        
    }

    // Update is called once per frame
    void Update()
    {

 

            
            QE();
            ZoomIn();
            //WASD();
            if(rocket != null)
            {
                //Rocket();
            }
            rocket = GameObject.FindGameObjectWithTag("capsule");
            

        
    }



    public void ZoomIn()
    {
        float scrollData;
        scrollData = Input.GetAxis("Mouse ScrollWheel");

        targetZoom -= scrollData * zoomFactor;
        cam.orthographicSize = Mathf.Lerp(cam.orthographicSize, targetZoom, Time.deltaTime*zoomLerp);
    }

    public void Rocket()
    {
        if (Input.GetKey(KeyCode.E))
        {
            CamRef.transform.position = new Vector3(rocket.transform.position.x, rocket.transform.position.y, -10);
        }
    }

    public void WASD()
    {
        Vector2 dist = this.transform.position;
        float xAxisValue = Input.GetAxis("Horizontal");
        float yAxisValue = Input.GetAxis("Vertical");
        if(cam != null)
        {

            cam.transform.Translate(new Vector2(xAxisValue, yAxisValue));
            
        }
    }

    public void QE()
    {
        if (Input.GetKey(KeyCode.E))
        {
            cam.transform.RotateAround(earth.transform.position, earth.transform.forward*-1, 20*Time.deltaTime);
        }

        if (Input.GetKey(KeyCode.Q))
        {
            cam.transform.RotateAround(earth.transform.position, earth.transform.forward, 20*Time.deltaTime);
        }
    }

}

