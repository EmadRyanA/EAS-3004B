using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using UnityEngine.UI;

public class InstantiateBeatMaps : MonoBehaviour
{

    public GameObject prefab;
    public Transform mapsContent;
    // Start is called before the first frame update
    void Start()
    {
        mapsContent = mapsContent.GetComponent<Transform>();
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
        beatMaps.Sort((x, y) => {
          return y.lastPlayed > x.lastPlayed ? 1 : 0;
        });

        foreach(BeatMap beatMap in beatMaps) {
          GameObject beatMapPanel = (GameObject)Instantiate(prefab, new Vector3(0, 0, 0), new Quaternion(0, 0, 0, 0), mapsContent);
          BeatMapEntryController controller = beatMapPanel.GetComponentInChildren<BeatMapEntryController>();
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

    // Update is called once per frame
    void Update()
    {
        
    }
}
