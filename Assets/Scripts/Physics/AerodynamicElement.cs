namespace Physics
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEditor;
    using System;

    [RequireComponent(typeof(MeshCollider))]
    public class AerodynamicElement : MonoBehaviour
    {
        new private MeshCollider collider;

        public Vector3 forward = Vector3.down;

        public Vector3 up = Vector3.forward;

        public float angle = 0.0f;

        private Vector3 center;

        public float surfaceMultiplier = 1.0f;

        public float area;

        public WingCharacteristics wing;

        private void Awake() // First time initialization
        {
            Debug.Assert(wing != null, "No wing characteristics!", gameObject);
            
            collider = GetComponent<MeshCollider>();

            area = CalculateLiftingSurfaceArea() * surfaceMultiplier;

            center = Utilities.CalculateCenterOfMass(collider.sharedMesh);
        }
        
        private float CalculateLiftingSurfaceArea()
        {
            Debug.Assert(collider != null);

            Debug.Assert(collider.convex, "Collider must be convex for AerodynamicBody.", collider); // Collider must be convex.

            Mesh mesh = collider.sharedMesh;

            float area = 0.0f;

            for (int i = 0; i < mesh.triangles.Length / 3; i++)
            {
                Vector3 a = mesh.vertices[mesh.triangles[i * 3 + 0]];
                Vector3 b = mesh.vertices[mesh.triangles[i * 3 + 1]];
                Vector3 c = mesh.vertices[mesh.triangles[i * 3 + 2]];
                
                a = Vector3.ProjectOnPlane(a, up);
                b = Vector3.ProjectOnPlane(b, up);
                c = Vector3.ProjectOnPlane(c, up);

                area += Vector3.Cross((b - a), (c - a)).magnitude / 2; // We want the top-down area, so project it on the up vector.
            }

            area /= 2; // Only want half the area

            return area;
        }

        private void OnEnable() // On (re)activation
        {

        }

        private void Start() // First time initialization (late)
        {

        }

        private void FixedUpdate() // Physics update
        {
            // Use `Time.fixedDeltaTime` as delta-t


        }

        internal Impulse GetImpulse(AerodynamicBody body)
        {
            Vector3 upWorld = transform.TransformDirection(up);
            Vector3 centerOfLiftWorld = transform.TransformPoint(center);
            Vector3 forwardWorld = transform.TransformDirection(forward);
            Vector3 velocityVector = body.Rigidbody.GetPointVelocity(centerOfLiftWorld);

            Vector3 liftForce = GetLift(body, velocityVector, velocityVector.magnitude, Utilities.AtmosphericDensity(centerOfLiftWorld), centerOfLiftWorld, upWorld, forwardWorld);
            Vector3 dragForce = GetDrag(body, velocityVector, velocityVector.magnitude, Utilities.AtmosphericDensity(centerOfLiftWorld), centerOfLiftWorld, upWorld, forwardWorld);

            Vector3 totalForceVector = liftForce + dragForce;
            
            return new Impulse { force = totalForceVector, position = centerOfLiftWorld };
        }

        private Vector3 GetDrag(AerodynamicBody body, Vector3 velocityVector, float velocity, float density, Vector3 centerOfLiftWorld, Vector3 upWorld, Vector3 forwardWorld)
        {
            float angleToWingUp = Vector3.Dot(upWorld.normalized, velocityVector.normalized) * 90;

            float dragCoefficient = wing.DragCoefficient(angleToWingUp);

            float dragNorm = dragCoefficient * ((density * (velocity * velocity)) / 2) * area;

            return -velocityVector.normalized * dragNorm;
        }

        private Vector3 GetLift(AerodynamicBody body, Vector3 velocityVector, float velocity, float density, Vector3 centerOfLiftWorld, Vector3 upWorld, Vector3 forwardWorld)
        {
            Vector3 velocityDirection = velocityVector.normalized;

            float angleOfAttack = Vector3.Dot(velocityDirection, -upWorld.normalized) * 90;
            
            float liftCoefficient = wing.LiftCoefficient(angleOfAttack);

            float liftNorm = liftCoefficient * ((density * (velocity * velocity)) / 2) * area;

            Vector3 rotationDirection = Vector3.Cross(velocityDirection, upWorld);

            Vector3 liftVector = Quaternion.AngleAxis(90, rotationDirection) * velocityDirection;
            
            return liftVector * liftNorm;
        }

        private void Update() // Frame update
        {
            // Use `Time.deltaTime` as delta-t
        }

        private void LateUpdate() // Post-animation (integration) frame update
        {
            // Again, use `Time.deltaTime` as delta-t
        }

        private void OnDisable() // On de-activation
        {

        }

        private void OnDestroy() // Finalization
        {

        }
    }
}
