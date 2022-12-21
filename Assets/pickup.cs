using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class pickup : MonoBehaviour
{
    public float pickupRange = 1f; // the distance at which the object can be picked up
    public float moveForce = 10f;// the force applied to the object when it is being moved

    private Rigidbody heldObject; // the Rigidbody component of the picked up object
    public GameObject player; // the player GameObject
    public bool flag = false; 

    void Update()
    {
        // make sure the player and object have been assigned
        if (player == null || gameObject == null) return; 

        // check if the player is within range of the object
        if (Vector3.Distance(player.transform.position, transform.position) < pickupRange)
        { 
            // check if the player is pressing the E key
            heldObject = GetComponent<Rigidbody>();
            if (Input.GetKeyDown(KeyCode.E))
            {
                // pick up the object
                flag = true;
                PickupObject();
                // move the object
                MoveObject();
                
            }
        }
        // if(flag){
        // MoveObject();
        // }
    }

    void MoveObject()
    {
        // add force to the object to move it towards the player
        Vector3 moveDirection = (new Vector3(player.transform.position.x+ 0.1f, 1f+ player.transform.position.y,player.transform.position.z + 0.001f) - heldObject.transform.position);
        // heldObject.AddForce(moveDirection * moveForce);
        heldObject.transform.position =  new Vector3(player.transform.position.x + 0.9f, + player.transform.position.y+1.5f,player.transform.position.z);
        heldObject.transform.rotation = Quaternion.Euler(3f,88f,3f); 
    }
    
    void PickupObject()
    {
        // get the Rigidbody component of the object
        
        if (heldObject != null)
        {
            // disable gravity on the object
            heldObject.useGravity = false;
            // set the object's parent to the player's transform
            heldObject.transform.parent = player.transform; 
        }
    }
}
