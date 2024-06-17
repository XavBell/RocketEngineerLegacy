using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class RocketController : MonoBehaviour
{
    //For rocket wide variables
    public string rocketName;
    public float rocketMass;
    public float throttle;
    public List<string> lineNames = new List<string>();
    public List<Guid> lineGuids = new List<Guid>();

    public void InitializeComponents()
    {
        this.gameObject.AddComponent<Rigidbody2D>();
        this.GetComponent<Rigidbody2D>().simulated = true;
        this.GetComponent<Rigidbody2D>().collisionDetectionMode = CollisionDetectionMode2D.Continuous;
        this.GetComponent<Rigidbody2D>().freezeRotation = true;
        this.GetComponent<Rigidbody2D>().angularDrag = 0;
        this.GetComponent<Rigidbody2D>().gravityScale = 0;
        this.gameObject.AddComponent<DoubleTransform>();
        this.gameObject.AddComponent<PlanetGravity>();
        this.GetComponent<PlanetGravity>().core = this.gameObject;
        this.GetComponent<PlanetGravity>().initializeRocket();
        this.GetComponent<PlanetGravity>().setPlanet(this.GetComponent<PlanetGravity>().getPlanet());
        this.GetComponent<PlanetGravity>().storedVelocity = this.GetComponent<PlanetGravity>().storedVelocity;
        this.GetComponent<PlanetGravity>().velocityStored = this.GetComponent<PlanetGravity>().velocityStored;
        this.GetComponent<PlanetGravity>().possessed = false;
        this.gameObject.AddComponent<RocketStateManager>();
        this.gameObject.AddComponent<DoubleVelocity>();
        this.gameObject.AddComponent<RocketPath>();
        this.gameObject.GetComponent<RocketStateManager>().prediction = this.gameObject.GetComponent<RocketPath>();
        UpdateMass();
    }

    public void TransferStateFromDecoupling(RocketController rc)
    {
        InitializeComponents();
        this.lineNames = rc.lineNames;
        this.lineGuids = rc.lineGuids;
        this.rocketName = rc.rocketName + "1";
        this.GetComponent<Rigidbody2D>().velocity = this.GetComponent<Rigidbody2D>().velocity;
    }

    public void UpdateMass()
    {
        foreach (PhysicsPart child in GetComponentsInChildren<PhysicsPart>())
        {

            rocketMass += child.GetComponent<PhysicsPart>().mass;
            
        }
    }
}
