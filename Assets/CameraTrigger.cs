using UnityEngine;

public class CameraTrigger : MonoBehaviour
{
    public DialogueManager dialogueManager;
    private bool hasTriggered = false;
    public CameraGuide cameraGuide;
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !hasTriggered)
        {
            hasTriggered = true;

            var player = other.gameObject;
            var movement = player.GetComponent<PlayerMovement>();
            var combat = player.GetComponentInChildren<FiringSystem>();

            if (movement != null) movement.enabled = false;
            if (combat != null) combat.enabled = false;

            cameraGuide.StartGuide();

            dialogueManager.GlobalShowMessage("Artemis: The rotating hazard is linked to these bio-signals. We need to eliminate all Beetles to force a system shutdown. ");
            dialogueManager.GlobalShowMessage("Artemis: Once the path is clear, retrieve the Access Card. We need it for the next sector. ");

            StartCoroutine(RestoreControl(movement, combat, cameraGuide.waitTime + (1f / cameraGuide.flySpeed) * 2));
        }
    }

    private System.Collections.IEnumerator RestoreControl(PlayerMovement m, FiringSystem c, float delay)
    {
        yield return new WaitForSeconds(delay);
        if (m != null) m.enabled = true;
        if (c != null) c.enabled = true;
    }
}