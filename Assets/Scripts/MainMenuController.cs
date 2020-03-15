using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;

public class MainMenuController : MonoBehaviour
{
    // Start is called before the first frame update
    public static bool finishedMap = false;
    public static Vector3[] cameraLocations;
    public static Quaternion[] cameraRotations;
    public TextAsset jsonFile;
    public static PlayerClass player;
    public static bool updated = false;
    WinDataClass winData;
    public static GameObject[] cars;
    public static Vector3[] carPositions; // proper car positions so that the car is one the plane
    
    
    void Start()
    {
        //Screen.SetResolution(1920, 1080, true, 60);
        // prevents reading on game launch
        if(finishedMap){
            WinDataClassHelper.LoadFromExternal(ref winData);
            handlePlayerUpdate();
            finishedMap = false; 
        }
        
        Time.timeScale = 1f;
        cameraLocations = new Vector3[] {new Vector3(22.42f, 3.46f, 17.65f), new Vector3(-1.91f, 6.15f, 41.69f), new Vector3(36.81f, 4.09f, 29.95f)};
        cameraRotations = new Quaternion[] {Quaternion.Euler(0f ,0f , -3.446f), Quaternion.Euler(2.148f, 141.484f, 2.696f), Quaternion.Euler(-0.667f, -81.023f, 1.109f)};

        // load cars
        GameObject car_gt86 = GameObject.Find("car_gt86");
        GameObject car_merc = GameObject.Find("car_merc");
        GameObject car_lambo = GameObject.Find("car_lambo");
       cars = new GameObject[]{car_gt86, car_merc, car_lambo}; // do not change indices
        carPositions = new Vector3[]{new Vector3(-516, -592.95f, 96.9325f), new Vector3(-516, -592.94f, 96.9325f), new Vector3(-516, -589.8253f, 96.9325f)};

        //saveToJSON(player);
        player = new PlayerClass("", 0, 0, 0, 0, 0);
        
        //saveToJSON(player);
        // loads the player's data upon startup
        LoadPlayerFromExternal(ref player);
        print(player.name);
        
        // make the user's current car active.
        int counter = 0;
        cars[2].SetActive(false);
        foreach(GameObject obj in cars){
            //print(obj.transform.position);
            if(counter == player.currentCarID){
                //obj.transform.position = carPositions[]
                obj.SetActive(true);
            }else{
                obj.SetActive(false);
            }
            counter++;
        }
        
    }

    void handlePlayerUpdate(){
        player.money += winData.moneyEarned;
        player.currentExperience += winData.expEarned;
        // handling level up
        if(player.currentExperience >= player.experienceForNextLevel){
            player.level += 1;
            //player.experienceForPrevLevel = player.experienceForNextLevel;
            player.experienceForNextLevel = player.experienceForNextLevel * 2;
        }
        savePlayerToExternal(player);
    }

    // Update is called once per frame
    void Update()
    {
        //Debug.Log("test");
    }

    private void initializePlayer(){

    }

    // parses a json, saves the data into a PlayerClass object.
    public static void LoadPlayerFromExternal(ref PlayerClass player){
        string dest = Application.persistentDataPath + "/player.dat";
        FileStream file;

        if(File.Exists(dest)){
            file = File.OpenRead(dest);
        }else{
            // create a default player json if it doesn't exist
            //file = File.Create(dest);
            player = new PlayerClass("NewPlayer", 0, 0, 1000, 1500, 0);
            /*player.name = "NewPlayer";
            player.level = 0;
            player.currentExperience = 0;
            player.experienceForNextLevel = 1000;
            player.money = 1500;*/
            print(player.money);
            //print("here");
            savePlayerToExternal(player);
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
    public static void savePlayerToExternal(PlayerClass player){
         
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

    

    /* public void LoadFromExternal(ref WinDataClass wd){
        string dest = Application.persistentDataPath + "/winData.dat";
        FileStream file;

        if(File.Exists(dest)){
            file = File.OpenRead(dest);
        }else{
            // create a default player json if it doesn't exist
            //file = File.Create(dest);
            //player = new PlayerClass("NewPlayer", 0, 0, 1000, 1500);
            //player.name = "NewPlayer";
            //player.level = 0;
            //player.currentExperience = 0;
            //player.experienceForNextLevel = 1000;
            //player.money = 1500;
            print("error loading victory file");
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
    } */
}
