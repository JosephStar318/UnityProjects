using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPusher : MonoBehaviour
{
    private List<GameObject> objects = new List<GameObject>();

    private void Update()
    {
        //when hit the and point
        //if()
        //{
        //    objects.ForEach(obj => obj.GetComponent<Rigidbody>().AddForce(Vector3.forward * 100));
        //}
        //objects.Clear();

    }
    private void OnTriggerEnter(Collider other)
    {
        objects.Add(other.gameObject);
    }
}
