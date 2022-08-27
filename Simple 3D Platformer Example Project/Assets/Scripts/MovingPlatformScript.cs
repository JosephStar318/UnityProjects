using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingPlatformScript : MonoBehaviour
{

    [Header("Physics")]

    public float moveSpeed;
    public Vector3 moveDirection;
    public enum MoveStyle
    {
        Lineer = 0,
        Circle,
        
    }
    public MoveStyle moveStyle;
    public float moveDistance;
    private float rotationAngle;
    public float rotationRadius;
    public bool loopMovement;
    private int direction = 1;
    private Vector3 moveVector;
    private Vector3 startPosition;
    private Vector3 rotationOrigin;
    private Vector3 rotationDirection;
    private Vector3 previousPosition;
    private GameObject player;
    // Start is called before the first frame update
    void Start()
    {
        startPosition = transform.position;
        player = GameObject.FindGameObjectWithTag("Player");
    }

    private void FixedUpdate()
    {
        MovePlatform();
        CheckPlayer();
    }

    private void MovePlatform()
    {
        if (moveStyle == MoveStyle.Lineer)
        {
            if (((startPosition - transform.position).magnitude >= moveDistance) && (direction == 1)) direction = -1;
            else if (((startPosition - transform.position).magnitude >= moveDistance) && (direction == -1)) direction = 1;

            moveVector = moveDirection * moveSpeed * direction * Time.fixedDeltaTime;
            transform.position += moveVector;
        }
        else if (moveStyle == MoveStyle.Circle)
        {
            rotationAngle += moveSpeed * Time.fixedDeltaTime;

            moveVector = new Vector3(Mathf.Sin(rotationAngle), Mathf.Cos(rotationAngle)) * rotationRadius;
            previousPosition = transform.position;
            transform.position = startPosition + moveVector;
        }
    }

    private void CheckPlayer()
    {
        if (Physics.CheckBox(transform.position, transform.localScale * 0.8f, transform.rotation, LayerMask.GetMask("Player")))
        {
            if(moveStyle == MoveStyle.Lineer)
            {
                player.transform.position += moveVector;
                player.transform.position.Set(player.transform.position.x, transform.position.y + player.GetComponent<MyCharacterController>().maxHeight, player.transform.position.z);
                //todo get height of the player somehow
            }
            else if (moveStyle == MoveStyle.Circle)
            {
                player.transform.position += transform.position - previousPosition;
            }

        }
    }
}
