namespace Physics
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEditor;

    [RequireComponent(typeof(Rigidbody))]
    [RequireComponent(typeof(Collider))]
    public class AerodynamicBody : MonoBehaviour
    {
        new private Rigidbody rigidbody;
        new private Collider collider;

        [Range(0, 1)]
        public float liftyness = .01f;

        public Vector3 liftDirection = new Vector3(0, 1.0f, 0);

        private void Awake() // First time initialization
        {
            rigidbody = GetComponent<Rigidbody>();
            collider = GetComponent<Collider>();
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

            liftDirection.Normalize();

            var liftVector = liftDirection * liftyness;

            liftVector *= rigidbody.velocity.magnitude;
            liftVector *= Time.fixedDeltaTime;

            var liftVectorWorld = transform.localToWorldMatrix.MultiplyVector(liftVector);

            var centerOfLiftWorld = transform.position + rigidbody.centerOfMass;

            Debug.DrawLine(centerOfLiftWorld, centerOfLiftWorld + liftVectorWorld, Color.magenta);

            rigidbody.AddForceAtPosition(liftVectorWorld, centerOfLiftWorld, ForceMode.Impulse);
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
