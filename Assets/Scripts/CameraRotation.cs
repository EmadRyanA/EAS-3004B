using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraRotation : MonoBehaviour
{
    // Start is called before the first frame update
    public float rotateIntensity = 0.0f;
    private Vector3 velocity = Vector3.zero;
    public float smoothTime = 20f;
    private Quaternion rotatedValue;
    private Quaternion oppositeRotatedValue;
    private Vector3 initialRotation;
    public GameObject player;
    private GameObject bgImage;
    
    // stuff for tilt controls
    // stuff for tilt controls
    private Vector3 zeroAcc; 
    private Vector3 currAcc;  
    private float moveHorizontalTilt = 0;
    private float moveVerticalTilt = 0;
    private float step;

    void Start(){
        //Camera cam = gameObject.GetComponent<Camera>();
        
        rotatedValue = Quaternion.Euler(transform.eulerAngles.x, transform.eulerAngles.y, 0.0f);
        oppositeRotatedValue = Quaternion.Euler(transform.eulerAngles.x, transform.eulerAngles.y, 0.0f);
        
        // stores the initial rotation values
        initialRotation = new Vector3(transform.eulerAngles.x, transform.eulerAngles.y, 0.0f);

        bgImage = transform.GetChild(0).gameObject;

        // resetting axes for tilt controls
        zeroAcc = Input.acceleration;
        currAcc = Vector3.zero;

        step = GameObject.Find("Player").GetComponent<MovementScript>().shift_speed*Time.deltaTime; // gets the step value from movementscript

        Debug.Log(initialRotation);
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        //Debug.Log(rotatedValue.x + " " + rotatedValue.y);
        //float moveHorizontal = Input.GetAxis("Horizontal");
        float moveHorizontal = 0;
        float moveVertical = Input.GetAxis("Vertical");

        currAcc = Vector3.Lerp(currAcc, Input.acceleration-zeroAcc, step); // tilt controls
        moveHorizontalTilt = Mathf.Clamp(currAcc.x * MovementScript.tiltSensHorizontal, -1, 1); // side to side
        moveVerticalTilt = Mathf.Clamp(currAcc.y * MovementScript.tiltSensVertical, -1, 1); // forwards and backwards
        
        foreach(Touch touch in Input.touches){
                
            print(Screen.currentResolution.width);
            if(touch.position.x < Screen.currentResolution.width / 2){ // left side of the screen
                moveHorizontal = -1;
            }else{
                moveHorizontal = 1;
            }
        }

        //float rotatedZ = 0.0f;
        if(moveHorizontal != 0){
            //if(Math.Abs(transform.eulerAngles.z) < rotateIntensity){
            //Debug.Log(moveHorizontal);
            if(MovementScript.movementState == 1){
                rotatedValue = Quaternion.Euler(initialRotation.x, initialRotation.y, player.transform.eulerAngles.z+rotateIntensity*moveHorizontalTilt);
            }else{
                rotatedValue = Quaternion.Euler(initialRotation.x, initialRotation.y, player.transform.eulerAngles.z+rotateIntensity*moveHorizontal);
            }
            //rotatedValue = Quaternion.Euler(initialRotation.x, initialRotation.y, player.transform.eulerAngles.z+rotateIntensity*moveHorizontal);
            oppositeRotatedValue = Quaternion.Euler(initialRotation.x, initialRotation.y, player.transform.eulerAngles.z+(rotateIntensity/8)*moveHorizontal);
            //}
        }else{
            rotatedValue = Quaternion.Euler(initialRotation.x, initialRotation.y, player.transform.eulerAngles.z); // rotate the camera
            oppositeRotatedValue = rotatedValue;
        }

        //transform.eulerAngles = rotatedValue;
        //transform.eulerAngles = Vector3.Lerp(transform.eulerAngles, rotatedValue, 0.25f);
        //Debug.Log(transform.rotation + " " + rotatedValue);
        //transform.rotation = Vector3.SmoothDamp(transform.rotation, rotatedValue, ref velocity, smoothTime);
        //print(transform.rotation);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, rotatedValue, smoothTime*Time.deltaTime);
        if(MovementScript.movementState == 1){
            oppositeRotatedValue = Quaternion.Euler(initialRotation.x, initialRotation.y, player.transform.eulerAngles.z+(rotateIntensity/8)+20*moveHorizontalTilt*-1); // tilt according to tilt controls
            bgImage.transform.rotation = Quaternion.RotateTowards(bgImage.transform.rotation, oppositeRotatedValue, smoothTime*2*Time.deltaTime); // temp code; rotate the bg image opposite to camera to keep it level w/ stage

        }else if(MovementScript.movementState == 2){
            print("movement state 2");
            oppositeRotatedValue = Quaternion.Euler(initialRotation.x, initialRotation.y, player.transform.eulerAngles.z+(rotateIntensity/8)+20*moveHorizontal*-1);
            bgImage.transform.rotation = Quaternion.RotateTowards(bgImage.transform.rotation, oppositeRotatedValue, smoothTime*2*Time.deltaTime); // temp code; rotate the bg image opposite to camera to keep it level w/ stage
        }else{
            bgImage.transform.rotation = Quaternion.RotateTowards(bgImage.transform.rotation, oppositeRotatedValue, smoothTime*2*Time.deltaTime);

        }
    }
}
