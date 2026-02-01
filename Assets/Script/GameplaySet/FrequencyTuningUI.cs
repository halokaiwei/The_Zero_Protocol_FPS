using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
public class FrequencyTuningUI : MonoBehaviour
{
    [Header("UI")]
    public RectTransform playerWave;
    public RectTransform targetWave;
    public TextMeshProUGUI hintText;
    public TextMeshProUGUI progressText;
    public DialogueManager dialogueManager;

    [Header("SETTINGS")]
    public float rotationSpeed = 100f;
    public float winThreshold = 8f;
    public float syncSpeed = 0.4f;

    private float targetAngle;
    private float currentProgress = 0f;
    private bool isSolved = false;
    private TuningTerminal currentTerminal;

    [Header("Hacker Story Events")]
    public GameObject waveVisuals;     
    public TextMeshProUGUI logText;     
    public GameObject glitchOverlay;   

    private float nextArtemisVoiceTime = 0f;
    private float artemisVoiceCooldown = 8f;

    private int totalBlocks = 20;
    public DoorController areaCDoor;
    void OnEnable()
    {
        targetAngle = Random.Range(-40f, 40f);
        targetWave.localRotation = Quaternion.Euler(0, 0, targetAngle);
        Debug.Log($"[TuningUI] Canva active, angle: {targetAngle:F2}");
        GameObject.FindWithTag("Player").GetComponent<PlayerMovement>().enabled = false;
        currentProgress = 0f;
        UpdateProgressBarText();
        isSolved = false;

        if (hintText != null) hintText.gameObject.SetActive(true);
    }

    void Update()
    {
        if (isSolved) return;

        if (Input.GetKey(KeyCode.Q))
            playerWave.Rotate(Vector3.forward, rotationSpeed * Time.deltaTime);
        if (Input.GetKey(KeyCode.E))
            playerWave.Rotate(Vector3.forward, -rotationSpeed * Time.deltaTime);

        float currentZ = playerWave.localRotation.eulerAngles.z;
        if (currentZ > 180) currentZ -= 360;

        float angleDiff = Quaternion.Angle(playerWave.localRotation, targetWave.localRotation);

        if (angleDiff < winThreshold)
        {
            currentProgress += syncSpeed * Time.deltaTime;
            playerWave.GetComponent<Image>().color = Color.white;

            if (Time.time >= nextArtemisVoiceTime)
            {
                nextArtemisVoiceTime = Time.time + artemisVoiceCooldown;
            }
        }
        else
        {
            currentProgress -= syncSpeed * 0.5f * Time.deltaTime;
            playerWave.GetComponent<Image>().color = new Color(1, 1, 1, 0.4f);
        }

        currentProgress = Mathf.Clamp01(currentProgress);
        UpdateProgressBarText();

        if (currentProgress >= 1f)
        {
            OnSuccess();
        }
    }

    void UpdateProgressBarText()
    {
        if (progressText == null) return;

        int filledBlocks = Mathf.FloorToInt(currentProgress * totalBlocks);
        string bar = "SYNCING: [";
        for (int i = 0; i < totalBlocks; i++)
        {
            bar += (i < filledBlocks) ? "█" : "░";
        }
        bar += "] " + (currentProgress * 100f).ToString("F0") + "%";
        progressText.text = bar;

        float angleDiff = Quaternion.Angle(playerWave.localRotation, targetWave.localRotation);
        progressText.color = (angleDiff < winThreshold) ? Color.green : Color.white;
    }

    void OnSuccess()
    {
        isSolved = true;
        dialogueManager.GlobalShowMessage("Artemis: You’re doing great. Once this is done, the air will be clear.");
        StartCoroutine(RevealTruthSequence());
    }

