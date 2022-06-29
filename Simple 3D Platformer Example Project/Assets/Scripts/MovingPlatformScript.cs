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
    public float rotationRadius;
    public bool loopMovement;
    private int direction = 1;
    private Vector3 moveVector;
    private Vector3 startPosition;
    private Vector3 rotationOrigin;
    private Vector3 rotationDirection;
    private GameObject player;
    // Start is called before the first frame update
    void Start()
    {
        startPosition = transform.position;
        player = GameObject.Find("Player");
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
            rotationOrigin = transform.TransformPoint(new Vector3(0, rotationRadius, 0));
            //rotationDirection = 
            moveVector = rotationDirection * moveSpeed * direction * Time.fixedDeltaTime;
            transform.position += moveVector;
            //moveVector = 
        }
    }

    private void CheckPlayer()
    {
        if (Physics.CheckBox(transform.position, transform.localScale * 0.8f, transform.rotation, LayerMask.GetMask("Player")))
        {
            player.transform.position += moveVector;
            player.transform.position.Set(player.transform.position.x, transform.position.y + 1, player.transform.position.z);
            //todo get height of the player somehow
        }
    }
}
