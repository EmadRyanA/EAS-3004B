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
    private Vector3 initialRotation;
    public GameObject player;
    void Start(){
        //Camera cam = gameObject.GetComponent<Camera>();
        
        rotatedValue = Quaternion.Euler(transform.eulerAngles.x, transform.eulerAngles.y, 0.0f);
        
        // stores the initial rotation values
        initialRotation = new Vector3(transform.eulerAngles.x, transform.eulerAngles.y, 0.0f);
        Debug.Log(initialRotation);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        //Debug.Log(rotatedValue.x + " " + rotatedValue.y);
        float moveHorizontal = Input.GetAxis("Horizontal");
        float moveVertical = Input.GetAxis("Vertical");
        //float rotatedZ = 0.0f;
        if(moveHorizontal != 0){
            //if(Math.Abs(transform.eulerAngles.z) < rotateIntensity){
            //Debug.Log(moveHorizontal);
            rotatedValue = Quaternion.Euler(initialRotation.x, initialRotation.y, player.transform.eulerAngles.z+rotateIntensity*moveHorizontal);
            //}
        }else{
            rotatedValue = Quaternion.Euler(initialRotation.x, initialRotation.y, player.transform.eulerAngles.z);
        }

        //transform.eulerAngles = rotatedValue;
        //transform.eulerAngles = Vector3.Lerp(transform.eulerAngles, rotatedValue, 0.25f);
        //Debug.Log(transform.rotation + " " + rotatedValue);
        //transform.rotation = Vector3.SmoothDamp(transform.rotation, rotatedValue, ref velocity, smoothTime);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, rotatedValue, smoothTime*Time.deltaTime);
        
    }
}
