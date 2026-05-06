using System.Collections;
using UnityEngine;

public class Goal : MonoBehaviour
{
    [SerializeField] private WinScreen winScreen;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            StartCoroutine(HandleGoalReached());
        }
    }

    IEnumerator HandleGoalReached()
    {
        Time.timeScale = 0f;

        string sceneName = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
        float currentTime = HUD.Instance.elapsedTime;
        string timeKey = "BestTime_" + sceneName;
        float bestTime = PlayerPrefs.GetFloat(timeKey, float.MaxValue);
        if (currentTime < bestTime)
        {
            PlayerPrefs.SetFloat(timeKey, currentTime);
        }
        PlayerCollector collector = FindFirstObjectByType<PlayerCollector>();
        int currentCount = 0;
        int total = 0;
        if (collector != null)
        {
            currentCount = collector.GetCount();
            total = collector.GetTargetCount();
            string pickupKey = "BestPickups_" + sceneName;
            string totalKey = "TotalPickups_" + sceneName;
            if (currentCount > PlayerPrefs.GetInt(pickupKey, 0))
                PlayerPrefs.SetInt(pickupKey, currentCount);
            PlayerPrefs.SetInt(totalKey, total);
        }

        PlayerPrefs.Save();

        yield return new WaitForSecondsRealtime(1f);
        if (winScreen != null)
            winScreen.Show(currentTime, currentCount, total);
    }
}