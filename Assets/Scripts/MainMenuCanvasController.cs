using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine.SceneManagement;
using System.Linq;
using UnityEngine.Networking;



public class MainMenuCanvasController : MonoBehaviour
{
    //Button playButton;
    private bool updated = false; // prevents constant updating of the buttons, etc.
    //Vector3 comparisonVector3;
    PlayerClass player;
    GameObject playButton;
    GameObject exitGameButton;
    GameObject garageButton;
    GameObject nextCarButton;
    GameObject prevCarButton;
    GameObject returnToMainMenuGarageButton;
    GameObject title;
    GameObject coinImage; // does this include the money text as well?
    GameObject currentMoney;
    // userprofilecanvas stuff
    GameObject userProfileCanvas;
    GameObject usernameText;
    GameObject levelText;
    GameObject experienceText;
    GameObject experienceBar;
    GameObject editNameButton;
    Button exitButton;
    //public Button generateBeatMapButton;
    GameObject generateBeatMapButton;
    Camera mainCamera;
    GameObject MapSelect;
    GameObject Garage;
    GameObject mapsContent;
    GameObject loadBeatMapButton;
    //public static string beatmapDir; 
    GameObject purchaseButton;
    GameObject carPrice;
    private int currentCarIndex;
    // car switching 
    private GameObject playerCar;

    //BGA Menu GameObjects
    GameObject BGAMenu;
    GameObject easyButton;
    GameObject normalButton;
    GameObject hardButton;
    GameObject BGAExitButton;
    GameObject selectSongButton;
    GameObject generateButton;
    GameObject seedInput;
    GameObject okButton;
    public Text songNameText;
    GameObject songDurationText;
    public int currentSeed = 0;
    GameObject generatingText;
    GameObject loadingAnimationText;
    GameObject generatingCanvas;
    GameObject songArt;

    //For BGA
    public BGA bga;
    public AudioClip inputAudioClip; //To be loaded from web url or file url

    #if !UNITY_ANDROID
    #endif

    public enum STATE
    {
        READY,
        AUDIO_CLIP_LOADING,
        AUDIO_CLIP_ERROR,
        AUDIO_CLIP_LOADED,
        START_BGA,
        BGA_STARTED,
        BGA_FINISHED
    }
    public enum DIFFICULTY
    {
        EASY,
        NORMAL,
        HARD
    }
    public DIFFICULTY difficulty;
    public STATE state;
    public string path;
    public song_meta_struct song;

    //for cars
    public struct carBundle{
        
        public carBundle(GameObject c, int p) : this(){
            carObject = c;
            price = p;
        }
        public GameObject carObject{get;}
        public int price{get;}
        
    }
    public carBundle[] cars;
    
