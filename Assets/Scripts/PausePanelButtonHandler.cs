using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PausePanelButtonHandler : MonoBehaviour
{
    // Start is called before the first frame update
    private Button btnPause;
    private Button btnRetry;
    private Button btnQuit;
    private Button btnSettings;
    void Start()
    {
        btnPause = this.transform.Find("PauseButton").GetComponent<Button>();
        btnRetry = this.transform.Find("RetryButton").GetComponent<Button>();
        btnQuit = this.transform.Find("QuitButton").GetComponent<Button>();
        btnSettings = this.transform.Find("SettingsButton").GetComponent<Button>();

        btnPause.onClick.AddListener(handlePause);
        btnQuit.onClick.AddListener(handleQuit);

    }

    // pauses and unpauses the game.
    private void handlePause(){
        Time.timeScale = Mathf.Approximately(Time.timeScale, 0.0f) ? 1.0f : 0.0f;
        PauseButton.paused = !PauseButton.paused;
        PauseButton.pauseBtn.enabled = !PauseButton.pauseBtn.enabled;
        gameController.audioSrc.Play();
    }

    // resets the stage, restarts the game.
    private void handleRetry(){

    }

    // quits the current game, brings the game back to menu screen
    private void handleQuit(){
        Time.timeScale = Mathf.Approximately(Time.timeScale, 0.0f) ? 1.0f : 0.0f;
        PauseButton.paused = false;
        PauseButton.pauseBtn.enabled = true;
        SceneManager.LoadScene("Main Menu");
    }

    // brings up another panel containing editable settings
    private void handleSettings(){

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
