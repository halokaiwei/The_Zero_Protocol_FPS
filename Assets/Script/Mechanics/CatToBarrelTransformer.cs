using UnityEngine;
using System.Collections;
using UnityEngine.UI; // For glitch effect

public class CatToBarrelTransformer : MonoBehaviour
{
    [Header("References")]
    public GameObject catModel;    
    public GameObject explosiveBarrelPrefab;
    public DialogueManager dialogueManager;

    [Header("Transformation Settings")]
    public float playerDetectionRange = 5f; 
    public GameObject glitchOverlayUI;   
    public float glitchDuration = 1.0f; 

    private bool hasTransformed = false;
    private Transform player;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        catModel.SetActive(true);
    }

    void Update()
    {
        if (!hasTransformed && player != null)
        {
            if (Vector3.Distance(transform.position, player.position) <= playerDetectionRange)
            {
                StartCoroutine(TransformCatToBarrelSequence());
            }
        }
    }

    IEnumerator TransformCatToBarrelSequence()
    {
        hasTransformed = true;
        dialogueManager.GlobalShowMessage("Zero: Woah, look at that... cat?");
        yield return new WaitForSeconds(2f);

        dialogueManager.GlobalShowMessage("Artemis: Strange. Its bio-signature is fluctuating wildly. Zero, keep your distance from that anomaly!"); 
        yield return new WaitForSeconds(3f);

        if (glitchOverlayUI != null)
        {
            Image glitchImage = glitchOverlayUI.GetComponent<Image>();
            if (glitchImage != null)
            {
                glitchImage.color = new Color(1, 0, 0, 0.4f); 
                glitchOverlayUI.SetActive(true);
            }
        }

        for (int i = 0; i < 10; i++)
        {
            catModel.SetActive(!catModel.activeSelf);
            yield return new WaitForSeconds(glitchDuration / 20f);
        }
        catModel.SetActive(false);

        GameObject barrel = Instantiate(explosiveBarrelPrefab, transform.position, Quaternion.identity, null);
        barrel.transform.position = new Vector3(this.transform.position.x, this.transform.position.y + 0.1f, this.transform.position.z);
        Rigidbody rb = barrel.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }


        if (glitchOverlayUI != null) glitchOverlayUI.SetActive(false); 
        dialogueManager.GlobalShowMessage("Zero: It turned into... a barrel? And sees like it could be blew up. I think I know what to do.");
    }
}