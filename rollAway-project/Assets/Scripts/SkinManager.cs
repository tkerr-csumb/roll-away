using System.Collections;
using UnityEngine;
using TMPro;

public class SkinManager : MonoBehaviour
{
    [Header("References")]
    public MeshRenderer ballRenderer;   
    public Material[] skins;            
    public TextMeshProUGUI selectButtonText; 

    [Header("Animation Settings")]
    public float rollDuration = 0.6f;
    public float rollSpeed = 500f;
    
    private Vector3 centerPosition = new Vector3(-0.15f, 0.57f, 0.2f); 
    private Vector3 offScreenPosition = new Vector3(-12f, 0.57f, 0.2f); 

    private int currentSkinIndex = 0;

    void Start()
    {
        currentSkinIndex = PlayerPrefs.GetInt("SelectedSkin", 0);
        UpdatePreview(false);
    }

    public void NextSkin()
    {
        currentSkinIndex = (currentSkinIndex + 1) % skins.Length;
        UpdatePreview(true);
    }

    public void PreviousSkin()
    {
        currentSkinIndex--;
        if (currentSkinIndex < 0) currentSkinIndex = skins.Length - 1;
        UpdatePreview(true);
    }

    void UpdatePreview(bool animate)
    {
        if (skins.Length > 0)
        {
            ballRenderer.material = skins[currentSkinIndex];

            // Check if current matches saved
            int savedSkin = PlayerPrefs.GetInt("SelectedSkin", -1);
            selectButtonText.text = (currentSkinIndex == savedSkin) ? "SELECTED" : "SELECT";

            // Trigger the roll-in
            if (animate)
            {
                StopAllCoroutines();
                StartCoroutine(RollInAnimation());
            }
        }
    }

    IEnumerator RollInAnimation()
    {
        float elapsed = 0;
        
        // Move ball to starting point
        ballRenderer.transform.position = offScreenPosition;

        while (elapsed < rollDuration)
        {
            elapsed += Time.deltaTime;
            
            // This creates a smooth stop effect
            float percent = Mathf.Sin((elapsed / rollDuration) * Mathf.PI * 0.5f);
            
            // Position
            ballRenderer.transform.position = Vector3.Lerp(offScreenPosition, centerPosition, percent);
            
            // Rotation (Roll effect)
            ballRenderer.transform.Rotate(Vector3.forward, -rollSpeed * Time.deltaTime);
            
            yield return null;
        }

        // Snap to perfect center
        ballRenderer.transform.position = centerPosition;
    }
public void BackToMenu()
{
    if (SceneTransition.Instance != null)
    {
        SceneTransition.Instance.LoadScene("MainMenu"); 
    }
}
    public void SelectSkin()
    {
        PlayerPrefs.SetInt("SelectedSkin", currentSkinIndex);
        PlayerPrefs.Save();
        selectButtonText.text = "SELECTED";
    }
}