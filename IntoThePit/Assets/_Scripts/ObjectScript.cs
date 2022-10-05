using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectScript : MonoBehaviour
{
    private AudioSource audioSource;
    public ParticleSystem destroyParticles;
    public AudioClip popSound;
    // Start is called before the first frame update
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }
    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Sack"))
        {
            audioSource.PlayOneShot(popSound);
        }
        else if(other.CompareTag("Pit"))
        {
            audioSource.PlayOneShot(popSound);
            Instantiate(destroyParticles, transform);
            Destroy(gameObject,0.2f);
        }
    }
}
