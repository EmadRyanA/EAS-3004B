using System.Collections;
using System.Collections.Generic;
using System.IO;
using System;
using UnityEngine;
using UnityEngine.UI;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;

public class gameController : MonoBehaviour
{
    // Start is called before the first frame update
    // serves to initialize and store some important values
    public const float TOTAL_PLAYER_HEALTH = 100;
    
    public static int _playerScore;
    public static int _playerCombo;
    public static float _playerHealth;
    public static string _gameState; // switch this to indicate different states, e.g. menu, gameplay, death, etc.
    public float _playerHealthDecreaseRate; // decrease rate should change depending on BPM of song (faster BPM = faster rate)
    public static float _playerHealthRecoveryRate = 10;
    public static float _damageRate = 3; // potentially change this according to difficulty?
    public static AudioSource audioSrc;

    private float lastTime;
    private Text _pCombo;
    private Text _pScore;
    private System.Random rand = new System.Random();
    public GameObject gameOverPanel;
    
    void Start()
    {
        _gameState = null;
       _playerScore = 0;
       _playerCombo = 1; 
       _pCombo = GameObject.Find("scoreText").GetComponent<Text>();
       _pScore = GameObject.Find("comboText").GetComponent<Text>();
       _playerHealth = 100f;
       _playerHealthDecreaseRate = 5f;
       lastTime = Time.time;

       //gameOverPanel = GameObject.Find("GameOverPanel");

       audioSrc = this.GetComponent<AudioSource>(); // there is an audiosource in beatmapplayer, but it is more convenient to create one here
       
       // do not render the pause canvas on launch
       GameObject.Find("PauseCanvas").SetActive(false);

       // temp, generate gameobjects based on beatmap
       //Stream openFileStream = File.OpenRead(Application.persistentDataPath + "/BeatMaps/testBeatmap.dat");
        //BinaryFormatter deserializer = new BinaryFormatter();
        //BeatMap beatMap = (BeatMap)deserializer.Deserialize(openFileStream);
        BeatMap beatMap = BeatMapPlayer.loadBeatMap(BeatMapPlayer.fileName);
        GameObject objective = GameObject.Find("Objective");
        GameObject badObjective = GameObject.Find("BadObjective");
        
        audioSrc.clip = beatMap.getAudioClip();

        float movementSpeed = GameObject.Find("Player").GetComponent<MovementScript>().movement_speed; // public, non static variable
        foreach(LaneObject laneObj in beatMap.initLaneObjectQueue()){
            //float laneObjX = laneObj.lane;
            
            Instantiate(objective, new Vector3(laneToX(laneObj.lane), 0, (movementSpeed * laneObj.time)), Quaternion.Euler(0,0,0));
            for(int i = 0; i<rand.Next(-1, 3); i++){ // generate up to 3 badobjectives per objective
                // generates a badobjective at a random offset between x and y, either behind or in front the objective in a random lane
                Instantiate(badObjective, new Vector3(laneToX(rand.Next(0, 3)), 0, (movementSpeed * laneObj.time) + (randPosNeg() * rand.Next(25, 50))), Quaternion.Euler(0,0,0));
            }
            print(laneObj.lane);
            
        }
        //print(beatMap.initLaneObjectQueue().Peek().lane);
        audioSrc.Play();
    }

    // Update is called once per frame
    private void Update() {
        // score handler
        _pCombo.text = _playerCombo + "x";
        _pScore.text = _playerScore + "";

        // health/healthbar handler
        //Debug.Log(_playerHealth);
        healthBarHandler();
        GameObject.Find("HealthBar").GetComponent<Slider>().value = _playerHealth / TOTAL_PLAYER_HEALTH; 
    }

    private void healthBarHandler(){
        // decreases playerhealth at a fixed rate
        if(_playerHealth <=0){
            _gameState = "game_over";
            Debug.Log(_gameState); 
            gameOverPanel.SetActive(true);
            return;
        }
        if(Time.time - lastTime >= 0.1){
            _playerHealth -= _playerHealthDecreaseRate;
            lastTime = Time.time;
        }
    }

    private float laneToX(int lane){
        float laneObjX = 0;
        switch(lane){
            case 0: // first lane
                laneObjX = -25;
                break;
            case 1:
                laneObjX = 0;
                break;
            case 2:
                laneObjX = 25;
                break;
        }
        return laneObjX;
    }

    private int randPosNeg(){
        int rnd = rand.Next(0, 2);
        if(rnd == 0){
            return 1;
        }else{
            return -1;
        }

    }
}
