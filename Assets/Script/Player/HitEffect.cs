using UnityEngine;
using UnityEngine.UI;
using System.Collections;
public class HitEffect : MonoBehaviour
{
    private Image img;
    public float fadeDuration = 3f;

    void Start()
    {
        img = GetComponent<Image>();
        if (img != null)
        {
            Color c = img.color;
            c.a = 0f;
            img.color = c;
        }
    }

    void Update()
    {
        if (img != null && img.color.a > 0)
        {
            Color c = img.color;
            c.a -= (1.0f / fadeDuration) * Time.deltaTime;
            if (c.a < 0) c.a = 0;

            img.color = c;
        }
    }

    public void ShowFlash()
    {
        if (img != null)
        {
            Color c = img.color;
            c.a = 0.6f;
            img.color = c;
        }
    }
}
