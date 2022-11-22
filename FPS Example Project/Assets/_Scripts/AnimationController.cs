using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationController : MonoBehaviour
{
    private Animator animator;
    private MyCharachterController characterCtrl;
    private int horizontalHash;
    private int verticalHash;

    private float horizontalSpeed;
    private float verticalSpeed;

    [SerializeField] private float speedAcceleration = 1;
    private void Start()
    {
        animator = GetComponent<Animator>();
        characterCtrl = GetComponent<MyCharachterController>();
        verticalHash    = Animator.StringToHash("Vertical");
        horizontalHash  = Animator.StringToHash("Horizontal");

    }

    private void Update()
    {
        if(characterCtrl.moveVector.x > 0)
        {
            horizontalSpeed += Time.deltaTime * speedAcceleration;
        }
        else if (characterCtrl.moveVector.x < 0)
        {
            horizontalSpeed -= Time.deltaTime * speedAcceleration * 2;
        }
        else
        {
            horizontalSpeed = Mathf.Lerp(horizontalSpeed, 0, Time.deltaTime * speedAcceleration * 2);
        }
        if (characterCtrl.moveVector.z > 0)
        {
            verticalSpeed += Time.deltaTime * speedAcceleration;
        }
        else if (characterCtrl.moveVector.z < 0)
        {
            verticalSpeed -= Time.deltaTime * speedAcceleration * 2;
        }
        else
        {
            verticalSpeed = Mathf.Lerp(verticalSpeed, 0, Time.deltaTime * speedAcceleration * 2);
        }
        horizontalSpeed = Mathf.Clamp(horizontalSpeed, -1, 1);
        verticalSpeed = Mathf.Clamp(verticalSpeed, -1, 1);

        animator.SetFloat(verticalHash, verticalSpeed);
        animator.SetFloat(horizontalHash, horizontalSpeed);

    }

}
