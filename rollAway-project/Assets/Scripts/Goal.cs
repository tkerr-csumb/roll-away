using UnityEngine;
using TMPro;

public class Goal : MonoBehaviour
{
    

    public TextMeshProUGUI winText;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            winText.gameObject.SetActive(true);
            Time.timeScale = 0f;
        }
    }
}
