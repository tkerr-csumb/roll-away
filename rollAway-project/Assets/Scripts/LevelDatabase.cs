using UnityEngine;

[CreateAssetMenu(fileName = "LevelDatabase", menuName = "Scriptable Objects/LevelDatabase")]
public class LevelDatabase : ScriptableObject
{
    [System.Serializable]
    public struct LevelInfo
    {
        public string sceneName;     // Scene you want to load
        public string displayName;   // Display level name
        public Sprite previewImage;  // Thumbnail for the level
        public float  parTime;
    }

    public LevelInfo[] allLevels;
}