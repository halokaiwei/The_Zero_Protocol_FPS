using UnityEngine;

public class AccessCard : MonoBehaviour
{
    [Header("References")]
    public Animator modelAnimator;
    public DialogueManager dialogueManager;
    public DoorController areaBDoor;

    [Header("Settings")]
    public string collectTag = "Player";
    private bool hasTriggered = false; 

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(collectTag) && !hasTriggered)
        {
            Debug.Log("Player detected! Starting sequence...");
            ExecuteSequence();
        }
    }

    private void ExecuteSequence()
    {
        hasTriggered = true;

        if (modelAnimator != null)
        {
            modelAnimator.SetTrigger("PlayAnimation");
            modelAnimator.Play("opening");
            Debug.Log("Animation Bool 'PlayAnimation' set to True.");
        }
        else
        {
            Debug.LogError("Model Animator is not assigned in the Inspector!");
        }

        if (areaBDoor != null)
        {
            areaBDoor.isAccessGranted = true;
        }

        if (dialogueManager != null)
        {
            dialogueManager.GlobalShowMessage("Artemis: Analysis complete. We have the sector B credentials. Good job, Zero. Let's head to the gate B.");
        }

    }
}