using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[ExecuteInEditMode][RequireComponent(typeof(CelestialBody))][RequireComponent(typeof(LineRenderer))]
public class OrbitDisplay : MonoBehaviour
{
    [SerializeField]
    private bool drawOrbitLine = true;
    [SerializeField]
    private bool drawMarker = true;
    [SerializeField]
    private bool cacheOrbitPath = true;
    [SerializeField][MinMax()]
    private Vector2 minMaxLineWidth = new Vector2(8f, 20f);
    [SerializeField][Range(3, 5000)]
    private int lineResolution = 100;
    [SerializeField]
    private Color lineColor = Color.white;

    private Vector3[] cachedPath;
    private CelestialBody celestialBody;
    private LineRenderer lineRenderer;
    private SolarCamController solarCam;

    private void Awake()
    {
        celestialBody = GetComponent<CelestialBody>();
        lineRenderer = GetComponent<LineRenderer>();
        solarCam = FindObjectOfType<SolarCamController>();

        lineRenderer.material = new Material(Shader.Find("Particles/Standard Unlit"));
        lineRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
        lineRenderer.receiveShadows = false;
        cachedPath = celestialBody.SampleOrbitPath(lineResolution);

        UpdateOrbitDisplay();
        InitializeMarker();
    }

    private void InitializeMarker()
    {
        if(!drawMarker || !Application.isPlaying){
            return;
        }

        float maxDisplayDist = 50000f;
        switch (celestialBody.BodyType)
        {
            case BodyType.Star:
                maxDisplayDist = 1E+10f;
                break;
            case BodyType.Planet:
                maxDisplayDist = 50000f;
                break;
            case BodyType.Satellite:
                maxDisplayDist = 5000f;
                break;
            case BodyType.Comet:
                maxDisplayDist = 500000f;
                break;
        }
        MarkerManager.RegisterMarker(celestialBody, maxDisplayDist);
    }


    private void Update(){
        UpdateOrbitDisplay();
    }

    public void SetOrbitDrawState(bool state){
        drawOrbitLine = state;
    }

    private void UpdateOrbitDisplay()
    {
        Vector3 observerPoint = solarCam.transform.position;
        #if UNITY_EDITOR
        if(!Application.isPlaying && SceneView.lastActiveSceneView != null)
        {
            Camera sceneCamera = SceneView.lastActiveSceneView.camera;
            observerPoint = sceneCamera.transform.position;
        }
        #endif

        float orbitalRadius = Vector3.Distance(celestialBody.transform.position, celestialBody.ParentBody.transform.position);
        float distToOrbit = DistanceToOrbitalPlane(observerPoint, celestialBody.ParentBody.transform.position, orbitalRadius);

        if (drawOrbitLine)
        {
            if(!lineRenderer.enabled){
                lineRenderer.enabled = true;
            }

            float widthMultiplier = Mathf.Min(distToOrbit * (minMaxLineWidth.x / 1000f), minMaxLineWidth.y);
            lineRenderer.widthMultiplier = widthMultiplier;
            lineRenderer.startColor = lineColor;
            lineRenderer.endColor = lineColor;
            DrawPath(!Application.isPlaying || !cacheOrbitPath ? celestialBody.SampleOrbitPath(lineResolution) : cachedPath);
        }
        else if(lineRenderer.enabled){
            lineRenderer.enabled = false;
        }
    }

    private void DrawPath(Vector3[] points)
    {
        if(!lineRenderer){
            return;
        }

        lineRenderer.positionCount = points.Length;
        for (int i = 0; i < points.Length; i++){
            lineRenderer.SetPosition(i, points[i]);
        }
    }

    private float DistanceToOrbitalPlane(Vector3 point, Vector3 orbitCentre, float orbitRadius)
    {
        Vector3 direction = point - orbitCentre;
        Vector3 projectedPoint = orbitCentre + Vector3.ProjectOnPlane(direction, Vector3.up).normalized * orbitRadius;
        return Vector3.Distance(point, projectedPoint);
    }
}