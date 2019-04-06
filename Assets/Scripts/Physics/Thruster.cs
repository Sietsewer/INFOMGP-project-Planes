namespace Physics
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    public class Thruster : MonoBehaviour
    {
        public Vector3 direction;

        [Range(0, 1)]
        public float throttle;

        public float maxForce = 1000;

        new private Rigidbody rigidbody;

        private void Awake()
        {
            rigidbody = GetComponentInParent<Rigidbody>();

            Debug.Assert(rigidbody != null, "Parent must have a rigidbody", this);

            direction.Normalize();
        }

        private void FixedUpdate()
        {
            Debug.Assert(direction == direction.normalized, "Direction vector is not norm", this);

            Vector3 forceVector = transform.localToWorldMatrix.MultiplyVector(direction);
            
            forceVector *= throttle * maxForce * Time.fixedDeltaTime;
            Debug.DrawLine(transform.position, transform.position + forceVector, Color.red);

            rigidbody.AddForceAtPosition(forceVector, transform.position, ForceMode.Impulse);
        }
    }
}