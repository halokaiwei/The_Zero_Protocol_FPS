using UnityEngine;
using TMPro;
using System.Collections;

public class BiometricScanner : MonoBehaviour
{
    [Header("TextMeshPro 3D")]
    public TextMeshPro statusText;     
    public TextMeshPro progressBarText; 
    public TextMeshPro hackerText;     

    [Header("Colors")]
    public Color normalColor = Color.green;
    public Color errorColor = Color.red;

    [Header("Systems")]
    public DialogueManager dialogueManager;

    private bool isScanning = false;
    private bool scanFinished = false;
    private bool isPurging = false;
    private Coroutine currentScanCoroutine;
    private Coroutine blinkCoroutine;

    public CatAI theCat;
    public static bool IsScanCompleted = false;

    void Start()
    {
        progressBarText.gameObject.SetActive(false);
        hackerText.gameObject.SetActive(false);
        blinkCoroutine = StartCoroutine(BlinkWaitingText());
    }

    IEnumerator BlinkWaitingText()
    {
        while (!isScanning && !scanFinished)
        {
            statusText.color = normalColor;
            statusText.text = "WAITING FOR SUBJECT...";
            yield return new WaitForSeconds(0.6f);
            statusText.text = "";
            yield return new WaitForSeconds(0.4f);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !scanFinished)
        {
            if (blinkCoroutine != null) StopCoroutine(blinkCoroutine);
            isScanning = true;
            currentScanCoroutine = StartCoroutine(StartScanSequence());
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player") && isScanning && !scanFinished)
        {
            if (currentScanCoroutine != null) StopCoroutine(currentScanCoroutine);
            isScanning = false;
            statusText.text = "SCAN INTERRUPTED\nPLEASE STAND ON PLATFORM";
            blinkCoroutine = StartCoroutine(BlinkWaitingText());
        }
    }

    IEnumerator StartScanSequence()
    {
        statusText.text = "BIOMETRIC SYNCING...";
        statusText.color = normalColor;
        dialogueManager.GlobalShowMessage("Artemis: Synchronizing biometric data...");

        yield return new WaitForSeconds(5f);

        isScanning = false;
        scanFinished = true;

        statusText.color = errorColor;
        statusText.text = "CRITICAL ERROR\nCPU USAGE: 88%\nCLOCK SPEED: 4.2 GHz";

        yield return new WaitForSeconds(1f);
        dialogueManager.GlobalShowMessage("Zero: Artemis... Why is it showing Clock Speed?");

        yield return new WaitForSeconds(3f);

        statusText.color = normalColor;
        statusText.text = "VITAL SIGN: OPTIMAL\nHEART RATE: 72 BPM\nSTATUS: STABLE";
        dialogueManager.GlobalShowMessage("Artemis: A local logic overflow detected. Ignore it.");

        yield return new WaitForSeconds(3f);
        dialogueManager.GlobalShowMessage("Artemis: Your vitals are looking sharp, Zero. Every indicator is perfect. You're more than ready for Sector C");

        yield return new WaitForSeconds(4f);
        statusText.text = "<color=yellow>PRESS [E] TO PURGE GAS</color>";
    }

    void Update()
    {
        if (scanFinished && !isPurging && Input.GetKeyDown(KeyCode.E))
        {
            float dist = Vector3.Distance(transform.position, GameObject.FindGameObjectWithTag("Player").transform.position);
            if (dist < 4f) StartCoroutine(PurgeGasSequence());
        }
    }

    IEnumerator PurgeGasSequence()
    {
        isPurging = true;
        statusText.text = "PURGING IN PROGRESS...";
        progressBarText.gameObject.SetActive(true);

        float duration = 8f;
        float elapsed = 0f;
        int totalBlocks = 20;


        Vector3 originalHackerPos = hackerText.rectTransform.localPosition;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float percent = elapsed / duration;


            int filledBlocks = Mathf.FloorToInt(percent * totalBlocks);
            string bar = "[";
            for (int i = 0; i < totalBlocks; i++)
            {
                bar += (i < filledBlocks) ? "█" : "░";
            }
            bar += "] " + (percent * 100f).ToString("F0") + "%";
            progressBarText.text = bar;

            if (elapsed >= 2f)
            {
                if (!hackerText.gameObject.activeSelf)
                {
                    hackerText.gameObject.SetActive(true);
                    hackerText.text = "<color=red>ZERO. YOU ARE AN ITERATION.</color>";
                }

                if (elapsed >= 6f)
                {
                    hackerText.text = "<color=red>THE CAT IS WATCHING. WAKE UP.</color>";
                }

                hackerText.enabled = (Random.value > 0.08f);

                float shakeIntensity = 0.005f;
                hackerText.rectTransform.localPosition = originalHackerPos + new Vector3(
                    Random.Range(-shakeIntensity, shakeIntensity),
                    Random.Range(-shakeIntensity, shakeIntensity),
                    0
                );
            }

            yield return null;
        }


        statusText.text = "PURGE COMPLETE.";

        IsScanCompleted = true;

        //cat setting
        CatAI[] cats = FindObjectsOfType<CatAI>();
        foreach (var cat in cats)
        {
            cat.isCreepyLooking = true;
        }
        if (theCat != null)
        {
            theCat.SwitchToFollowMode();

            Destroy(gameObject);
        }

        progressBarText.gameObject.SetActive(false);
        hackerText.gameObject.SetActive(false);

        hackerText.rectTransform.localPosition = originalHackerPos;
        hackerText.enabled = true;


        yield return new WaitForSeconds(3f);
        dialogueManager.GlobalShowMessage("Zero: What was that flashing on the screen just now? It said I was an iteration... what does that even mean?");

        yield return new WaitForSeconds(4f);
        dialogueManager.GlobalShowMessage("Artemis: ...The central system was attacked. We are still investigating on it.");

        yield return new WaitForSeconds(4f);
        dialogueManager.GlobalShowMessage("Artemis: Zero. Don't be distracted by that. Continue immediately, that's the order.");

        yield return new WaitForSeconds(2f);
        dialogueManager.GlobalShowMessage("Artemis: You need to stabilize the local node. Go to the terminal near the main gate and tune the frequency.");
    }
}