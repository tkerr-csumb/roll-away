using UnityEngine;

public class ScriptBackground : MonoBehaviour
{
    public float xSpeed = 0.001f;
    public float ySpeed = 0.001f;
    private Material mat;

    void Start()
    {
        mat = GetComponent<Renderer>().material;
    }

    void Update()
    {
        Vector2 offset = new Vector2(Time.time * xSpeed, Time.time * ySpeed);
        
        // Apply it to the Base Map
        mat.SetTextureOffset("_BaseMap", offset);
    }
}