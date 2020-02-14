using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMoveLogic : MonoBehaviour
{
    private CharacterController controller;
    
    private float speed = 5.0f;
    private float verticalSpeed;
    private Vector3 moveSpeed;
    

    // Start is called before the first frame update
    void Start()
    {
        controller = GetComponent<CharacterController>();        
    }

    // Update is called once per frame
    void Update()
    {
        moveSpeed = Vector3.zero;

        //Falling logic
        if (controller.isGrounded)
        {
            verticalSpeed = 0.0f; //on ground no falling
        }
        else
        {
            verticalSpeed -= 10 * Time.deltaTime; //falling speed
        }
        //X val, left & right
        moveSpeed.x = Input.GetAxisRaw("Horizontal") * speed *5;   //allows changing the x val of the vector
        //Y val, up & down 
        moveSpeed.y = verticalSpeed;
        //Z val, forwards & backwards
        moveSpeed.z = speed;


        controller.Move(moveSpeed * Time.deltaTime);
    }
}
