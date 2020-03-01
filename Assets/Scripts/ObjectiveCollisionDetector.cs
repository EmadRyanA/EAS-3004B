using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectiveCollisionDetector : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject obj;
    void Start()
    {

        //Debug.Log(GetComponent<Renderer>().enabled);
        GetComponent<Renderer>().enabled = true;
    }

    // Update is called once per frame

    //private void OnDestroy() {
        
    //}

    private void OnTriggerEnter(Collider other){
       print(other.name);
       /*
       if(other.name == "PlayerHitbox"){
            Destroy(this);
       }
       */
       if(other.name == "PlayerHitbox"){
           if(this.tag == "BadObjective"){
                gameController._playerCombo = 1;
                gameController._playerHealth -= gameController._damageRate;
            }else if(this.tag == "Objective"){ // good objective
                Debug.Log("here");
                gameController._playerScore += gameController._playerCombo * 100;
                gameController._playerCombo += 1;
                gameController._playerHealth += gameController._playerHealthRecoveryRate;
            if(gameController._playerHealth > 100){
                gameController._playerHealth = 100;
            }
        }
       } 
       


        GetComponent<Animation>().Play("objective_collide");
        //Destroy(this);
        //GetComponent<Renderer>().enabled = false; 
       
    }
}
