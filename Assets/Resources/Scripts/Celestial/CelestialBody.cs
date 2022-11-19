using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]
public class CelestialBody : MonoBehaviour
{
    [SerializeField]
    private BodyType bodyType = BodyType.Planet;
    [SerializeField]
    private string bodyName = "body";
    [SerializeField]
    private CelestialBody parentBody;
    [SerializeField]
    private float radius = 10f;
    [SerializeField]
    private float orbitalPeriod;
    [SerializeField]
    private float orbitalInclination;
    [SerializeField][Tooltip("Point of furthest distance")]
    private float apoapsis;
    [SerializeField][Tooltip("Point of closest approach")]
    private float periapsis;
    [SerializeField][Range(0f, 360f)][Tooltip("Angle from the body's ascending node to its periapsis")]
    private float argumentOfPeriapsis;
    [SerializeField][Range(0f, 180f)][Tooltip("Also known as obliquity")]
    private float axialTilt;
    [SerializeField]
    private float rotationPeriod;

    [SerializeField]
    private float orbitalProgress;
    [SerializeField]
    private float rotationProgress;
    [SerializeField]
    private int completeOrbits;
    [SerializeField]
    private float rawOrbits;

    public BodyType BodyType { get { return bodyType; } }
    public CelestialBody ParentBody { get { return parentBody; } }
    public float NormalizedOrbit { get { return MathHelper.NormalizeValue(orbitalProgress, 0f, 365f); } }
    public float NormalizedRotation { get { return MathHelper.NormalizeValue(rotationProgress, 0f, 360f); } }

    private void OnValidate()
    {
        gameObject.name = bodyName == string.Empty ? "" : bodyName;
        transform.localScale = new Vector3(radius * 2f, radius * 2f, radius * 2f);

        if (bodyType == BodyType.Star || !parentBody){
            return;
        }

        apoapsis = Mathf.Max(apoapsis, 0f);
        periapsis = Mathf.Max(periapsis, 0f);
        CalculateOrbit(NormalizedOrbit, NormalizedRotation);
    }

    public void InitializeBody(BodyData data, CelestialBody parentBody)
    {
        float scalar = 10f; // Use of scalar to bring values within a reasonable range

        this.parentBody = parentBody;

        bodyName = data.name;
        radius = (data.diameter / 400f) / 2f;
        rotationPeriod = data.rotationPeriod;
        periapsis = data.perihelion * scalar;
        apoapsis = data.aphelion * scalar;
        orbitalPeriod = data.orbitalPeriod;
        orbitalInclination = data.orbitalInclination;
        axialTilt = data.obliquityToOrbit;

        gameObject.name = bodyName;
        transform.localScale = new Vector3(radius * 2f, radius * 2f, radius * 2f);
        if (bodyType != BodyType.Star && this.parentBody){
            CalculateOrbit(NormalizedOrbit, NormalizedRotation);
        }
    }

    /// <summary> Editor functionality to forcefully advance the orbit given a scrub value in the range 0 - 1 </summary>
    public void ScrubOrbit(float scrub)
    {
        if (bodyType == BodyType.Star || !parentBody){
            return;
        }

        orbitalProgress = scrub * 365f;
        rotationProgress = scrub * rotationPeriod * 360f;
        CalculateOrbit(NormalizedOrbit, NormalizedRotation);
    }

    public void UpdateBody()
    {
        if (bodyType == BodyType.Star || !parentBody){
            return;
        }

        AdvanceOrbit();
        CalculateOrbit(NormalizedOrbit, NormalizedRotation);
    }

    private void AdvanceOrbit()
    {
        float orbitStep = (365f / orbitalPeriod) * GlobalTime.YearTick;
        float rotationStep = (360f / -rotationPeriod) * GlobalTime.DayTick;

        orbitalProgress += orbitStep;
        orbitalProgress %= 365f;

        rotationProgress += rotationStep;
        rotationProgress %= 360f;

        rawOrbits += orbitStep / 365f;
        completeOrbits = (int)rawOrbits;
    }

    /// <summary> Computes the transform along an orbital path given a progress value in the range 0 - 1 </summary>
    public void CalculateOrbit(float orbitalProgress, float rotationProgress)
    {
        Vector3 orbitPoint = KeplerHelper.ComputePointOnOrbit(apoapsis, periapsis, argumentOfPeriapsis, orbitalInclination, orbitalProgress);

        // Sidereal day = time taken for a body to rotate once about its axis, OR, the time taken for 'fixed' stars to appear in the same spot in the sky again
        float siderealDay = rotationProgress * 360f * (bodyType == BodyType.Satellite ? 0f : 1f); // Satellites should be tidally locked, axial rotation should therefore be synced with the orbital period
        
        // Solar day = time taken for the sun to appear in the same spot in the sky (24 hours on earth)
        float solarDay = siderealDay - orbitalProgress * 360f;

        transform.rotation = Quaternion.Euler(0f, 0f, -axialTilt) * Quaternion.Euler(0f, solarDay, 0f);
        transform.position = orbitPoint + parentBody.transform.position;
    }

    public Vector3[] SampleOrbitPath(int resolution = 1000){
        return KeplerHelper.SampleOrbitPath(apoapsis, periapsis, argumentOfPeriapsis, orbitalInclination, parentBody.transform.position, resolution);
    }

    [System.Serializable]
    public class BodyData
    {
        public int id;
        public string name;
        public float mass;
        public float diameter;
        public float density;
        public float gravity;
        public float escapeVelocity;
        public float rotationPeriod;
        public float lengthOfDay;
        public float distanceFromSun;
        public float perihelion;
        public float aphelion;
        public float orbitalPeriod;
        public float orbitalVelocity;
        public float orbitalInclination;
        public float orbitalEccentricity;
        public float obliquityToOrbit;
        public float meanTemperature;
        public float surfacePressure;
        public int numberOfMoons;
        public bool hasRingSystem;
        public bool hasGlobalMagneticField;
    }
}