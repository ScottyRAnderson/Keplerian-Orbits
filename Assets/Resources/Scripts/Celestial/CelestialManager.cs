using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class CelestialManager : MonoBehaviour
{
    [SerializeField]
    private bool simulated;
    [SerializeField][Range(0f, 1f)]
    private float systemScrub;
    [SerializeField]
    private CelestialBody systemStar;
    [SerializeField]
    private TextAsset systemBodyData;

    private static CelestialManager instance;
    private List<OrbitDisplay> orbitalDisplays;
    private List<CelestialBody> systemBodies;

    public static CelestialBody centralBody { get { return Instance.systemStar; } }
    public static CelestialManager Instance
    {
        get
        {
            if(!instance){
                instance = FindObjectOfType<CelestialManager>();
            }
            return instance;
        }
    }

    #if UNITY_EDITOR
    private void OnValidate(){
        UnityEditor.EditorApplication.update += NotifyOfUpdatedValues;
    }

    private void NotifyOfUpdatedValues()
    {
        UnityEditor.EditorApplication.update -= NotifyOfUpdatedValues;
        OnValuesChanged();
    }

    private void OnValuesChanged()
    {
        GlobalTime.OnValuesChanged -= OnValuesChanged;
        GlobalTime.OnValuesChanged += OnValuesChanged;
        
        if(!Application.isPlaying)
        {
            RegisterSystem();
            for (int i = 0; i < systemBodies.Count; i++){
                systemBodies[i].ScrubOrbit(systemScrub);
            }
        }
    }
    #endif

    private void Awake(){
        RegisterSystem();
    }

    public static void SetSimulationState(bool simulated){
        Instance.simulated = simulated;
    }

    private void RegisterSystem()
    {
        systemBodies = FindObjectsOfType<CelestialBody>().ToList();
        orbitalDisplays = FindObjectsOfType<OrbitDisplay>().ToList();
    }

    private void Update()
    {
        if(!simulated){
            return;
        }

        // Update all bodies
        for (int i = 0; i < systemBodies.Count; i++)
        {
            CelestialBody body = systemBodies[i];
            body.UpdateBody();
        }
    }

    public static void SetBodyVisibility(bool visible, BodyType bodyType)
    {
        for (int i = 0; i < Instance.systemBodies.Count; i++)
        {
            CelestialBody body = Instance.systemBodies[i];
            if (body.BodyType == bodyType){
                body.gameObject.SetActive(visible);
            }
        }
    }

    public static void SetOrbitDrawState(bool state)
    {
        for (int i = 0; i < Instance.orbitalDisplays.Count; i++){
            Instance.orbitalDisplays[i].SetOrbitDrawState(state);
        }
    }

    /// <summary> Simple procedure to load a solar system from a set of JSON files. </summary>
    public void LoadSystemData(TextAsset bodyData)
    {
        if (!bodyData){
            return;
        }

        orbitalDisplays = new List<OrbitDisplay>();
        systemBodies = new List<CelestialBody>();

        for (int i = transform.childCount - 1; i >= 0; i--)
        {
            CelestialBody body = transform.GetChild(i).GetComponent<CelestialBody>();
            if (!body || body == systemStar){
                continue;
            }
            DestroyImmediate(body.gameObject);
        }

        // Load bodies
        BodyCollection bodies = JsonUtility.FromJson<BodyCollection>(bodyData.text);
        for (int i = 0; i < bodies.bodies.Length; i++)
        {
            CelestialBody.BodyData data = bodies.bodies[i];
            CelestialBody planetInstance = GameObject.CreatePrimitive(PrimitiveType.Sphere).AddComponent<CelestialBody>();
            planetInstance.transform.SetParent(transform);
            planetInstance.InitializeBody(data, systemStar);

            orbitalDisplays.Add(planetInstance.gameObject.AddComponent<OrbitDisplay>());
            systemBodies.Add(planetInstance);

            #if UNITY_EDITOR
            // Moving celestial components up in the inspector stack for better visual clarity
            for (int c = 0; c < 4; c++)
            {
                UnityEditorInternal.ComponentUtility.MoveComponentUp(planetInstance);
                UnityEditorInternal.ComponentUtility.MoveComponentUp(planetInstance.gameObject.GetComponent<OrbitDisplay>());
            }
            #endif
        }
    }

    [System.Serializable]
    private class BodyCollection{
        public CelestialBody.BodyData[] bodies;
    }
}