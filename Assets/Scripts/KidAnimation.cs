using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class KidAnimation : MonoBehaviour
{
    private Animator animation_controller;
    private GameObject bear;
    private CharacterController character_controller;
    public float walking_velocity;
    public float velocity;
    public float xdirection;
    public float zdirection;
    public Vector3 movement_direction;
    public bool isIdle;
    Collider m_ObjectCollider;
    // bearMovement bearClass = null;
    // Start is called before the first frame update
    void Start()
    {
        animation_controller = GetComponent<Animator>();
        character_controller = GetComponent<CharacterController>();   
        movement_direction = new Vector3(0.0f, 0.0f, 0.0f);
        walking_velocity = 2.5f;
        velocity = 0.0f;
    }

    // Update is called once per frame
    void Update()
    {

        if(isIdle){
            velocity = 0.0f;
        }
        isIdle = true;
        bool upArrow=Input.GetKey(KeyCode.UpArrow);
        bool downArrow=Input.GetKey(KeyCode.DownArrow);
        bool shiftPressed=Input.GetKey(KeyCode.LeftShift)||Input.GetKey(KeyCode.RightShift);
        if(shiftPressed && upArrow){
            Debug.Log("Should be running");
            animation_controller.SetBool("isRunning",true);
            run();
            isIdle = false;
        }
        else{
            animation_controller.SetBool("isRunning",false);

        }
        if(upArrow){
            animation_controller.SetBool("toWalk",true);
            walk_forward();
            isIdle = false;
        }
        else{
            animation_controller.SetBool("toWalk",false);
        }
        
        if(downArrow){
            animation_controller.SetBool("toJump",true);
            walk_backward();
            isIdle = false;
        }
        else{
            animation_controller.SetBool("toJump",false);
        }
        if(Input.GetKey(KeyCode.LeftArrow)){
                // Debug.Log("bruh");
                GetComponent<Rigidbody>().rotation = Quaternion.Euler(transform.rotation.eulerAngles.x, transform.rotation.eulerAngles.y-0.5f, transform.rotation.eulerAngles.z);
            }
        if(Input.GetKey(KeyCode.RightArrow)){
GetComponent<Rigidbody>().rotation = Quaternion.Euler(transform.rotation.eulerAngles.x,transform.rotation.eulerAngles.y + 0.5f, transform.rotation.eulerAngles.z);
        }
        xdirection = Mathf.Sin(Mathf.Deg2Rad * transform.rotation.eulerAngles.y);
        zdirection = Mathf.Cos(Mathf.Deg2Rad * transform.rotation.eulerAngles.y);
        movement_direction = new Vector3(xdirection, 0.0f, zdirection);
    }
    public void walk_forward(){
        // Debug.Log("transform check step 2");
        if(velocity<walking_velocity){
            velocity+=0.1f;
        }
        else{
            velocity-=0.2f;
        }
        // Debug.Log(transform.position+" is position");
        GetComponent<Rigidbody>().MovePosition(GetComponent<Rigidbody>().position + new Vector3(xdirection * velocity * Time.deltaTime, 0.0f, zdirection * velocity * Time.deltaTime));
    }
    
    public void run(){
        if(velocity<walking_velocity*2.0){
            velocity+=0.35f;
        }
        GetComponent<Rigidbody>().MovePosition(GetComponent<Rigidbody>().position + new Vector3(xdirection * velocity * Time.deltaTime, 0.0f, zdirection * velocity * Time.deltaTime));
    }
    public void jump(){ // Very useless state
        if(velocity<walking_velocity*3.0){
            velocity+=0.5f;
        }
        transform.position = new Vector3(transform.position.x+ xdirection*velocity*Time.deltaTime, transform.position.y*velocity*Time.deltaTime*25, transform.position.z+zdirection*velocity*Time.deltaTime);
        //transform.position=new Vector3(transform.position.x+ xdirection*velocity*Time.deltaTime, transform.position.y, transform.position.z+zdirection*velocity*Time.deltaTime);
    }
    public void walk_backward(){

        if(velocity>-1*walking_velocity/2.0){
            velocity-=0.02f;
        }
        GetComponent<Rigidbody>().MovePosition(GetComponent<Rigidbody>().position + new Vector3(xdirection * velocity * Time.deltaTime, 0.0f, zdirection * velocity * Time.deltaTime));
    }
    void OnCollisionEnter(Collision collision){
        Debug.Log("Collsion from Kids");
        if (collision.gameObject.name == "Bear"){
            Debug.Log("bear collided with player");
            //Figure out a way to get xdirection and ydirection from Bear.cs
            //Tried alot do not get it
            
            // bear = GameObject.Find("Bear");
            // Debug.Log(bear.GetComponent("Bear Movements").velocity);
            // Debug.Log(collision.gameObject.GetComponent<Bear>());
            // int zdir = collision.gameObject.GetComponent("bearMovement").zdirection;
            // // .GetComponent<bearMovement>().Zdirection;
            // int xdir = collision.gameObject.GetComponent("bearMovement").xdirection;
            // GetComponent<Rigidbody>().MovePosition(GetComponent<Rigidbody>().position + new Vector3(xdir* 5 * Time.deltaTime, 0.0f, zdir * 5 * Time.deltaTime));
            Physics.IgnoreCollision(GetComponent<Collider>(), collision.collider);
            m_ObjectCollider = GetComponent<Collider>();
        }
        else{
      
            Debug.Log("Name is " +collision.gameObject.name+ " ok?");
        }
         Vector3 normal = collision.contacts[0].normal;

    // Reflect the object's velocity off the surface
         Vector3 reflectedVelocity = Vector3.Reflect(GetComponent<Rigidbody>().velocity, normal);

    // Set the object's velocity to the reflected velocity
       // GetComponent<Rigidbody>().velocity = reflectedVelocity;
        

    }
  
   
}
