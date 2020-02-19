using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable()]
public class BeatMap
{

    public bga_settings settings;
    public string name;
    public BeatMap (bga_settings settings, string name) {
        this.settings = settings;
        this.name = name;
    }
}