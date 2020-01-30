using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToggleRender : MonoBehaviour
{
    // Start is called before the first frame update
    public bool toggled = false;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        GetComponent<Renderer>().enabled = toggled;
    }
}
