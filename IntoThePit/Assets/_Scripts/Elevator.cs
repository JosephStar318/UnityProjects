using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Elevator : MonoBehaviour
{
    public void ElevatorUp()
    {
        GetComponent<Animation>().Play();
    }
}
