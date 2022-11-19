using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewtonianOrbit : MonoBehaviour
{
    public const float GravitationalConstant = 0.0001f;
    public const float TimeStep = 0.1f;

    [SerializeField]
    private float mass;
    [SerializeField]
    private float radius;
    [SerializeField]
    private Vector3 initialVelocity;
    [SerializeField]
    private NewtonianOrbit parentBody;

    private Vector3 velocityVector;

    public float Mass { get { return mass; } }
    public Vector3 InitialVelocity { get { return initialVelocity; } }
    public NewtonianOrbit ParentBody { get { return parentBody; } }

    private void Awake(){
        velocityVector = initialVelocity;
    }

    private void OnValidate(){
        transform.localScale = Vector3.one * radius * 2f;
    }

    private void Update()
    {
        if(!parentBody){
            return;
        }

        Vector3 difference = parentBody.transform.position - transform.position;

        float sqrLength = difference.sqrMagnitude;
        Vector3 direction = difference.normalized;

        Vector3 acceleration = direction * GravitationalConstant * (mass * parentBody.Mass) / sqrLength; // Appliction of Newton's law of universal gravitation, F = G ((m1*m2) / r^2)
        velocityVector += acceleration * TimeStep;

        transform.position += velocityVector;
    }
}