using System.Collections;
using System.Collections.Generic;
using System.IO;
using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;

public class VictorySceneController : MonoBehaviour
{
    /*
    [Serializable]
    public struct WinData{
        public int score;
        public int maxCombo;
        public int notesHit;
        public int mapTotalNotes;
        public int moneyEarned;
        public float expEarned;
    }
    */
    WinDataClass winData;
    GameObject continueButton;
    GameObject scoreText;
    GameObject maxComboText;
    GameObject notesHitText;
    GameObject moneyEarnedText;
    GameObject expText;
    GameObject gradeText;

    // Start is called before the first frame update
    void Start()
    {
        // finding gameobjects
        continueButton = GameObject.Find("continueButton");
        scoreText= GameObject.Find("scoreText");
        maxComboText = GameObject.Find("maxComboText");
        notesHitText = GameObject.Find("notesHitText");
        moneyEarnedText = GameObject.Find("moneyEarnedText");
        expText = GameObject.Find("expText");
        gradeText = GameObject.Find("gradeText");

        // listeners
        continueButton.GetComponent<Button>().onClick.AddListener(handleContinueClick);

        handleWinData();
    }

    // Update is called once per frame 
    void Update()
    {
        
    }

    private void handleContinueClick(){
        print("test");
        MainMenuController.finishedMap = true; // sets this bool so that the data isnt read on game launch
        SceneManager.LoadScene("Main Menu");
    }

    // load map complete file from persistent storage 
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

    public void handleWinData(){
        WinDataClassHelper.LoadFromExternal(ref winData);
        print("testtestestsetestset" + winData.expEarned);
        //decimal noteHitPercentage = (winData.notesHit/winData.mapTotalNotes)*100; // need to force a decimal value

        scoreText.GetComponent<Text>().text = winData.score + "";
        maxComboText.GetComponent<Text>().text = winData.maxCombo + "x";
        // returns the notes hit out of the total, and the percentage of notes hit.
        notesHitText.GetComponent<Text>().text = winData.notesHit + "/" + winData.mapTotalNotes + "(" + Math.Round((((decimal)winData.notesHit/(decimal)winData.mapTotalNotes)*100), 2) + "%)";
        moneyEarnedText.GetComponent<Text>().text = winData.moneyEarned + "";
        expText.GetComponent<Text>().text = winData.expEarned + "";
        // calculate grade
        gradeText.GetComponent<Text>().text = winData.grade;
    }
}
