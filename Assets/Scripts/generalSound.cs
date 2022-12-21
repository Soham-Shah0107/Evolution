using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class generalSound : MonoBehaviour
{
    // Start is called before the first frame update
    private AudioSource audioSource;
    public AudioClip clip;
    public float volume=0.5f;
    void Start()
    {
        GetComponent<AudioSource>().PlayOneShot(clip, 0.2f);
    }
}
