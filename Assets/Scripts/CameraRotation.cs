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
    void Start(){
        //Camera cam = gameObject.GetComponent<Camera>();
        
        rotatedValue = Quaternion.Euler(new Vector3(transform.rotation.x, transform.rotation.y, 0.0f));
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        Debug.Log(rotatedValue.x + " " + rotatedValue.y);
        float moveHorizontal = Input.GetAxis("Horizontal");
        float moveVertical = Input.GetAxis("Vertical");
        //float rotatedZ = 0.0f;
        if(moveHorizontal != 0){
            //if(Math.Abs(transform.eulerAngles.z) < rotateIntensity){
            //Debug.Log(moveHorizontal);
            rotatedValue = Quaternion.Euler(new Vector3(rotatedValue.x, rotatedValue.y, rotateIntensity*moveHorizontal));
            //}
        }else{
            rotatedValue = Quaternion.Euler(new Vector3(rotatedValue.x, rotatedValue.y,  0.0f));
        }

        //transform.eulerAngles = rotatedValue;
        //transform.eulerAngles = Vector3.Lerp(transform.eulerAngles, rotatedValue, 0.25f);
        //Debug.Log(transform.rotation + " " + rotatedValue);
        //transform.rotation = Vector3.SmoothDamp(transform.rotation, rotatedValue, ref velocity, smoothTime);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, rotatedValue, smoothTime*Time.deltaTime);
        
    }
}
