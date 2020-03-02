using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

public class BGAMenu : MonoBehaviour
{

    public Button selectFileButton;
    public Button generateBeatMapButton;
    public Button backButton;
    public Slider thresholdMultiplierSlider;
    public Slider minPeakSeperationTimeSlider;
    public Slider thresholdWindowLengthSlider;
    public BGA bga;

    public AudioClip inputAudioClip; //To be loaded from web url or file uri

    public enum STATE
    {
        READY,
        AUDIO_CLIP_LOADING,
        AUDIO_CLIP_ERROR,
        AUDIO_CLIP_LOADED,
        BGA_STARTED,
        BGA_FINISHED
    }

    public STATE state;

    public string path;
    
    public song_meta_struct song;

    // Start is called before the first frame update
    void Start()
    {
        bga = new BGA();
        selectFileButton = selectFileButton.GetComponent<Button>();
        generateBeatMapButton = generateBeatMapButton.GetComponent<Button>();
        thresholdMultiplierSlider = thresholdMultiplierSlider.GetComponent<Slider>();
        minPeakSeperationTimeSlider = minPeakSeperationTimeSlider.GetComponent<Slider>();
        thresholdWindowLengthSlider = thresholdWindowLengthSlider.GetComponent<Slider>();
        backButton = backButton.GetComponent<Button>();

        backButton.onClick.AddListener(() => {
          SceneManager.LoadScene(sceneBuildIndex:0);
        });

        selectFileButton.onClick.AddListener(selectFileListener);
        generateBeatMapButton.onClick.AddListener(generateBeatMapListener);

        //path = "file:///D:\\Music\\UnitySongs\\01 Dreams.ogg"; //for testing on windows: set the path var to a song
    }


    #if UNITY_ANDROID

    void selectFileListener()
    {
        Debug.Log("Opening select file on android...");

        //https://docs.unity3d.com/ScriptReference/AndroidJavaRunnable.html
        AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        AndroidJavaObject activity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
        string filePath = Application.persistentDataPath + "/Songs";
        Debug.Log(filePath);
        if (!Directory.Exists(filePath)) {
            Directory.CreateDirectory(filePath);
        }
        //path = "file://" + filePath;
        activity.Call("CallFromUnity", filePath);

        //FileBrowser.ShowLoadDialog(onSelectFileSuccess, onSelectFileCancel);
    }

    //void selectFileAndroid()
    //{
    //    AndroidJavaClass cls = new AndroidJavaClass("com.DefaultCompany.NewUnityProject.CustomUnityPlayerActivity");
    //    Debug.Log(cls);
    //    cls.CallStatic("CallFromUnity");
    //}

    //call the java code
   // void selectFileAndroid()
    //{
     // AndroidJavaClass class = new AndroidJavaClass("CustomUnityPlayerActivity");
     // class.CallStatic("CallFromUnity");
    //}

    void resultFromJava(string s) 
    {
      Debug.Log("Got a result from java");
      Debug.Log(s);
      string[] strings = s.Split(new char[] {BGACommon.DELIMITER});
      song = new song_meta_struct(strings[0], strings[1], strings[2]);
      path = Application.persistentDataPath + "/Songs/" + s;
      //path = s;
    }

    #else

    void selectFileListener()
    {
        //not supported on windows, function still needs to be declared
    }

    #endif

    IEnumerator getAudioClipFromPath(string path)
    {
        //see https://docs.unity3d.com/ScriptReference/Networking.UnityWebRequestMultimedia.GetAudioClip.html

        AudioType audioType;
        #if UNITY_ANDROID
          audioType = AudioType.MPEG; //for android use MPEG (.mp3)
        #else
          audioType = AudioType.OGGVORBIS; //for testing on windows use OGGVORBIS (.ogg) since windows does not have mpeg codec native
        #endif

        Debug.Log(audioType);

        using (UnityWebRequest www = UnityWebRequestMultimedia.GetAudioClip("file://" + path, audioType))
        {
            yield return www.Send();

            if (www.isNetworkError)
            {
                Debug.Log("err");
                Debug.Log(www.error);
                state = STATE.AUDIO_CLIP_ERROR;
            }
            else
            {
                inputAudioClip = DownloadHandlerAudioClip.GetContent(www);
                Debug.Log("Loaded");
                state = STATE.AUDIO_CLIP_LOADED;
                //callBGA(ref audioClip);
                //bga.StartBGA(ref audioClip);
            }
        }
    }

    //void callBGA(ref AudioClip audioClip)
    //{
    //    
    //}

    void onSelectFileSuccess(string path)
    {
        Debug.Log(path);
        //this.path = "file://" + path;
        this.path = path;
    }

    void onSelectFileCancel()
    {

    }

    void generateBeatMapListener()
    {
        Debug.Log("generate");
        Debug.Log(path);
        if (path != "")
        {
            state = STATE.AUDIO_CLIP_LOADING;
            StartCoroutine(getAudioClipFromPath(path + ".mp3"));
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (state == STATE.AUDIO_CLIP_LOADED && inputAudioClip != null)
        {
            Debug.Log("Starting BGA");
            state = STATE.BGA_STARTED;

            bga_settings settings = new bga_settings(1024, thresholdWindowLengthSlider.value, thresholdMultiplierSlider.value, minPeakSeperationTimeSlider.value, 5f, 0);
            bga.StartBGA(ref inputAudioClip, settings, song, path);
        }

        if (state == STATE.BGA_STARTED) {
            Debug.Log(bga.state.ToString());
            if (bga.state == BGA.STATE.DONE) {
                SceneManager.LoadScene(sceneBuildIndex:0);
            }
        }
    }
}
