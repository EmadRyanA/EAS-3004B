using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// switches the car's current state to the selected, upon colliding with an invisible trigger.
public class MovementStateTrigger : MonoBehaviour
{
    // Start is called before the first frame update
    public enum movementStates{
        Driving, // 0
        Drifting, // 1
        Flying, // 2
        GameEnd
    };
    private GameObject notificationText;
    private bool startTimer = false;
    private float timer = 0.0f;
    public movementStates stateToTrigger;

    void Start()
    {
        notificationText = GameObject.Find("notificationText");
    }

    // Update is called once per frame
    void Update()
    {
        if(startTimer){
            // count down to remove the notification text.
            // removes the text at 4 seconds.
            if(timer > 4.0f){
                notificationText.GetComponent<Text>().text = "";
                startTimer = false;
            }else{
                timer+= Time.deltaTime;
            } 
        }
    }

    private void OnTriggerEnter(Collider other) {
      print("test");
        switch((int)stateToTrigger){
            case 0: // driving
                print("movement state changed to driving");
                MovementScript.movementState = 0;
                break;
            case 1: // drifting
                print("movement state changed to drifting");
                notificationText.GetComponent<Text>().text = "Tilt your phone to drift!";
                startTimer = true;
                timer = 0.0f;
                MovementScript.movementState = 1;
                break;
            case 2: // flying
                print("movement state changed to flying");
                notificationText.GetComponent<Text>().text = "Tap right or left to fly!";
                startTimer = true;
                timer = 0.0f;
                MovementScript.movementState = 2;
                break;
            case 3: // gameend, place a gameend trigger at the end of every stage
                GameObject.Find("Game Controller").GetComponent<gameController>().handleGameWin();
                break;
            default:
                break;
        }
        //Destroy(this);  
    } 
}
