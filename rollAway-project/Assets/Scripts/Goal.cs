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
        yield return new WaitForSecondsRealtime(3f);
        Time.timeScale = 1f;
        SceneTransition.Instance.LoadScene("Credits");
    }
}