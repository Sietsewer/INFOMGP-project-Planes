namespace Physics
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEditor;
    using UnityEngine;

    [RequireComponent(typeof(Rigidbody))]
    public class AerodynamicBody : MonoBehaviour
    {
        new private Rigidbody rigidbody;

        private List<AerodynamicElement> elements;

        internal Rigidbody Rigidbody { get { return rigidbody; } }

        public void RegisterElement(AerodynamicElement element)
        {
            elements.Add(element);
        }

        private void Awake()
        {
            rigidbody = FindObjectOfType<Rigidbody>();
        }

        private void Start()
        {
            elements = new List<AerodynamicElement>(GetComponentsInChildren<AerodynamicElement>());
        }

        private void FixedUpdate()
        {
            _currentImpulses.Clear();

            float totalForceNorm = 0;
            Vector3 totalPositionForce = Vector3.zero;
            Vector3 totalForce = Vector3.zero;

            foreach (AerodynamicElement element in elements)
            {
                Impulse impulse = element.GetImpulse(this);

                _currentImpulses.Push(impulse);

                float forceNorm = impulse.force.magnitude;

                if (forceNorm != 0.0f)
                {
                    totalPositionForce += impulse.position * forceNorm;
                    totalForceNorm += forceNorm;
                    totalForce += impulse.force;
                }
            }

            if (totalForceNorm > float.Epsilon)
            {
                Vector3 centerOfForce = totalPositionForce / totalForceNorm;

                rigidbody.AddForceAtPosition(totalForce * Time.fixedDeltaTime, centerOfForce, ForceMode.Impulse);

                _currentImpulse = new Impulse { position = centerOfForce, force = totalForce };
            }
        }

        private Impulse _currentImpulse;
        private Stack<Impulse> _currentImpulses = new Stack<Impulse>();

        private void OnDrawGizmos()
        {
            if (_currentImpulse.force.magnitude != 0.0f)
            {
                Gizmos.color = Color.yellow;
                Gizmos.DrawSphere(_currentImpulse.position, 1f);

                Gizmos.color = Color.red;
                Gizmos.DrawLine(_currentImpulse.position, _currentImpulse.position + _currentImpulse.force.normalized);
                
                Handles.color = Color.magenta;
                Handles.Label(_currentImpulse.position, "Force : " + _currentImpulse.force.magnitude);
            }

            foreach (Impulse impulse in _currentImpulses)
            {
                Gizmos.color = Color.blue;
                Gizmos.DrawSphere(impulse.position, 0.5f);

                Gizmos.color = Color.cyan;
                Gizmos.DrawLine(impulse.position, impulse.position + impulse.force.normalized);

                Handles.color = Color.green;
                Handles.Label(impulse.position, "" + impulse.force.magnitude);
            }
        }
    }
}