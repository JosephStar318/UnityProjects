using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Move : MonoBehaviour
{
    public float speed;
    public float xLimit;
    private Rigidbody rb;
    private bool moveAllowed;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        //VerticalMove();
        HorizontalMove();
    }

    private void VerticalMove()
    {
        //rb.velocity = transform.forward * speed;
        rb.MovePosition(new Vector3(rb.position.x, rb.position.y, rb.position.z + speed));

    }

    private void HorizontalMove()
    {
        //if there player is touching the screen
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            Ray touchRay = Camera.main.ScreenPointToRay(touch.position);
            RaycastHit hit;
            Physics.Raycast(touchRay, out hit);

            if (touch.phase == TouchPhase.Began)
            {
                moveAllowed = true;
            }
            if (touch.phase == TouchPhase.Moved)
            {
                if (moveAllowed)
                {
                    if (rb.position.x > xLimit)
                    {
                        rb.position = new Vector3(xLimit, rb.position.y, rb.position.z);
                    }
                    else if (rb.position.x < -xLimit)
                    {
                        rb.position = new Vector3(-xLimit, rb.position.y, rb.position.z);
                    }
                    else
                    {
                        rb.MovePosition(new Vector3(hit.point.x, rb.position.y, rb.position.z));
                    }
                }
            }
            if (touch.phase == TouchPhase.Ended)
            {
                moveAllowed = false;
            }
        }
    }
}
