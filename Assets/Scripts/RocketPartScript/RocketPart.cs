using System.Diagnostics;
using System;
using System.Numerics;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RocketPart : MonoBehaviour
{
    public ParticleSystem explosion;
    [field: SerializeField] public string _partType;

    public string _partName{get; set;}
    public float _partCost{get; set;}

    public Guid _partID{get; set;}

    [field: SerializeField] public float _partMass{get; set;}

    public string _path{get; set;}

    public UnityEngine.Vector2 _position{get; set;}

    [field: SerializeField] public GameObject _attachTop{get; set;}
    [field: SerializeField] public GameObject _attachBottom{get; set;}
    [field: SerializeField] public GameObject _attachRight{get; set;}
    [field: SerializeField] public GameObject _attachLeft{get; set;}

    public void SetGuid()
    {
        _partID = Guid.NewGuid();
    }

    void OnDestroy()
    {
        if(explosion != null && SceneManager.GetActiveScene().name == "SampleScene")
        {
            explosion.gameObject.transform.parent = null;
            explosion.Play();
        }
        
    }
}
