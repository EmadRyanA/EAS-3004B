using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine.SceneManagement;
using System.Linq;

public class MainMenuCanvasController : MonoBehaviour
{
    // Start is called before the first frame update

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
    public Button generateBeatMapButton;
    Camera mainCamera;
    GameObject MapSelect;
    GameObject Garage;
    //GameObject mapsContent;
    GameObject loadBeatMapButton;
    //public static string beatmapDir; 
    GameObject purchaseButton;
    GameObject carPrice;
    private int currentCarIndex;
    // car switching 
    private GameObject playerCar;

    public struct carBundle{
        
        public carBundle(GameObject c, int p) : this(){
            carObject = c;
            price = p;
        }
        public GameObject carObject{get;}
        public int price{get;}
        
    }
    public carBundle[] cars;
    
    
    
    int currentState = 2;
    void Start()
    {
        currentCarIndex = MainMenuController.player.currentCarID;
        //print(Application.persistentDataPath);
        //player = 
        //playButton = GameObject.Find("PlayButton").GetComponent<Button>();
        // initializing gameobjects
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
        generateBeatMapButton = generateBeatMapButton.GetComponent<Button>();
        //mapsContent = mapsContent.GetComponent<GameObject>();
        
        // user profile canvas initializers
        userProfileCanvas = GameObject.Find("UserProfileCanvas");
        usernameText = userProfileCanvas.transform.Find("Username").gameObject;
        levelText = userProfileCanvas.transform.Find("LevelText").gameObject;
        experienceText = userProfileCanvas.transform.Find("ExperienceText").gameObject;
        experienceBar = userProfileCanvas.transform.Find("ExperienceBar").gameObject;
        editNameButton = GameObject.Find("EditNameButton").gameObject;

        purchaseButton = GameObject.Find("PurchaseButton").gameObject;
        carPrice = GameObject.Find("CarPrice").gameObject;
        
        // listeners
        playButton.GetComponent<Button>().onClick.AddListener(toBeatmapSelection);
        exitButton.onClick.AddListener(toBeatmapSelection);
        exitGameButton.GetComponent<Button>().onClick.AddListener(exitGame);
        garageButton.GetComponent<Button>().onClick.AddListener(toGarage);
        returnToMainMenuGarageButton.GetComponent<Button>().onClick.AddListener(toGarage);
        generateBeatMapButton.onClick.AddListener(() => {
          SceneManager.LoadScene(sceneBuildIndex:2);
        });
        editNameButton.GetComponent<Button>().onClick.AddListener(toEditUsername);
        nextCarButton.GetComponent<Button>().onClick.AddListener(handleNextCarButton);
        prevCarButton.GetComponent<Button>().onClick.AddListener(handlePrevCarButton);
        purchaseButton.GetComponent<Button>().onClick.AddListener(handlePurchaseButton);
        
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
    }

    // Update is called once per frame
    void Update()
    {
        //Debug.Log(currentState);
        // beatmap select pov
        if(!updated){
            if(currentState == 1){
                userProfileCanvas.SetActive(false);
                
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
                
                MapSelect.SetActive(false);
                Garage.SetActive(false);

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
        //print(beatMap.initLaneObjectQueue().Peek());
        //GameObject beatMapPanel = (GameObject)Instantiate(prefab, new Vector3(0, 0, 0), new Quaternion(0, 0, 0, 0), mapsContent);
        //BeatMapEntryController controller = beatMapPanel.GetComponentInChildren<BeatMapEntryController>();
        //controller.fileName = fileName;
        //beatMapPanel.GetComponentInChildren<Text>().text = beatMap.name;
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
        
        //Vector3 oldPos = MainMenuController.cars[currentCarIndex].transform.position;
        //MainMenuController.cars[currentCarIndex].transform.position = playerCar.transform.position;
        //MainMenuController.cars[currentCarIndex].transform.rotation = playerCar.transform.rotation;
        //playerCar.transform.position = oldPos; 
        //MainMenuController.cars[prevCarIndex].transform.position = oldPos;
        // need to set the player car here

        
    }

    private void handlePrevCarButton(){
        int prevCarIndex = currentCarIndex;
        if(currentCarIndex - 1 < 0){
            currentCarIndex = cars.Length-1;
        }else{
            currentCarIndex--;
        }
        //Vector3 oldPos = MainMenuController.cars[currentCarIndex].transform.position;
        //MainMenuController.cars[currentCarIndex].transform.position = playerCar.transform.position;
        //MainMenuController.cars[currentCarIndex].transform.rotation = playerCar.transform.rotation;
        //playerCar.transform.position = oldPos;
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

}
