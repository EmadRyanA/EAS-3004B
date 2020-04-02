using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementScript : MonoBehaviour
{
    //public float speed;
    public static int movementState;
    public float shift_amount;
    public float movement_speed = 10;
    private float lastTime = 0f;
    //public float shiftPercentPerFrame = 0.2f;
    public float shift_speed = 1000;
    private float currentShiftAmount = 0;
    private float lerpTime = 1f;
    private float currentLerpTime;

    private Rigidbody rb;

    // stuff for tilt controls
    private Vector3 zeroAcc; 
    private Vector3 currAcc;  
    private float moveHorizontalTilt = 0;
    private float moveVerticalTilt = 0;
    public static float tiltSensHorizontal = 2.5f;
    public static float tiltSensVertical = 2.5f;

    void Start(){
        rb = GetComponent<Rigidbody>();
        movementState = 0;

        // resetting axes for tilt controls
        zeroAcc = Input.acceleration;
        currAcc = Vector3.zero;
    }
    // Update is called once per frame
    void Update()
    {
        
        float step = shift_speed*Time.deltaTime;
        //Vector3 movement = new Vector3(moveHorizontal, 0.0f, moveVertical);

        //rb.AddForce(movement * speed);


        //rb.position = Vector3.Lerp(Vector3.zero, new Vector3(moveHorizontal * shift_amount, rb.position.y, rb.position.z), 0.25f);
        // the default position of the car should always be in the centre of the track. 
        
        //currentShiftAmount += shiftPercentPerFrame;
        
        /*
        currentLerpTime += Time.deltaTime*2;
        Debug.Log(currentLerpTime + " " + lerpTime);
        if(currentLerpTime > lerpTime){
            currentLerpTime = lerpTime;
        }
        */

        //float perc = currentLerpTime / lerpTime;
        //perc = Mathf.Sin(perc * Mathf.PI * 0.5f); // smoothens the camera movement
        //rb.position = Vector3.Lerp(rb.position, new Vector3(moveHorizontal * shift_amount, rb.position.y, rb.position.z), perc); // orig: 0.25f
        //rb.position = Vector3.Lerp(rb.position, new Vector3(moveHorizontal * shift_amount, rb.position.y, rb.position.z), 0.25f); // orig: 0.25f
        
        //Debug.Log(rb.position.x);
        
        // this should always be moving in the direction of the plane
        rb.velocity = new Vector3(0, rb.velocity.y, movement_speed); 

        if(movementState == 0){ // default driving state
            //float moveHorizontal = Input.GetAxis("Horizontal");
            float moveHorizontal = 0;
            float moveVertical = Input.GetAxis("Vertical");

            foreach(Touch touch in Input.touches){
                
                //print(Screen.currentResolution.width);
                if(touch.position.x < Screen.currentResolution.width / 2){ // left side of the screen
                    moveHorizontal = -1;
                }else{
                    moveHorizontal = 1;
                }
            }

            transform.position = Vector3.MoveTowards(transform.position, new Vector3(moveHorizontal * shift_amount, transform.position.y, transform.position.z), step);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.Euler(0,0,0), step);

            

        }else if(movementState == 1){ // drifting state
            
            currAcc = Vector3.Lerp(currAcc, Input.acceleration-zeroAcc, step); // tilt controls
            moveHorizontalTilt = Mathf.Clamp(currAcc.x * tiltSensHorizontal, -1, 1); // side to side
            moveVerticalTilt = Mathf.Clamp(currAcc.y * tiltSensVertical, -1, 1); // forwards and backwards
            
            
            float moveHorizontal = Input.GetAxis("Horizontal");
            float moveVertical = Input.GetAxis("Vertical");

            // rotates the car depending on how hard the user tilts the phone
            print(rb.velocity);
            transform.position = Vector3.MoveTowards(transform.position, new Vector3(moveHorizontalTilt * shift_amount, transform.position.y, transform.position.z), step);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.Euler(0, moveHorizontalTilt*45*-1, 0), step);
            
        }else if(movementState == 2){ // flying state
            float moveHorizontal = 0;
            float moveVertical = Input.GetAxis("Vertical");

            foreach(Touch touch in Input.touches){
                
                //print(Screen.currentResolution.width);
                if(touch.position.x < Screen.currentResolution.width / 2){ // left side of the screen
                    moveHorizontal = -1;
                }else{
                    moveHorizontal = 1;
                }
            }

            transform.position = Vector3.MoveTowards(transform.position, new Vector3(moveHorizontal * shift_amount, 10, transform.position.z), step);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.Euler(0, 0, moveHorizontal*20), step);

        }
        

    }
}
