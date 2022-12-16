using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KidAnimation : MonoBehaviour
{
    private Animator animation_controller;
    private CharacterController character_controller;
    public float walking_velocity;
    public float velocity;
    public float xdirection;
    public float zdirection;
    public Vector3 movement_direction;
    public bool isIdle;
    // Start is called before the first frame update
    void Start()
    {
        animation_controller = GetComponent<Animator>();
        character_controller = GetComponent<CharacterController>();   
        movement_direction = new Vector3(0.0f, 0.0f, 0.0f);
        walking_velocity = 1.5f;
        velocity = 0.0f;
    }

    // Update is called once per frame
    void Update()
    {
        if(isIdle){
            velocity = 0.0f;
        }
        
        bool upArrow=Input.GetKey(KeyCode.UpArrow);
        bool downArrow=Input.GetKey(KeyCode.DownArrow);
        bool space=Input.GetKey(KeyCode.Space);
        if(upArrow){
            animation_controller.SetBool("toWalk",true);
            walk_forward();
            isIdle = false;
        }
        else{
            animation_controller.SetBool("toWalk",false);
            isIdle = true;
        }
        if(space){

            animation_controller.SetBool("toJump",true);
            jump();
            isIdle = false;
        }
        else{
            animation_controller.SetBool("toJump",false);
            isIdle = true;
        }
        if(Input.GetKey(KeyCode.LeftArrow)){
                Debug.Log("bruh");
                transform.Rotate(0.0f, -0.5f, 0.0f);
            }
        if(Input.GetKey(KeyCode.RightArrow)){
            transform.Rotate(0.0f, 0.5f, 0.0f);
        }
        xdirection = Mathf.Sin(Mathf.Deg2Rad * transform.rotation.eulerAngles.y);
        zdirection = Mathf.Cos(Mathf.Deg2Rad * transform.rotation.eulerAngles.y);
        movement_direction = new Vector3(xdirection, 0.0f, zdirection);
    }
    public void walk_forward(){
        Debug.Log("transform check step 2");
        if(velocity<walking_velocity){
            velocity+=0.1f;
        }
        else{
            velocity-=0.2f;
        }
        Debug.Log(transform.position+" is position");
        transform.position=new Vector3(transform.position.x+ xdirection*velocity*Time.deltaTime, transform.position.y, transform.position.z+zdirection*velocity*Time.deltaTime);
    }
    
    public void run(){
        if(velocity<walking_velocity*2.0){
            velocity+=0.35f;
        }
        transform.position=new Vector3(transform.position.x+ xdirection*velocity*Time.deltaTime, transform.position.y, transform.position.z+zdirection*velocity*Time.deltaTime);
    }
    public void jump(){
        if(velocity<walking_velocity*3.0){
            velocity+=0.5f;
        }
        transform.position = new Vector3(transform.position.x+ xdirection*velocity*Time.deltaTime, transform.position.y*velocity*Time.deltaTime, transform.position.z+zdirection*velocity*Time.deltaTime);
        //transform.position=new Vector3(transform.position.x+ xdirection*velocity*Time.deltaTime, transform.position.y, transform.position.z+zdirection*velocity*Time.deltaTime);
    }
}
