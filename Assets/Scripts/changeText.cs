using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class changeText : MonoBehaviour
{
   // public Text buttonText;
    public GameObject buttonpause;
    public GameObject buttonresume;
    // Start is called before the first frame update
    void Start()
    {
        buttonpause = GameObject.Find("pause");
        buttonresume = GameObject.Find("resume");
        buttonpause.SetActive(true);
        buttonresume.SetActive(false);
        Time.timeScale = 1;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void onClickChangeScenePause(){

        if(Time.timeScale == 1){
            Time.timeScale = 0;
            buttonpause.SetActive(false);
            buttonresume.SetActive(true);
        }
        else{
            Time.timeScale = 1;
            buttonpause.SetActive(true);
            buttonresume.SetActive(false);
        }


    }
}
