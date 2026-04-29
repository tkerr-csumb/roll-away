using UnityEngine;

[CreateAssetMenu(fileName = "SkinDatabase", menuName = "Scriptable Objects/SkinDatabase")]
public class SkinDatabase : ScriptableObject
{
    public Material[] allSkins;
}