using System.Threading;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Linq;
using TMPro;
using Unity.Mathematics;

public class RocketPath : MonoBehaviour
{
    public PlanetGravity planetGravity;
    public GameObject WorldSaveManager;
    public MasterManager MasterManager;
    public Rigidbody2D rb;
    public float G;
    public float rocketMass;
    public float gravityParam = 0;

    public KeplerParams KeplerParams =  new KeplerParams();
    public bool updated;
    public TimeManager MyTime;

    float Ho;
    float Mo;
    float n;
    float a;
    float e;
    float i;
    public float startTime;
    
    // Start is called before the first frame update
    void Start()
    {
        WorldSaveManager = GameObject.FindGameObjectWithTag("WorldSaveManager");
        if(MyTime == null)
        {
            MyTime = FindObjectOfType<TimeManager>();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(MasterManager == null)
        {
            GameObject MastRef = GameObject.FindGameObjectWithTag("MasterManager");
            if(MastRef)
            {
                MasterManager = MastRef.GetComponent<MasterManager>();
            }
        }

        if(MasterManager != null && MasterManager.ActiveRocket != null)
        {
            planetGravity = MasterManager.ActiveRocket.GetComponent<PlanetGravity>();
            rb = planetGravity.rb;
            G = planetGravity.G;
            rocketMass = planetGravity.rb.mass;
            gravityParam = G*(planetGravity.Mass + rocketMass);

        }

        if(planetGravity != null)
        {
            double time = MyTime.time;
            UnityEngine.Vector2 rocketPosition2D = rb.position;
            UnityEngine.Vector2 rocketVelocity2D = rb.velocity;
            UnityEngine.Vector2 planetPosition2D = planetGravity.planet.transform.position;
        }


    }

    public void CalculateParameters()
    {
        startTime = (float)MyTime.time;
        SetKeplerParams(KeplerParams, rb.position, planetGravity.planet.transform.position, rb.velocity, gravityParam, startTime);
        if(KeplerParams.eccentricity > 1)
        {
            CalculateParametersHyperbolic(rb.position, rb.velocity, planetGravity.planet.transform.position, gravityParam, startTime);
        }
    }

    public Vector2 updatePosition()
    {
        if(MyTime != null)
        {
            if(KeplerParams.eccentricity < 1)
            {
                double x = 0;
                double y = 0;
                double vX;
                double vY;
                GetOrbitPositionKepler(gravityParam, (float)MyTime.time, KeplerParams.semiMajorAxis, KeplerParams.eccentricity, KeplerParams.argumentOfPeriapsis, KeplerParams.longitudeOfAscendingNode, KeplerParams.inclination, KeplerParams.trueAnomalyAtEpoch, out x, out y, out vX, out vY);
                Vector2 transformV = new Vector3((float)x, (float)y, 0) + planetGravity.planet.transform.position;
                return transformV;
            }

            if(KeplerParams.eccentricity > 1)
            {
                double x = 0;
                double y = 0;
                double vX;
                double vY;
                GetOrbitalPositionHyperbolic(Mo, MyTime.time, Ho, e, a, i, n, startTime, out x, out y, out vX, out vY);
                Vector2 transformV = new Vector3((float)x, (float)y, 0) + planetGravity.planet.transform.position;
                return transformV;
            }

            if(KeplerParams.eccentricity == 1)
            {
                double x = 0;
                double y = 0;
                double vX;
                double vY;
                GetOrbitalPositionHyperbolic(Mo, MyTime.time, Ho, e, a, i, n, startTime, out x, out y, out vX, out vY);
                Vector2 transformV = new Vector3((float)x, (float)y, 0) + planetGravity.planet.transform.position;
                return transformV;
            }
        }

        if(MyTime == null)
        {
            MyTime = FindObjectOfType<TimeManager>();
            if(KeplerParams.eccentricity < 1)
            {
                double x = 0;
                double y = 0;
                double vX;
                double vY;
                GetOrbitPositionKepler(gravityParam, (float)MyTime.time, KeplerParams.semiMajorAxis, KeplerParams.eccentricity, KeplerParams.argumentOfPeriapsis, KeplerParams.longitudeOfAscendingNode, KeplerParams.inclination, KeplerParams.trueAnomalyAtEpoch, out x, out y, out vX, out vY);
                Vector2 transformV = new Vector3((float)x, (float)y, 0) + planetGravity.planet.transform.position;
                return transformV;
            }

            if(KeplerParams.eccentricity > 1)
            {
                double x = 0;
                double y = 0;
                double vX;
                double vY;
                GetOrbitalPositionHyperbolic(Mo, MyTime.time, Ho, e, a, i, n, startTime, out x, out y, out vX, out vY);
                Vector2 transformV = new Vector3((float)x, (float)y, 0) + planetGravity.planet.transform.position;
                return transformV;
            }

            if(KeplerParams.eccentricity == 1)
            {
                double x = 0;
                double y = 0;
                double vX;
                double vY;
                GetOrbitalPositionHyperbolic(Mo, MyTime.time, Ho, e, a, i, n, startTime, out x, out y, out vX, out vY);
                Vector2 transformV = new Vector3((float)x, (float)y, 0) + planetGravity.planet.transform.position;
                return transformV;
            }
        }

        return rb.position;
    }

    double timeConstant = 0.00001f;
    public Vector2 updateVelocity()
    {
        if(MyTime != null)
        {
            if(KeplerParams.eccentricity < 1)
            {
                double x2 = 0;
                double y2 = 0;
                double x1 = 0;
                double y1 = 0;
                double time2 = MyTime.time + timeConstant;
                double time1 = MyTime.time;
                double vX;
                double vY;
                GetOrbitPositionKepler(gravityParam, time2, KeplerParams.semiMajorAxis, KeplerParams.eccentricity, KeplerParams.argumentOfPeriapsis, KeplerParams.longitudeOfAscendingNode, KeplerParams.inclination, KeplerParams.trueAnomalyAtEpoch, out x2, out y2, out vX, out vY);
                GetOrbitPositionKepler(gravityParam, time1, KeplerParams.semiMajorAxis, KeplerParams.eccentricity, KeplerParams.argumentOfPeriapsis, KeplerParams.longitudeOfAscendingNode, KeplerParams.inclination, KeplerParams.trueAnomalyAtEpoch, out x1, out y1, out vX, out vY);
                return new Vector2((float)vX, (float)vY);
            }

            if(KeplerParams.eccentricity > 1)
            {
                double x2 = 0;
                double y2 = 0;
                double x1 = 0;
                double y1 = 0;
                double time2 = MyTime.time + timeConstant;
                double time1 = MyTime.time;
                double vX;
                double vY;
                GetOrbitalPositionHyperbolic(Mo, time2, Ho, e, a, i, n, startTime, out x2, out y2, out vX, out vY);
                GetOrbitalPositionHyperbolic(Mo, time1, Ho, e, a, i, n, startTime, out x1, out y1, out vX, out vY);
                return new Vector2((float)vX, (float)vY);
            }

            if(KeplerParams.eccentricity == 1)
            {
                double x2 = 0;
                double y2 = 0;
                double x1 = 0;
                double y1 = 0;
                double time2 = MyTime.time + timeConstant;
                double time1 = MyTime.time;
                double vX;
                double vY;
                GetOrbitalPositionHyperbolic(Mo, time2, Ho, e, a, i, n, startTime, out x2, out y2, out vX, out vY);
                GetOrbitalPositionHyperbolic(Mo, time1, Ho, e, a, i, n, startTime, out x1, out y1, out vX, out vY);
                return new Vector2((float)vX, (float)vY);
            }
        }

        if(MyTime == null)
        {
            MyTime = FindObjectOfType<TimeManager>();
            if(KeplerParams.eccentricity < 1)
            {
                double x2 = 0;
                double y2 = 0;
                double x1 = 0;
                double y1 = 0;
                double time2 = MyTime.time + timeConstant;
                double time1 = MyTime.time;
                double vX;
                double vY;
                GetOrbitPositionKepler(gravityParam, time2, KeplerParams.semiMajorAxis, KeplerParams.eccentricity, KeplerParams.argumentOfPeriapsis, KeplerParams.longitudeOfAscendingNode, KeplerParams.inclination, KeplerParams.trueAnomalyAtEpoch, out x2, out y2, out vX, out vY);
                GetOrbitPositionKepler(gravityParam, time1, KeplerParams.semiMajorAxis, KeplerParams.eccentricity, KeplerParams.argumentOfPeriapsis, KeplerParams.longitudeOfAscendingNode, KeplerParams.inclination, KeplerParams.trueAnomalyAtEpoch, out x1, out y1, out vX, out vY);
                return new Vector2((float)vX, (float)vY);
            }

            if(KeplerParams.eccentricity > 1)
            {
                double x2 = 0;
                double y2 = 0;
                double x1 = 0;
                double y1 = 0;
                double time2 = MyTime.time + timeConstant;
                double time1 = MyTime.time;
                double vX;
                double vY;
                GetOrbitalPositionHyperbolic(Mo, time2, Ho, e, a, i, n, startTime, out x2, out y2, out vX, out vY);
                GetOrbitalPositionHyperbolic(Mo, time1, Ho, e, a, i, n, startTime, out x1, out y1, out vX, out vY);
                return new Vector2((float)vX, (float)vY);
            }

            if(KeplerParams.eccentricity == 1)
            {
                double x2 = 0;
                double y2 = 0;
                double x1 = 0;
                double y1 = 0;
                double time2 = MyTime.time + timeConstant;
                double time1 = MyTime.time;
                double vX;
                double vY;
                GetOrbitalPositionHyperbolic(Mo, time2, Ho, e, a, i, n, startTime, out x2, out y2, out vX, out vY);
                GetOrbitalPositionHyperbolic(Mo, time1, Ho, e, a, i, n, startTime, out x1, out y1, out vX, out vY); 
                return new Vector2((float)vX, (float)vY);
            }
        }
        return rb.velocity;
    }

    

    void DrawLine(float time, KeplerParams keplerParams, UnityEngine.Vector2 rocketPosition2D, UnityEngine.Vector2 rocketVelocity2D, UnityEngine.Vector2 planetPosition2D, float gravityParam)
    {
        int numPoints = 1000;
        double[] times = new double[numPoints];
        UnityEngine.Vector3[] positions = new UnityEngine.Vector3[numPoints];

        if(Input.GetKey("z") || updated == false)
        {
            SetKeplerParams(keplerParams, rocketPosition2D, planetPosition2D, rocketVelocity2D, gravityParam, time);
            if(rb.velocity.magnitude != 0 && keplerParams.eccentricity < 1)
            {
                CalculatePoints(time, numPoints, gravityParam, planetPosition2D, keplerParams, ref times, ref positions);
            }

            if(rb.velocity.magnitude != 0 && keplerParams.eccentricity > 1)
            {
                CalculateParametersHyperbolic(rocketPosition2D, rocketVelocity2D, planetPosition2D, gravityParam, time);
            }

            updated = true;
        }

        
    }

    public static void GetOrbitPositionKepler(double gravityParam, double time, double semiMajorAxis, double eccentricity, double argPeriapsis, double LAN, double inclination, double trueAnomalyAtEpoch, out double X, out double Y, out double VX, out double VY)
    {
        // Compute MA (Mean Anomaly)
        // n = 2pi / T (T = time for one orbit)
        // M = n (t)
        double meanAngularMotion = Math.Sqrt(gravityParam / Math.Pow(semiMajorAxis, 3));
        double timeWithOffset = time + GetTimeOffsetFromTrueAnomaly(trueAnomalyAtEpoch, meanAngularMotion, eccentricity);
        double MA = timeWithOffset * meanAngularMotion;
        

        // Compute EA (Eccentric Anomaly)
        double EA = MA;
        

        for (int count = 0; count < 3; count++)
        {
            double dE = (EA - eccentricity * Math.Sin(EA) - MA) / (1 - eccentricity * Math.Cos(EA));
            EA -= dE;
            if (Math.Abs(dE) < 1e-6)
            {
                break;
            } 
        }

        // Compute TA (True Anomaly)
        double TA = 2 * Math.Atan(Math.Sqrt((1 + eccentricity) / (1 - eccentricity)) * Math.Tan(EA / 2));

        // Compute r (radius)
        double r = semiMajorAxis * (1 - eccentricity * Math.Cos(EA));
        double h = Math.Sqrt(gravityParam * semiMajorAxis * (1- Math.Pow(eccentricity, 2)));
        double p = semiMajorAxis*(1-Math.Pow(eccentricity,2));

        //Vector3 odot = new Vector3((float)Math.Sin(EA), (float)(Math.Sqrt(1 - eccentricity * eccentricity) * Math.Cos(EA)), 0);
        //odot = (float)(Math.Sqrt(gravityParam * semiMajorAxis) / r)*odot;
        

        // Compute XYZ positions
        X = r * (Math.Cos(LAN) * Math.Cos(argPeriapsis + TA) - Math.Sin(LAN) * Math.Sin(argPeriapsis + TA) * Math.Cos(inclination));
        double Z = r * (Math.Sin(LAN) * Math.Cos(argPeriapsis + TA) + Math.Cos(LAN) * Math.Sin(argPeriapsis + TA) * Math.Cos(inclination));
        Y = r * (Math.Sin(inclination) * Math.Sin(argPeriapsis + TA));

        VX = (X*h*eccentricity/(r*p))*Math.Sin(TA) - (h/r)*(Math.Cos(LAN)* Math.Sin(argPeriapsis+TA) + Math.Sin(LAN)*Math.Cos(argPeriapsis+TA)*Math.Cos(inclination));
        VY = (Y*h*eccentricity/(r*p))*Math.Sin(TA) + (h/r)*(Math.Cos(argPeriapsis+TA)*Math.Sin(inclination));

        // FLIP Y-Z FOR UNITY
    }
    public static Vector3 GetOrbitVelocityKepler(double gravityParam, double time, double semiMajorAxis, double eccentricity, double argPeriapsis, double LAN, double inclination, double trueAnomalyAtEpoch, double AP, double X, double Y, double Z)
    {
        // Compute MA (Mean Anomaly)
        // n = 2pi / T (T = time for one orbit)
        // M = n (t)
        double meanAngularMotion = Math.Sqrt(gravityParam / Math.Pow(semiMajorAxis, 3)); 
        double timeWithOffset = time + GetTimeOffsetFromTrueAnomaly(trueAnomalyAtEpoch, meanAngularMotion, eccentricity);
        double MA = timeWithOffset * meanAngularMotion;
        

        // Compute EA (Eccentric Anomaly)
        double EA = MA;
        

        for (int count = 0; count < 3; count++)
        {
            double dE = (EA - eccentricity * Math.Sin(EA) - MA) / (1 - eccentricity * Math.Cos(EA));
            EA -= dE;
            if (Math.Abs(dE) < 1e-6)
            {
                break;
            } 
        }

        // Compute TA (True Anomaly)
        double TA = 2 * Math.Atan(Math.Sqrt((1 + eccentricity) / (1 - eccentricity)) * Math.Tan(EA / 2));

        // Compute r (radius)
        double r = semiMajorAxis * (1 - eccentricity * Math.Cos(EA));
        

        double velocityMagnitude = Math.Sqrt(gravityParam * (2 / r - 1 / semiMajorAxis));

        double vX = -velocityMagnitude * Math.Sin(EA);
        double vY = velocityMagnitude * (Math.Sqrt(1 - Math.Pow(eccentricity, 2)) * Math.Cos(EA));

        return new((float)vX, (float)vY, 0); // FLIP Y-Z FOR UNITY
    }

    public static float Modulo(float x, float m)
    {
        return (x % m + m) % m;
    }

    public static double Modulo(double x, double m)
    {
        return (x % m + m) % m;
    }

    public static double GetOrbitalPeriod(double gravityParam, double semiMajorAxis)
    {
        return (Math.Sqrt(4 * Math.Pow(Mathf.PI, 2) * Math.Pow(semiMajorAxis, 3) / gravityParam));
    }

    public static double GetTrueAnomalyFromTimeOffset(double timeOffset, double gravityParam, double semiMajorAxis, double eccentricity)
    {
        if (timeOffset < 0)
        {
            timeOffset += GetOrbitalPeriod(gravityParam, semiMajorAxis);
        }

        double meanAngularMotion = Math.Sqrt(gravityParam / Math.Pow(semiMajorAxis, 3));
        double MA = timeOffset * meanAngularMotion;

        double EA = MA;

        for (int count = 0; count < 10; count++)
        {
            double dE = (EA - eccentricity * Math.Sin(EA) - MA) / (1 - eccentricity * Math.Cos(EA));
            EA -= dE;
            if (Math.Abs(dE) < 1e-12)
            {
                break;
            } 
        }

        // Compute TA (True Anomaly)
        double TA = 2 * Math.Atan(Math.Sqrt((1 + eccentricity) / (1 - eccentricity)) * Math.Tan(EA / 2));

        //Some corrections
        if (timeOffset > 0)
        {
            TA = 2 * Mathf.PI - TA;
        }

        TA = Modulo(TA, 2 * Mathf.PI);

        return TA;
    }

    public static double GetTimeOffsetFromTrueAnomaly(double trueAnomaly, double meanAngularMotion, double eccentricity)
        {
        // Offset by Mathf.Pi so 0 TA lines up with default start position from GetOrbitPositionKepler.
        // Wrap into -pi to +pi range.
        double TA_Clean = Modulo((trueAnomaly + Math.PI), (Math.PI * 2)) - Math.PI;
        double EA = Math.Acos((eccentricity + Math.Cos(TA_Clean)) / (1 + eccentricity * Math.Cos(TA_Clean)));
        if (TA_Clean < 0)
        {
            EA *= -1;
        }
        double MA = EA - eccentricity * Math.Sin(EA);
        double t = MA / meanAngularMotion;
        

        return t;
    }

    public static void KtoCfromC(UnityEngine.Vector2 rocketPosition2D, UnityEngine.Vector2 planetPosition2D, UnityEngine.Vector2 rocketVelocity2D, double gravityParam, double time, out double semiMajorAxis, out double eccentricity, out double argPeriapsis, out double LAN, out double inclination, out double timeToPeriapsis, out double trueAnomalyAtEpoch, out double AP)
    {   
        //Calculate rocket position in 3D and transform it for Kepler
        UnityEngine.Vector3 rocketPosition3D = new UnityEngine.Vector3(rocketPosition2D.x, 0, rocketPosition2D.y); //FLIP for Unity
        UnityEngine.Vector3 planetPosition3D = new UnityEngine.Vector3(planetPosition2D.x, 0, planetPosition2D.y); //FLIP for Unity
        
        rocketPosition3D = rocketPosition3D - planetPosition3D; //Assume planet at (0,0,0)

        //Calculate velocity
        UnityEngine.Vector3 rocketVelocity3D = new UnityEngine.Vector3(rocketVelocity2D.x, 0, rocketVelocity2D.y); //FLIP for Unity

        //Find position and velocity magnitude
        double r = rocketPosition3D.magnitude;
        double v = rocketVelocity3D.magnitude;

        //Calculate specific angular momentum
        UnityEngine.Vector3 h_bar = UnityEngine.Vector3.Cross(rocketPosition3D, rocketVelocity3D);

        double h = h_bar.magnitude;

        //Compute specific energy
        double E = (0.5f * Math.Pow(v, 2)) - gravityParam/r;

        //Compute semi-major axis
        double a = -gravityParam/(2*E);

        //Compute eccentricity
        double e = Math.Sqrt(1 - Math.Pow(h,2)/(a*gravityParam));
      
        //Compute inclination
        double i = Math.Acos(h_bar.z/h);

        //Compute right ascension of ascending node
        double omega_LAN = Math.Atan2(h_bar.x, -h_bar.y);

        //Compute argument of latitude v+w
        double lat = Math.Atan2((rocketPosition3D[2]/Math.Sin(i)), (rocketPosition3D[0]*Math.Cos(omega_LAN) + rocketPosition3D[1] * Math.Sin(omega_LAN)));

        // Compute true anomaly, v, (not actual true anomaly)
        double p = a * (1 - Math.Pow(e, 2));
        double nu = Math.Atan2(Math.Sqrt(p / gravityParam) * UnityEngine.Vector3.Dot(rocketPosition3D, rocketVelocity3D), p - r);

        // Compute argument of periapse, w (not actual argperi)
        double omega_AP = lat - nu;

        // Compute eccentric anomaly, EA
        double EA = 2 * Math.Atan(Math.Sqrt((1 - e) / (1 + e)) * Math.Tan(nu / 2));

        // Compute the time of periapse passage, T
        double n = Math.Sqrt(gravityParam / Math.Pow(a, 3));
        double T = time - (1 / n) * (EA - e * Math.Sin(EA));

        double TA = GetTrueAnomalyFromTimeOffset(T, gravityParam, a, e);
        

        semiMajorAxis = a;
        eccentricity = e;
        argPeriapsis = Modulo(omega_AP, Mathf.PI*2);
        LAN = omega_LAN;
        inclination = i;
        timeToPeriapsis = T;
        trueAnomalyAtEpoch = TA;
        AP = omega_AP;
    }

    public void SetKeplerParams(KeplerParams keplerParams, UnityEngine.Vector2 rocketPosition2D, UnityEngine.Vector2 planetPosition2D, UnityEngine.Vector2 rocketVelocity2D, double gravityParam, double time)
    {
        KtoCfromC(rocketPosition2D, planetPosition2D,rocketVelocity2D, gravityParam, time, out keplerParams.semiMajorAxis, out keplerParams.eccentricity, out keplerParams.argumentOfPeriapsis, out keplerParams.longitudeOfAscendingNode, out keplerParams.inclination, out keplerParams.timeToPeriapsis, out keplerParams.trueAnomalyAtEpoch, out keplerParams.AP);
    }

    public static void CalculatePoints(double time, int numPoints, double gravityParam, UnityEngine.Vector2 planetPosition2D, KeplerParams keplerParams, ref double[] times, ref UnityEngine.Vector3[] positions)
    {
        var period = GetOrbitalPeriod(gravityParam, keplerParams.semiMajorAxis);
        var timeIncrement = period / numPoints;

        for (int count = 0; count < numPoints; count++)
        {
            double x = 0;
            double y = 0;
            double vX;
            double vY;
            GetOrbitPositionKepler(gravityParam, time, keplerParams.semiMajorAxis, keplerParams.eccentricity, keplerParams.argumentOfPeriapsis, keplerParams.longitudeOfAscendingNode, keplerParams.inclination, keplerParams.trueAnomalyAtEpoch, out x, out y, out vX, out vY);
            UnityEngine.Vector3 pos = new Vector3((float)x, (float)y, 0) + new UnityEngine.Vector3(planetPosition2D.x, planetPosition2D.y, 0);
            times[count] = time;
            positions[count] = pos;

            time += timeIncrement;
        }
    }

    public void CalculateParametersHyperbolic(UnityEngine.Vector2 rocketPosition2D, UnityEngine.Vector2 rocketVelocity2D, UnityEngine.Vector2 planetPosition2D, float gravityParam, float time)
    {
        //Calculate rocket position in 3D and transform it for Kepler
        UnityEngine.Vector3 rocketPosition3D = new UnityEngine.Vector3(rocketPosition2D.x, rocketPosition2D.y, 0);
        UnityEngine.Vector3 planetPosition3D = new UnityEngine.Vector3(planetPosition2D.x, planetPosition2D.y, 0); 
        
        rocketPosition3D = rocketPosition3D - planetPosition3D; //Assume planet at (0,0,0)

        //Calculate velocity
        UnityEngine.Vector3 rocketVelocity3D = new UnityEngine.Vector3(rocketVelocity2D.x, rocketVelocity2D.y, 0); 

        //Find position and velocity magnitude
        float r = rocketPosition3D.magnitude;
        float v = rocketVelocity3D.magnitude;

        //Calculate specific angular momentum
        UnityEngine.Vector3 h_bar = UnityEngine.Vector3.Cross(rocketPosition3D, rocketVelocity3D);
        float h = h_bar.magnitude;

        //Calculate eccentricity vector
        UnityEngine.Vector3 eccentricity_bar = UnityEngine.Vector3.Cross(rocketVelocity3D, h_bar)/gravityParam - rocketPosition3D/r;
        e = eccentricity_bar.magnitude;
        
        //Calculate inclination
        i = Mathf.Atan2(-eccentricity_bar.y, -eccentricity_bar.x);

        //Calculate semi-major axis
        a  = 1/(2/r - Mathf.Pow(v, 2)/gravityParam);
        
        //Calculate raw position
        UnityEngine.Vector2 p = new UnityEngine.Vector2(rocketPosition3D.x*Mathf.Cos(i)+rocketPosition3D.y*Mathf.Sin(i), rocketPosition3D.y*Mathf.Cos(i)-rocketPosition3D.x*Mathf.Sin(i));
        //Moon.transform.position = p;

        //Calculate Hyperbolic anomaly
        Ho = (float)Math.Atanh((p.y/(a*Mathf.Sqrt(Mathf.Pow(e, 2)-1)))/(e-p.x/a));

        
        Mo = (float)(Math.Sinh(Ho)*e-Ho);


        //Determine branch of hyperbola
        float dot = UnityEngine.Vector3.Dot(rocketPosition3D, rocketVelocity3D);
        float det = rocketPosition3D.x*rocketVelocity3D.y - rocketVelocity3D.x * rocketPosition3D.y;

        float angle = Mathf.Atan2(det, dot);
        //Calculate mean velocity
        n = Mathf.Sqrt(gravityParam/Mathf.Abs(Mathf.Pow(a, 3)))*Mathf.Sign(angle);

    }

    public static void GetOrbitalPositionHyperbolic(double Mo, double time, double Ho, double e, double a, double i, double n, double startTime, out double x, out double y, out double VX, out double VY)
    {
        //Calculate mean anomaly
        double M = Mo + (time - startTime)*n;
        double H = Ho;

        //Calculate current hyperbolic anomaly
        H = H + (M - e*Math.Sinh(H) + H)/(e*Math.Cosh(H)-1);

        double rawX = a*(e - Math.Cosh(H));
        double rawY = a*Math.Sqrt(Math.Pow(e, 2)-1)*Math.Sinh(H);
        
        x = rawX*Math.Cos(i)-rawY*Math.Sin(i);
        y = rawX*Math.Sin(i)+rawY*Math.Cos(i);

        double t = (e * Math.Cosh(H)-1)/n;
        double rawVX = (-a*Math.Sinh(H))/t;
        double rawVY = Math.Sqrt(Math.Pow(e, 2) - 1)* a * Math.Cosh(H)/t;
        VX = rawVX * Math.Cos(i) - rawVY*Math.Sin(i);
        VY = rawVX * Math.Sin(i) + rawVY*Math.Cos(i);



    }

}