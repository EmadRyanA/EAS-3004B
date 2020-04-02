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
    
    /*
    [Serializable]
    public struct WinData{
        public int score;
        public int maxCombo;
        public int notesHit;
        public int mapTotalNotes;
        public int moneyEarned;
        public float expEarned;
    }
    */
    private PlayerClass playerClass;
    private GameObject[] cars;
    private Vector3[] cameraPositions;
    private Quaternion[] cameraRotations;
    private GameObject mainCamera;
    private int totalNotes;
    public const float TOTAL_PLAYER_HEALTH = 100;
    public static int _playerScore;
    public static int _playerCombo;
    public static int _playerMaxCombo;
    public static float _playerHealth;
    public static int _playerNotesHit;
    public static GameState _gameState; // switch this to indicate different states, e.g. menu, gameplay, death, etc.
    public enum GameState{
        playing,
        win,
        game_over
    }
    public float _playerHealthDecreaseRate; // decrease rate should change depending on BPM of song (faster BPM = faster rate)
    public static float _playerHealthRecoveryRate = 10;
    public static float _damageRate = 3; // potentially change this according to difficulty?
    public static AudioSource audioSrc;
    private float lastTime;
    private Text _pCombo;
    private Text _pScore;
    public static List<GameObject> objectiveList = new List<GameObject>();
    //private System.Random rand = new System.Random();
    
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
        //Screen.SetResolution(1280, 720, true, 60);
        //mainCamera = GameObject.Find("Main Camera").GetComponent<Camera>();
        // first index is initial camera position/rotation
        //cameraPositions = new Vector3[] {mainCamera.transform.position, new Vector3(-313, -527, 1880)}; 
        //cameraRotations = new Quaternion[] {mainCamera.transform.rotation, Quaternion.Euler(-1.813f, -181.159f, 0)};
        load_state = LOAD_STATE.NOT_LOADED;
        _gameState = GameState.playing;
       _playerScore = 0;
       _playerCombo = 1;
       _playerNotesHit = 0;
       _playerMaxCombo = 1;
       _pCombo = GameObject.Find("scoreText").GetComponent<Text>();
       _pScore = GameObject.Find("comboText").GetComponent<Text>();
       _playerHealth = 100f;
       _playerHealthDecreaseRate = 1f;
       lastTime = Time.time;
       totalNotes = 0; 

       mainCamera = GameObject.Find("Main Camera");

       //gameOverPanel = GameObject.Find("GameOverPanel");
       gameOverPanel.SetActive(false);
       //gameOverRetryButton = GameObject.Find("GameOverRetryButton");
       //gameOverQuitButton = GameObject.Find("GameOverQuitButton");

       gameOverRetryButton.GetComponent<Button>().onClick.AddListener(handleGameOverRetry);
       gameOverQuitButton.GetComponent<Button>().onClick.AddListener(handleGameOverQuit);

       audioSrc = this.GetComponent<AudioSource>(); // there is an audiosource in beatmapplayer, but it is more convenient to create one here
       
       // do not render the pause canvas on launch
       GameObject.Find("PauseCanvas").SetActive(false);

       LoadPlayerFromExternal(ref playerClass);

        // handle car visibility
        GameObject car_gt86 = GameObject.Find("car_gt86");
        GameObject car_merc = GameObject.Find("car_merc"); 
        GameObject car_lambo = GameObject.Find("car_lambo");
        
        cars = new GameObject[]{car_gt86, car_merc, car_lambo}; // keep the order here the same as in MainMenuCanvasController

        handleCarVisibility();

       beatMap = BeatMap.loadBeatMap();
       beatMap.loadSamples(this);

        // initial camera angle
        //mainCamera.transform.position = Vector3.MoveTowards(cameraPositions[1], cameraPositions[0], 20f);
        //mainCamera.transform.rotation = Quaternion.RotateTowards(cameraRotations[1], cameraRotations[0], 20f);
        

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
            //healthBarHandler();
            
            // decreases playerhealth at a fixed rate
            if(_playerHealth <=0){
                _gameState = GameState.game_over;
                //return;
            }
            if(_gameState == GameState.game_over){ // game over state
                //Debug.Log(_playerHealth); 
                gameOverPanel.SetActive(true);

                Time.timeScale = 0; // pauses game
                audioSrc.Stop(); // stops audio playback
            }else if(_gameState == GameState.win){ // victory state
                SceneManager.LoadScene("VictoryRoyaleScene");
            }else{ // otherwise keep decreasing the player's health
                // _gamestate == gamestate.playing
                if(Time.time - lastTime >= 0.1){ // decreases at a specified rate
                    _playerHealth -= _playerHealthDecreaseRate;
                    lastTime = Time.time;
                }
            } 
            GameObject.Find("HealthBar").GetComponent<Slider>().value = _playerHealth / TOTAL_PLAYER_HEALTH; 
        }
        
    }

    //private void healthBarHandler(){
         
    //}

    // when the quit button on the game over screen is clicked
    private void handleGameOverQuit(){
        Time.timeScale = 1; // unpause
        SceneManager.LoadScene("Main Menu");
    }
    // when the retry button on the game over screen is clicked
    private void handleGameOverRetry(){
        Time.timeScale = 1; // unpause

        // reset everything
        // delete all spawned game objects
        while(objectiveList.Count > 0){
            Destroy(objectiveList[0]);
            objectiveList.RemoveAt(0);
            //print(objectiveList.Count); 
        }
        totalNotes = 0;
        _playerHealth = 100f;
        _playerCombo = 1;
        _playerMaxCombo = 1;
        _playerScore = 0;
        _playerNotesHit = 0;
        _gameState = GameState.playing;
        GameObject.Find("TileManager").GetComponent<TileManager>().init(); // reset tilemanager (refactored Emad's code)
        gameOverPanel.SetActive(false);
        player.transform.position = new Vector3(0, 5, 0);
        generateBeats(); // respawn elements
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

    /*
    private int randPosNeg(){
        int rnd = rand.Next(0, 2);
        if(rnd == 0){
            return 1;
        }else{
            return -1;
        }

    }
    */

    void generateBeats(){
        
         //load the samples from the songFilePath
        float lastObjectiveZ = 0;
        GameObject objective = GameObject.Find("Objective");
        GameObject badObjective = GameObject.Find("BadObjective");
        GameObject trigger = GameObject.Find("MovementStateTrigger");
        Queue<LaneObject> bmQueue = beatMap.initLaneObjectQueue();
        
        audioSrc.clip = beatMap.getAudioClip();

        //float movementSpeed = GameObject.Find("Player").GetComponent<MovementScript>().movement_speed; // public, non static variable
        float movementSpeed = 100;

        //totalNotes = bmQueue.Count; // counting total notes to be used in win calculations
        // generate each objective 
        foreach(LaneObject laneObj in bmQueue){
            //float laneObjX = laneObj.lane;
            //GameObject spawnedObject;
            lastObjectiveZ = movementSpeed * laneObj.time;
            if (laneObj.type == LANE_OBJECT_TYPE.START_DRIFT_TRIGGER) {
                GameObject driftTrigger = Instantiate(trigger, new Vector3(laneToX(1), 0, lastObjectiveZ), Quaternion.Euler(0,0,0));
                driftTrigger.GetComponent<MovementStateTrigger>().stateToTrigger = MovementStateTrigger.movementStates.Drifting;
            }
            else if (laneObj.type == LANE_OBJECT_TYPE.START_FLY_TRIGGER) {
                GameObject flyTrigger = Instantiate(trigger, new Vector3(laneToX(1), 0, lastObjectiveZ), Quaternion.Euler(0,0,0));
                flyTrigger.GetComponent<MovementStateTrigger>().stateToTrigger = MovementStateTrigger.movementStates.Flying;
            }
            else if (laneObj.type == LANE_OBJECT_TYPE.START_NORMAL_TRIGGER) {
                GameObject driftTrigger = Instantiate(trigger, new Vector3(laneToX(1), 0, lastObjectiveZ), Quaternion.Euler(0,0,0));
                driftTrigger.GetComponent<MovementStateTrigger>().stateToTrigger = MovementStateTrigger.movementStates.Driving;
            }
            else if(laneObj.type == LANE_OBJECT_TYPE.Obstacle){ // obstacle
                objectiveList.Add(Instantiate(badObjective, new Vector3(laneToX(laneObj.lane), 0, lastObjectiveZ), Quaternion.Euler(0,0,0)));
            }else if (laneObj.type == LANE_OBJECT_TYPE.Beat) { // beat
                totalNotes++;
                objectiveList.Add(Instantiate(objective, new Vector3(laneToX(laneObj.lane), 0, lastObjectiveZ), Quaternion.Euler(0,0,0)));
            }
            
            // populate objective list
            

            //for(int i = 0; i<rand.Next(-1, 3); i++){ // generate up to 3 badobjectives per objective
                // generates a badobjective at a random offset between x and y, either behind or in front the objective in a random lane
                //Instantiate(badObjective, new Vector3(laneToX(rand.Next(0, 3)), 0, (movementSpeed * laneObj.time) + (randPosNeg() * rand.Next(25, 50))), Quaternion.Euler(0,0,0));
            //}
            
            //print(laneObj.lane);
            
        }
        // generate a trigger at the end of the track, 5 seconds after the last object spawned
        GameObject gameEndTrigger = Instantiate(trigger, new Vector3(laneToX(1), 0, lastObjectiveZ + (5*movementSpeed)), Quaternion.Euler(0,0,0)); //new Vector3(laneToX(1), 0, lastObjectiveZ + (5*movementSpeed))
        gameEndTrigger.GetComponent<MovementStateTrigger>().stateToTrigger = MovementStateTrigger.movementStates.GameEnd;
    }

   /*  public void saveToExternal(WinDataClass wd){
         
        string dest = Application.persistentDataPath + "/winData.dat";
        FileStream file;
        //string jsonStr = JsonUtility.ToJson(player);

        if(File.Exists(dest)){
            file = File.OpenWrite(dest);
        }else{
            file = File.Create(dest);
        }

        BinaryFormatter bf = new BinaryFormatter();
        bf.Serialize(file, wd);
        file.Close();
    } */

    // handles updating the player data after a win
    // calculating the grade, etc.
    public void handleGameWin(){
        print("handleGameWin()");
        //decimal moneyEarnedDecimal = (decimal)(Math.Sqrt(_playerScore) * (_playerNotesHit/totalNotes) + _playerMaxCombo)/100;
        //decimal expEarnedDecimal = (decimal)(Math.Sqrt(_playerScore)) * (_playerNotesHit/totalNotes) + (_playerMaxCombo == 1 ? 0 : _playerMaxCombo);


        WinDataClass winData = new WinDataClass(0,0,0,0,0,0, "");
        string calculatedGrade = "";
        float notePercentage = (float)((decimal)_playerNotesHit/(decimal)totalNotes);

        if(notePercentage < 0.5){
            calculatedGrade = "D";
        }else if(notePercentage >= 0.5 && notePercentage < 0.6){
            calculatedGrade = "C";
        }else if(notePercentage >= 0.6 && notePercentage < 0.7){
            calculatedGrade = "B";
        }else if(notePercentage >= 0.7 && notePercentage < 0.8){
            calculatedGrade = "A";
        }else if(notePercentage >= 0.8 && notePercentage < 0.9){
            calculatedGrade = "S";
        }else if(notePercentage >= 0.9 && notePercentage < 1){
            calculatedGrade = "SS";
        }else{
            calculatedGrade = "ACE";
        }
        

        winData.score = _playerScore;
        winData.maxCombo = _playerMaxCombo;
        winData.notesHit = _playerNotesHit;
        winData.mapTotalNotes = totalNotes; // can calculate the percentage using this and above
        winData.moneyEarned = (int)Math.Round((Math.Sqrt(_playerScore) * (float)((decimal)_playerNotesHit/(decimal)totalNotes) + _playerMaxCombo)/100.0); // round money earned to closest integer
        winData.expEarned = (float)Math.Round(((Math.Sqrt(_playerScore)) * (float)((decimal)_playerNotesHit/(decimal)totalNotes) + (_playerMaxCombo == 1 ? 0 : _playerMaxCombo)), 2); // round exp to 2 digits
        winData.grade = calculatedGrade;

        print("big equation thing: " + Math.Round(((Math.Sqrt(_playerScore) * (float)((decimal)_playerNotesHit/(decimal)totalNotes) + _playerMaxCombo)/100.0), 2));
        /* 
        print("square root: " + (float)(Math.Sqrt(_playerScore)));
        print("percentage: " + (float)((_playerNotesHit/totalNotes)*1.0));
        print("winData.moneyEarned: " + winData.moneyEarned);
        print("winData.expEarned: " + winData.expEarned); */

        WinDataClassHelper.saveToExternal(winData);

        _gameState = GameState.win;
        //SceneManager.LoadScene("VictoryRoyaleScene");
    }

    private void handleCarVisibility(){
        int counter = 0;
        print(playerClass.currentCarID);
        cars[2].SetActive(false);
        foreach(GameObject obj in cars){
            print(counter);
            if(counter == playerClass.currentCarID){
                //obj.transform.position = carPositions[]
                obj.SetActive(true);
                mainCamera.GetComponent<SmoothCameraController>().player = cars[counter];
            }else{
                obj.SetActive(false);
            }
            counter++;
        }
    }

    public static void LoadPlayerFromExternal(ref PlayerClass player){
        string dest = Application.persistentDataPath + "/player.dat";
        FileStream file;

        if(File.Exists(dest)){
            file = File.OpenRead(dest);
        }else{
            // create a default player json if it doesn't exist
            //file = File.Create(dest);
            player = new PlayerClass("NewPlayer", 0, 0, 1000, 1500, 0);
            /*player.name = "NewPlayer";
            player.level = 0;
            player.currentExperience = 0;
            player.experienceForNextLevel = 1000;
            player.money = 1500;*/
            print(player.money);
            //print("here");
            //savePlayerToExternal(player);
            return; // something here
        }
        BinaryFormatter bf = new BinaryFormatter();
        
        player = (PlayerClass) bf.Deserialize(file);
        print(player.name);
        print(player.money);
        //player = new PlayerClass("", 0,0,0,0);
        
        file.Close();


        //return JsonUtility.FromJson<PlayerClass>(json.text);
    }
}
