using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public class BeatMapPlayer : MonoBehaviour
{
    //public static string beatLocations = "C:/Beatmaps";
    public static string fileName;

    public enum STATE {
        NOT_PLAYING,
        PLAYING
    }

    public STATE state;

    public AudioSource audioSource;

    public BeatMap beatMap;

    public Queue<LaneObject> objQueue;
  
    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("BeatMapPlayer gameObject loaded");
        beatMap = loadBeatMap(fileName);
        state = STATE.NOT_PLAYING;
    }

    BeatMap loadBeatMap(string fileName)
    {
        //see https://docs.microsoft.com/en-us/dotnet/csharp/programming-guide/concepts/serialization/walkthrough-persisting-an-object-in-visual-studio
        Stream openFileStream = File.OpenRead(fileName);
        BinaryFormatter deserializer = new BinaryFormatter();
        BeatMap beatMap = (BeatMap)deserializer.Deserialize(openFileStream);
        beatMap.loadSamples(this); //load the samples from the songFilePath
        Debug.Log("Beatmap loaded");
        Debug.Log(beatMap.name);
        return beatMap;
    }

    // Update is called once per frame
    void Update()
    {

        if (beatMap.state == BeatMap.STATE.SAMPLES_LOADED && this.state == STATE.NOT_PLAYING) {
          objQueue = beatMap.initLaneObjectQueue();
          audioSource = audioSource.GetComponent<AudioSource>();
          audioSource.clip = beatMap.getAudioClip();
          Debug.Log("Playing audioSource");
          audioSource.Play();
          this.state = STATE.PLAYING;
        }

        //Read from objQueue and spawn beats and obstacles :)
        //get current time from audioSource
    }
}
