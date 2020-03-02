using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class BeatMapEntryController : MonoBehaviour
{

    public string fileName;

    public Button songPlay;
    // Start is called before the first frame update
    void Start()
    {
        songPlay = songPlay.GetComponent<Button>();
        songPlay.onClick.AddListener(songPlayListener);
    }

    void songPlayListener()
    {
        BeatMapPlayer.fileName = fileName;
        SceneManager.LoadScene(sceneBuildIndex:3);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
