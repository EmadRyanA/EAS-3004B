using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmoothCameraController : MonoBehaviour
{
    // Start is called before the first frame update

    public GameObject player;
    public float smoothTime = 0.3F;
    private Vector3 velocity = Vector3.zero;
    private Vector3 offset;

    // use this for initialization
    void Start()
    {
        offset = transform.position - player.transform.position;
    }

    // Update is called once per frame
    // late update is called when everything else has been called.
    void FixedUpdate()
    {
        // transform.position = player.transform.position + offset;
        transform.position = Vector3.SmoothDamp(transform.position , player.transform.position + offset, ref velocity, smoothTime);
    }

    /* public Transform target;
`    private Vector3 offset;
    
    private void Start() {
        offset = transform.position - target.transform.position;
    }
    void Update() {
        if (target){
            transform.position = Vector3.Lerp(transform.position + offset, target.transform.position, 0.5f);
        }
    } */
}
