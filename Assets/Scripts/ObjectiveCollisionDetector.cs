using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectiveCollisionDetector : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject obj;
    AudioSource badObjectiveAudio;
    AudioSource objectiveAudio;
    AudioSource[] sounds;
    void Start()
    {
        badObjectiveAudio = GameObject.Find("BadObjective").GetComponent<AudioSource>();
        objectiveAudio = GameObject.Find("Objective").GetComponent<AudioSource>();
        //Debug.Log(GetComponent<Renderer>().enabled);
        GetComponent<Renderer>().enabled = true;
        sounds = gameController.sounds;
    }

    // Update is called once per frame

    private void OnTriggerEnter(Collider other){
       print(other.name);
       /*
       if(other.name == "PlayerHitbox"){
            Destroy(this);
       }
       */
       // delete this object from the objectiveList
        //gameController.objectiveList.RemoveAt(gameController.objectiveList.FindIndex(o => o == this.gameObject));

       if(other.name == "PlayerHitbox"){
           if(this.tag == "BadObjective"){ 
               // play bad hit sound
                sounds[2].Play();
                gameController._playerCombo = 1;
                gameController._playerHealth -= gameController._damageRate;
            }else if(this.tag == "Objective"){ // good objective
                Debug.Log("here");
                // play good hit sound
                sounds[1].Play();
                gameController._playerScore += gameController._playerCombo * 100;
                gameController._playerCombo += 1;
                if(gameController._playerCombo > gameController._playerMaxCombo){
                    gameController._playerMaxCombo = gameController._playerCombo;
                }
                gameController._playerHealth += gameController._playerHealthRecoveryRate;
                gameController._playerNotesHit++;
            if(gameController._playerHealth > 100){
                gameController._playerHealth = 100;
            }
        }
        //this.GetComponent<AudioSource>().Play();
       } 
        GetComponent<Animation>().Play("objective_collide");
        
        //Destroy(this);
        //GetComponent<Renderer>().enabled = false; 
       
    }

}
