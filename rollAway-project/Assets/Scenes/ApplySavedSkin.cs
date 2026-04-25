using UnityEngine;

public class ApplySavedSkin : MonoBehaviour
{
    public MeshRenderer ballRenderer;
    public Material[] skins; // Same order as skinmanager

    void Start()
    {
        int savedSkinIndex = PlayerPrefs.GetInt("SelectedSkin", 0);

        if (savedSkinIndex < skins.Length)
        {
            ballRenderer.material = skins[savedSkinIndex];
        }
    }
}