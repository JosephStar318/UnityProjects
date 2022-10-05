using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PitScript : MonoBehaviour
{
    public static event Action OnLevelPassed;

    public GameObject scoreText;

    private List<GameObject> objects = new List<GameObject>();
    private int targetScore = 1;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Object"))
        {
            objects.Add(other.gameObject);
            ChangeScoreText();
            if (objects.Count >= targetScore)
            {
                OnLevelPassed?.Invoke();
            }
        }
    }
    public void SetTargetScore(int newTarget)
    {
        targetScore = newTarget;
    }

    private void ChangeScoreText()
    {
        scoreText.GetComponent<TextMeshPro>().SetText(objects.Count + " / " + targetScore);
    }
}
