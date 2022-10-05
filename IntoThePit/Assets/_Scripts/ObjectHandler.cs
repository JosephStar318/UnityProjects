using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectHandler : MonoBehaviour
{
    private List<GameObject> objects = new List<GameObject>();

    private void OnEnable()
    {
        CollectorScript.OnFinished += PushObjects;
    }
    private void OnDisable()
    {
        CollectorScript.OnFinished -= PushObjects;
    }
    private void PushObjects(GameObject none)
    {
        objects.ForEach(obj => obj.GetComponent<Rigidbody>().AddForce(Vector3.forward * 200, ForceMode.Impulse));
        StartCoroutine(DestroyObjects());
    }

    private IEnumerator DestroyObjects()
    {
        yield return new WaitForSeconds(3);
        foreach (GameObject obj in objects)
        {
            Destroy(obj);
        }
        objects.Clear();
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Object"))
        {
            objects.Add(other.gameObject);
        }
    }
}
