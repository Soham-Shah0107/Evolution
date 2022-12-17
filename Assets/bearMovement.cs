using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class bearMovement : MonoBehaviour
{
    private GameObject fps_player_obj;
    private GameObject level;
    private float radius_of_search_for_player;
    private float bear_speed;
    private Animator animation_controller;
    private CharacterController character_controller;
    public float velocity = 0.0f; 
    public float walking_velocity = 2.5f;
    public float xdirection;
    public float zdirection;
    // Start is called before the first frame update
    void Start()
    {
        animation_controller = GetComponent<Animator>();
        character_controller = GetComponent<CharacterController>();  
        level = GameObject.FindGameObjectWithTag("Level");
        fps_player_obj = GameObject.Find("player");
        Bounds bounds = level.GetComponent<Collider>().bounds;
        radius_of_search_for_player = (bounds.size.x + bounds.size.z) / 10.0f;
        bear_speed = level.GetComponent<Stage_1>().bear_speed;
    }

    // Update is called once per frame
    void Update()
    {
        
        if (Vector3.Distance(transform.position, fps_player_obj.transform.position) < radius_of_search_for_player){
            Debug.Log("Intereseitng??");
            animation_controller.SetBool("WalkForward",true);
            Vector3 targetDirection = fps_player_obj.transform.position - transform.position;
            float singleStep = velocity * Time.deltaTime;
            Vector3 newDirection = Vector3.RotateTowards(transform.forward, targetDirection, singleStep, 0.0f);
            transform.rotation = Quaternion.LookRotation(newDirection);
            walk_forward();
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
        transform.position=new Vector3(transform.position.x+ xdirection*velocity*Time.deltaTime, transform.position.y, transform.position.z+zdirection*velocity*Time.deltaTime);
    }
}