    // Start is called before the first frame update
    int currentState = 2;
    void Start()
    {
        bga = new BGA();
        mapsContent = GameObject.Find("MapsContent");
        currentCarIndex = MainMenuController.player.currentCarID;
        playButton = GameObject.Find("PlayButton");
        exitGameButton = GameObject.Find("ExitGameButton");
        garageButton = GameObject.Find("GarageButton");
        returnToMainMenuGarageButton = GameObject.Find("ReturnToMainMenuGarageButton");
        nextCarButton = GameObject.Find("NextCarButton");
        prevCarButton = GameObject.Find("PrevCarButton");
        coinImage = GameObject.Find("MoneyImage");
        currentMoney = coinImage.transform.GetChild(0).gameObject;
        mainCamera = GameObject.Find("Main Camera").GetComponent<Camera>();
        exitButton = GameObject.Find("ExitButton").GetComponent<Button>();
        MapSelect = GameObject.Find("MapSelectCanvas");
        Garage = GameObject.Find("CarSelectCanvas");
        title = GameObject.Find("Title");
        generateBeatMapButton = GameObject.Find("GenerateBeatMapButton");
        okButton = GameObject.Find("OKButton");
        // user profile canvas initializers
        userProfileCanvas = GameObject.Find("UserProfileCanvas");
        usernameText = userProfileCanvas.transform.Find("Username").gameObject;
        levelText = userProfileCanvas.transform.Find("LevelText").gameObject;
        experienceText = userProfileCanvas.transform.Find("ExperienceText").gameObject;
        experienceBar = userProfileCanvas.transform.Find("ExperienceBar").gameObject;
        editNameButton = GameObject.Find("EditNameButton").gameObject;

        purchaseButton = GameObject.Find("PurchaseButton").gameObject;
        carPrice = GameObject.Find("CarPrice").gameObject;
        
        //BGA canvas initializers
        BGAMenu = GameObject.Find("BGACanvas");
        easyButton = GameObject.Find("Easy");
        normalButton = GameObject.Find("Normal");
        hardButton = GameObject.Find("Hard");
        BGAExitButton = GameObject.Find("BGAExit");
        selectSongButton = GameObject.Find("SelectSongButton");
        generateButton = GameObject.Find("GenerateButton");
        songNameText = GameObject.Find("SongTitle").GetComponent<Text>();
        seedInput = GameObject.Find("SeedInput");
        generatingText = GameObject.Find("GeneratingText");
        loadingAnimationText = GameObject.Find("LoadingAnimationText");
        generatingCanvas = GameObject.Find("GeneratingCanvas");
        songDurationText = GameObject.Find("SongDuration");
        songArt = GameObject.Find("SongArt");


        // listeners
        playButton.GetComponent<Button>().onClick.AddListener(toBeatmapSelection);
        exitButton.onClick.AddListener(toBeatmapSelection);
        exitGameButton.GetComponent<Button>().onClick.AddListener(exitGame);
        garageButton.GetComponent<Button>().onClick.AddListener(toGarage);
        returnToMainMenuGarageButton.GetComponent<Button>().onClick.AddListener(toGarage);
        generateBeatMapButton.GetComponent<Button>().onClick.AddListener(toBGA);
        editNameButton.GetComponent<Button>().onClick.AddListener(toEditUsername);
        nextCarButton.GetComponent<Button>().onClick.AddListener(handleNextCarButton);
        prevCarButton.GetComponent<Button>().onClick.AddListener(handlePrevCarButton);
        purchaseButton.GetComponent<Button>().onClick.AddListener(handlePurchaseButton);
        
        //BGA Menu listeners
        easyButton.GetComponent<Button>().onClick.AddListener(easyButtonListener);
        normalButton.GetComponent<Button>().onClick.AddListener(normalButtonListener);
        hardButton.GetComponent<Button>().onClick.AddListener(hardButtonListener);
        selectSongButton.GetComponent<Button>().onClick.AddListener(selectFileListener);
        generateButton.GetComponent<Button>().onClick.AddListener(generateBeatMapListener);
        BGAExitButton.GetComponent<Button>().onClick.AddListener(toBGA);
        okButton.GetComponent<Button>().onClick.AddListener(okButtonListener);

        MapSelect.SetActive(false);

        // the user's profile should be visible in all states, so set visible here
        //userProfileCanvas.SetActive(true);

        //initiate the user's data
        usernameText.GetComponent<Text>().text = MainMenuController.player.name;
        levelText.GetComponent<Text>().text = "Level: " + MainMenuController.player.level;
        experienceText.GetComponent<Text>().text = "XP: " + MainMenuController.player.currentExperience + " / " + (MainMenuController.player.experienceForNextLevel);
        experienceBar.GetComponent<Slider>().value = MainMenuController.player.currentExperience/(MainMenuController.player.experienceForNextLevel);

        //comparisonVector3 = new Vector3(0.01f, 0.01f, 0.01f);
        
        playerCar = GameObject.Find("PlayerCar");

        // load cars
        GameObject car_gt86 = GameObject.Find("car_gt86");
        GameObject car_merc = GameObject.Find("car_merc");
        GameObject car_lambo = GameObject.Find("car_lambo");
        //GameObject car_gtr_test = GameObject.Find("car_gtr"); 
        //new carBundle(car_gtr_test, 1)
        cars = new carBundle[]{new carBundle(car_gt86, 0), new carBundle(car_merc, 10000), new carBundle(car_lambo, 75000)}; // do not change indices

        // make the user's current car active.
        handleCarVisibility();
        /*
        path = Application.persistentDataPath + "/Songs/Dreams~Lost Sky~Dreams"; //for testing on windows: set the path var to a song
        song = new song_meta_struct("Dreams", "Lost Sky", "Dreams"); //for testing; set the meta data
        songNameText.text = song.title + " by " + song.artist;//for testing; set the text UI
        */
    }

