using UnityEngine;

public class MatrixWorldTrigger : MonoBehaviour
{
    public GameObject matrixSphere; 
    public AudioSource glitchSound; 

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            RevealTruth();
        }
    }

    void RevealTruth()
    {
        if (matrixSphere != null)
        {
            matrixSphere.SetActive(true);
        }

        RenderSettings.ambientMode = UnityEngine.Rendering.AmbientMode.Flat;
        RenderSettings.ambientLight = Color.green;

        if (glitchSound != null) glitchSound.Play();

        Debug.Log("Unauthorized access.");
    }
}