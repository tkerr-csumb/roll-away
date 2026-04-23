using UnityEngine;

[CreateAssetMenu(fileName = "MovementProfileSO", menuName = "Scriptable Objects/MovementProfileSO")]
public class MovementProfileSO : ScriptableObject
{
    public float speedMultiplier = 1f;
    public float drag = 1f;
    public float steeringMultiplier = 1f;
    public float reverseControlMultiplier = 1f;
    // public float gravityMultiplier = 1f;
}
