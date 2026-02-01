using UnityEngine;
using System.Collections;

public class GlitchCatMaterial : MonoBehaviour
{
    public SkinnedMeshRenderer catRenderer;

    public Material normalMaterial;
    public Material codeMaterial;

    public float minGlitchTime = 0.05f;
    public float maxGlitchTime = 0.2f;
    public float normalInterval = 3f;

    void Start()
    {
        if (catRenderer == null) catRenderer = GetComponentInChildren<SkinnedMeshRenderer>();

        StartCoroutine(MaterialGlitchLoop());
    }

    IEnumerator MaterialGlitchLoop()
    {
        while (true)
        {
            catRenderer.material = normalMaterial;
            yield return new WaitForSeconds(Random.Range(2f, normalInterval));

            catRenderer.material = codeMaterial;
            yield return new WaitForSeconds(Random.Range(minGlitchTime, maxGlitchTime));
        }
    }
}