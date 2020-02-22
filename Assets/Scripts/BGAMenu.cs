using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

public class BGAMenu : MonoBehaviour
{

    public Button selectFileButton;
    public Button generateBeatMapButton;
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
        BGA_STARTED
    }

    public STATE state;

    public string path;

    // Start is called before the first frame update
    void Start()
    {
        selectFileButton = selectFileButton.GetComponent<Button>();
        generateBeatMapButton = generateBeatMapButton.GetComponent<Button>();
        thresholdMultiplierSlider = thresholdMultiplierSlider.GetComponent<Slider>();
        minPeakSeperationTimeSlider = minPeakSeperationTimeSlider.GetComponent<Slider>();
        thresholdWindowLengthSlider = thresholdWindowLengthSlider.GetComponent<Slider>();


        selectFileButton.onClick.AddListener(selectFileListener);
        generateBeatMapButton.onClick.AddListener(generateBeatMapListener);

        path = "file:///D:\\Music\\UnitySongs\\01 Dreams.ogg";
    }

    void selectFileListener()
    {
        Debug.Log("Test");
        //FileBrowser.ShowLoadDialog(onSelectFileSuccess, onSelectFileCancel);
    }

    IEnumerator getAudioClipFromPath(string path)
    {
        //see https://docs.unity3d.com/ScriptReference/Networking.UnityWebRequestMultimedia.GetAudioClip.html
        using (UnityWebRequest www = UnityWebRequestMultimedia.GetAudioClip(path, AudioType.OGGVORBIS))
        {
            yield return www.Send();

            if (www.isNetworkError)
            {
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
        this.path = "file:///" + path;
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
            StartCoroutine(getAudioClipFromPath(path));
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
            bga.StartBGA(ref inputAudioClip, settings);
        }
    }
}
