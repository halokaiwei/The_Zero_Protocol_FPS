using UnityEngine;

public class Skyflow : MonoBehaviour
{
    public float scrollSpeed = 0.5f;
    private Material mat;

    void Start()
    {
        mat = GetComponent<Renderer>().material;
    }

    void Update()
    {
        float offset = Time.time * scrollSpeed;
        mat.SetTextureOffset("_MainTex", new Vector2(0, -offset));
    }
}