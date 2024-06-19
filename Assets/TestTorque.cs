using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestTorque : MonoBehaviour
{
    public float force = 1f;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        GetComponent<Rigidbody>().AddTorque(Vector3.up * force);
    }
}
