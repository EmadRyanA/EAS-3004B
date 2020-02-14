using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class downforce : MonoBehaviour
{
    // Start is called before the first frame update
    public float checkingDistance = 10.0f;
    private Rigidbody rb;
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        //rb.AddForce()
    }
}
