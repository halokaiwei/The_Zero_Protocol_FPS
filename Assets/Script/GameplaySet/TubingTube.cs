using UnityEngine;

public class TubingTube : MonoBehaviour
{
    public GameObject tubingCanvas;
    private bool isPlayerNearby = false;
    private bool isCompleted = false;
    private bool hasSpokenHint = false;

    [Header("Systems")]
    public DialogueManager dialogueManager;

    void Update()
    {
        if (isCompleted) return;

        if (isPlayerNearby)
        {
            Debug.Log("Pleayer nearby");
            if (!hasSpokenHint && !tubingCanvas.activeSelf)
            {
                dialogueManager.GlobalShowMessage("Artemis: Zero, the pressure lines are fluctuating. Press [E] to stablize the pressure.");
                hasSpokenHint = true;
            }

            if (Input.GetKeyDown(KeyCode.E))
            {
                OpenTuningInterface();
            }
        }
    }

    void OpenTuningInterface()
    {
        if (tubingCanvas != null)
        {
            tubingCanvas.SetActive(true);

            dialogueManager.GlobalShowMessage("Artemis: Synchronizing pressure... Do NOT let the pointer leave the green sector");

            PressureStabilizationUI uiScript = tubingCanvas.GetComponent<PressureStabilizationUI>();
            if (uiScript != null)
            {
                uiScript.Setup(this);
            }

            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;

            GameObject.FindWithTag("Player").GetComponent<PlayerMovement>().enabled = false;
        }
    }

    public void OnTaskComplete()
    {
        isCompleted = true;

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        GameObject.FindWithTag("Player").GetComponent<PlayerMovement>().enabled = true;

        Invoke("ArtemisPanicDialogue", 1.0f);
    }

    void ArtemisPanicDialogue()
    {
        dialogueManager.GlobalShowMessage("Artemis: Good Job, Zero. All the tasks had been completed. Let's leave the zone using the unlocked door.");
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerNearby = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerNearby = false;
            if (!tubingCanvas.activeSelf) hasSpokenHint = false;
        }
    }
}