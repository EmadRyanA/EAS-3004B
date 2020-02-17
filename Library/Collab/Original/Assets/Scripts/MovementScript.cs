using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementScript : MonoBehaviour
{
    //public float speed;
    public float shift_amount;
    public float movement_speed = 10;
    private float lastTime = 0f;
    //public float shiftPercentPerFrame = 0.2f;
    public float shift_speed = 1000;
    private float currentShiftAmount = 0;
    private float lerpTime = 1f;
    private float currentLerpTime;

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
        
        //currentShiftAmount += shiftPercentPerFrame;
        
        currentLerpTime += Time.deltaTime*2;
        Debug.Log(currentLerpTime + " " + lerpTime);
        if(currentLerpTime > lerpTime){
            currentLerpTime = lerpTime;
        }

        //float perc = currentLerpTime / lerpTime;
        //perc = Mathf.Sin(perc * Mathf.PI * 0.5f); // smoothens the camera movement
        //rb.position = Vector3.Lerp(rb.position, new Vector3(moveHorizontal * shift_amount, rb.position.y, rb.position.z), perc); // orig: 0.25f
        //rb.position = Vector3.Lerp(rb.position, new Vector3(moveHorizontal * shift_amount, rb.position.y, rb.position.z), 0.25f); // orig: 0.25f
        
        //Debug.Log(rb.position.x);
        float step = shift_speed*Time.deltaTime;
        transform.position = Vector3.MoveTowards(transform.position, new Vector3(moveHorizontal * shift_amount, transform.position.y, transform.position.z), step);

        // this should always be moving in the direction of the plane
        rb.velocity = new Vector3(0, rb.velocity.y, movement_speed);

    }
}
