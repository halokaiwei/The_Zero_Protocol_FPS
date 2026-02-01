using UnityEngine;
using TMPro; 
using System.Collections;

public class StreamData : MonoBehaviour
{
    public TextMeshProUGUI screenText;
    public float typeSpeed = 0.05f;  

    [TextArea(3, 10)]
    public string[] dataLogs;        

    private int currentLine = 0;

    void Start()
    {
        StartCoroutine(PlayDataStream());
    }

    IEnumerator PlayDataStream()
    {
        while (true)
        {
            screenText.text = ""; 
            string fullText = dataLogs[currentLine];

            foreach (char c in fullText)
            {
                screenText.text += c;
                yield return new WaitForSeconds(typeSpeed);
            }

            yield return new WaitForSeconds(2f);
            currentLine = (currentLine + 1) % dataLogs.Length;
        }
    }
}