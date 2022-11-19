using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[ExecuteInEditMode][RequireComponent(typeof(NewtonianOrbit))][RequireComponent(typeof(LineRenderer))]
public class NewtonianDisplay : MonoBehaviour
{
    [SerializeField]
    private int iterationCount = 1000;
    [SerializeField]
    private float displayWidth = 5f;

    private NewtonianOrbit body;
    private LineRenderer lineRenderer;

    private void Awake()
    {
        body = GetComponent<NewtonianOrbit>();
        lineRenderer = GetComponent<LineRenderer>();

        lineRenderer.material = new Material(Shader.Find("Particles/Standard Unlit"));
        lineRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
        lineRenderer.receiveShadows = false;

        if(Application.isPlaying)
        {
            lineRenderer.enabled = false;
            enabled = false;
        }
    }

    private void Update()
    {
        lineRenderer.startWidth = displayWidth;
        lineRenderer.endWidth = displayWidth;
        UpdateOrbitDisplay();
    }

    private void UpdateOrbitDisplay()
    {
        if(!body.ParentBody){
            return;
        }

        Vector3[] points = new Vector3[iterationCount];

        Vector3 velocityVector = body.InitialVelocity;
        Vector3 currentPosition = body.transform.position;

        for (int i = 0; i < iterationCount; i++)
        {
            Vector3 difference = body.ParentBody.transform.position - currentPosition;

            float sqrLength = difference.sqrMagnitude;
            Vector3 direction = difference.normalized;

            Vector3 acceleration = direction * NewtonianOrbit.GravitationalConstant * (body.Mass * body.ParentBody.Mass) / sqrLength;
            velocityVector += acceleration * NewtonianOrbit.TimeStep;

            currentPosition += velocityVector;
            points[i] = currentPosition;
        }

        DrawPath(points);
    }

    private void DrawPath(Vector3[] points)
    {
        lineRenderer.positionCount = points.Length;
        for (int i = 0; i < points.Length; i++){
            lineRenderer.SetPosition(i, points[i]);
        }
    }
}