    // Update is called once per frame
    void Update()
    {
        //Debug.Log(currentState);
        // beatmap select pov
        if(!updated){
            if(currentState == 1){
                generatingCanvas.SetActive(false);
                userProfileCanvas.SetActive(false);
                BGAMenu.SetActive(false);
                MapSelect.SetActive(true);
                
                playButton.SetActive(false);
                exitGameButton.SetActive(false);
                garageButton.SetActive(false);

                coinImage.SetActive(false);

                purchaseButton.SetActive(false);
                carPrice.SetActive(false);

                title.SetActive(false);
                

                //Debug.Log(MainMenuController.cameraLocations + " " + MainMenuController.cameraRotations);
                mainCamera.transform.position = Vector3.Lerp(mainCamera.transform.position, MainMenuController.cameraLocations[1], Time.deltaTime);
                mainCamera.transform.rotation = Quaternion.RotateTowards(mainCamera.transform.rotation, MainMenuController.cameraRotations[1], 70*Time.deltaTime);  

                if(mainCamera.transform.position == MainMenuController.cameraLocations[1] && mainCamera.transform.rotation == MainMenuController.cameraRotations[1]){
                    // only stops updating once the camera completes its transition
                    updated = true;
                }

            
            // main menu pov
            }else if(currentState == 2){
                // disabling other canvases
                userProfileCanvas.SetActive(true);
                generatingCanvas.SetActive(false);
                MapSelect.SetActive(false);
                Garage.SetActive(false);
                BGAMenu.SetActive(false);

                playButton.SetActive(true);
                exitGameButton.SetActive(true);
                garageButton.SetActive(true);

                coinImage.SetActive(false);

                title.SetActive(true);

                mainCamera.transform.position = Vector3.Lerp(mainCamera.transform.position, MainMenuController.cameraLocations[0], Time.deltaTime);
                mainCamera.transform.rotation = Quaternion.RotateTowards(mainCamera.transform.rotation, MainMenuController.cameraRotations[0], 60*Time.deltaTime);
            
                if(mainCamera.transform.position == MainMenuController.cameraLocations[0] && mainCamera.transform.rotation == MainMenuController.cameraRotations[0]){
                    // only stops updating once the camera completes its transition
                    updated = true;
                }
                
                handleCarVisibility(); // sets the visible car to the player's currently selected one

            // garage pov
            }else if(currentState == 3){
                // enabling the car selection canvas 
                Garage.SetActive(true);

                userProfileCanvas.SetActive(true);

                //disabling buttons
                playButton.SetActive(false);
                exitGameButton.SetActive(false);
                garageButton.SetActive(false);
                BGAMenu.SetActive(false);

                coinImage.SetActive(true);
                currentMoney.GetComponent<Text>().text = MainMenuController.player.money + "";

                purchaseButton.SetActive(true);
                carPrice.SetActive(true);
                
                // the player owns the current car
                if(MainMenuController.player.ownedCars.Contains(currentCarIndex)){
                    purchaseButton.GetComponent<Button>().interactable = true; // greys out the button
                    purchaseButton.GetComponentInChildren<Text>().text = "Select";
                    carPrice.SetActive(false); 
                }else{
                    // player cannot afford car
                    if(cars[currentCarIndex].price > MainMenuController.player.money){
                        purchaseButton.GetComponent<Button>().interactable = false;
                        purchaseButton.GetComponentInChildren<Text>().text = "Insufficient Funds";
                    // player can afford car
                    }else{
                        purchaseButton.GetComponent<Button>().interactable = true;
                        purchaseButton.GetComponentInChildren<Text>().text = "Purchase";
                    }
                    carPrice.transform.GetChild(0).GetComponent<Text>().text = cars[currentCarIndex].price + "";
                    carPrice.SetActive(true);
                }
                
                title.SetActive(false);

                mainCamera.transform.position = Vector3.Lerp(mainCamera.transform.position, MainMenuController.cameraLocations[2], Time.deltaTime);
                mainCamera.transform.rotation = Quaternion.RotateTowards(mainCamera.transform.rotation, MainMenuController.cameraRotations[2], 30*Time.deltaTime);

                if(mainCamera.transform.position == MainMenuController.cameraLocations[2] && mainCamera.transform.rotation == MainMenuController.cameraRotations[2]){
                    // only stops updating once the camera completes its transition
                    updated = true;
                }

            }else if(currentState == 4){
                //disable other canvases, enable bgacanvas
                
                MapSelect.SetActive(false);
                BGAMenu.SetActive(true);

                if(seedInput.GetComponent<InputField>().text != ""){
                    currentSeed = int.Parse(seedInput.GetComponent<InputField>().text);
                }

                if(state == STATE.AUDIO_CLIP_LOADED && inputAudioClip != null){
                    float songLength = inputAudioClip.length;
                    songDurationText.GetComponent<Text>().text = (int)songLength/60 + ":" + (int)(songLength%60);
                    //load album art of song
                    try {
                        print("ALJFLAHSLKFLKAHFLKASLKFALKSFJLKAJSLKFJSA");
                        byte[] bytes = System.IO.File.ReadAllBytes(path + ".png");
                        Texture2D texture2D = new Texture2D(1, 1);
                        texture2D.LoadImage(bytes);
                        Sprite sprite = Sprite.Create(texture2D, new Rect(0, 0, texture2D.width, texture2D.height), new Vector2(0, 0));
                        if (sprite != null) songArt.GetComponent<Image>().sprite = sprite;
                    }
                    catch (IOException e) {
                        Debug.Log("err");
                            Debug.Log(e);
                    }               
                }

                //if generateBGA button is hit
                if (state == STATE.START_BGA)
                {
                    Debug.Log("Starting BGA");
                    print(currentSeed);
                    state = STATE.BGA_STARTED;
                    bga_settings settings;
                    if(difficulty == DIFFICULTY.EASY){
                        settings = new bga_settings(1024, 0.3f, 1.5f, 30f, 1f, 25f, 0.8f, 5f, currentSeed);
                    } else if(difficulty == DIFFICULTY.HARD){
                        settings = new bga_settings(1024, 0.3f, 1.5f, 30f, 1f, 25f, 0.2f, 5f, currentSeed);
                    } else {
                        settings = new bga_settings(1024, 0.3f, 1.5f, 30f, 1f, 25f, 0.5f, 5f, currentSeed);
                    }
                    bga.StartBGA(ref inputAudioClip, settings, song, path);
                }
                if(state == STATE.BGA_STARTED){
                    //animation for loading
                    BGAMenu.SetActive(false);
                    generatingCanvas.SetActive(true);
                    okButton.SetActive(false);
                    if(loadingAnimationText.GetComponent<Text>().text.Length < 12){
                        loadingAnimationText.GetComponent<Text>().text += ".";
                    }else{
                        loadingAnimationText.GetComponent<Text>().text = "";
                    }
                }
                if(bga.state == BGA.STATE.DONE)
                {
                    //set bga's state to ready so that beatmaps can be generated more than once
                    bga.state = BGA.STATE.READY;
                    state = STATE.BGA_FINISHED;
                }
                if(state == STATE.BGA_FINISHED){
                    BGAMenu.SetActive(false);
                    loadingAnimationText.GetComponent<Text>().text = "";
                    generatingText.GetComponent<Text>().text = "BeatMap Generated";
                    okButton.SetActive(true);
                }
            }   
        }
    }
// switches between beatmap selection and main menu
    private void toBeatmapSelection(){
        updated = false;
        if(currentState == 2){
            //MapSelect.SetActive(false);
            currentState = 1;
        }else{
            //MapSelect.SetActive(true);
            currentState = 2;
        }     
    }

