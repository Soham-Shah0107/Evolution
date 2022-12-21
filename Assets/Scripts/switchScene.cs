using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class switchScene : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }
    public void onClickChangeScene(){
        Debug.Log("Entered");
        SceneManager.LoadScene("stage_1");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
