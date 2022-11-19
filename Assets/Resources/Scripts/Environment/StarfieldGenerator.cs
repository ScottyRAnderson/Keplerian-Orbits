using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;
using UnityEngine.Rendering;

public class StarfieldGenerator : MonoBehaviour
{
    private const float visibleMagnitude = 6.5f; // The minimum magnitude of stars visible to the naked eye
    private const float colorIndexUpper = 1.40f;
    private const float colorIndexLower = -0.33f;

    [SerializeField]
    private TextAsset starData;
    [SerializeField]
    private Shader starShader;
    [SerializeField]
    private bool renderStars = true;

    [Space]

    [SerializeField]
    private float starScale;
    [SerializeField]
    private float sharpness;
    [SerializeField]
    private float magnitudeScalar;
    [SerializeField][Tooltip("Based on sample calibration colors")]
    private Gradient colorIndexGradient;

    private int starVisiblity;
    private Camera mainCam;
    private List<Star> starField;
    private Material starMaterial;
    private ComputeBuffer argsBuffer;
    private ComputeBuffer starBuffer;
    private CommandBuffer renderBuffer;
    private uint[] args = new uint[5] { 0, 0, 0, 0, 0 };

    private void Awake()
    {
        starVisiblity = 1;
        mainCam = Camera.main;
        starMaterial = new Material(starShader);
        starMaterial.SetFloat("starScale", starScale * 0.001f);
        starMaterial.SetFloat("sharpness", sharpness);
        starMaterial.SetFloat("magnitudeScalar", magnitudeScalar * starVisiblity);

        LoadStarfield(starData);
        InitializeStarRenderBuffer();
        SetStarfieldVisibility(renderStars);
    }

    private void Update()
    {
        starMaterial.SetFloat("starScale", starScale * 0.001f);
        starMaterial.SetFloat("sharpness", sharpness);
        starMaterial.SetFloat("magnitudeScalar", magnitudeScalar * starVisiblity);
    }

    public void SetStarfieldVisibility(bool visible)
    {
        renderStars = visible;
        starVisiblity = renderStars ? 1 : 0;
    }

    // Loads a CSV file populated with star data and renders it with quads
    private void LoadStarfield(TextAsset starData)
    {
        if(!starData){
            return;
        }

        starField = new List<Star>();
        string[] lines = starData.text.Split("\n");
        float maxMagnitude = float.Parse(lines[2].Split(",")[13]);
        float minMagnitude = visibleMagnitude;

        for (int i = 0; i < lines.Length; i++)
        {
            // Ignore the headers and our sun
            if(i <= 1){
                continue;
            }

            // components[13] = magnitude, components[17 - 19] = x,y,z pos
            string[] components = lines[i].Split(",");
            if(components.Length < 23){
                continue;
            }

            float magnitude = float.Parse(components[13]);

            // We don't to draw all 100,000+ stars, so exlude stars not visible to the naked eye
            if (magnitude <= visibleMagnitude)
            {
                // Normalized position projects the starfield to a sphere
                Vector3 position = new Vector3(float.Parse(components[17]), float.Parse(components[18]), float.Parse(components[19])).normalized * 10f; // Arbitrary field radius

                // Obtain color value by interpolating between values in the color index. Source: https://en.wikipedia.org/wiki/Color_index
                UnityEngine.Color hue = UnityEngine.Color.white;
                float colorIndex;
                if (float.TryParse(components[16], out colorIndex)){
                    hue = colorIndexGradient.Evaluate(MathHelper.NormalizeValue(colorIndex, colorIndexLower, colorIndexUpper));
                }

                float normalizedMagnitude = MathHelper.NormalizeValue(magnitude, minMagnitude, maxMagnitude);
                starField.Add(new Star(position, normalizedMagnitude, hue));
            }
        }
    }

    /// Rendering the star field using a drawMeshInstanced command buffer allows us to minimise batch counts to improve performance
    /// Implmenentation reference: https://docs.unity3d.com/ScriptReference/Graphics.DrawMeshInstancedIndirect.html
    private void InitializeStarRenderBuffer()
    {
        if(starField == null){
            return;
        }
        TryReleaseBuffers();

        // Indirect args
        Mesh quad = QuadMesh();
        args[0] = (uint)quad.GetIndexCount(0);
        args[1] = (uint)starField.Count;
        args[2] = (uint)quad.GetIndexStart(0);
        args[3] = (uint)quad.GetBaseVertex(0);
        argsBuffer = new ComputeBuffer(1, args.Length * sizeof(uint), ComputeBufferType.IndirectArguments);
        argsBuffer.SetData(args);

        // Star buffer
        int stride = System.Runtime.InteropServices.Marshal.SizeOf(typeof(Star));
        starBuffer = new ComputeBuffer(starField.Count, stride);
        starBuffer.SetData(starField.ToArray());

        // Send buffer information to the shader
        starMaterial.SetBuffer("starData", starBuffer);

        // Attach draw buffer to the cameras pre-render event
        renderBuffer = new CommandBuffer();
        renderBuffer.name = "Star Rendering";
        renderBuffer.DrawMeshInstancedIndirect(quad, 0, starMaterial, 0, argsBuffer, 0);
        mainCam.AddCommandBuffer(CameraEvent.BeforeForwardOpaque, renderBuffer);
    }

    /// <summary> Function to generate a simple quad mesh. </summary>
    /// Further reference: https://docs.unity3d.com/Manual/Example-CreatingaBillboardPlane.html
    private Mesh QuadMesh()
    {
        Vector3[] vertices = { new Vector3(-1, -1), new Vector3(1, -1), new Vector3(1, 1), new Vector3(-1, 1) };
        int[] triangles = { 0, 2, 1, 0, 3, 2 };

        Mesh mesh = new Mesh();
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.RecalculateBounds();
        mesh.RecalculateNormals();
        return mesh;
    }

    private void TryReleaseBuffers()
    {
        if (starBuffer != null){
            starBuffer.Release();
        }
        if (argsBuffer != null){
            argsBuffer.Release();
        }
    }

    // Always dispose of buffers to avoid gc issues
    private void OnDisable()
    {
        TryReleaseBuffers();
        starBuffer = null;
        argsBuffer = null;
    }

    private struct Star
    {
        public Vector3 position;
        public float magnitude;
        public UnityEngine.Color hue;

        public Star(Vector3 position, float magnitude, UnityEngine.Color hue)
        {
            this.position = position;
            this.magnitude = magnitude;
            this.hue = hue;
        }
    }
}