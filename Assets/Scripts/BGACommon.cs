using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public enum LANE_OBJECT_TYPE
{
    Obstacle,
    Beat,
    START_DRIFT_TRIGGER,
    START_NORMAL_TRIGGER,
    START_FLY_TRIGGER
}

[Serializable()]
public struct LaneObject
{
    public int sampleIndex; //Index of this obj in peaks[]
    public float time; //Timestamp approximation of sampleIndex
    public int lane; //0 - (lanes - 1)
    public LANE_OBJECT_TYPE type;
    public LaneObject(int sampleIndex, float time, int lane, LANE_OBJECT_TYPE type)
    {
        this.sampleIndex = sampleIndex;
        this.time = time;
        this.lane = lane;
        this.type = type;
    }
}

[Serializable()]
public struct bga_settings
{
    public int n_bins; //the number of bins to use for FFT - must be power of 2
    public float threshold_time; //time in seconds for how long the threshold should be
    public float threshold_multiplier; //the multiplier used for the threshold
    public float drift_threshold_time;
    public float drifts_per_minute; //# of drift sections
    public float min_drift_seperation_time; //The minimum time between drift sections
    public float min_peak_seperation_time; //The minimum time between peaks in seconds
    public float warm_up_time; //time in seconds of how long the period where no Beat LaneObjects are generated
    public int rng_seed; //If rng_seed == 0 a new random seed is created

    public bga_settings(int n_bins, float threshold_time, float threshold_multiplier, float drift_threshold_time,
     float drifts_per_minute, float min_drift_seperation_time, float min_peak_seperation_time, float warm_up_time, int rng_seed)
    {
        this.n_bins = n_bins;
        this.threshold_time = threshold_time;
        this.threshold_multiplier = threshold_multiplier;
        this.drift_threshold_time = drift_threshold_time;
        this.drifts_per_minute = drifts_per_minute;
        this.min_drift_seperation_time = min_drift_seperation_time;
        this.min_peak_seperation_time = min_peak_seperation_time;
        this.warm_up_time = warm_up_time;
        this.rng_seed = rng_seed;
    }
}

[Serializable()]
public struct song_meta_struct
{
    public String title;
    public String artist;
    public String album;

    public song_meta_struct (String title, String artist, String album) {
        this.title = title;
        this.artist = artist;
        this.album = album;
    }

}

[Serializable()]
public struct song_info_struct
    {
        //public AudioClip audioClip;
        public float length;
        public int frequency;
        public int sampleCount;
        public float sampleLength; //Length of each sample in seconds
        public int channels;
        public float[] samples;

        public song_info_struct(AudioClip audioClip)
        {
            //this.audioClip = audioClip;
            sampleCount = audioClip.samples;
            channels = audioClip.channels;
            length = audioClip.length;
            frequency = audioClip.frequency;
            sampleLength = 0;
            samples = null;
        }
    }

public static class BGACommon
{
    public const int NUMBER_LANES = 3;
    public const Char DELIMITER = '~';
    public const bool IS_PC = true; // debug
    public const string SONG_FORMAT = (IS_PC ? ".ogg" : ".mp3");
    public const AudioType AUDIO_TYPE = (IS_PC ? AudioType.OGGVORBIS : AudioType.MPEG);
}

/*
    public class LaneObject
    {
        int sampleIndex; //Index of this obj in peaks[]
        float time; //Timestamp approximation of sampleIndex
        int lane; //0 - (lanes - 1)
    }

    public class Obstacle : LaneObject
    {

    }

    public class Beat : LaneObject
    {
        float amplitude; //0 - 1.0; The amplitude of the beat
    }
    private Queue<LaneObject> laneObjects;
    */
