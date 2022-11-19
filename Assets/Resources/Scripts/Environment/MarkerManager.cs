using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MarkerManager : MonoBehaviour
{
    [SerializeField]
    private BodyMarker markerPrefab;
    [SerializeField]
    private Canvas markerCanvas;
    [SerializeField]
    private Transform markerParent;

    private List<BodyMarker> activeMarkers;
    private static MarkerManager instance;

    public static MarkerManager Instance
    {
        get
        {
            if (!instance){
                instance = FindObjectOfType<MarkerManager>();
            }
            return instance;
        }
    }

    public static void RegisterMarker(CelestialBody celestialBody, float maxDisplayDist)
    {
        if(Instance.activeMarkers == null){
            Instance.activeMarkers = new List<BodyMarker>();
        }

        BodyMarker markerInstance = Instantiate(Instance.markerPrefab);
        markerInstance.InitializeMarker(Instance.markerCanvas, celestialBody, maxDisplayDist);
        markerInstance.transform.SetParent(Instance.markerParent);
        Instance.activeMarkers.Add(markerInstance);
    }

    public static void SetMarkerVisibility(bool visible)
    {
        if(Instance.activeMarkers == null){
            return;
        }

        for (int i = 0; i < Instance.activeMarkers.Count; i++){
            Instance.activeMarkers[i].gameObject.SetActive(visible);
        }
    }
}