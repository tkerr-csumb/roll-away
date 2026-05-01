using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Goal : MonoBehaviour
{
    public TextMeshProUGUI winText;
    public SceneTransition sceneTransition;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            winText.gameObject.SetActive(true);
            StartCoroutine(LoadCredits());
        }
    }

    IEnumerator LoadCredits()
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
        if (collector != null)
        {
            int currentCount = collector.GetCount();
            int total = collector.GetTargetCount();
            string pickupKey = "BestPickups_" + sceneName;
            string totalKey = "TotalPickups_" + sceneName;
            if (currentCount > PlayerPrefs.GetInt(pickupKey, 0))
                PlayerPrefs.SetInt(pickupKey, currentCount);
            PlayerPrefs.SetInt(totalKey, total);
        }

        PlayerPrefs.Save();

        yield return new WaitForSecondsRealtime(3f);
        Time.timeScale = 1f;
        SceneTransition.Instance.LoadScene("Credits");
    }
}