    private void toGarage(){
        print(currentState);
        updated = false;
        if(currentState == 3){
            currentState = 2;
            currentCarIndex = MainMenuController.player.currentCarID;
        }else{
            currentState = 3;
        }
    }
    
    private void toBGA(){
        updated = false;
        if(currentState == 1){
            currentState = 4;
        }else{
            currentState = 1;
        }     
    }
// quits the game
    private void exitGame(){
        Application.Quit();
    }

    private void loadBeatmap(){
        string beatMapDir = Application.persistentDataPath + "/BeatMaps/";
        Debug.Log(beatMapDir);
        //see https://docs.microsoft.com/en-us/dotnet/csharp/programming-guide/concepts/serialization/walkthrough-persisting-an-object-in-visual-studio
        //if (!fileName.EndsWith(".dat")) continue;
        Stream openFileStream = File.OpenRead(beatMapDir + "testBeatmap.dat");
        BinaryFormatter deserializer = new BinaryFormatter();
        //BeatMap beatMap = (BeatMap)deserializer.Deserialize(openFileStream);
        SceneManager.LoadScene("New Scene");
    }

    // edit username
    private void toEditUsername(){
        //TouchScreenKeyboard keyboard;
        print("test");
        string nameToEdit = "test";
        
        TouchScreenKeyboard.Open(nameToEdit, TouchScreenKeyboardType.Default);
        MainMenuController.player.name = nameToEdit;
        // saving user data
        MainMenuController.savePlayerToExternal(MainMenuController.player);
        MainMenuController.LoadPlayerFromExternal(ref MainMenuController.player);
        usernameText.GetComponent<Text>().text = nameToEdit;
    }

