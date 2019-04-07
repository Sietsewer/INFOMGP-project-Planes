using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class InitialVelocity : MonoBehaviour
{
    public Vector3 direction = Vector3.forward;
    public float kmh = 400;
    
    void Start()
    {
        GetComponent<Rigidbody>().velocity = direction.normalized * (kmh / 3.6f);
    }
}
