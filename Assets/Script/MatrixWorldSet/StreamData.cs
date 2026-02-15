using UnityEngine;
using TMPro;
using System.Collections;

public class StreamData : MonoBehaviour
{
    public TextMeshProUGUI screenText;
    public float typeSpeed = 0.05f;
    public float activationDelay = 1.0f;
    [TextArea(3, 10)]
    public string[] dataLogs;

    private bool hasStarted = false;

    void Start() { if (screenText != null) screenText.text = ""; }

    public void StartDataStream()
    {
        if (!hasStarted)
        {
            hasStarted = true;
            StartCoroutine(SequenceActivation());
        }
    }

    IEnumerator SequenceActivation()
    {
        yield return new WaitForSeconds(activationDelay);

        int currentLine = 0;
        while (currentLine < dataLogs.Length)
        {
            screenText.text = "";
            string fullText = dataLogs[currentLine];
            foreach (char c in fullText)
            {
                screenText.text += c;
                yield return new WaitForSeconds(typeSpeed);
            }
            yield return new WaitForSeconds(2.5f); 
            currentLine++;
        }
    }
}