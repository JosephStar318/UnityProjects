using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationToRagdoll : MonoBehaviour
{
    [SerializeField] Collider myCollider;
    [SerializeField] float respawnTime = 3f;
    Rigidbody[] rigidbodies;
    bool bIsRagdoll = false;


    void Start()
    {
        rigidbodies = GetComponentsInChildren<Rigidbody>();
        ToggleRagdoll(true);
    }

    private void ToggleRagdoll(bool bIsAnimating)
    {
        bIsRagdoll = !bIsAnimating;
        myCollider.enabled = bIsAnimating;
        foreach (Rigidbody ragdollBone in rigidbodies)
        {
            ragdollBone.isKinematic = bIsAnimating;
        }
        GetComponent<Animator>().enabled = bIsAnimating;

    }
   
    private void OnTriggerEnter(Collider col)
    {
        if (!bIsRagdoll && col.gameObject.CompareTag("Projectile"))
        {
            ToggleRagdoll(false);
            Invoke(nameof(GetBackUp), respawnTime);
        }
    }

    private void GetBackUp()
    {
        ToggleRagdoll(true);
    }

    void Update()
    {
        //ToggleRagdoll(true);
    }
}
