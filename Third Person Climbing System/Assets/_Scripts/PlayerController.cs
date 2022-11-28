using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Animator), typeof(Rigidbody), typeof(Collider))]
[RequireComponent(typeof(PlayerInput))]
public class PlayerController : MonoBehaviour
{
    [SerializeField] private Camera mainCamera;

    [Header("Physics")]
    [SerializeField] private float walkSpeed = 1;
    [SerializeField] private float sprintSpeed = 2;

    [SerializeField] private bool isGrounded = true;
    [SerializeField] private bool sprinting = false;

    #region Foot IK
    [Header("Foot IK Options")]
    [SerializeField] private bool footIKEnable;
    [SerializeField] private bool showDebugRays = true;

    [SerializeField] private LayerMask walkableLayers;
    [Range(0, 1f)] [SerializeField] private float raycastOffset = 1.14f;
    [Range(0, 1f)] [SerializeField] private float raycastDistance = 1.5f;
    [Range(0, 1f)] [SerializeField] private float pelvisUpAndDownSpeed = 0.28f;
    [Range(0, 1f)] [SerializeField] private float feetToIKPositionSpeed = 0.5f;
    [SerializeField] private float pelvisOffset = 0f;
    private Vector3 rightFootPosition, leftFootPosition, rightFootIKPos, leftFootIKPos;
    private Quaternion rightFootIKRotation, leftFootIKRotation;
    private float lastPelvisPositionY, lastRightFootPositionY, lastLeftFootPositionY;
    #endregion

    #region Animator Related
    private Animator animator;

    private int speedHash;
    private int speedXHash;
    private int speedZHash;
    private int jumpTriggerHash;
    private int groundedHash;

    private Vector3 moveVector;

    private float targetX;
    private float targetZ;
    private float speedFactor;
    #endregion

    void Start()
    {
        animator = GetComponent<Animator>();

        speedHash               = Animator.StringToHash("Speed");
        speedXHash              = Animator.StringToHash("SpeedX");
        speedZHash              = Animator.StringToHash("SpeedZ");
        jumpTriggerHash         = Animator.StringToHash("Jump");
        groundedHash            = Animator.StringToHash("Grounded");

        Cursor.lockState = CursorLockMode.Confined;
        Cursor.visible = false;
    }

    void LateUpdate()
    {
        Cursor.visible = false;
        MovePlayer();
        CheckGrounding();
        RotatePlayer();
    }

    private void RotatePlayer()
    {
        if (moveVector.magnitude > 0.5f)
        {
            Vector3 rotationVector = transform.position - mainCamera.transform.position;
            rotationVector.y = 0;
            transform.forward = Vector3.Lerp(transform.forward, rotationVector, Time.deltaTime);
        }
    }

    private void OnDrawGizmos()
    {
        Ray ray = new Ray(transform.position, Vector3.down * 10);
        Gizmos.color = Color.red;
        Gizmos.DrawRay(ray);
    }
    private void CheckGrounding()
    {
        if(Physics.Raycast(transform.position + Vector3.up ,Vector3.down, out RaycastHit hit, 5f, walkableLayers))
        {
            if(hit.distance < 2)
            {
                isGrounded = true;
                animator.SetBool(groundedHash, true);
            }
            else
            {
                isGrounded = false;
                animator.SetBool(groundedHash, false);
            }
        }
        else
        {
            isGrounded = false;
            animator.SetBool(groundedHash, false);
        }
    }

    private void MovePlayer()
    {
        if(sprinting)
        {
            speedFactor = Mathf.Lerp(speedFactor, sprintSpeed, Time.deltaTime * 2);
        }
        else
        {
            speedFactor = Mathf.Lerp(speedFactor, walkSpeed, Time.deltaTime * 2);
        }
        moveVector.x = Mathf.Lerp(moveVector.x, targetX * speedFactor, Time.deltaTime * 2);
        moveVector.z = Mathf.Lerp(moveVector.z, targetZ * speedFactor, Time.deltaTime * 2);

        animator.SetFloat(speedHash, speedFactor);
        animator.SetFloat(speedXHash, moveVector.x);
        animator.SetFloat(speedZHash, moveVector.z);
    }

    private void OnMove(InputValue value)
    {
        targetX = value.Get<Vector2>().x;
        targetZ = value.Get<Vector2>().y;
     
    }
    private void OnJump(InputValue value)
    {
        if(isGrounded)
        {
            Debug.Log($"Jump {value.isPressed}");
            animator.SetTrigger(jumpTriggerHash);
            animator.SetBool(groundedHash, false);
            isGrounded = false;
        }
    }
    private void OnSprint(InputValue value)
    {
        sprinting = value.isPressed;
    }
    private void FixedUpdate()
    {
        if (!footIKEnable) return;
        if (animator == null) return;

        AdjustFeetTarget(ref rightFootPosition, HumanBodyBones.RightFoot);
        AdjustFeetTarget(ref leftFootPosition, HumanBodyBones.LeftFoot);
        
        FeetPositionSolver(rightFootPosition, ref rightFootIKPos, ref rightFootIKRotation);
        FeetPositionSolver(leftFootPosition, ref leftFootIKPos, ref leftFootIKRotation);

    }
    private void OnAnimatorIK(int layerIndex)
    {
        if (!footIKEnable) return;
        if (animator == null) return;

        MovePelvisHeight();

        //right foot
        animator.SetIKPositionWeight(AvatarIKGoal.RightFoot, 1);
        animator.SetIKRotationWeight(AvatarIKGoal.RightFoot, 1);

        MoveFeetToIkPoint(AvatarIKGoal.RightFoot, rightFootIKPos, rightFootIKRotation, ref lastRightFootPositionY);

        //left foot
        animator.SetIKPositionWeight(AvatarIKGoal.LeftFoot, 1);
        animator.SetIKRotationWeight(AvatarIKGoal.LeftFoot, 1);

        MoveFeetToIkPoint(AvatarIKGoal.LeftFoot, leftFootIKPos, leftFootIKRotation, ref lastLeftFootPositionY);
    }
   
