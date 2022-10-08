using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectScript : MonoBehaviour
{
    private AudioSource audioSource;
    private Rigidbody rb;
    public ParticleSystem destroyParticles;
    public AudioClip popSound;
    public float pushForce;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        rb = GetComponent<Rigidbody>();
        rb.isKinematic = true;
    }
    private void OnTriggerEnter(Collider other)
    {
        rb.isKinematic = false;

        if (other.CompareTag("Sack"))
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
    public void PushForward()
    {
        rb.AddForce(pushForce * Vector3.forward, ForceMode.VelocityChange);
    }
}
