using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;

public class MainMenuController : MonoBehaviour
{
    // Start is called before the first frame update
    public static Vector3[] cameraLocations;
    public static Quaternion[] cameraRotations;
    public TextAsset jsonFile;
    public static PlayerClass player;
    void Start()
    {
        cameraLocations = new Vector3[] {new Vector3(22.42f, 3.46f, 17.65f), new Vector3(-1.91f, 6.15f, 41.69f), new Vector3(36.81f, 4.09f, 29.95f)};
        cameraRotations = new Quaternion[] {Quaternion.Euler(0f ,0f , -3.446f), Quaternion.Euler(2.148f, 141.484f, 2.696f), Quaternion.Euler(-0.667f, -81.023f, 1.109f)};
    
        
        //saveToJSON(player);
        player = new PlayerClass("", 0, 0, 0, 0);
        
        //saveToJSON(player);
        LoadFromJSON(ref player);
        print(player.name);
        
        
    }

    // Update is called once per frame
    void Update()
    {
        //Debug.Log("test");
    }

    private void initializePlayer(){

    }

    // parses a json, saves the data into a PlayerClass object.
    public void LoadFromJSON(ref PlayerClass player){
        string dest = Application.persistentDataPath + "/player.dat";
        FileStream file;

        if(File.Exists(dest)){
            file = File.OpenRead(dest);
        }else{
            // create a default player json if it doesn't exist
            //file = File.Create(dest);
            player = new PlayerClass("NewPlayer", 0, 0, 1000, 1500);
            /*player.name = "NewPlayer";
            player.level = 0;
            player.currentExperience = 0;
            player.experienceForNextLevel = 1000;
            player.money = 1500;*/
            print(player.money);
            //print("here");
            saveToJSON(player);
            return; // something here
        }
        BinaryFormatter bf = new BinaryFormatter();
        
        player = (PlayerClass) bf.Deserialize(file);
        print(player.name);
        print(player.money);
        //player = new PlayerClass("", 0,0,0,0);
        
        file.Close();


        //return JsonUtility.FromJson<PlayerClass>(json.text);
    }
    // saves a player to json
    public void saveToJSON(PlayerClass player){
         
        string dest = Application.persistentDataPath + "/player.dat";
        FileStream file;
        //string jsonStr = JsonUtility.ToJson(player);

        if(File.Exists(dest)){
            file = File.OpenWrite(dest);
        }else{
            file = File.Create(dest);
        }

        BinaryFormatter bf = new BinaryFormatter();
        bf.Serialize(file, player);
        file.Close();
        
    }
}
