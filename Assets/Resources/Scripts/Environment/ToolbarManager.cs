using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ToolbarManager : MonoBehaviour
{
    public enum LightingMode
    { 
        Flood,
        Shadow,
        Natural
    }

    [SerializeField]
    private bool toolbarOpen = true;
    [SerializeField]
    private float toolbarToggleRate = 10f;
    [SerializeField]
    private VerticalLayoutGroup toobarLayoutGroup;
    [SerializeField]
    private Animator toolbarToggleAnim;
    [SerializeField]
    private GameObject layersPanel;
    [SerializeField]
    private GameObject shadingPanel;
    [SerializeField]
    private Light globalLight;
    [SerializeField]
    private GameObject[] interfaceElements;

    private SolarCamController camController;
    private GridPlaneGenerator gridPlane;
    private StarfieldGenerator starfield;

    private void Awake()
    {
        camController = FindObjectOfType<SolarCamController>();
        gridPlane = FindObjectOfType<GridPlaneGenerator>();
        starfield = FindObjectOfType<StarfieldGenerator>();
        layersPanel.SetActive(false);
        shadingPanel.SetActive(false);
        toolbarToggleAnim.SetBool("Closing", !toolbarOpen);
    }

    private void Update(){
        toobarLayoutGroup.spacing = Mathf.Lerp(toobarLayoutGroup.spacing, toolbarOpen ? 5f : -40f, toolbarToggleRate * Time.deltaTime);
    }

    public void LayerToggle_Planets(bool toggled){
        CelestialManager.SetBodyVisibility(toggled, BodyType.Planet);
    }

    public void LayerToggle_Satellites(bool toggled){
        CelestialManager.SetBodyVisibility(toggled, BodyType.Satellite);
    }

    public void LayerToggle_Comets(bool toggled){
        CelestialManager.SetBodyVisibility(toggled, BodyType.Comet);
    }

    public void LayerToggle_Orbits(bool toggled){
        CelestialManager.SetOrbitDrawState(toggled);
    }

    public void LayerToggle_Labels(bool toggled){
        MarkerManager.SetMarkerVisibility(toggled);
    }

    public void LayerToggle_Starfield(bool toggled){
        starfield.SetStarfieldVisibility(toggled);
    }

    public void LayerToggle_Interface(bool toggled)
    {
        for (int i = 0; i < interfaceElements.Length; i++){
            interfaceElements[i].SetActive(toggled);
        }
        if(!toggled){
            ToggleToolbar();
        }
    }

    public void ToggleInfiniteGrid(){
        gridPlane.ToggleGridVisbility();
    }

    public void SwapLightingMode(int mode)
    {
        LightingMode lightingMode = (LightingMode)mode;
        switch(lightingMode)
        {
            case LightingMode.Flood:
                RenderSettings.ambientLight = globalLight.color / 2f;
                break;
            case LightingMode.Shadow:
                RenderSettings.ambientLight = new Color(0.02830189f, 0.02830189f, 0.02830189f);
                break;
            case LightingMode.Natural:
                RenderSettings.ambientLight = Color.black;
                break;
        }
    }

    public void ToggleLayersPanel()
    {
        if(!layersPanel.activeSelf){
            CloseActivePopups();
        }
        layersPanel.SetActive(!layersPanel.activeSelf);
    }

    public void ToggleShadingPanel()
    {
        if (!shadingPanel.activeSelf){
            CloseActivePopups();
        }
        shadingPanel.SetActive(!shadingPanel.activeSelf);
    }

    public void ToggleFullScreen(){
        Screen.fullScreen = !Screen.fullScreen;
    }

    public void ResetCamera(){
        camController.ResetCamera();
    }

    public void CloseActivePopups()
    {
        layersPanel.SetActive(false);
        shadingPanel.SetActive(false);
    }

    public void ToggleToolbar()
    {
        toolbarOpen = !toolbarOpen;
        if(!toolbarOpen){
            CloseActivePopups();
        }
        toolbarToggleAnim.SetBool("Closing", !toolbarOpen);
    }
}