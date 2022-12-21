using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fox : MonoBehaviour
{
    // Start is called before the first frame update
    private GameObject fps_player_obj;
    private GameObject level;
    private float radius_of_search_for_player;
    public float fox_speed;
    private Animator animation_controller;
    private CharacterController character_controller;
    public float velocity = 0.0f; 
    public float walking_velocity = 1f;
    public float xdirection;
    public float zdirection;
     
    // Start is called before the first frame update
    void Start()
    {
        
        animation_controller = GetComponent<Animator>();
        character_controller = GetComponent<CharacterController>();  
        level = GameObject.FindGameObjectWithTag("Level");
        fps_player_obj = GameObject.Find("Human");
        Bounds bounds = level.GetComponent<Collider>().bounds;
        radius_of_search_for_player = (bounds.size.x + bounds.size.z) / 10.0f;
        fox_speed = level.GetComponent<Stage_1>().fox_speed;
       // fps_player_obj = level.fps_player_obj;
    }


    void Update()
    {
        Vector3 pos = transform.position;
        if(pos.y!=0.0f){
            transform.position= new Vector3(pos.x, 0.0f, pos.z);
        }
        if (Vector3.Distance(transform.position, fps_player_obj.transform.position) < 1){
            // Debug.Log("Should be attacking!!");
            //animation_controller.SetTrigger("Attack1");
            animation_controller.SetBool("attack",true);
            animation_controller.SetBool("idle",false);
            Vector3 targetDirection = fps_player_obj.transform.position - transform.position;
            float singleStep = velocity * Time.deltaTime;
            Vector3 newDirection = Vector3.RotateTowards(transform.forward, targetDirection, singleStep, 0.0f);
            transform.rotation = Quaternion.LookRotation(newDirection);
            
        }
        else{
            animation_controller.SetBool("attack",false);
            animation_controller.SetBool("idle",true);
        }

        if (Vector3.Distance(transform.position, fps_player_obj.transform.position) < radius_of_search_for_player/2.5f){

            animation_controller.SetBool("toRun",true);
            animation_controller.SetBool("idle",false);
            Vector3 targetDirection = fps_player_obj.transform.position - transform.position;
            float singleStep = velocity * Time.deltaTime;
            Vector3 newDirection = Vector3.RotateTowards(transform.forward, targetDirection, singleStep, 0.0f);
            transform.rotation = Quaternion.LookRotation(newDirection);
            run();
        }
        else{
            animation_controller.SetBool("toRun",false);
            animation_controller.SetBool("idle",true);
        }
        if (Vector3.Distance(transform.position, fps_player_obj.transform.position) < radius_of_search_for_player/5.0f){
        //    Debug.Log("walk forward");
            animation_controller.SetBool("walk",true);
            animation_controller.SetBool("idle",false);
            Vector3 targetDirection = fps_player_obj.transform.position - transform.position;
            float singleStep = velocity * Time.deltaTime;
            Vector3 newDirection = Vector3.RotateTowards(transform.forward, targetDirection, singleStep, 0.0f);
            transform.rotation = Quaternion.LookRotation(newDirection);
            walk_forward();
        }
        else{
            animation_controller.SetBool("walk",false);
            animation_controller.SetBool("idle",true);
        }

        xdirection = Mathf.Sin(Mathf.Deg2Rad * transform.rotation.eulerAngles.y);
        zdirection = Mathf.Cos(Mathf.Deg2Rad * transform.rotation.eulerAngles.y);
        // movement_direction = new Vector3(xdirection, 0.0f, zdirection);
        
    }
    public void walk_forward(){
        

        if(velocity<walking_velocity){
            velocity+=0.1f;
        }
        else{
            velocity-=0.2f;
        }
    //    / Debug.Log(GetComponent<Rigidbody>().position);
        transform.position=new Vector3(transform.position.x+ xdirection*velocity*Time.deltaTime, transform.position.y, transform.position.z+zdirection*velocity*Time.deltaTime);
    }
    public void run(){
        if(velocity<walking_velocity*1.5){
            velocity+=0.1f;
        }
        else{
            velocity-=0.2f;
        }
       transform.position=new Vector3(transform.position.x+ xdirection*velocity*Time.deltaTime, transform.position.y, transform.position.z+zdirection*velocity*Time.deltaTime);
    }
 }
