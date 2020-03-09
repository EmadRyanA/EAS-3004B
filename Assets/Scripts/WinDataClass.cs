using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

using UnityEngine;

// provides data to be handled when the user succesfully completes a beatmap

[System.Serializable]
public class WinDataClass
{
    public int score{get;set;}
    public int maxCombo{get;set;}
    public int notesHit{get;set;}
    public int mapTotalNotes{get;set;}
    public int moneyEarned{get;set;}
    public float expEarned{get;set;}
    public string grade{get;set;}

    public WinDataClass(int s, int mc, int nh, int mtn, int me, float ee, string g){
        score = s;
        maxCombo = mc;
        notesHit = nh;
        mapTotalNotes = mtn;
        moneyEarned = me;
        expEarned = ee;
        grade = g;
    }


    
}
