using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

// contains the saving and loading classes for handling windata
public class WinDataClassHelper
{
    public static void saveToExternal(WinDataClass wd){
         
        string dest = Application.persistentDataPath + "/winData.dat";
        FileStream file;
        //string jsonStr = JsonUtility.ToJson(player);

        if(File.Exists(dest)){
            file = File.OpenWrite(dest);
        }else{
            file = File.Create(dest);
        }

        BinaryFormatter bf = new BinaryFormatter();
        bf.Serialize(file, wd);
        file.Close();
    }

    public static void LoadFromExternal(ref WinDataClass wd){
        string dest = Application.persistentDataPath + "/winData.dat";
        FileStream file;

        if(File.Exists(dest)){
            file = File.OpenRead(dest);
        }else{
            // create a default player json if it doesn't exist
            //file = File.Create(dest);
            //player = new PlayerClass("NewPlayer", 0, 0, 1000, 1500);
            /*player.name = "NewPlayer";
            player.level = 0;
            player.currentExperience = 0;
            player.experienceForNextLevel = 1000;
            player.money = 1500;*/
            Debug.Log("error loading victory file");
            //print("here");
            //saveToJSON(player);
            return; // something here
        }
        BinaryFormatter bf = new BinaryFormatter();
        
        wd = (WinDataClass) bf.Deserialize(file);
        //print(player.name);
        //print(player.money);
        //player = new PlayerClass("", 0,0,0,0);
        
        file.Close();


        //return JsonUtility.FromJson<PlayerClass>(json.text);
    }

}
