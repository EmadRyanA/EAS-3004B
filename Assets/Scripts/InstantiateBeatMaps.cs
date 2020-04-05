using System.Collections;
using System.Collections.Generic;
using System.IO;
using System;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using UnityEngine.UI;

/* Instantiate the beatmapentrys in the beatmap list
 * Authors: Evan Jesty (101078735), Robert Occleston-Pratt (101044703)
 */

public class InstantiateBeatMaps : MonoBehaviour
{

    public GameObject prefab;
    public Transform mapsContent;
    public Text leaderBoard;

    public Button removeBeatMapButton;
    public Text selectedBeatMapText;
    public BeatMap selectedBeatMap;
    public BeatMapEntryController selectedBeatMapEntryController;
    public List<BeatMapEntryController> controllers = new List<BeatMapEntryController>();
    public List<GameObject> panes = new List<GameObject>();
    // Start is called before the first frame update
    public void Start()
    {
        mapsContent = mapsContent.GetComponent<Transform>();
        leaderBoard = leaderBoard.GetComponent<Text>();
        selectedBeatMapText = selectedBeatMapText.GetComponent<Text>();
        removeBeatMapButton = removeBeatMapButton.GetComponent<Button>();
        removeBeatMapButton.onClick.AddListener(removeBeatMapListener);
        string beatMapDir = Application.persistentDataPath + "/BeatMaps/";
        Debug.Log(beatMapDir);
        Directory.CreateDirectory(beatMapDir); //create if it does not exist

        List<BeatMap> beatMaps = new List<BeatMap>();

        foreach (string fileName in Directory.EnumerateFiles(beatMapDir)) {
          //see https://docs.microsoft.com/en-us/dotnet/csharp/programming-guide/concepts/serialization/walkthrough-persisting-an-object-in-visual-studio
          if (!fileName.EndsWith(".dat")) continue;
          BeatMap beatMap = BeatMap.loadBeatMap(fileName);
          beatMaps.Add(beatMap);
        }

        //sort beatmaps
        beatMaps.Sort((x, y) => DateTime.Compare(y.lastPlayed, x.lastPlayed));

        foreach(BeatMap beatMap in beatMaps) {
          GameObject beatMapPanel = (GameObject)Instantiate(prefab, new Vector3(0, 0, 0), new Quaternion(0, 0, 0, 0), mapsContent);
          BeatMapEntryController controller = beatMapPanel.GetComponentInChildren<BeatMapEntryController>();
          controllers.Add(controller);
          panes.Add(beatMapPanel);
          controller.instantiateBeatMaps = this;
          controller.fileName = beatMapDir + beatMap.fileName;
          controller.setCoverArt(beatMap.songFilePath + ".png");
          controller.beatMap = beatMap;
          foreach(Text text in beatMapPanel.GetComponentsInChildren<Text>()) {
            if (text.name == "SongName") text.text = beatMap.song_meta.title + " by " + beatMap.song_meta.artist;
            else if (text.name == "SongInfo")  {
              text.text = "Times Played: " + beatMap.timesPlayed + "  Last Played: " + beatMap.lastPlayed.ToShortDateString() + " " + beatMap.lastPlayed.ToShortTimeString()
                          + "\nRNG Seed: " + beatMap.get_settings().rng_seed.ToString() + " Duration: " + (Mathf.FloorToInt(beatMap.get_song_info().length) / 60f).ToString("0.0") + " minutes";
            }
          }
        }
        
    }

    //for now, more info == scoreboard
    public void moreInfo(BeatMap beatMap, BeatMapEntryController controller)
    {
      selectedBeatMap = beatMap;
      selectedBeatMapEntryController = controller;
      selectedBeatMapText.text = "Selected: " + beatMap.song_meta.title + " by " + beatMap.song_meta.artist + " (" + beatMap.get_settings().rng_seed.ToString() + ")";
      leaderBoard.text = "";
      List<WinDataClass> wins = beatMap.getScoreBoard();
      foreach (WinDataClass win in wins)
      {
        leaderBoard.text += " " + win.score.ToString() + " on: " + win.date.ToShortDateString() + "\n";
      }
    }

    void removeBeatMapListener()
    {
      if (selectedBeatMap != null && selectedBeatMapEntryController != null) {
        selectedBeatMap.delete_self(Application.persistentDataPath);
        selectedBeatMapEntryController.destroy();
      }
      selectedBeatMap = null;
      selectedBeatMapEntryController = null;
    }

    public void refreshBeatMaps()
    {
      foreach(BeatMapEntryController c in controllers){
        if(c != null){
          c.destroy();
        }
      }
      Start();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
