using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class CollectorScript : MonoBehaviour
{
    public static event Action OnFinished;
    public float speed;
    public float xLimit;
    public LayerMask discludeCollector;
    private Rigidbody rb;
    private bool moveAllowed;
    private bool isColliding;
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
        if (other.CompareTag("Finish") && !isColliding)
        {
            OnFinished?.Invoke();
            Destroy(other.gameObject);
            isColliding = true;
            StartCoroutine(WaitTrigger());
        }
    }
    private IEnumerator WaitTrigger()
    {
        yield return new WaitForEndOfFrame();
        isColliding = false;
    }

    private void HorizontalMove()
    {
        //if there player is touching the screen
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            Ray touchRay = Camera.main.ScreenPointToRay(touch.position);
            RaycastHit hit;
            Physics.Raycast(touchRay, out hit, discludeCollector);

            if (touch.phase == TouchPhase.Began)
            {
                moveAllowed = true;
            }
            if (touch.phase == TouchPhase.Moved)
            {
                if (moveAllowed)
                {
                    if (hit.point.x >= xLimit)
                    {
                        rb.position = new Vector3(xLimit, rb.position.y, rb.position.z);
                    }
                    else if (hit.point.x <= -xLimit)
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