    private void handleNextCarButton(){ 
        print(MainMenuController.player.ownedCars.Contains(currentCarIndex));
        int prevCarIndex = currentCarIndex;
        if(currentCarIndex + 1 > cars.Length-1){
            currentCarIndex = 0;
        }else{
            currentCarIndex++;
        }
        print(prevCarIndex + " " + currentCarIndex);
        
        cars[prevCarIndex].carObject.SetActive(false);
        cars[currentCarIndex].carObject.SetActive(true);
                
    }

    private void handlePrevCarButton(){
        int prevCarIndex = currentCarIndex;
        if(currentCarIndex - 1 < 0){
            currentCarIndex = cars.Length-1;
        }else{
            currentCarIndex--;
        }
        cars[prevCarIndex].carObject.SetActive(false);
        cars[currentCarIndex].carObject.SetActive(true);
    }

    private void handlePurchaseButton(){
        int price = cars[currentCarIndex].price;
        if(MainMenuController.player.ownedCars.Contains(currentCarIndex)){
            MainMenuController.player.currentCarID = currentCarIndex;
            MainMenuController.savePlayerToExternal(MainMenuController.player);
            MainMenuController.LoadPlayerFromExternal(ref MainMenuController.player);
            currentState = 2;
            return;
        }
        if(price <= MainMenuController.player.money){
            // handle purchase here
            MainMenuController.player.money -= price;

            currentMoney.GetComponent<Text>().text = MainMenuController.player.money + "";
            MainMenuController.player.ownedCars.Add(currentCarIndex); // this should update the availability of the car
            
            MainMenuController.savePlayerToExternal(MainMenuController.player); // save player data in dat file
            MainMenuController.LoadPlayerFromExternal(ref MainMenuController.player); // reload player data
        }
    }

    private void handleCarVisibility(){
        int counter = 0;
        cars[2].carObject.SetActive(false);
        foreach(carBundle obj in cars){
            //print(obj.transform.position);
            if(counter == MainMenuController.player.currentCarID){
                //obj.transform.position = carPositions[]
                obj.carObject.SetActive(true);
            }else{
                obj.carObject.SetActive(false);
            }
            counter++;
        }
    }

    /*###########################
      ######## BGA STUFF ########
      ###########################*/

    #if UNITY_ANDROID

