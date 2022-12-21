using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bush1 : MonoBehaviour
{
    // Start is called before the first frame update
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.name == "human")
        {
            // level. = true;
            //Implement health
            //
            Destroy(gameObject);
        }
    }
}
