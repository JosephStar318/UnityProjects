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
    private void PushObjects()
    {
        objects.RemoveAll(item => item == null);
        foreach (GameObject obj in objects)
        {
            obj.GetComponent<ObjectScript>().PushForward();
        }
    }
    private void Update()
    {
        if(GameManager.Instance.isLevelPassed)
        {
            foreach (GameObject obj in objects)
            {
                Destroy(obj);
            }
            objects.Clear();
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Object"))
        {
            objects.Add(other.gameObject);
        }
    }
    private void OnTriggerExit(Collider other)
    {
        objects.Remove(other.gameObject);
    }
}
