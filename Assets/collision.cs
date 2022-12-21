using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class collision : MonoBehaviour
{
    // Start is called before the first frame update
    // 
    private Animator animation_controller;
    void Start(){

        animation_controller = GetComponent<Animator>();
    }
    IEnumerator WaitForAttackToFinish(){
        yield return new WaitForSeconds(1);
        // fps_player_obj.transform.position = new Vector3(pos.x-9, pos.y, pos.z);
    }
    void OnCollisionEnter(Collision collision){
        Debug.Log(gameObject.name);
        if (collision.gameObject.name == "TREE"){
             Debug.Log("Name is " +collision.gameObject.name+ " ok?");
            Physics.IgnoreCollision(GetComponent<Collider>(), collision.collider);
         }
         if(collision.gameObject.name == "MOUNT"){
            Physics.IgnoreCollision(GetComponent<Collider>(), collision.collider);
        }
        if (collision.gameObject.name == "Bear"){
            Debug.Log("bear collided with player");
            //Figure out a way to get xdirection and ydirection from Bear.cs
            //Tried alot do not get it
            Debug.Log(animation_controller.GetBool("gothit"));
            animation_controller.SetBool("gothit",true);
             WaitForAttackToFinish();
        }
         Vector3 normal = collision.contacts[0].normal;
         Vector3 reflectedVelocity = Vector3.Reflect(GetComponent<Rigidbody>().velocity, normal);
         GetComponent<Rigidbody>().velocity = reflectedVelocity;
    }
}
