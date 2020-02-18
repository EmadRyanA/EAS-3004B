using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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
    Button exitButton;
    Camera mainCamera;
    GameObject MapSelect;
    GameObject Garage;
    
    int currentState = 2;
    void Start()
    {
<<<<<<< HEAD

        //print(Application.persistentDataPath);
        //player = 
=======
        //print(Application.persistentDataPath);
        //player = 
        
>>>>>>> 1340a0937943f54a00200a9e0e4f49a9acd5aec5
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
        
        // user profile canvas initializers
        userProfileCanvas = GameObject.Find("UserProfileCanvas");
<<<<<<< HEAD
        usernameText = userProfileCanvas.transform.Find("Username").gameObject;
        levelText = userProfileCanvas.transform.Find("LevelText").gameObject;
        experienceText = userProfileCanvas.transform.Find("ExperienceText").gameObject;
        experienceBar = userProfileCanvas.transform.Find("ExperienceBar").gameObject;
=======
        usernameText = GameObject.Find("Username");
        levelText = GameObject.Find("LevelText");
        experienceText = GameObject.Find("ExperienceText");
        experienceBar = GameObject.Find("ExperienceBar");
>>>>>>> 1340a0937943f54a00200a9e0e4f49a9acd5aec5
        
        // listeners
        playButton.GetComponent<Button>().onClick.AddListener(toBeatmapSelection);
        exitButton.onClick.AddListener(toBeatmapSelection);
        exitGameButton.GetComponent<Button>().onClick.AddListener(exitGame);
        garageButton.GetComponent<Button>().onClick.AddListener(toGarage);
        returnToMainMenuGarageButton.GetComponent<Button>().onClick.AddListener(toGarage);
        
        MapSelect.SetActive(false);

        // the user's profile should be visible in all states, so set visible here
<<<<<<< HEAD
        //userProfileCanvas.SetActive(true);

        //initiate the user's data
        usernameText.GetComponent<Text>().text = MainMenuController.player.name;
        levelText.GetComponent<Text>().text = "Level: " + MainMenuController.player.level;
        experienceText.GetComponent<Text>().text = "XP: " + MainMenuController.player.currentExperience + " / " + (MainMenuController.player.currentExperience + MainMenuController.player.experienceForNextLevel);
        experienceBar.GetComponent<Slider>().value = MainMenuController.player.currentExperience/(MainMenuController.player.currentExperience + MainMenuController.player.experienceForNextLevel);
=======
        userProfileCanvas.SetActive(true);
>>>>>>> 1340a0937943f54a00200a9e0e4f49a9acd5aec5

        //comparisonVector3 = new Vector3(0.01f, 0.01f, 0.01f);
    }

    // Update is called once per frame
    void Update()
    {
        Debug.Log(currentState);
        // beatmap select pov
        if(!updated){
            if(currentState == 1){
<<<<<<< HEAD
                userProfileCanvas.SetActive(false);
                
=======
>>>>>>> 1340a0937943f54a00200a9e0e4f49a9acd5aec5
                MapSelect.SetActive(true);
                
                playButton.SetActive(false);
                exitGameButton.SetActive(false);
                garageButton.SetActive(false);

                coinImage.SetActive(false);

                title.SetActive(false);

                Debug.Log(MainMenuController.cameraLocations + " " + MainMenuController.cameraRotations);
                mainCamera.transform.position = Vector3.Lerp(mainCamera.transform.position, MainMenuController.cameraLocations[1], Time.deltaTime);
                mainCamera.transform.rotation = Quaternion.RotateTowards(mainCamera.transform.rotation, MainMenuController.cameraRotations[1], 70*Time.deltaTime);  

                if(mainCamera.transform.position == MainMenuController.cameraLocations[1] && mainCamera.transform.rotation == MainMenuController.cameraRotations[1]){
                    // only stops updating once the camera completes its transition
                    updated = true;
                }
            
            // main menu pov
            }else if(currentState == 2){
                // disabling other canvases
<<<<<<< HEAD
                userProfileCanvas.SetActive(true);
                
=======
>>>>>>> 1340a0937943f54a00200a9e0e4f49a9acd5aec5
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

            // garage pov
            }else if(currentState == 3){
<<<<<<< HEAD
                // enabling the car selection canvas 
                Garage.SetActive(true);

                userProfileCanvas.SetActive(true);

=======
                // enabling the car selection canvas
                Garage.SetActive(true);

>>>>>>> 1340a0937943f54a00200a9e0e4f49a9acd5aec5
                //disabling buttons
                playButton.SetActive(false);
                exitGameButton.SetActive(false);
                garageButton.SetActive(false);

                coinImage.SetActive(true);
                currentMoney.GetComponent<Text>().text = MainMenuController.player.money + "";

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
        updated = false;
        if(currentState == 3){
            currentState = 2;
        }else{
            currentState = 3;
        }
    }
// quits the game
    private void exitGame(){
        Application.Quit();
    }

}
