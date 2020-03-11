using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

using UnityEngine;

[System.Serializable]
public class PlayerClass
{
    /*
        "Name": "NewPlayer",
        "Level": 0,
        "CurrentExperience": 0,
        "ExperienceForNextLevel": 1000,
        "Money": 1500
    */
    public string name{get;set;}
    public int level{get;set;}
    public float currentExperience{get;set;}
    public float experienceForNextLevel{get;set;}
    public int money{get;set;}
    public int currentCarID{get;set;}
    public int[] ownedCars{get;set;}

    public PlayerClass(string n, int l, float ce, float next, int m, int carID){
        name = n;
        level = l;
        currentExperience = ce;
        experienceForNextLevel = next;
        money = m;
        currentCarID = carID;
        ownedCars = new int[]{0};
    }


    
}
