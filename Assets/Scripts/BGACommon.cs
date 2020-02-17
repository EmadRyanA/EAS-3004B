using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum LANE_OBJECT_TYPE
{
    Obstacle,
    Beat
}

[Serializable()]
public struct LaneObject
{
    int sampleIndex; //Index of this obj in peaks[]
    float time; //Timestamp approximation of sampleIndex
    int lane; //0 - (lanes - 1)
    LANE_OBJECT_TYPE type;
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
    public int n_bins;
    public float threshold_time;
    public float threshold_multiplier;
    public float min_peak_seperation_time;
    public int rng_seed; //If rng_seed == 0 a new random seed is created

    public bga_settings(int n_bins, float threshold_time, float threshold_multiplier, float min_peak_seperation_time, int rng_seed)
    {
        this.n_bins = n_bins;
        this.threshold_time = threshold_time;
        this.threshold_multiplier = threshold_multiplier;
        this.min_peak_seperation_time = min_peak_seperation_time;
        this.rng_seed = rng_seed;
    }
}

public static class BGACommon
{
    public const int NUMBER_LANES = 3;
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

