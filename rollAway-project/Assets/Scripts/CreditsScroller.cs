using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CreditsScroller : MonoBehaviour
{
    [SerializeField] float normalSpeed = 80f;
    [SerializeField] float fastSpeed = 300f;
    [SerializeField] float endY = 1200f;
    [SerializeField] string nextScene = "MainMenu";
    [SerializeField] float fadeDuration = 1f;

    CanvasGroup canvasGroup;
    bool scrolling = false;

    IEnumerator Start()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        canvasGroup.alpha = 0;

        yield return new WaitForSeconds(0.5f);
        float t = 0;
        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            canvasGroup.alpha = t / fadeDuration;
            yield return null;
        }

        canvasGroup.alpha = 1;
        scrolling = true;
    }

    void Update()
    {
        if (!scrolling) return;

        float speed = UnityEngine.InputSystem.Keyboard.current.spaceKey.isPressed ? fastSpeed : normalSpeed;
        transform.Translate(Vector3.up * speed * Time.deltaTime);

        if (transform.localPosition.y >= endY)
        {
            scrolling = false;
            SceneTransition.Instance.LoadScene(nextScene);
        }
    }
}