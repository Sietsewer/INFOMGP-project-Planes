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

        new private Rigidbody rigidbody;

        private void Awake() // First time initialization
        {
            Debug.Assert(wing != null, "No wing characteristics!", gameObject);
            
            collider = GetComponent<MeshCollider>();

            area = CalculateLiftingSurfaceArea(transform.localToWorldMatrix) * surfaceMultiplier;

            center = Utilities.CalculateCenterOfMass(collider.sharedMesh);

            rigidbody = GetComponentInParent<Rigidbody>();
        }
        
        private float CalculateLiftingSurfaceArea(Matrix4x4 localToWorldMatrix)
        {
            Debug.Assert(collider != null);

            Debug.Assert(collider.convex, "Collider must be convex for AerodynamicBody.", collider); // Collider must be convex.

            Mesh mesh = collider.sharedMesh;

            Vector3 worldUp = localToWorldMatrix.MultiplyVector(up);

            float area = 0.0f;

            for (int i = 0; i < mesh.triangles.Length / 3; i++)
            {
                Vector3 a = localToWorldMatrix.MultiplyPoint(mesh.vertices[mesh.triangles[i * 3 + 0]]);
                Vector3 b = localToWorldMatrix.MultiplyPoint(mesh.vertices[mesh.triangles[i * 3 + 1]]);
                Vector3 c = localToWorldMatrix.MultiplyPoint(mesh.vertices[mesh.triangles[i * 3 + 2]]);
                
                a = Vector3.ProjectOnPlane(a, worldUp);
                b = Vector3.ProjectOnPlane(b, worldUp);
                c = Vector3.ProjectOnPlane(c, worldUp);

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

            Vector3 upWorld = transform.TransformDirection(up);
            Vector3 centerOfLiftWorld = transform.TransformPoint(center);
            Vector3 forwardWorld = transform.TransformDirection(forward);
            Vector3 velocityVector = rigidbody.GetPointVelocity(centerOfLiftWorld);

            Vector3 liftForce = GetLift(velocityVector, velocityVector.sqrMagnitude, Utilities.AtmosphericDensity(centerOfLiftWorld), centerOfLiftWorld, upWorld, forwardWorld);
            Vector3 dragForce = GetDrag(velocityVector, velocityVector.sqrMagnitude, Utilities.AtmosphericDensity(centerOfLiftWorld), centerOfLiftWorld, upWorld, forwardWorld);
            
#if DEBUG
            {
                Debug.DrawLine(centerOfLiftWorld, centerOfLiftWorld + liftForce * Time.fixedDeltaTime, Color.blue);
                Debug.DrawLine(centerOfLiftWorld, centerOfLiftWorld + dragForce * Time.fixedDeltaTime, Color.yellow);

                Debug.DrawLine(centerOfLiftWorld, centerOfLiftWorld + upWorld, Color.black);
                Debug.DrawLine(centerOfLiftWorld, centerOfLiftWorld + forwardWorld, Color.white);
                Debug.DrawLine(centerOfLiftWorld, centerOfLiftWorld + (rigidbody.mass * rigidbody.velocity) * Time.fixedDeltaTime, Color.red);
            }
#endif
            
            rigidbody.AddForceAtPosition(liftForce * Time.fixedDeltaTime, centerOfLiftWorld, ForceMode.Impulse);
            rigidbody.AddForceAtPosition(dragForce * Time.fixedDeltaTime, centerOfLiftWorld, ForceMode.Impulse);
        }

        private Vector3 GetDrag(Vector3 velocityVector, float velocitySqr, float density, Vector3 centerOfLiftWorld, Vector3 upWorld, Vector3 forwardWorld)
        {
            float angleToWingUp = Vector3.Angle(upWorld, velocityVector) - 90;

            float dragCoefficient = wing.DragCoefficient(angleToWingUp);

            float dragNorm = .5f * dragCoefficient * density * velocitySqr * area;

            return -velocityVector.normalized * Mathf.Abs(dragNorm);
        }

        private float _liftCoefficient = 0.0f;
        private Vector3 _centerOfLiftWorld = Vector3.zero;

        private bool stalling;

        private Vector3 GetLift(Vector3 velocityVector, float velocitySqr, float density, Vector3 centerOfLiftWorld, Vector3 upWorld, Vector3 forwardWorld)
        {
            Vector3 velocityDirection = velocityVector.normalized;

            float angleOfAttack = Vector3.SignedAngle(forwardWorld, velocityVector, Vector3.Cross(upWorld, forwardWorld));

            float liftCoefficient = wing.LiftCoefficient(angleOfAttack);

            bool wasStalling = stalling;

            if (stalling = (Mathf.Abs(liftCoefficient) <= float.Epsilon))
            {
                if (!wasStalling)
                {
                    Debug.Log(string.Format("Entered stall at '{0}' degrees AoA.", angleOfAttack));
                }
            }
            else
            {
                if (wasStalling)
                {
                    Debug.Log(string.Format("Left stall at '{0}' degrees AoA.", angleOfAttack));
                }
            }

            _liftCoefficient = liftCoefficient;
            _centerOfLiftWorld = centerOfLiftWorld;

            float liftNorm = .5f * liftCoefficient * density * velocitySqr * area;

            Vector3 rotationDirection = Vector3.Cross(velocityDirection, upWorld);

            Vector3 liftVector = Quaternion.AngleAxis(90, rotationDirection) * velocityDirection;
            
            return liftVector * liftNorm;
        }

        private void OnDrawGizmos()
        {
            if (collider != null && _liftCoefficient <= float.Epsilon)
            {
                Gizmos.DrawWireMesh(collider.sharedMesh, transform.position, transform.rotation, transform.lossyScale);
                Handles.Label(_centerOfLiftWorld, "Stalling!");
            }
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
