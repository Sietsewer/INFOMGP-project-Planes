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

        private Impulse _currentImpulse;

        private void OnDrawGizmos()
        {
            if (rigidbody != null)
            {
                Gizmos.color = Color.yellow;
                Gizmos.DrawSphere(rigidbody.worldCenterOfMass, 0.5f);
            }
        }
    }
}