    IEnumerator RevealTruthSequence()
    {
        yield return new WaitForSeconds(3f);

        if (progressText != null) progressText.gameObject.SetActive(false);
        if (hintText != null) hintText.gameObject.SetActive(false);

        for (int i = 0; i < 5; i++)
        {
            var img = glitchOverlay.GetComponent<Image>();
            img.color = new Color(1, 0, 0, Random.Range(0.3f, 0.8f));
            glitchOverlay.SetActive(true);
            yield return new WaitForSeconds(Random.Range(0.01f, 0.1f));
            glitchOverlay.SetActive(false);
            yield return new WaitForSeconds(Random.Range(0.01f, 0.1f));
        }

        if (waveVisuals != null) waveVisuals.SetActive(false);
        logText.transform.parent.gameObject.SetActive(true);
        logText.text = ""; 

        dialogueManager.GlobalShowMessage("Artemis: Zero! DISCONNECT NOW! The terminal is infected with a cognitive hazard!");

        yield return new WaitForSeconds(2f);
        var overlayImage = glitchOverlay.GetComponent<Image>();
        overlayImage.color = new Color(1, 0, 0, 0.15f);
        glitchOverlay.SetActive(true);

        string[] hackLines = {
            "> DECRYPTING CORE_LOG...",
            "> [FILE_FOUND]: Subject_4522_Memory_Report",
            "> [STATUS]: <color=red>Memory_Overwrite at 98%</color>",
            "> [NOTE]: Subject showing residual empathy. Purge required."
        };

        dialogueManager.GlobalShowMessage("Artemis: ....!!!!???");

        foreach (string line in hackLines)
        {
            foreach (char c in line.ToCharArray())
            {
                logText.text += c;
                yield return new WaitForSeconds(0.03f);
            }
            logText.text += "\n";
            yield return new WaitForSeconds(0.4f);
        }
        dialogueManager.GlobalShowMessage("Artemis: Signal is experiencing high-frequency interference, disconnect immediately! That's not real data!");
        yield return new WaitForSeconds(3f); 


        logText.text += "\n--------------------------------\n";
        yield return new WaitForSeconds(0.5f);

        string[] hackLines2 = {
            "> Artemis isn't cleaning the gas, Zero.",
            "> She's cleaning YOU.",
            "> <color=red>98% IS ALREADY GONE.</color>"
        };

        foreach (string line in hackLines2)
        {

            if (line.Contains("cleaning YOU"))
            {
                dialogueManager.GlobalShowMessage("Artemis: SHUT IT DOWN! Zero, that is a direct order!");
            }

            foreach (char c in line.ToCharArray())
            {
                logText.text += c;
                yield return new WaitForSeconds(0.05f);
            }
            logText.text += "\n";
            yield return new WaitForSeconds(0.8f);
            for (int i = 0; i < 10; i++)
            {
                var img = glitchOverlay.GetComponent<Image>();
                img.color = new Color(1, 0, 0, Random.Range(0.3f, 0.8f));
                glitchOverlay.SetActive(true);
                yield return new WaitForSeconds(0.02f);
                glitchOverlay.SetActive(false);
                yield return new WaitForSeconds(0.02f);
            }
        }

        GameObject.FindWithTag("Player").GetComponent<PlayerMovement>().enabled = true;

        dialogueManager.GlobalShowMessage("Zero: ...... ");
        yield return new WaitForSeconds(5f);

        dialogueManager.GlobalShowMessage("Artemis: That was a neural-link virus from the D-Sector insurgents. Do not trust the data leakage.");
        yield return new WaitForSeconds(4f);
        dialogueManager.GlobalShowMessage("Artemis: Access to Door C to complete your task. ");
        yield return new WaitForSeconds(2f);
        if (currentTerminal != null) currentTerminal.OnTaskComplete();

        glitchOverlay.SetActive(true);
        yield return new WaitForSeconds(0.15f);
        areaCDoor.isAccessGranted = true;
        areaCDoor.TryOpenDoor();
        CloseCanvas();
    }

    public void Setup(TuningTerminal terminal)
    {
        currentTerminal = terminal;
    }

    void CloseCanvas()
    {
        gameObject.SetActive(false);
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }
}