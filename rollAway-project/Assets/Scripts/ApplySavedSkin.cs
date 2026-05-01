using UnityEngine;

public class ApplySavedSkin : MonoBehaviour
{
    public MeshRenderer ballRenderer;
    public SkinDatabase skinDb;
    void Awake()
    {
        if (skinDb != null)
        {
            int savedSkinIndex = PlayerPrefs.GetInt("SelectedSkin", 0);
            
            if (savedSkinIndex < skinDb.allSkins.Length)
            {
                ballRenderer.material = skinDb.allSkins[savedSkinIndex];
            }
        }
    }
}