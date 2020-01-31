using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectiveCollisionDetector : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject obj;
    void Start()
    {

        Debug.Log(GetComponent<Renderer>().enabled);
        GetComponent<Renderer>().enabled = true;
    }

    // Update is called once per frame

    private void OnDestroy() {
        if(this.name == "BadObjective"){
            gameController._playerCombo = 1;
        }else{
            gameController._playerScore += gameController._playerCombo * 100;
            gameController._playerCombo += 1;
        }

        GetComponent<Animation>().Play("objective_collide");
        //GetComponent<Renderer>().enabled = false; 
    }

    private void OnTriggerEnter(Collider other){
       Debug.Log(other.name);
       if(other.name == "PlayerHitbox"){
            Destroy(this);
       }
       
    }
}
