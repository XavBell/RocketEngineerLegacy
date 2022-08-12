using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CustomCursor : MonoBehaviour
{
    private SpriteRenderer sp;
    private BoxCollider2D box;
    public GameObject earth;
    public bool constructionAllowed = true;

    public float zRot;
    public string type;

    void Start()
    {
        sp = this.GetComponent<SpriteRenderer>();
        box = this.GetComponent<BoxCollider2D>();
        sp.color = Color.green;
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        transform.position = mousePosition;

        if(sp != null && SceneManager.GetActiveScene().name == "SampleScene")
        {
            if(type == "designer")
            {
                Vector2 position = this.transform.position;
                Vector2 v = new Vector2(earth.transform.position.x, earth.transform.position.y) - position;
                float lookAngle = 90 + Mathf.Atan2(v.y, v.x) * Mathf.Rad2Deg;
                position = (v.normalized*-(127420f + sp.size.y/2));
                position+= new Vector2(earth.transform.position.x, earth.transform.position.y);
                this.transform.rotation = Quaternion.Euler(0f, 0f, lookAngle);
                this.transform.position = position;
            }

            if(type == "GSEtank")
            {
                Vector2 position = this.transform.position;
                Vector2 v = new Vector2(earth.transform.position.x, earth.transform.position.y) - position;
                float lookAngle = 90 + Mathf.Atan2(v.y, v.x) * Mathf.Rad2Deg;
                position = (v.normalized*-(127420f + sp.size.y/2));
                this.transform.rotation = Quaternion.Euler(0f, 0f, lookAngle);
                position+= new Vector2(earth.transform.position.x, earth.transform.position.y);
                this.transform.position = position;
            }

            if(type == "pipe")
            {
                if(Input.GetKey(KeyCode.Z))
                {
                    this.transform.Rotate(0f, 0f, 40f*Time.deltaTime, Space.Self);
                    zRot = this.transform.rotation.z;
                }

                if(Input.GetKey(KeyCode.X))
                {
                    this.transform.Rotate(0f, 0f, 40f*-Time.deltaTime, Space.Self);
                    zRot = this.transform.eulerAngles.z;
                }
            }

        }

        
    }

    void OnTriggerStay2D(Collider2D other)
    {
        if(box != null && SceneManager.GetActiveScene().name == "SampleScene")
        {
            if(other.tag == "building")
            {
                sp.color = Color.red;
                constructionAllowed = false;
            }
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if(box != null && SceneManager.GetActiveScene().name == "SampleScene")
        {
            if(other.tag == "building")
            {
                sp.color = Color.green;
                constructionAllowed = true;
            }
        }
    }
}
