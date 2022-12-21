using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bear : MonoBehaviour
{
    private GameObject fps_player_obj;
    private GameObject level;
    private float radius_of_search_for_player;
    public float bear_speed;
    private Animator animation_controller;
    private CharacterController character_controller;
    public float velocity = 0.0f; 
    public float walking_velocity = 5f;
    public float xdirection;
    public float zdirection;
     
    // public GameObject level;
    // Start is called before the first frame update
    void Start()
    {
        
        animation_controller = GetComponent<Animator>();
        character_controller = GetComponent<CharacterController>();  
        level = GameObject.FindGameObjectWithTag("Level");
        fps_player_obj = GameObject.Find("Human");
        Bounds bounds = level.GetComponent<Collider>().bounds;
        radius_of_search_for_player = (bounds.size.x + bounds.size.z) / 10.0f;
        // Debug.Log(level.GetComponent<Collider>().bounds);
        bear_speed = level.GetComponent<Stage_1>().bear_speed;
        // fps_player_obj = level.fps_player_obj;
    }

    // Update is called once per frame
    IEnumerator WaitForAttackToFinish(Vector3 pos){
        yield return new WaitForSeconds(1);
        fps_player_obj.transform.position = new Vector3(pos.x-9, pos.y, pos.z);
    }
    void Update()
    {
        
        if (Vector3.Distance(transform.position, fps_player_obj.transform.position) < 1.5){
            // Debug.Log("Should be attacking!!");
            animation_controller.SetTrigger("Attack1");
            Vector3 targetDirection = fps_player_obj.transform.position - transform.position;
            float singleStep = velocity * Time.deltaTime;
            Vector3 newDirection = Vector3.RotateTowards(transform.forward, targetDirection, singleStep, 0.0f);
            transform.rotation = Quaternion.LookRotation(newDirection);
            Vector3 pos = fps_player_obj.transform.position;
            // We should have a textbox on top saying bear strike.
            // StartCoroutine(WaitForAttackToFinish(pos));
            
        }
        else{
            animation_controller.SetBool("Idle",true);
        }
        // Debug.Log(bounds.size.x);
        // Debug.Log(Vector3.Distance(transform.position, fps_player_obj.transform.position));
        if (Vector3.Distance(transform.position, fps_player_obj.transform.position) < radius_of_search_for_player/1.5f){

            animation_controller.SetBool("Run Forward",true);
            Vector3 targetDirection = fps_player_obj.transform.position - transform.position;
            float singleStep = velocity * Time.deltaTime;
            Vector3 newDirection = Vector3.RotateTowards(transform.forward, targetDirection, singleStep, 0.0f);
            transform.rotation = Quaternion.LookRotation(newDirection);
            run();
        }
        else{
            animation_controller.SetBool("Run Forward",false);
            animation_controller.SetBool("Idle",true);
        }
        if (Vector3.Distance(transform.position, fps_player_obj.transform.position) < radius_of_search_for_player/3.0f){
        //    Debug.Log("walk forward");
           animation_controller.SetBool("WalkForward",true);
            Vector3 targetDirection = fps_player_obj.transform.position - transform.position;
            float singleStep = velocity * Time.deltaTime;
            Vector3 newDirection = Vector3.RotateTowards(transform.forward, targetDirection, singleStep, 0.0f);
            transform.rotation = Quaternion.LookRotation(newDirection);
            walk_forward();
        }
        else{
            animation_controller.SetBool("Run Forward",false);
            animation_controller.SetBool("Idle",true);
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
    private void OnCollisionEnter(Collision collision)
    {
        // Debug.Log("yes");
        // Debug.Log(collision.contacts);
        Vector3 normal = collision.contacts[0].normal;
        // Debug.Log("Yes");
        //GetComponent<Rigidbody>().MovePosition(new Vector3(GetComponent<Rigidbody>().position[0] , 0.0f , GetComponent<Rigidbody>().position[2]));
    // Reflect the object's velocity off the surface
        // Vector3 reflectedVelocity = Vector3.Reflect(GetComponent<Rigidbody>().velocity, normal);

    // Set the object's velocity to the reflected velocity
        //GetComponent<Rigidbody>().velocity = reflectedVelocity;
        if (collision.gameObject.name == "player")
        {
            Debug.Log("Namaster");
        }
    }
}
