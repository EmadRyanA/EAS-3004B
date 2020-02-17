using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuController_old : MonoBehaviour
{
    // Start is called before the first frame update
    public static Vector3[] cameraLocations;
    public static Quaternion[] cameraRotations;
    void Start()
    {
        cameraLocations = new Vector3[] {new Vector3(22.42f, 3.46f, 17.65f), new Vector3(-1.91f, 6.15f, 41.69f)};
        cameraRotations = new Quaternion[] {Quaternion.Euler(0f ,0f , -3.446f), Quaternion.Euler(2.148f, 141.484f, 2.696f)};
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
