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
    void Start()
    {
        pauseBtn = GetComponent<Button>();
        pauseBtn.onClick.AddListener(TogglePause);
    }

    private void TogglePause(){
        Time.timeScale = Mathf.Approximately(Time.timeScale, 0.0f) ? 1.0f : 0.0f;
        paused = !paused;
        pauseBtn.enabled = !pauseBtn.enabled;
    }


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
