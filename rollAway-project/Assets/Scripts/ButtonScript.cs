using UnityEngine;
using UnityEngine.UI;

public class ButtonScript : MonoBehaviour
{
    public string sceneToLoad;

    void Start()
    {
        GetComponent<Button>().onClick.AddListener(() => {
            SceneTransition.Instance.LoadScene(sceneToLoad);
        });
    }
}
