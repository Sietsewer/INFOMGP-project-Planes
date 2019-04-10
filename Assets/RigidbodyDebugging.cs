using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Physics;

[RequireComponent(typeof(Rigidbody))]
[ExecuteInEditMode]
public class RigidbodyDebugging : MonoBehaviour
{
    new private Rigidbody rigidbody;
    private void OnDrawGizmos()
    {
        if (rigidbody == null) rigidbody = GetComponent<Rigidbody>();

        Debug.Assert(rigidbody != null);

        Gizmos.color = Color.red;
        Gizmos.DrawSphere(rigidbody.worldCenterOfMass, 0.3f);

        foreach(AerodynamicElement ae in GetComponentsInChildren<AerodynamicElement>())
        {
            
        }
    }
}
