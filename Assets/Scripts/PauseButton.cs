using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PauseButton : MonoBehaviour
{
    // Start is called before the first frame update
    public static bool paused;
    public GameObject menu;
    public static Button pauseBtn;
    //public Button btnSettings;
    //public GameObject pausePanel;
    //public GameObject settingsCanvas;
    //public Button exitSettingsButton;
    void Start()
    {
        pauseBtn = GetComponent<Button>();
        pauseBtn.onClick.AddListener(TogglePause);

        //btnSettings.onClick.AddListener(handleSettings);
        //exitSettingsButton.onClick.AddListener(handleExitSettings);
    }

    private void TogglePause(){
        Time.timeScale = Mathf.Approximately(Time.timeScale, 0.0f) ? 1.0f : 0.0f;
        paused = !paused;
        gameController.audioSrc.Pause();
        pauseBtn.enabled = !pauseBtn.enabled;
    }

    /*private void handleSettings(){
        print("test");
        pausePanel.SetActive(false);
        settingsCanvas.SetActive(true);
    }*/

    /*private void handleExitSettings(){
        pausePanel.SetActive(true);
        settingsCanvas.SetActive(false);
    }*/

    // Update is called once per frame
    void Update()
    {
        if(paused){
            menu.SetActive(true);
        }else{
            menu.SetActive(false);
        }
    }
}
