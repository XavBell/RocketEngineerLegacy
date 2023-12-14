using System.Threading;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Linq;
using TMPro;

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
            gravityParam = G*(planetGravity.Mass);

        }

        if(planetGravity != null)
        {
            float time = Time.time;
            UnityEngine.Vector2 rocketPosition2D = rb.position;
            UnityEngine.Vector2 rocketVelocity2D = rb.velocity;
            UnityEngine.Vector2 planetPosition2D = planetGravity.planet.transform.position;
        }

    }

    public void CalculateParameters()
    {
        SetKeplerParams(KeplerParams, rb.position, planetGravity.planet.transform.position, rb.velocity, gravityParam, Time.time);
    }

    public Vector2 updatePosition()
    {
        if(MyTime != null)
        {
            Vector2 transformV = GetOrbitPositionKepler(gravityParam, MyTime.time, KeplerParams.semiMajorAxis, KeplerParams.eccentricity, KeplerParams.argumentOfPeriapsis, KeplerParams.longitudeOfAscendingNode, KeplerParams.inclination, KeplerParams.trueAnomalyAtEpoch) + planetGravity.planet.transform.position;
            return transformV;
        }

        if(MyTime == null)
        {
            Vector2 transformV = GetOrbitPositionKepler(gravityParam, Time.time, KeplerParams.semiMajorAxis, KeplerParams.eccentricity, KeplerParams.argumentOfPeriapsis, KeplerParams.longitudeOfAscendingNode, KeplerParams.inclination, KeplerParams.trueAnomalyAtEpoch) + planetGravity.planet.transform.position;
            return transformV;
        }

        return Vector2.zero;

    }

    public Vector2 updateVelocity()
    {
        float X = GetOrbitPositionKepler(gravityParam, Time.time, KeplerParams.semiMajorAxis, KeplerParams.eccentricity, KeplerParams.argumentOfPeriapsis, KeplerParams.longitudeOfAscendingNode, KeplerParams.inclination, KeplerParams.trueAnomalyAtEpoch).x;
        float Y = GetOrbitPositionKepler(gravityParam, Time.time, KeplerParams.semiMajorAxis, KeplerParams.eccentricity, KeplerParams.argumentOfPeriapsis, KeplerParams.longitudeOfAscendingNode, KeplerParams.inclination, KeplerParams.trueAnomalyAtEpoch).z;
        float Z = GetOrbitPositionKepler(gravityParam, Time.time, KeplerParams.semiMajorAxis, KeplerParams.eccentricity, KeplerParams.argumentOfPeriapsis, KeplerParams.longitudeOfAscendingNode, KeplerParams.inclination, KeplerParams.trueAnomalyAtEpoch).y;
        Vector2 transform = GetOrbitVelocityKepler(gravityParam, Time.time, KeplerParams.semiMajorAxis, KeplerParams.eccentricity, KeplerParams.argumentOfPeriapsis, KeplerParams.longitudeOfAscendingNode, KeplerParams.inclination, KeplerParams.trueAnomalyAtEpoch, KeplerParams.AP, X, Y, Z);
        return transform;
    }

    void DrawLine(float time, KeplerParams keplerParams, UnityEngine.Vector2 rocketPosition2D, UnityEngine.Vector2 rocketVelocity2D, UnityEngine.Vector2 planetPosition2D, float gravityParam)
    {
        int numPoints = 1000;
        float[] times = new float[numPoints];
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

    public static Vector3 GetOrbitPositionKepler(float gravityParam, float time, float semiMajorAxis, float eccentricity, float argPeriapsis, float LAN, float inclination, float trueAnomalyAtEpoch)
    {
        // Compute MA (Mean Anomaly)
        // n = 2pi / T (T = time for one orbit)
        // M = n (t)
        float meanAngularMotion = Mathf.Sqrt(gravityParam / Mathf.Pow(semiMajorAxis, 3));
        float timeWithOffset = time + GetTimeOffsetFromTrueAnomaly(trueAnomalyAtEpoch, meanAngularMotion, eccentricity);
        float MA = timeWithOffset * meanAngularMotion;
        

        // Compute EA (Eccentric Anomaly)
        float EA = MA;
        

        for (int count = 0; count < 3; count++)
        {
            float dE = (EA - eccentricity * Mathf.Sin(EA) - MA) / (1 - eccentricity * Mathf.Cos(EA));
            EA -= dE;
            if (Mathf.Abs(dE) < 1e-6)
            {
                break;
            } 
        }

        // Compute TA (True Anomaly)
        float TA = 2 * Mathf.Atan(Mathf.Sqrt((1 + eccentricity) / (1 - eccentricity)) * Mathf.Tan(EA / 2));

        // Compute r (radius)
        float r = semiMajorAxis * (1 - eccentricity * Mathf.Cos(EA));
        

        // Compute XYZ positions
        float X = r * (Mathf.Cos(LAN) * Mathf.Cos(argPeriapsis + TA) - Mathf.Sin(LAN) * Mathf.Sin(argPeriapsis + TA) * Mathf.Cos(inclination));
        float Y = r * (Mathf.Sin(LAN) * Mathf.Cos(argPeriapsis + TA) + Mathf.Cos(LAN) * Mathf.Sin(argPeriapsis + TA) * Mathf.Cos(inclination));
        float Z = r * (Mathf.Sin(inclination) * Mathf.Sin(argPeriapsis + TA));

        return new(X, Z, 0); // FLIP Y-Z FOR UNITY
    }
    public static Vector3 GetOrbitVelocityKepler(float gravityParam, float time, float semiMajorAxis, float eccentricity, float argPeriapsis, float LAN, float inclination, float trueAnomalyAtEpoch, float AP, float X, float Y, float Z)
    {
        // Compute MA (Mean Anomaly)
        // n = 2pi / T (T = time for one orbit)
        // M = n (t)
        float meanAngularMotion = Mathf.Sqrt(gravityParam / Mathf.Pow(semiMajorAxis, 3)); 
        float timeWithOffset = time + GetTimeOffsetFromTrueAnomaly(trueAnomalyAtEpoch, meanAngularMotion, eccentricity);
        float MA = timeWithOffset * meanAngularMotion;
        

        // Compute EA (Eccentric Anomaly)
        float EA = MA;
        

        for (int count = 0; count < 3; count++)
        {
            float dE = (EA - eccentricity * Mathf.Sin(EA) - MA) / (1 - eccentricity * Mathf.Cos(EA));
            EA -= dE;
            if (Mathf.Abs(dE) < 1e-6)
            {
                break;
            } 
        }

        // Compute TA (True Anomaly)
        float TA = 2 * Mathf.Atan(Mathf.Sqrt((1 + eccentricity) / (1 - eccentricity)) * Mathf.Tan(EA / 2));

        // Compute r (radius)
        float r = semiMajorAxis * (1 - eccentricity * Mathf.Cos(EA));
        

        float velocityMagnitude = Mathf.Sqrt(gravityParam * (2 / r - 1 / semiMajorAxis));

        float vX = -velocityMagnitude * Mathf.Sin(EA);
        float vY = velocityMagnitude * (Mathf.Sqrt(1 - Mathf.Pow(eccentricity, 2)) * Mathf.Cos(EA));

        return new(vX, vY, 0); // FLIP Y-Z FOR UNITY
    }

    public static float Modulo(float x, float m)
    {
        return (x % m + m) % m;
    }

    public static double Modulo(double x, double m)
    {
        return (x % m + m) % m;
    }

    public static float GetOrbitalPeriod(float gravityParam, float semiMajorAxis)
    {
        return (Mathf.Sqrt(4 * Mathf.Pow(Mathf.PI, 2) * Mathf.Pow(semiMajorAxis, 3) / gravityParam));
    }

    public static float GetTrueAnomalyFromTimeOffset(float timeOffset, float gravityParam, float semiMajorAxis, float eccentricity)
    {
        if (timeOffset < 0)
        {
            timeOffset += GetOrbitalPeriod(gravityParam, semiMajorAxis);
        }

        float meanAngularMotion = Mathf.Sqrt(gravityParam / Mathf.Pow(semiMajorAxis, 3));
        float MA = timeOffset * meanAngularMotion;

        float EA = MA;

        for (int count = 0; count < 10; count++)
        {
            float dE = (EA - eccentricity * Mathf.Sin(EA) - MA) / (1 - eccentricity * Mathf.Cos(EA));
            EA -= dE;
            if (Mathf.Abs(dE) < 1e-12)
            {
                break;
            } 
        }

        // Compute TA (True Anomaly)
        float TA = 2 * Mathf.Atan(Mathf.Sqrt((1 + eccentricity) / (1 - eccentricity)) * Mathf.Tan(EA / 2));

        //Some corrections
        if (timeOffset > 0)
        {
            TA = 2 * Mathf.PI - TA;
        }

        TA = Modulo(TA, 2 * Mathf.PI);

        return TA;
    }

    public static float GetTimeOffsetFromTrueAnomaly(float trueAnomaly, float meanAngularMotion, float eccentricity)
        {
        // Offset by Mathf.Pi so 0 TA lines up with default start position from GetOrbitPositionKepler.
        // Wrap into -pi to +pi range.
        float TA_Clean = Modulo((trueAnomaly + Mathf.PI), (Mathf.PI * 2)) - Mathf.PI;
        float EA = Mathf.Acos((eccentricity + Mathf.Cos(TA_Clean)) / (1 + eccentricity * Mathf.Cos(TA_Clean)));
        if (TA_Clean < 0)
        {
            EA *= -1;
        }
        float MA = EA - eccentricity * Mathf.Sin(EA);
        float t = MA / meanAngularMotion;
        

        return t;
    }

    public static void KtoCfromC(UnityEngine.Vector2 rocketPosition2D, UnityEngine.Vector2 planetPosition2D, UnityEngine.Vector2 rocketVelocity2D, float gravityParam, float time, out float semiMajorAxis, out float eccentricity, out float argPeriapsis, out float LAN, out float inclination, out float timeToPeriapsis, out float trueAnomalyAtEpoch, out float AP)
    {   
        //Calculate rocket position in 3D and transform it for Kepler
        UnityEngine.Vector3 rocketPosition3D = new UnityEngine.Vector3(rocketPosition2D.x, 0, rocketPosition2D.y); //FLIP for Unity
        UnityEngine.Vector3 planetPosition3D = new UnityEngine.Vector3(planetPosition2D.x, 0, planetPosition2D.y); //FLIP for Unity
        
        rocketPosition3D = rocketPosition3D - planetPosition3D; //Assume planet at (0,0,0)

        //Calculate velocity
        UnityEngine.Vector3 rocketVelocity3D = new UnityEngine.Vector3(rocketVelocity2D.x, 0, rocketVelocity2D.y); //FLIP for Unity

        //Find position and velocity magnitude
        float r = rocketPosition3D.magnitude;
        float v = rocketVelocity3D.magnitude;

        //Calculate specific angular momentum
        UnityEngine.Vector3 h_bar = UnityEngine.Vector3.Cross(rocketPosition3D, rocketVelocity3D);

        float h = h_bar.magnitude;

        //Compute specific energy
        float E = (0.5f * Mathf.Pow(v, 2)) - gravityParam/r;

        //Compute semi-major axis
        float a = -gravityParam/(2*E);

        //Compute eccentricity
        float e = Mathf.Sqrt(1 - Mathf.Pow(h,2)/(a*gravityParam));
      
        //Compute inclination
        float i = Mathf.Acos(h_bar.z/h);

        //Compute right ascension of ascending node
        float omega_LAN = Mathf.Atan2(h_bar.x, -h_bar.y);

        //Compute argument of latitude v+w
        float lat = Mathf.Atan2((rocketPosition3D[2]/Mathf.Sin(i)), (rocketPosition3D[0]*Mathf.Cos(omega_LAN) + rocketPosition3D[1] * Mathf.Sin(omega_LAN)));

        // Compute true anomaly, v, (not actual true anomaly)
        float p = a * (1 - Mathf.Pow(e, 2));
        float nu = Mathf.Atan2(Mathf.Sqrt(p / gravityParam) * UnityEngine.Vector3.Dot(rocketPosition3D, rocketVelocity3D), p - r);

        // Compute argument of periapse, w (not actual argperi)
        float omega_AP = lat - nu;

        // Compute eccentric anomaly, EA
        float EA = 2 * Mathf.Atan(Mathf.Sqrt((1 - e) / (1 + e)) * Mathf.Tan(nu / 2));

        // Compute the time of periapse passage, T
        float n = Mathf.Sqrt(gravityParam / Mathf.Pow(a, 3));
        float T = time - (1 / n) * (EA - e * Mathf.Sin(EA));

        float TA = GetTrueAnomalyFromTimeOffset(T, gravityParam, a, e);
        

        semiMajorAxis = a;
        eccentricity = e;
        argPeriapsis = Modulo(omega_AP, Mathf.PI*2);
        LAN = omega_LAN;
        inclination = i;
        timeToPeriapsis = T;
        trueAnomalyAtEpoch = TA;
        AP = omega_AP;
    }

    public void SetKeplerParams(KeplerParams keplerParams, UnityEngine.Vector2 rocketPosition2D, UnityEngine.Vector2 planetPosition2D, UnityEngine.Vector2 rocketVelocity2D, float gravityParam, float time)
    {
        KtoCfromC(rocketPosition2D, planetPosition2D,rocketVelocity2D, gravityParam, time, out keplerParams.semiMajorAxis, out keplerParams.eccentricity, out keplerParams.argumentOfPeriapsis, out keplerParams.longitudeOfAscendingNode, out keplerParams.inclination, out keplerParams.timeToPeriapsis, out keplerParams.trueAnomalyAtEpoch, out keplerParams.AP);
    }

    public static void CalculatePoints(float time, int numPoints, float gravityParam, UnityEngine.Vector2 planetPosition2D, KeplerParams keplerParams, ref float[] times, ref UnityEngine.Vector3[] positions)
    {
        var period = GetOrbitalPeriod(gravityParam, keplerParams.semiMajorAxis);
        var timeIncrement = period / numPoints;

        for (int count = 0; count < numPoints; count++)
        {
            UnityEngine.Vector3 pos = GetOrbitPositionKepler(gravityParam, time, keplerParams.semiMajorAxis, keplerParams.eccentricity, keplerParams.argumentOfPeriapsis, keplerParams.longitudeOfAscendingNode, keplerParams.inclination, keplerParams.trueAnomalyAtEpoch) + new UnityEngine.Vector3(planetPosition2D.x, planetPosition2D.y, 0);
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
        float e = eccentricity_bar.magnitude;
        
        //Calculate inclination
        float i = Mathf.Atan2(-eccentricity_bar.y, -eccentricity_bar.x);

        //Calculate semi-major axis
        float a  = 1/(2/r - Mathf.Pow(v, 2)/gravityParam);
        
        //Calculate raw position
        UnityEngine.Vector2 p = new UnityEngine.Vector2(rocketPosition3D.x*Mathf.Cos(i)+rocketPosition3D.y*Mathf.Sin(i), rocketPosition3D.y*Mathf.Cos(i)-rocketPosition3D.x*Mathf.Sin(i));
        //Moon.transform.position = p;

        //Calculate Hyperbolic anomaly
        float Ho = (float)Math.Atanh((p.y/(a*Mathf.Sqrt(Mathf.Pow(e, 2)-1)))/(e-p.x/a));

        
        float Mo = (float)(Math.Sinh(Ho)*e-Ho);


        //Determine branch of hyperbola
        float dot = UnityEngine.Vector3.Dot(rocketPosition3D, rocketVelocity3D);
        float det = rocketPosition3D.x*rocketVelocity3D.y - rocketVelocity3D.x * rocketPosition3D.y;

        float angle = Mathf.Atan2(det, dot);

        //Calculate mean velocity
        float n = Mathf.Sqrt(gravityParam/Mathf.Abs(Mathf.Pow(a, 3)))*Mathf.Sign(angle);

        //Plot positions
        int maxStep = 300;
        UnityEngine.Vector3[] positions = new UnityEngine.Vector3[maxStep];
        float H = Ho;

        for(int ia = 0; ia<maxStep; ia++)
        {
            //Calculate mean anomaly
            float M = Mo + time*n;

            //Calculate current hyperbolic anomaly
            H = (float)(H + (M - e*Math.Sinh(H) + H)/(e*Math.Cosh(H)-1));

            //Raw position vector
            UnityEngine.Vector2 rawP = new UnityEngine.Vector2((float)(a*(e - Math.Cosh(H))), (float)(a*Mathf.Sqrt(Mathf.Pow(e, 2)-1)*Math.Sinh(H)));
            //positions[ia] = new UnityEngine.Vector2((float)(a*(e - Math.Cosh(H))), (float)(a*Mathf.Sqrt(Mathf.Pow(e, 2)-1)*Math.Sinh(H)));
            positions[ia] = new UnityEngine.Vector2(rawP.x*Mathf.Cos(i)-rawP.y*Mathf.Sin(i), rawP.x*Mathf.Sin(i)+rawP.y*Mathf.Cos(i)) + planetPosition2D;     
        }
    }

    public static Vector3 GetOrbitalPositionHyperbolic(float Mo, float time, float Ho, float e, float a, float i, Vector2 planetPosition2D)
    {
        //Calculate mean anomaly
        float M = Mo + time;
        float H = Ho;

        //Calculate current hyperbolic anomaly
        H = (float)(H + (M - e*Math.Sinh(H) + H)/(e*Math.Cosh(H)-1));

        Vector2 rawP = new Vector2((float)(a*(e - Math.Cosh(H))), (float)(a*Mathf.Sqrt(Mathf.Pow(e, 2)-1)*Math.Sinh(H)));
        return new Vector2(rawP.x*Mathf.Cos(i)-rawP.y*Mathf.Sin(i), rawP.x*Mathf.Sin(i)+rawP.y*Mathf.Cos(i));    
    }

}