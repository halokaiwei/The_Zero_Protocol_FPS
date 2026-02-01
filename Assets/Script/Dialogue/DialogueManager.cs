using System.Collections;
using UnityEngine;
using TMPro;

public class DialogueManager : MonoBehaviour
{
    [Header("UI Components")]
    public TextMeshProUGUI textDisplay;
    public GameObject dialoguePanel;
    public float typingSpeed = 0.05f;

    [Header("Opening Script")]
    [TextArea(3, 10)]
    public string[] openingLines;

    private int currentLineIndex = 0;
    private Coroutine typingCoroutine;
    private bool isInterrupting = false;

    [HideInInspector] public bool isOpeningFinished = false;

    void Start()
    {
        if (dialoguePanel != null) dialoguePanel.SetActive(true);
        StartCoroutine(PlayOpeningSequence());
    }

    IEnumerator PlayOpeningSequence()
    {
        isOpeningFinished = false;

        if (Camera.main != null)
        {
            Camera.main.transform.localPosition = new Vector3(0, 1.6f, 0.15f);
        }

        while (currentLineIndex < openingLines.Length)
        {
            if (isInterrupting)
            {
                yield return null;
                continue;
            }

            string line = openingLines[currentLineIndex];
            yield return typingCoroutine = StartCoroutine(TypeSentence(line));

            currentLineIndex++;
            yield return new WaitForSeconds(2.0f);
        }
        isOpeningFinished = true;
        Debug.Log("System: Opening sequence complete. Scanner platform now active.");
    }

    public void GlobalShowMessage(string newMessage, bool immediate = false)
    {
        StartCoroutine(HandleInterruption(newMessage,immediate));
    }

    IEnumerator HandleInterruption(string message, bool immediate)
    {
        isInterrupting = true;
        if (typingCoroutine != null) StopCoroutine(typingCoroutine);

        if (immediate)
        {
            textDisplay.text = "> " + message;
        }
        else
        {
            yield return typingCoroutine = StartCoroutine(TypeSentence(message));
        }
        yield return new WaitForSeconds(1.5f);

        isInterrupting = false;
    }

    IEnumerator TypeSentence(string sentence)
    {
        textDisplay.text = "> ";
        foreach (char letter in sentence.ToCharArray())
        {
            textDisplay.text += letter;
            yield return new WaitForSeconds(typingSpeed);
        }
        typingCoroutine = null;
    }
}