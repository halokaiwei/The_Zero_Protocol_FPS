using UnityEngine;

public class ScannerPlatform : MonoBehaviour
{
    [Header("UI & Logic")]
    public DoorController areaADoor;
    public DialogueManager dialogueManager;
    public GameObject invisibleWall; 

    [Header("Settings")]
    public float scanDuration = 5f;

    private float timer = 0f;
    private bool isPlayerOnPlatform = false;
    private bool hasScanned = false;
    private int lastStep = -1;

    void Start()
    {
        if (invisibleWall != null)
        {
            invisibleWall.SetActive(true);
        }
    }

    private void Update()
    {
        if (dialogueManager.isOpeningFinished && invisibleWall != null && invisibleWall.activeSelf)
        {
            invisibleWall.SetActive(false);
            Debug.Log("System: Barrier Deactivated.");
        }

        if (isPlayerOnPlatform && !hasScanned && dialogueManager.isOpeningFinished)
        {
            timer += Time.deltaTime;

            int totalBlocks = 10;
            float progress = timer / scanDuration;
            int currentStep = Mathf.FloorToInt(progress * totalBlocks);

            if (currentStep > lastStep && currentStep <= totalBlocks)
            {
                string progressBar = "";
                for (int i = 0; i < totalBlocks; i++)
                {
                    progressBar += (i < currentStep) ? "█" : "░";
                }
                dialogueManager.GlobalShowMessage("SCANNING: " + progressBar + " " + (int)(progress * 100) + "%", true);
                lastStep = currentStep;
            }

            if (timer >= scanDuration)
            {
                CompleteScan();
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !hasScanned)
        {
            isPlayerOnPlatform = true;
            timer = 0f;
            lastStep = -1;
            dialogueManager.GlobalShowMessage("BIOMETRIC SCAN IN PROGRESS... PLEASE HOLD STILL.");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player") && !hasScanned)
        {
            isPlayerOnPlatform = false;
            timer = 0f;
            lastStep = -1;
            dialogueManager.GlobalShowMessage("SCAN INTERRUPTED. RETURN TO PLATFORM.");
        }
    }

    void CompleteScan()
    {
        hasScanned = true;
        isPlayerOnPlatform = false;
        areaADoor.isAccessGranted = true;
        dialogueManager.GlobalShowMessage("SCAN COMPLETE. [██████████] 100% - UNIT ZERO IDENTIFIED.");
        areaADoor.TryOpenDoor();
    }
}