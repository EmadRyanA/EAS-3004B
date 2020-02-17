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
    public string name { get; set; }

    public BeatMap (bga_settings bga_settings, string name)
    {
        this.bga_settings = bga_settings;
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
        //todo: load all LaneObject from laneObjectStore into a queue
        return null;
    }
}