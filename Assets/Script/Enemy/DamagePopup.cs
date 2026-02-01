using UnityEngine;
using TMPro;

public class DamagePopup : MonoBehaviour
{
    public float moveSpeed = 2f;
    public float totalVisibleTime = 0.5f;
    private float timer;

    private TextMeshPro textMesh;
    private Color originalColor;

    void Awake()
    {
        textMesh = GetComponent<TextMeshPro>();
        originalColor = textMesh.color;
    }

    void OnEnable()
    {
        timer = totalVisibleTime; 
        if (textMesh != null)
        {
            textMesh.color = originalColor; 
        }
    }

    void Update()
    {
        transform.position += Vector3.up * moveSpeed * Time.deltaTime;

        if (Camera.main != null)
        {
            transform.LookAt(transform.position + Camera.main.transform.forward);
        }

        timer -= Time.deltaTime;
        if (timer < 0)
        {
            float fadeSpeed = 5f;
            Color tempColor = textMesh.color;
            tempColor.a -= fadeSpeed * Time.deltaTime;
            textMesh.color = tempColor;
        }
    }
}