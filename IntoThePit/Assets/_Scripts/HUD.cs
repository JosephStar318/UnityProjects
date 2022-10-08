using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HUD : MonoBehaviour
{
    public TextMeshProUGUI currentLevelBox;
    public TextMeshProUGUI nextLevelBox;

    private void Start()
    {
        UpdateLevelIndicator();
    }
    public void UpdateLevelIndicator()
    {
        currentLevelBox.SetText(GameManager.Instance.CurrentLevelCount.ToString());
        nextLevelBox.SetText((GameManager.Instance.CurrentLevelCount + 1).ToString());
    }
}
