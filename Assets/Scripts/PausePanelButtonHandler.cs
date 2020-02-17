using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
<<<<<<< HEAD
using UnityEngine.SceneManagement;
=======
>>>>>>> 431160aa739fa61569cf147d0576d59f0d0da843

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
<<<<<<< HEAD
        btnQuit.onClick.AddListener(handleQuit);
=======
>>>>>>> 431160aa739fa61569cf147d0576d59f0d0da843

    }

    // pauses and unpauses the game.
    private void handlePause(){
        Time.timeScale = Mathf.Approximately(Time.timeScale, 0.0f) ? 1.0f : 0.0f;
        PauseButton.paused = !PauseButton.paused;
        PauseButton.pauseBtn.enabled = !PauseButton.pauseBtn.enabled;
    }

    // resets the stage, restarts the game.
    private void handleRetry(){

    }

    // quits the current game, brings the game back to menu screen
    private void handleQuit(){
<<<<<<< HEAD
        Time.timeScale = Mathf.Approximately(Time.timeScale, 0.0f) ? 1.0f : 0.0f;
        SceneManager.LoadScene(sceneBuildIndex:1);
=======

>>>>>>> 431160aa739fa61569cf147d0576d59f0d0da843
    }

    // brings up another panel containing editable settings
    private void handleSettings(){

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
