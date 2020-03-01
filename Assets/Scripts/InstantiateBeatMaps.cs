using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using UnityEngine.UI;

public class InstantiateBeatMaps : MonoBehaviour
{

    public GameObject prefab;
    public Transform mapsContent;
    // Start is called before the first frame update
    void Start()
    {
        mapsContent = mapsContent.GetComponent<Transform>();
        string beatMapDir = Application.persistentDataPath + "/BeatMaps/";
        Debug.Log(beatMapDir);
        Directory.CreateDirectory(beatMapDir); //create if it does not exist
        foreach (string fileName in Directory.EnumerateFiles(beatMapDir)) {
          //see https://docs.microsoft.com/en-us/dotnet/csharp/programming-guide/concepts/serialization/walkthrough-persisting-an-object-in-visual-studio
          if (!fileName.EndsWith(".dat")) continue;
          Stream openFileStream = File.OpenRead(fileName);
          BinaryFormatter deserializer = new BinaryFormatter();
          BeatMap beatMap = (BeatMap)deserializer.Deserialize(openFileStream);
          GameObject beatMapPanel = (GameObject)Instantiate(prefab, new Vector3(0, 0, 0), new Quaternion(0, 0, 0, 0), mapsContent);
          BeatMapEntryController controller = beatMapPanel.GetComponentInChildren<BeatMapEntryController>();
          controller.fileName = fileName;
          beatMapPanel.GetComponentInChildren<Text>().text = beatMap.name;
        }
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
