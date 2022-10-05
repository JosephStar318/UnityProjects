using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class CollectorScript : MonoBehaviour
{
    public static event Action<GameObject> OnFinished;
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
        if (GameManager.Instance.isFinished == false)
        {
            HorizontalMove();
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Finish"))
        {
            OnFinished?.Invoke(other.transform.parent.gameObject);
            GetComponentInChildren<Animator>().CrossFade("Flip", 0);
            Destroy(other.gameObject);
        }
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
