using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyCharacterController : MonoBehaviour
{
    #region Variables
    
    [Header("Object Dimensions")]
    public float maxHeight;
    public float minWidth;

    [Header("Speeds")]
    private float moveSpeed;
    public float walkSpeed;
    public float sprintSpeed;
    public float axisSpeed;
    public float rotationSpeed;
    public float strafeSpeed;

    [Header("Physics (Horizonal)")]
    public float mass;
    public float gravity;
    private float curGrav;
    
    private float jumpForce;
    public float jumpAcceleration;
    public float jumpVelocity;
    public float jumpHeight;

    [Header("Physics (Vertical)")]
    public float accelerationX;
    public float velocityX;
    public float distanceX;

    public float accelerationZ;
    public float velocityZ;
    public float distanceZ;

    public float groundFractionCoeff;
    public float airFractionCoeff;
    private float curCoeffX;
    private float curCoeffZ;

    [Header("External")]
    public Camera playerCamera;
    public float mouseSensivity;

    public enum CameraMode
    {
        FPS = 0,
        TPS = 1,
        ISO = 2
    }
    public CameraMode cameraMode;
    public LayerMask discludePlayer;

    [Header("Surface Control")]
    public Vector3 sensorLocal;
    public float surfaceSlideSpeed;
    public float slopeClimbSpeed;
    public float slopeDecendSpeed;

    public bool grounded;
    private float horizontalAxis;
    private float verticalAxis;
    private float xMouseAxis;
    private float yMouseAxis;

    private Vector3 moveVector;
    private Vector3 horizontalMove;
    private Vector3 currentRotation;
    private float maxYAngle = 80;
    private bool isRunning = false;
    internal float defaultFOV;

    private Vector3 dashDestination;
    private bool isDashing = false;
    private int dashCooldownCounter = 0;
    private Animator animator;
    [Header("Sounds & Effects")]
    [SerializeField] public AudioSource fallingAudio;
    [SerializeField] public AudioSource fallHitAudio;
    [SerializeField] public AudioSource dashAudio;
    [SerializeField] public AudioSource walkingAudio;
    [SerializeField] public AudioSource runningAudio;

    [SerializeField] public ParticleSystem fallParticles;

    #endregion

    private void Start()
    {
        moveSpeed = walkSpeed;
        defaultFOV = playerCamera.fieldOfView;
        animator = GetComponent<Animator>();

    }
    private void Update()
    {
        GetUserInput();
        PlayerEffectsHandler();
        MovementFOV();
        Jump();
    }

    private void Dash()
    {
        if(isDashing)
        {
            //play dash sound

            transform.position = Vector3.Lerp(transform.position, dashDestination, Time.fixedDeltaTime);
            playerCamera.transform.position = Vector3.Lerp(playerCamera.transform.position, dashDestination, Time.fixedDeltaTime);
            if(dashCooldownCounter < 50*4.5f)
            {
                isDashing = false;
            }
        }
        dashCooldownCounter--;
    }

    private void PlayerEffectsHandler()
    {
        if (!fallingAudio.isPlaying && (jumpVelocity < -6))
        {
            fallingAudio.Play(0);
        }
        else if (fallingAudio.isPlaying)
        {
            if (grounded == true)
            {
                fallingAudio.Stop();
                //fallHitAudio.volume = Mathf
                fallParticles.transform.position = transform.position - new Vector3(0, maxHeight / 2 - 0.5f, 0);
                fallParticles.Play();
                fallHitAudio.Play(0);
            }
        }
        if(moveVector.magnitude != 0 && grounded)
        {
            if (isRunning && !runningAudio.isPlaying)
            {
                animator.SetBool("isRunning", true);
                animator.SetBool("isWalking", false);
                animator.SetBool("isIdleing", false);
                if ((Mathf.Abs(velocityZ) > 0.5f || Mathf.Abs(velocityX) > 0.5f))
                {
                    walkingAudio.Stop();
                    runningAudio.Play();
                }
            }
            else if (!isRunning && !walkingAudio.isPlaying)
            {
                animator.SetBool("isWalking", true);
                animator.SetBool("isRunning", false);
                animator.SetBool("isIdleing", false);
                if ((Mathf.Abs(velocityZ) > 0.5f || Mathf.Abs(velocityX) > 0.5f))
                {
                    runningAudio.Stop();
                    walkingAudio.Play();
                }
            }
        }
        else
        {
            GetComponent<Animator>().SetBool("isWalking", false);
            GetComponent<Animator>().SetBool("isRunning", false);
            GetComponent<Animator>().SetBool("isIdleing", true);
            walkingAudio.Stop();
            runningAudio.Stop();
        }
        
    }

    private void MovementFOV()
    {
        if (isRunning && (horizontalMove.z != 0))
        {
            playerCamera.fieldOfView = Mathf.Lerp(playerCamera.fieldOfView, defaultFOV + 20, 0.2f);
        }
    }

    public void FixedUpdate()
    {
        Gravity();
        MouseLook();
        FinalMovement();
        SimpleMove();
        Dash();
    }
    private void OnDrawGizmos()
    {
        Vector3 boxSize = new Vector3(minWidth / 2, 0.1f, minWidth / 2);
        Vector3 boxPos = new Vector3(transform.position.x, transform.position.y - maxHeight / 2 + boxSize.y / 2, transform.position.z);
        if (!grounded)
            Gizmos.color = Color.red;
        else
            Gizmos.color = Color.green;
        Gizmos.DrawCube(boxPos, boxSize);
        Gizmos.color = Color.yellow;
        Vector3 l = transform.TransformPoint(sensorLocal);
        Gizmos.DrawSphere(l, 0.2f);
    }


    private void GetUserInput()
    {
        if(Input.GetKeyDown(KeyCode.F5))
        {
            if(cameraMode == CameraMode.FPS)
            {
                cameraMode = CameraMode.TPS;
            }
            else
            {
                cameraMode = CameraMode.FPS;
            }
        }
        if(Input.GetKey(KeyCode.LeftShift))
        {
            moveSpeed = sprintSpeed;
            isRunning = true;
        }
        if(Input.GetKeyDown(KeyCode.LeftControl))
        {
            if(dashCooldownCounter <= 0)
            {
                Vector3 dashMovement = transform.TransformDirection(new Vector3(moveVector.x, 0, moveVector.z));
                dashDestination = transform.position + dashMovement*100;
                dashAudio.Play();
                isDashing = true;
                dashCooldownCounter = 50 * 5;
                Debug.Log(dashMovement);
            }
        }
        if (Input.GetMouseButtonDown(0)) Cursor.lockState = CursorLockMode.Locked;

        horizontalAxis = Input.GetAxis("Horizontal");
        verticalAxis = Input.GetAxis("Vertical");
        xMouseAxis = Input.GetAxis("Mouse X");
        yMouseAxis = Input.GetAxis("Mouse Y");
        
        if (grounded)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                jumpForce = 50;
                curGrav = gravity;
                animator.SetBool("isGrounded", false);
                animator.SetBool("isJumping", true);
            }
        }
        else
        {
            curGrav = gravity;
        }
    }
    private void MouseLook()
    {
        if (cameraMode == CameraMode.TPS)
        {
            /*For TPS camera*/
            currentRotation.x += xMouseAxis * mouseSensivity;
            currentRotation.y -= yMouseAxis * mouseSensivity;
            currentRotation.x = Mathf.Repeat(currentRotation.x, 360);
            currentRotation.y = Mathf.Clamp(currentRotation.y, -maxYAngle, maxYAngle);

            playerCamera.transform.rotation = Quaternion.Euler(currentRotation.y, currentRotation.x, 0);
            //transform.rotation = Quaternion.Euler(0, currentRotation.x, 0);

            Vector3 p = playerCamera.transform.rotation * new Vector3(0,2, -8);
            playerCamera.transform.position = transform.position + p;
        }
        else if (cameraMode == CameraMode.FPS)
        {
            /*For FPS camera*/
            currentRotation.x += xMouseAxis * mouseSensivity;
            currentRotation.y -= yMouseAxis * mouseSensivity;
            currentRotation.x = Mathf.Repeat(currentRotation.x, 360);
            currentRotation.y = Mathf.Clamp(currentRotation.y, -maxYAngle, maxYAngle);
            playerCamera.transform.rotation = Quaternion.Euler(currentRotation.y, currentRotation.x, 0);
            transform.rotation = Quaternion.Euler(0, currentRotation.x, 0);

            Vector3 p = transform.rotation * new Vector3(0, maxHeight*0.8f, 0.5f);
            playerCamera.transform.position = transform.position + p;
        }
        else if(cameraMode == CameraMode.ISO)
        {
            /* For mouse hovering positioning with isometric Cameras*/
            Ray ray = playerCamera.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, Mathf.Infinity, discludePlayer))
            {
                if (hit.distance >= 2)
                {
                    Vector3 rPos = hit.point;
                    rPos.y = transform.position.y;
                    transform.LookAt(rPos);
                }
            }
        }
    }

    private void FinalMovement()
    {
        Debug.DrawRay(transform.position, transform.forward, Color.red, 0.2f);
        Vector3 movement = new Vector3(moveVector.x, moveVector.y, moveVector.z);
        movement = transform.TransformDirection(movement);
        transform.position += movement;
    }
    private void SimpleMove()
    {
        HorizontalPhysics();
        moveVector = CollisionSlopeCheck(new Vector3(horizontalMove.x, 0, horizontalMove.z) * axisSpeed) * moveSpeed * Time.fixedDeltaTime;



        //For walking down stairs
        Ray ray = new Ray(transform.position, Vector3.down);
        RaycastHit hit;
        if(Physics.Raycast(ray,out hit,maxHeight,discludePlayer))
        {
            if(hit.distance > maxHeight*0.5f)
            transform.position -= new Vector3(0, 0.01f, 0);
        }
    }

    private void HorizontalPhysics()
    {
        //horizontal and vertical axis is between -1 to 1. because of that there is a need for condition to calculate the movement
        if(horizontalAxis >= 0)
        {
            accelerationX = (horizontalAxis * 1000 - curCoeffX) * Time.fixedDeltaTime;
            velocityX += accelerationX * 100 * Time.fixedDeltaTime;
            velocityX = Mathf.Clamp(velocityX, 0, 2) * horizontalAxis;
        }
        else
        {
            accelerationX = (horizontalAxis * 1000 + curCoeffX) * Time.fixedDeltaTime;
            velocityX += accelerationX * 100 * Time.fixedDeltaTime;
            velocityX = Mathf.Clamp(velocityX, -2, 0) * -horizontalAxis;
        }
        if(verticalAxis >= 0)
        {
            accelerationZ = (verticalAxis * 1000 - curCoeffZ) * Time.fixedDeltaTime;
            velocityZ += accelerationZ * 100 * Time.fixedDeltaTime;
            velocityZ = Mathf.Clamp(velocityZ, 0, 2) * verticalAxis;
        }
        else
        {
            accelerationZ = (verticalAxis * 1000 + curCoeffX) * Time.fixedDeltaTime;
            velocityZ += accelerationZ * 100 * Time.fixedDeltaTime;
            velocityZ = Mathf.Clamp(velocityZ, -2, 0) * -verticalAxis;
        }

        if (grounded)
        {

            curCoeffX = groundFractionCoeff;
            curCoeffZ = groundFractionCoeff;
            //if there is no movement input from user make the player stop
            if (horizontalAxis != 0)
            {
                horizontalMove.x = velocityX * Time.fixedDeltaTime * 6;
            }
            else
            {
                horizontalMove.x = 0;
            }
            if (verticalAxis != 0)
            {
                horizontalMove.z = velocityZ * Time.fixedDeltaTime * 6;
            }
            else
            {
                horizontalMove.z = 0; 
            }
        }
        else
        {
            //if the player is on air slow down the player with air fraction and make the player lose its momentum
            if(horizontalAxis != 0)
            {
                curCoeffX = airFractionCoeff;
            }
            if(verticalAxis != 0)
            {
                curCoeffZ = airFractionCoeff;
            }
            horizontalMove.x = velocityX * Time.fixedDeltaTime * 6;
            horizontalMove.z = velocityZ * Time.fixedDeltaTime * 6;

        }

        if (horizontalMove.x == 0)
        {
            accelerationX = 0;
            velocityX = 0;
        }
        if (horizontalMove.z == 0)
        {
            //looking direction is parallel with the z component of the input movement vector
            //so the running property is effected in the movement direction
            isRunning = false;
            moveSpeed = walkSpeed;
            playerCamera.fieldOfView = Mathf.Lerp(playerCamera.fieldOfView, defaultFOV, 0.2f);
            accelerationZ = 0;
            velocityZ = 0;
        }
    }

    private Vector3 CollisionSlopeCheck(Vector3 dir)
    {
        Vector3 d = transform.TransformDirection(dir);
        Vector3 l = transform.TransformPoint(sensorLocal);

        Ray ray = new Ray(l, d);
        RaycastHit hit;
        if (Physics.SphereCast(ray, minWidth / 2, out hit, 10, discludePlayer)) 
        {

            Vector3 heading = hit.point - transform.position;
            Vector3 projectVector = Vector3.Project(heading, hit.normal);
            Debug.DrawRay(transform.position, projectVector, Color.blue, 0.2f);
            if (projectVector.magnitude <= minWidth * 0.6f)
            {
                
                //collision vector perpendicular to hit object in y axis
                Vector3 temp = Vector3.Cross(hit.normal, d);
                Debug.DrawRay(hit.point, temp * 20, Color.green, 0.2f);

                Vector3 myDirection = Vector3.Cross(temp, hit.normal);
                //Vector3 myDirection = dir - hit.normal;
                Debug.DrawRay(hit.point, myDirection * 20, Color.red, 0.2f);

                Vector3 dir2 = myDirection * surfaceSlideSpeed * moveSpeed * axisSpeed;

                RaycastHit wCheck = WallCheckDetails(dir2);
                //if there is an object on the movement direction based on the distance to the object move the player
                if (wCheck.transform != null)
                {
                    if (wCheck.distance > minWidth * 0.6f) dir2 *= 0.5f;
                    else dir2 *= 0;
                }
                transform.position += dir2;
                return Vector3.zero; //no final movement
            }
            else return dir;
        }
        else return dir;
    }

    private RaycastHit WallCheckDetails(Vector3 dir)
    {
        Vector3 l = transform.TransformPoint(sensorLocal);
        Ray ray = new Ray(l, dir);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 2f, discludePlayer))
        {
            return hit;
        }
        return hit;
    }

    private void Gravity()
    {
        Vector3 boxSize = new Vector3(minWidth / 2, 0.1f, minWidth / 2);
        Vector3 boxPos = new Vector3(transform.position.x, transform.position.y - maxHeight / 2 + boxSize.y / 2, transform.position.z);

        grounded = Physics.CheckBox(boxPos, boxSize,transform.rotation,discludePlayer);
        animator.SetBool("isGrounded", grounded);
        if (grounded && jumpVelocity < 0)
        {
            Ray ray = new Ray(transform.position, Vector3.down * 2);
            RaycastHit hit;
            if (Physics.SphereCast(ray,minWidth/2, out hit, Mathf.Infinity, discludePlayer))
            {
                if (hit.distance <= 2f)
                {
                    Debug.DrawRay(ray.origin, ray.direction * 20, Color.green, 0.2f);
                    Vector3 needed = new Vector3(transform.position.x, hit.point.y + maxHeight / 2, transform.position.z);
                    transform.position = needed;
                }
            }
            jumpAcceleration = 0;
            jumpVelocity = 0;
            jumpHeight = 0;
            moveVector.y = 0;
            curGrav = 0;
            animator.SetBool("isJumping", false);
        }
    }

    private void Jump()
    {
        jumpAcceleration = (jumpForce / mass) - curGrav;
        jumpVelocity += jumpAcceleration * Time.fixedDeltaTime;
        jumpHeight = jumpVelocity * Time.fixedDeltaTime + (jumpForce/mass) * Time.fixedDeltaTime * 2;
        moveVector.y = jumpHeight;
        if(jumpForce>0)
        {
            jumpForce -= 1;
        }
    }

}
