using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementScript : MonoBehaviour
{
    //public float speed;
    public float shift_amount;

    private Rigidbody rb;

    void Start(){
        rb = GetComponent<Rigidbody>();
    }
    // Update is called once per frame
    void FixedUpdate()
    {
        float moveHorizontal = Input.GetAxis("Horizontal");
        float moveVertical = Input.GetAxis("Vertical");

        //Vector3 movement = new Vector3(moveHorizontal, 0.0f, moveVertical);

        //rb.AddForce(movement * speed);


        //rb.position = Vector3.Lerp(Vector3.zero, new Vector3(moveHorizontal * shift_amount, rb.position.y, rb.position.z), 0.25f);
        // the default position of the car should always be in the centre of the track. 
        
        rb.position = Vector3.Lerp(rb.position, new Vector3(moveHorizontal * shift_amount, rb.position.y, rb.position.z), 0.25f);

    }
}
