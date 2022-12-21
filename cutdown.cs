using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cutdown : MonoBehaviour
{
    // Start is called before the first frame update
    // void Start()
    // {
        
    // }

    // // Update is called once per frame
    // void Update()
    // {
        
    // }
    public Animator animation_controller;
    public GameObject bear;
    public GameObject fox;
    public GameObject tiger;
    public GameObject fourth;
    private bool flag;
    void OnCollisionEnter(Collision collision){
        flag = GameObject.Find("Player").GetComponent<pickup>().flag;
        // Destroy(gameObject);
        animation_controller.SetBool("gothit",true);

    }
}