    void selectFileListener()
    {

        if (BGACommon.IS_PC) {
            path = Application.persistentDataPath + "/Songs/Dreams~Lost Sky~Dreams"; //for testing on windows: set the path var to a song
            song = new song_meta_struct("Dreams", "Lost Sky", "Dreams"); //for testing; set the meta data
            songNameText.text = song.title + " by " + song.artist;//for testing; set the text UI
        }
        else {
            Debug.Log("Opening select file on android...");
            //https://docs.unity3d.com/ScriptReference/AndroidJavaRunnable.html
            AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            AndroidJavaObject activity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
            string filePath = Application.persistentDataPath + "/Songs";
            Debug.Log(filePath);
            if (!Directory.Exists(filePath)) {
                Directory.CreateDirectory(filePath);
            }
            //path = "file://" + filePath;
            activity.Call("CallFromUnity", filePath);

        }

        //start loading song
        if (path != "")
        {
            state = STATE.AUDIO_CLIP_LOADING;
            StartCoroutine(getAudioClipFromPath(path + BGACommon.SONG_FORMAT));
        }
    }

    void resultFromJava(string s) 
    {
      Debug.Log("Got a result from java");
      Debug.Log(s);
      string[] strings = s.Split(new char[] {BGACommon.DELIMITER});
      song = new song_meta_struct(strings[0], strings[1], strings[2]);
      songNameText.text = song.title + " by " + song.artist;
      path = Application.persistentDataPath + "/Songs/" + s;
    }

    #else

    void selectFileListener()
    {

    }

    #endif

    IEnumerator getAudioClipFromPath(string path)
    {
        //see https://docs.unity3d.com/ScriptReference/Networking.UnityWebRequestMultimedia.GetAudioClip.html

        Debug.Log(BGACommon.AUDIO_TYPE);
        Debug.Log(BGACommon.SONG_FORMAT);

        using (UnityWebRequest www = UnityWebRequestMultimedia.GetAudioClip("file://" + path, BGACommon.AUDIO_TYPE))
        {
            yield return www.Send();

            if (www.isNetworkError)
            {
                Debug.Log("err");
                Debug.Log(www.error);
                state = STATE.AUDIO_CLIP_ERROR;
            }
            else
            {
                inputAudioClip = DownloadHandlerAudioClip.GetContent(www);
                Debug.Log("Loaded");
                state = STATE.AUDIO_CLIP_LOADED;
            }
        }
    }

    void onSelectFileSuccess(string path)
    {
        Debug.Log(path);
        //this.path = "file://" + path;
        this.path = path;
    }

    void generateBeatMapListener()
    {
        Debug.Log("generate");
        Debug.Log(path);
        if(state == STATE.AUDIO_CLIP_LOADED && inputAudioClip != null)
            {
                Debug.Log("generate");
                Debug.Log(path);
                state = STATE.START_BGA;
            }
    }

    void easyButtonListener(){
        difficulty = DIFFICULTY.EASY;
        easyButton.GetComponent<Image>().color = new Color(255f, 0f, 255f);
        normalButton.GetComponent<Image>().color = new Color(255f, 255f, 255f);
        hardButton.GetComponent<Image>().color = new Color(255f, 255f, 255f);
    }

    void normalButtonListener(){
        difficulty = DIFFICULTY.NORMAL;
        easyButton.GetComponent<Image>().color = new Color(255f, 255f, 255f);
        normalButton.GetComponent<Image>().color = new Color(255f, 0f, 255f);
        hardButton.GetComponent<Image>().color = new Color(255f, 255f, 255f);
    }

    void hardButtonListener(){
        difficulty = DIFFICULTY.HARD;
        easyButton.GetComponent<Image>().color = new Color(255f, 255f, 255f);
        normalButton.GetComponent<Image>().color = new Color(255f, 255f, 255f);
        hardButton.GetComponent<Image>().color = new Color(255f, 0f, 255f);
    }
    
    void okButtonListener()
    {
        mapsContent.GetComponent<InstantiateBeatMaps>().refreshBeatMaps();
        print("AKJDKJASKJDHKJSAHDKJAHSKJDHAKJS");
        //set state for bga to ready so more songs can be generated, set state of UI to map select
        state = STATE.READY;
        currentState = 1;
        //reset values for next time the user generates a beat map
        generatingText.GetComponent<Text>().text = "Generating BeatMap";
        okButton.SetActive(false);
        songNameText.GetComponent<Text>().text = "No song selected.";
        songDurationText.GetComponent<Text>().text = "00:00";
        inputAudioClip = null;
        path = "";
        
    }
}