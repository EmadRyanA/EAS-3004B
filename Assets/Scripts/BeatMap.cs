using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using UnityEngine.Networking;

/*  Stores the generated beatmap
 * 
 */
 [Serializable()]
public class BeatMap
{

    public static string futureFileName;

    public enum STATE {
        SAMPLES_UNLOADED,
        SAMPLES_LOADED
    }

    public STATE state;
    private List<LaneObject> laneObjectStore; //Store as list since queue cannot be seralized
    private bga_settings bga_settings; //Settings used to create this Beatmap
    private song_info_struct song_info;
    public song_meta_struct song_meta {get;}
    public string fileName {get; set;} //not path; just the name in /BeatMaps/
    public string songFilePath {get; set;} //Where we store the .mp3 file
    public DateTime timeGenerated {get; set;}
    public DateTime lastPlayed {get; set;} //lastPlayed == last time we called loadSamples()
    public int timesPlayed {get; set;}

    public static BeatMap loadBeatMap() 
    {
        return loadBeatMap(futureFileName);
    }

    public bga_settings get_settings()
    {
        return this.bga_settings;
    }

    public song_info_struct get_song_info()
    {
        return this.song_info;
    }

    public static BeatMap loadBeatMap(string fn) //Todo rename this class to a static "BeatMapLoader" class
    {
        //see https://docs.microsoft.com/en-us/dotnet/csharp/programming-guide/concepts/serialization/walkthrough-persisting-an-object-in-visual-studio
        Stream openFileStream = File.OpenRead(fn);
        BinaryFormatter deserializer = new BinaryFormatter();
        BeatMap beatMap = (BeatMap)deserializer.Deserialize(openFileStream);
        Debug.Log("Beatmap loaded");
        Debug.Log(beatMap.song_meta.title);
        return beatMap;
    }

    public BeatMap (bga_settings bga_settings, song_info_struct song_info, song_meta_struct song_meta, string songFilePath)
    {
        this.song_meta = song_meta;
        this.bga_settings = bga_settings;
        this.song_info = song_info;
        this.state = STATE.SAMPLES_LOADED;
        //this.name = name;
        this.fileName = song_meta.title + "~" + song_meta.artist + "~" + song_meta.album + ";" + bga_settings.rng_seed.ToString() + ".dat";
        this.songFilePath = songFilePath;
        this.timesPlayed = 0;
        laneObjectStore = new List<LaneObject>();
    }

    public void addLaneObject(LaneObject laneObject)
    {
        //todo check if valid... for now this is left to bga to make sure
        laneObjectStore.Add(laneObject);
    }

    private IEnumerator getAudioClipFromPath(string path)
    {
        //see https://docs.unity3d.com/ScriptReference/Networking.UnityWebRequestMultimedia.GetAudioClip.html

        // AudioType audioType;
        // #if UNITY_ANDROID
        //   audioType = AudioType.MPEG; //for android use MPEG (.mp3)
        // #else
        //   audioType = AudioType.OGGVORBIS; //for testing on windows use OGGVORBIS (.ogg) since windows does not have mpeg codec native
        // #endif

        //Debug.Log(audioType);

        using (UnityWebRequest www = UnityWebRequestMultimedia.GetAudioClip("file://" + path, BGACommon.AUDIO_TYPE))
        {
            yield return www.Send();

            if (www.isNetworkError)
            {
                Debug.Log("err");
                Debug.Log(www.error);
                //state = STATE.AUDIO_CLIP_ERROR;
            }
            else
            {
                AudioClip inputAudioClip = DownloadHandlerAudioClip.GetContent(www);

                //Replace song info with new song info that is created from inputAudioClip, and set samples (normally we do not keep samples when saving (because they are 70+mb))
                song_info_struct song_i = new song_info_struct(inputAudioClip);
                song_i.samples = new float[song_i.sampleCount * song_i.channels];
                song_i.sampleLength = song_info.length / (float)song_i.sampleCount;
                inputAudioClip.GetData(song_i.samples, 0);
                this.song_info = song_i;
                Debug.Log("Loaded");
                state = STATE.SAMPLES_LOADED;
                //state = STATE.AUDIO_CLIP_LOADED;
                //callBGA(ref audioClip);
                //bga.StartBGA(ref audioClip);
            }
        }
    }

    public void loadSamples(MonoBehaviour callingMonoBehaviour) {
        state = STATE.SAMPLES_UNLOADED;
        callingMonoBehaviour.StartCoroutine(getAudioClipFromPath(songFilePath + BGACommon.SONG_FORMAT));
    }

    public void unloadSamples() {
        this.song_info.samples = null; //gc will unload samples later. we mostly want to unload when saving the beatmap so that we don't waste 70+mb of space
        state = STATE.SAMPLES_UNLOADED;
    }

    public Queue<LaneObject> initLaneObjectQueue ()
    {
        //todo this is in place
        laneObjectStore.Sort((x, y) => {
            return x.sampleIndex - y.sampleIndex;
        });
        return new Queue<LaneObject>(laneObjectStore);
    }

    public float[] getSamples() {
        return song_info.samples;
    }

    public AudioClip getAudioClip() {
        Debug.Log(song_info.channels);
        Debug.Log(song_info.samples.Length);
        Debug.Log(song_info.frequency);
        AudioClip audioClip = AudioClip.Create(song_meta.title, song_info.samples.Length, song_info.channels, song_info.frequency, false);
        audioClip.SetData(song_info.samples, 0);
        return audioClip;
    }

    //indicates that the beatmap is to be played; so we need to save an updated play count / last played
    public void play(string persistentDataPath) {
        lastPlayed = DateTime.Now;
        this.timesPlayed += 1;
        save(persistentDataPath);
    }

    public void save (string persistentDataPath) {
        unloadSamples();
        lastPlayed = DateTime.Now; //todo if never played
        timeGenerated = DateTime.Now;
        //See https://docs.microsoft.com/en-us/dotnet/csharp/programming-guide/concepts/serialization/walkthrough-persisting-an-object-in-visual-studio
        string fileDir = persistentDataPath + "/BeatMaps";
        if (!Directory.Exists(fileDir)) Directory.CreateDirectory(fileDir);
        string fn = fileDir + "/" + this.fileName;
        Stream saveFileStream = File.Create(fn);
        BinaryFormatter serializer = new BinaryFormatter();
        serializer.Serialize(saveFileStream, this);
        saveFileStream.Close();
    }
}
/*
public class LaneObjectComparer: IComparer<LaneObject>
{
    public int Compare(LaneObject x, LaneObject y) {
        return x.sampleIndex - y.sampleIndex;
    }
}
*/
