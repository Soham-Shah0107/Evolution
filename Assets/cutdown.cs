using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cutdown : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
         humanControl= GameObject.Find("Human").GetComponent<human>(); 
    }

    // // Update is called once per frame
    // void Update()
    // {
        
    // }
    human humanControl;
    public Animator animation_controller;
    public GameObject player;
    public GameObject Axe;
    public GameObject bear;
    public GameObject fox;
    public GameObject tiger;
    public GameObject fourth;
    public GameObject firewood;
    bool hasCut;
    private bool flag;
    void Update(){
        flag = Axe.GetComponent<pickup>().flag;
        // Destroy(gameObject)
        // IEnumerator waiter(){
        //     yield return new WaitForSeconds(2);
        //     animation_controller.SetBool("gothit",true);
        //     bear.SetActive(true);
        //     fox.SetActive(true);
        //     tiger.SetActive(true);
        //     fourth.SetActive(true);
        // }
        if(flag == null){
            flag = false;
        }
        if(flag && Input.GetKeyDown(KeyCode.S) && (Vector3.Distance(transform.position, player.transform.position) < 2f)){
            animation_controller.SetBool("gothit",true);
            if(animation_controller.GetBool("gothit")){
                // Destroy(Axe);
                Axe.SetActive(false);
                // Debug.Log(animation_controller.IsPlaying("MiningLoop"));
                StartCoroutine(waiter());
                bear.SetActive(false);
                humanControl.hasCut = true;
                fox.SetActive(false);
                GameObject txt = GameObject.Find("Human");
        //        txt.GetComponent<human>().text_box.GetComponent<Text>().text = "You have the wood!!";
                // tiger.SetActive(false);
                // fourth.SetActive(false);
            }
        }

    }
    IEnumerator waiter(){
            yield return new WaitForSeconds(3);
            animation_controller.SetBool("gothit",false);
            bear.SetActive(true);
            fox.SetActive(true);
            gameObject.SetActive(false);
            yield return new WaitForSeconds(1);
            firewood.SetActive(true);
            // tiger.SetActive(true);
            // fourth.SetActive(true);
        }
}
