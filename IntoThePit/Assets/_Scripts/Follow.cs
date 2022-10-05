using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Follow : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject followObject;
    public Vector3 offset;
    private void LateUpdate()
    {
        Vector3 pos = new Vector3(0, followObject.transform.position.y + offset.y, followObject.transform.position.z + offset.z);
        transform.position = pos;
    }
}
