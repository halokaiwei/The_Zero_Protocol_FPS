using UnityEngine;
using System.Collections;

public class MultiDialogueTrigger : MonoBehaviour
{
    public DialogueManager dialogueManager;

    [Header("Optional Task Tracking")]
    public ExplosiveBarrel targetBarrel;

    [Header("Dialogue Content")]
    [TextArea(3, 5)]
    public string[] dialogueLines;

    [Header("Timing Settings")]
    public float baseDelay = 1.5f;
    public float timePerChar = 0.05f;
    public float maxDelay = 8.0f;

    public bool triggerOnlyOnce = true;
    private bool hasTriggered = false;
    private bool missionAccomplished = false;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !hasTriggered)
        {
            StartCoroutine(PlayDialogueSequence());
            if (triggerOnlyOnce) hasTriggered = true;
        }
    }

    private IEnumerator PlayDialogueSequence()
    {
        foreach (string line in dialogueLines)
        {
            if (targetBarrel != null && CheckBarrel()) yield break;

            dialogueManager.GlobalShowMessage(line);

            float finalDelay = Mathf.Clamp(baseDelay + (line.Length * timePerChar), baseDelay, maxDelay);

            float elapsed = 0f;
            while (elapsed < finalDelay)
            {
                if (targetBarrel != null && CheckBarrel()) yield break;

                elapsed += 0.2f;
                yield return new WaitForSeconds(0.2f);
            }
        }
    }

    private bool CheckBarrel()
    {
        if (targetBarrel != null && targetBarrel.isExploded && !missionAccomplished)
        {
            missionAccomplished = true;
            string cleverZeroLine = "Clever as always, Zero. 'Explosive solution' is the easiest way to get access card. .";
            dialogueManager.GlobalShowMessage(cleverZeroLine);
            return true;
        }
        return false;
    }
}