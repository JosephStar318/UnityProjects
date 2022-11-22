using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class MyCharachterController : MonoBehaviour
{
    public GameObject playerCamera;

    //physics
    private Rigidbody rb;
    private Vector3 currentRotation;

    //inputs
    public float mouseSensivity = 1;
    public float moveSpeed = 4;
    public float sprintSpeed = 6;
    public float maxYAngle = 80;

    private float horizontalAxis;
    private float verticalAxis;
    private float mouseX;
    private float mouseY;

    public bool isSprinting;
    public Vector3 moveVector;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        GetUserInput();
    }
    private void FixedUpdate()
    {
        Move();
    }
    private void LateUpdate()
    {
        Rotate();
    }

    private void Rotate()
    {
        currentRotation.x += mouseX * mouseSensivity;
        currentRotation.y -= mouseY * mouseSensivity;
        currentRotation.x = Mathf.Repeat(currentRotation.x, 360);
        currentRotation.y = Mathf.Clamp(currentRotation.y, -maxYAngle, maxYAngle);

        transform.rotation = Quaternion.Euler(0, currentRotation.x, 0);
        playerCamera.transform.rotation = Quaternion.Euler(currentRotation.y, currentRotation.x, 0);
    }

    private void Move()
    {
        if(isSprinting) transform.Translate(moveVector * Time.fixedDeltaTime * sprintSpeed);
        else transform.Translate(moveVector * Time.fixedDeltaTime * moveSpeed);
    }

    private void GetUserInput()
    {
        horizontalAxis = Input.GetAxis("Horizontal");
        verticalAxis = Input.GetAxis("Vertical");
        mouseX = Input.GetAxis("Mouse X");
        mouseY = Input.GetAxis("Mouse Y");
        isSprinting = Input.GetKey(KeyCode.LeftShift);

        moveVector = new Vector3(horizontalAxis, 0, verticalAxis);
    }
}
