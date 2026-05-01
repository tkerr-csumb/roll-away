using UnityEngine;
using TMPro;

public class HUD : MonoBehaviour
{

    public static HUD Instance;
    public TMP_Text scoreText;
    public TMP_Text timeText;
    public TMP_Text winText;
    public float elapsedTime = 0f;

    
    void Awake()
    {
        Instance = this;
    }
    void Update()
    {
        elapsedTime += Time.deltaTime;

        int minutes = (int)(elapsedTime / 60f);
        int seconds = (int)(elapsedTime % 60f);
        timeText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }
}
