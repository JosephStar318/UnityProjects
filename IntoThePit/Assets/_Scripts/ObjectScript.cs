using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectScript : MonoBehaviour
{
    private AudioSource audioSource;
    public AudioClip popSound;
    // Start is called before the first frame update
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Sack") || other.CompareTag("Pit"))
        {
            audioSource.PlayOneShot(popSound);
        }
    }
}
