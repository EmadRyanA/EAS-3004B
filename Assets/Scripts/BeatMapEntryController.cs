﻿using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Networking;

/* Control each beatmap entry in the beatmap list in main menu
 * Author: Evan Jesty (101078735)
 */

public class BeatMapEntryController : MonoBehaviour
{

    public string fileName;
    public BeatMap beatMap;

    public Button songPlay;
    public Button moreInfo;
    public Image coverArtImage;

    public InstantiateBeatMaps instantiateBeatMaps;
    // Start is called before the first frame update
    void Start()
    {
        songPlay = songPlay.GetComponent<Button>();
        moreInfo = moreInfo.GetComponent<Button>();
        coverArtImage = coverArtImage.GetComponent<Image>();
        songPlay.onClick.AddListener(songPlayListener);
        moreInfo.onClick.AddListener(moreInfoListener);
    }

    void songPlayListener()
    {
        //Indicate that we are going to play the beatMap to update lastPlayed, timesPlayed, ...
        beatMap.play(Application.persistentDataPath);
        BeatMap.futureFileName = fileName;
        SceneManager.LoadScene("New Scene");
    }

    void moreInfoListener() {
        instantiateBeatMaps.moreInfo(beatMap, this);
    }

    public void setCoverArt(string path) {
        try {
            byte[] bytes = System.IO.File.ReadAllBytes(path);
            Texture2D texture2D = new Texture2D(1, 1);
            texture2D.LoadImage(bytes);
            Sprite sprite = Sprite.Create(texture2D, new Rect(0, 0, texture2D.width, texture2D.height), new Vector2(0, 0));
            if (sprite != null) coverArtImage.sprite = sprite;
        }
        catch (IOException e) {
            Debug.Log("err");
            Debug.Log(e);
        }
        //StartCoroutine(downloadCoverArt(path));
    }

    /*
    IEnumerator downloadCoverArt(string path)
    {
      Debug.Log("Getting cover art");
      Debug.Log(path);
      using (UnityWebRequest www = UnityWebRequestTexture.GetTexture(path))
      {
            yield return www.SendWebRequest();

            if (www.isNetworkError)
            {
                Debug.Log("err");
                Debug.Log(www.error);
            }
            else
            {
              Debug.Log("setting sprite..");
              Texture2D texture2D = DownloadHandlerTexture.GetContent(www);
              Sprite sprite = Sprite.Create(texture2D, new Rect(0, 0, texture2D.width, texture2D.height), new Vector2(0, 0));
              if (sprite != null) coverArtImage.sprite = sprite;
            }
      }
    }
    */

    // Update is called once per frame
    void Update()
    {
        
    }

    public void destroy()
    {
        Destroy(gameObject);
    }
}
