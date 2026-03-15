using UnityEngine;
using TMPro;

public class LevelCompletedTime : MonoBehaviour
{
    public TMP_Text timeText;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        timeText.text = "Time: " + LevelManager.timer;
    }
}
