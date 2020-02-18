using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuCanvasController : MonoBehaviour
{
    // Start is called before the first frame update

    //Button playButton;
    GameObject playButton;
    GameObject exitGameButton;
    GameObject garageButton;
    GameObject nextCarButton;
    GameObject prevCarButton;
    GameObject returnToMainMenuGarageButton;
    GameObject title;
    Button exitButton;
    Camera mainCamera;
    GameObject MapSelect;
    GameObject Garage;
    int currentState = 2;
    void Start()
    {
        //playButton = GameObject.Find("PlayButton").GetComponent<Button>();
        playButton = GameObject.Find("PlayButton");
        exitGameButton = GameObject.Find("ExitGameButton");
        garageButton = GameObject.Find("GarageButton");
        returnToMainMenuGarageButton = GameObject.Find("ReturnToMainMenuGarageButton");
        nextCarButton = GameObject.Find("NextCarButton");
        prevCarButton = GameObject.Find("PrevCarButton");

        mainCamera = GameObject.Find("Main Camera").GetComponent<Camera>();
        exitButton = GameObject.Find("ExitButton").GetComponent<Button>();
        MapSelect = GameObject.Find("MapSelectCanvas");
        Garage = GameObject.Find("CarSelectCanvas");
        title = GameObject.Find("Title");
        // listeners
        playButton.GetComponent<Button>().onClick.AddListener(toBeatmapSelection);
        exitButton.onClick.AddListener(toBeatmapSelection);
        exitGameButton.GetComponent<Button>().onClick.AddListener(exitGame);
        garageButton.GetComponent<Button>().onClick.AddListener(toGarage);
        returnToMainMenuGarageButton.GetComponent<Button>().onClick.AddListener(toGarage);
        MapSelect.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        Debug.Log(currentState);
        // beatmap select pov
        if(currentState == 1){
            MapSelect.SetActive(true);
            
            playButton.SetActive(false);
            exitGameButton.SetActive(false);
            garageButton.SetActive(false);

            title.SetActive(false);

            mainCamera.transform.position = Vector3.Lerp(mainCamera.transform.position, MainMenuController.cameraLocations[1], Time.deltaTime);
            mainCamera.transform.rotation = Quaternion.RotateTowards(mainCamera.transform.rotation, MainMenuController.cameraRotations[1], 70*Time.deltaTime);  
        // main menu pov
        }else if(currentState == 2){
            // disabling other canvases
            MapSelect.SetActive(false);
            Garage.SetActive(false);

            playButton.SetActive(true);
            exitGameButton.SetActive(true);
            garageButton.SetActive(true);

            title.SetActive(true);

            mainCamera.transform.position = Vector3.Lerp(mainCamera.transform.position, MainMenuController.cameraLocations[0], Time.deltaTime);
            mainCamera.transform.rotation = Quaternion.RotateTowards(mainCamera.transform.rotation, MainMenuController.cameraRotations[0], 60*Time.deltaTime);
        }else if(currentState == 3){
            // enabling the car selection canvas
            Garage.SetActive(true);

            //disabling buttons
            playButton.SetActive(false);
            exitGameButton.SetActive(false);
            garageButton.SetActive(false);

            title.SetActive(false);

            mainCamera.transform.position = Vector3.Lerp(mainCamera.transform.position, MainMenuController.cameraLocations[2], Time.deltaTime);
            mainCamera.transform.rotation = Quaternion.RotateTowards(mainCamera.transform.rotation, MainMenuController.cameraRotations[2], 30*Time.deltaTime);

        }
    }
// switches between beatmap selection and main menu
    private void toBeatmapSelection(){
        
        if(currentState == 2){
            //MapSelect.SetActive(false);
            currentState = 1;
        }else{
            //MapSelect.SetActive(true);
            currentState = 2;
        }     
    }

    private void toGarage(){
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
