using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EndZone : MonoBehaviour
{
    human humanControl;
    bool hasCut;
    // Start is called before the first frame update
    void Start()
    {
        humanControl= GameObject.Find("Human").GetComponent<human>(); 
        
    }

    // Update is called once per frame
    // void Update()
    // {
        
    // }
    private void OnTriggerEnter(Collider other)
    {
        ////////////////////////////////////////////////
        // WRITE CODE HERE:
        // if Claire reaches this platform, make it green, make "has_won" true in Claire.cs / see Claire.cs for what to do here
        ////////////////////////////////////////////////
        hasCut = humanControl.hasCut;
        if(other.gameObject.name=="Human" && hasCut){
            SceneManager.LoadScene("Won");
        }

    }
}
