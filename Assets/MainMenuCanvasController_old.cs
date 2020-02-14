using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuCanvasController_old : MonoBehaviour
{
    // Start is called before the first frame update

    Button playButton;
    Camera mainCamera;
    GameObject MapSelect;
    int currentState = 0;
    void Start()
    {
        playButton = GameObject.Find("PlayButton").GetComponent<Button>();
        mainCamera = GameObject.Find("Main Camera").GetComponent<Camera>();
        MapSelect = GameObject.Find("MapSelectCanvas");

        playButton.onClick.AddListener(toBeatmapSelection);

        MapSelect.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        Debug.Log(currentState);
        // beatmap select pov
        if(currentState == 1){
            MapSelect.SetActive(true);
            mainCamera.transform.position = Vector3.Lerp(mainCamera.transform.position, MainMenuController.cameraLocations[1], Time.deltaTime);
            mainCamera.transform.rotation = Quaternion.RotateTowards(mainCamera.transform.rotation, MainMenuController.cameraRotations[1], 60*Time.deltaTime);  
        // main pov
        }else if(currentState == 2){
            MapSelect.SetActive(false);
            mainCamera.transform.position = Vector3.Lerp(mainCamera.transform.position, MainMenuController.cameraLocations[0], Time.deltaTime);
            mainCamera.transform.rotation = Quaternion.RotateTowards(mainCamera.transform.rotation, MainMenuController.cameraRotations[0], 60*Time.deltaTime);
        }
    }

    private void toBeatmapSelection(){
        
        if(currentState == 2){
            //MapSelect.SetActive(false);
            currentState = 1;
        }else{
            //MapSelect.SetActive(true);
            currentState++;
        }     
    }
}
