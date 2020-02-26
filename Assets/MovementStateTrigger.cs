using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// switches the car's current state to the selected, upon colliding with an invisible trigger.
public class MovementStateTrigger : MonoBehaviour
{
    // Start is called before the first frame update
    public enum movementStates{
        Driving, // 0
        Drifting, // 1
        Flying // 2
    };
    public movementStates stateToTrigger;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
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
                MovementScript.movementState = 1;
                break;
            case 2: // flying
                print("movement state changed to flying");
                MovementScript.movementState = 2;
                break;
            default:
                break;
        }
        //Destroy(this);  
    } 
}
