using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*  Stores the generated beatmap
 * 
 */
 [Serializable()]
public class BeatMap
{

    private List<LaneObject> laneObjectStore; //Store as list since queue cannot be seralized
    private bga_settings bga_settings; //Settings used to create this Beatmap
    private song_info_struct song_info;
    public string name { get; set; }

    public BeatMap (bga_settings bga_settings, song_info_struct song_info, string name)
    {
        this.bga_settings = bga_settings;
        this.song_info = song_info;
        this.name = name;
        laneObjectStore = new List<LaneObject>();
    }

    public void addLaneObject(LaneObject laneObject)
    {
        //todo check if valid... for now this is left to bga to make sure
        laneObjectStore.Add(laneObject);
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
        AudioClip audioClip = AudioClip.Create(name, song_info.samples.Length, song_info.channels, song_info.frequency, false);
        audioClip.SetData(song_info.samples, 0);
        return audioClip;
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
