using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScrollingStars : MonoBehaviour
{
    [SerializeField] Vector2 scrollSpeed;
    Vector2 scroll;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        scroll += scrollSpeed * Time.deltaTime;
        GetComponent<Image>().material.SetVector("_Offset", scroll);
    }
}
