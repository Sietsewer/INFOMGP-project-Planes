namespace Physics
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEditor;
    using System;

    [RequireComponent(typeof(MeshCollider))]
    public class AerodynamicBody : MonoBehaviour
    {
        new private Rigidbody rigidbody;
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

            rigidbody = GetComponentInParent<Rigidbody>();
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

            var upWorld = transform.TransformDirection(up);
            
            var centerOfLiftWorld = transform.TransformPoint(center);

            var forwardWorld = transform.TransformDirection(forward);

            var velocityVector = rigidbody.GetPointVelocity(centerOfLiftWorld);

            var velocity = Vector3.Dot(forwardWorld, velocityVector);

            var angleOfAttack = Vector3.Dot(velocityVector.normalized, -upWorld.normalized) * Mathf.PI;

            var liftCoefficient = wing.LiftCoefficient(Mathf.Rad2Deg * angleOfAttack);  ///= 2 * Mathf.PI * (Mathf.Abs(angleOfAttack) > aoaLimit ? 0 : angleOfAttack);

            var lift = liftCoefficient * ((Utilities.AtmosphericDensity(centerOfLiftWorld) * (velocity * velocity)) / 2) * area;


            Debug.DrawLine(centerOfLiftWorld, centerOfLiftWorld + (upWorld * lift), Color.magenta);

            rigidbody.AddForceAtPosition(upWorld * lift * Time.fixedDeltaTime, centerOfLiftWorld, ForceMode.Impulse);
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