    void MoveFeetToIkPoint (AvatarIKGoal foot, Vector3 positionIKHolder, Quaternion rotationIKHolder, ref float lasfFootPositionY)
    {
        Vector3 targetIKPos = animator.GetIKPosition(foot);
        if(positionIKHolder != Vector3.zero)
        {
            targetIKPos = transform.InverseTransformPoint(targetIKPos);
            positionIKHolder = transform.InverseTransformPoint(positionIKHolder);

            float y = Mathf.Lerp(lasfFootPositionY, positionIKHolder.y, feetToIKPositionSpeed);
            targetIKPos.y += y;

            lasfFootPositionY = y;
            targetIKPos = transform.TransformPoint(targetIKPos);

            animator.SetIKRotation(foot, rotationIKHolder);
        }
        animator.SetIKPosition(foot, targetIKPos);
    }
    private void MovePelvisHeight()
    {
        rightFootIKPos = animator.GetIKPosition(AvatarIKGoal.RightFoot);
        leftFootIKPos = animator.GetIKPosition(AvatarIKGoal.LeftFoot);

        if(rightFootIKPos == Vector3.zero || leftFootIKPos == Vector3.zero || lastPelvisPositionY == 0)
        {
            lastPelvisPositionY = animator.bodyPosition.y;
            return;
        }
        float lOffsetPos = leftFootIKPos.y - transform.position.y;
        float rOffsetPos = rightFootIKPos.y - transform.position.y;
        float totalOffset = lOffsetPos < rOffsetPos ? lOffsetPos : rOffsetPos;
        Vector3 newPelvisPos = animator.bodyPosition + Vector3.up * totalOffset;

        newPelvisPos.y = Mathf.Lerp(lastPelvisPositionY, newPelvisPos.y, pelvisUpAndDownSpeed);
        animator.bodyPosition = newPelvisPos;
        lastPelvisPositionY = animator.bodyPosition.y;
    }

    private void MoveFoot(AvatarIKGoal foot, float footIKWeight)
    {
        animator.SetIKPositionWeight(foot, footIKWeight);
        animator.SetIKRotationWeight(foot, footIKWeight);
        RaycastHit hit;
        Ray ray = new Ray(animator.GetIKPosition(foot) + Vector3.up * 0.5f, Vector3.down);
        if (Physics.Raycast(ray, out hit, 1f, walkableLayers))
        {
            float clampedDistance = hit.distance.RemapClamped(0, 0.5f + raycastOffset, 0, 1);
            footIKWeight = 1 - clampedDistance;

            Debug.Log($"left foot hit {footIKWeight}");
            Vector3 footPos = hit.point;
            footPos.y += raycastOffset;
            animator.SetIKPosition(foot, footPos);
            Vector3 forward = Vector3.ProjectOnPlane(transform.forward, hit.normal);
            animator.SetIKRotation(foot, Quaternion.LookRotation(forward, hit.normal));
        }
    }
    private void FeetPositionSolver(Vector3 fromSkyPosition, ref Vector3 feetIKPositions, ref Quaternion feetIKRotations)
    {
        if (showDebugRays) Debug.DrawLine(fromSkyPosition, fromSkyPosition + Vector3.down * (raycastDistance + raycastOffset), Color.red);

        if(Physics.Raycast(fromSkyPosition, Vector3.down, out RaycastHit hit, raycastDistance + raycastOffset, walkableLayers))
        {
            feetIKPositions = fromSkyPosition;
            feetIKPositions.y = hit.point.y + pelvisOffset;
            Vector3 forward = Vector3.ProjectOnPlane(transform.forward, hit.normal);
            feetIKRotations = Quaternion.LookRotation(forward, hit.normal);
            return;
        }
        feetIKPositions = Vector3.zero;
    }
    private void AdjustFeetTarget(ref Vector3 feetPositions, HumanBodyBones foot)
    {
        feetPositions = animator.GetBoneTransform(foot).position;
        feetPositions.y = transform.position.y + raycastOffset;
    }
}
public static class Extensions
{
    public static float RemapClamped(this float aValue, float aIn1, float aIn2, float aOut1, float aOut2)
    {
        float t = (aValue - aIn1) / (aIn2 - aIn1);
        t = Mathf.Clamp01(t);
        return aOut1 + (aOut2 - aOut1) * t;
    }
}
