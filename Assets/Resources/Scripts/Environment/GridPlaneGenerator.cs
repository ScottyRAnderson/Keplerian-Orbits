using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshRenderer))][RequireComponent(typeof(MeshFilter))]
public class GridPlaneGenerator : MonoBehaviour
{
    [SerializeField]
    private Material gridMaterial;
    [SerializeField]
    private bool renderGrid = true;
    [SerializeField]
    private int width;
    [SerializeField]
    private int height;
    [SerializeField]
    private float length;

    private MeshRenderer meshRenderer;
    private MeshFilter meshFilter;

    private void Awake()
    {
        meshRenderer = GetComponent<MeshRenderer>();
        meshFilter = GetComponent<MeshFilter>();
        meshFilter.sharedMesh = GeneratePlane(width, height, length);
        meshRenderer.material = gridMaterial;
        SetGridVisiblity(renderGrid);
    }

    public void ToggleGridVisbility(){
        SetGridVisiblity(!renderGrid);
    }

    public void SetGridVisiblity(bool visible)
    {
        renderGrid = visible;
        meshRenderer.enabled = visible;
    }

    private Mesh GeneratePlane(int width, int height, float unitLength)
    {
        float halfWidth = width * unitLength / 2f;
        float halfHeight = height * unitLength / 2f;
        Vector3 centre = new Vector3(-halfWidth, 0, -halfHeight);
        width++;
        height++;

        int vertCount = width * height;
        int triCount = (width - 1) * (height - 1) * 2;

        Vector3[] vertices = new Vector3[vertCount];
        Vector2[] uvs = new Vector2[vertCount];
        int[] triangles = new int[triCount * 3];
        int triIndex = 0;

        for (int w = 0; w < width; w++)
        {
            for (int h = 0; h < height; h++)
            {
                int vertIndex = h * width + w;
                Vector3 position = Vector3.right * w * unitLength + Vector3.forward * h * unitLength;
                vertices[vertIndex] = position + centre;
                uvs[vertIndex] = new Vector2(w / (width - 1f), h / (height - 1f));
                
                if (w == width - 1 || h == height - 1){
                    continue;
                }

                triangles[triIndex++] = vertIndex;
                triangles[triIndex++] = vertIndex + width;
                triangles[triIndex++] = vertIndex + width + 1;
                triangles[triIndex++] = vertIndex;
                triangles[triIndex++] = vertIndex + width + 1;
                triangles[triIndex++] = vertIndex + 1;
            }
        }

        Mesh mesh = new Mesh();
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.uv = uvs;
        mesh.RecalculateBounds();
        mesh.RecalculateNormals();
        mesh.RecalculateTangents();
        return mesh;
    }
}