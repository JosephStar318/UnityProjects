using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PitScript : MonoBehaviour
{
    public static event Action OnLevelPassed;

    public GameObject scoreText;

    private int targetScore = 4;
    private int score;
    private bool levelPassed;
    private void Start()
    {
        ChangeScoreText();
    }
    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Object"))
        {
            //to prevent multiple triggers since blue object has box collider and capsule collider
            if(other.GetType() == typeof(BoxCollider) || other.GetType() == typeof(SphereCollider))
            {
                GameManager.Instance.objectTimer = 0;
                score += other.GetComponent<ObjectScript>().point;
                ChangeScoreText();
                if (score >= targetScore && !levelPassed)
                {
                    levelPassed = true;
                    OnLevelPassed?.Invoke();
                    transform.GetComponentInChildren<Elevator>().ElevatorUp();
                }
            }
        }
    }
   
    public void SetTargetScore(int newTarget)
    {
        targetScore = newTarget;
        ChangeScoreText();
    }

    private void ChangeScoreText()
    {
        scoreText.GetComponent<TextMeshPro>().SetText(score + " / " + targetScore);
    }
}
