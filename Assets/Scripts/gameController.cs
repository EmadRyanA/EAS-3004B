using System.Collections;
using System.Collections.Generic;
using System.IO;
using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
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
    
    // game over panel
    public GameObject gameOverPanel;
    public GameObject player;
    public GameObject gameOverRetryButton;
    public GameObject gameOverQuitButton;
    BeatMap beatMap;

    public enum LOAD_STATE {
        NOT_LOADED,
        LOADED
    }

    public LOAD_STATE load_state;

    void Start()
    {
        load_state = LOAD_STATE.NOT_LOADED;
        _gameState = null;
       _playerScore = 0;
       _playerCombo = 1; 
       _pCombo = GameObject.Find("scoreText").GetComponent<Text>();
       _pScore = GameObject.Find("comboText").GetComponent<Text>();
       _playerHealth = 100f;
       _playerHealthDecreaseRate = 1f;
       lastTime = Time.time;

       //gameOverPanel = GameObject.Find("GameOverPanel");
       gameOverPanel.SetActive(false);
       //gameOverRetryButton = GameObject.Find("GameOverRetryButton");
       //gameOverQuitButton = GameObject.Find("GameOverQuitButton");

       gameOverRetryButton.GetComponent<Button>().onClick.AddListener(handleGameOverRetry);
       gameOverQuitButton.GetComponent<Button>().onClick.AddListener(handleGameOverQuit);

       audioSrc = this.GetComponent<AudioSource>(); // there is an audiosource in beatmapplayer, but it is more convenient to create one here
       
       // do not render the pause canvas on launch
       GameObject.Find("PauseCanvas").SetActive(false);

       beatMap = BeatMapPlayer.loadBeatMap();
       beatMap.loadSamples(this);
       // temp, generate gameobjects based on beatmap
       //Stream openFileStream = File.OpenRead(Application.persistentDataPath + "/BeatMaps/testBeatmap.dat");
        //BinaryFormatter deserializer = new BinaryFormatter();
        //BeatMap beatMap = (BeatMap)deserializer.Deserialize(openFileStream);
    }

    // Update is called once per frame
    private void Update() {

        if (load_state == LOAD_STATE.NOT_LOADED) {
            if (beatMap.state == BeatMap.STATE.SAMPLES_LOADED) {
                generateBeats();
                audioSrc.Play();
                load_state = LOAD_STATE.LOADED;
            }
        }
        else {
            // score handler
            _pCombo.text = _playerCombo + "x";
            _pScore.text = _playerScore + "";

            // health/healthbar handler
            //Debug.Log(_playerHealth);
            healthBarHandler();
            GameObject.Find("HealthBar").GetComponent<Slider>().value = _playerHealth / TOTAL_PLAYER_HEALTH; 
        }
        
    }

    private void healthBarHandler(){
        // decreases playerhealth at a fixed rate
        if(_playerHealth <=0){
            _gameState = "game_over";
            //Debug.Log(_playerHealth); 
            gameOverPanel.SetActive(true);

            Time.timeScale = 0; // pauses game
            audioSrc.Stop(); // stops audio playback

            return;
        }
        if(Time.time - lastTime >= 0.1){
            _playerHealth -= _playerHealthDecreaseRate;
            lastTime = Time.time;
        }
    }

    // when the quit button on the game over screen is clicked
    private void handleGameOverQuit(){
        Time.timeScale = 1; // unpause
        SceneManager.LoadScene("Main Menu");
    }
    // when the retry button on the game over screen is clicked
    private void handleGameOverRetry(){
        Time.timeScale = 1; // unpause
        
        // reset everything
        _playerHealth = 100f;
        GameObject.Find("TileManager").GetComponent<TileManager>().init(); // reset tilemanager (refactored Emad's code)
        gameOverPanel.SetActive(false);
        player.transform.position = new Vector3(0, 5, 0);
        generateBeats();
        audioSrc.Play();
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

    void generateBeats(){
        
         //load the samples from the songFilePath
        GameObject objective = GameObject.Find("Objective");
        GameObject badObjective = GameObject.Find("BadObjective");
        
        audioSrc.clip = beatMap.getAudioClip();

        float movementSpeed = GameObject.Find("Player").GetComponent<MovementScript>().movement_speed; // public, non static variable
        foreach(LaneObject laneObj in beatMap.initLaneObjectQueue()){
            //float laneObjX = laneObj.lane;
            
            if(laneObj.type==0){ // obstacle
                Instantiate(badObjective, new Vector3(laneToX(laneObj.lane), 0, (movementSpeed * laneObj.time)), Quaternion.Euler(0,0,0));
            }else{ // beat
                Instantiate(objective, new Vector3(laneToX(laneObj.lane), 0, (movementSpeed * laneObj.time)), Quaternion.Euler(0,0,0));
            }
            
            
            //for(int i = 0; i<rand.Next(-1, 3); i++){ // generate up to 3 badobjectives per objective
                // generates a badobjective at a random offset between x and y, either behind or in front the objective in a random lane
                //Instantiate(badObjective, new Vector3(laneToX(rand.Next(0, 3)), 0, (movementSpeed * laneObj.time) + (randPosNeg() * rand.Next(25, 50))), Quaternion.Euler(0,0,0));
            //}
            print(laneObj.lane);
            
        }
    }
}
