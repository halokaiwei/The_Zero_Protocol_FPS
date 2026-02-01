using UnityEngine;

[CreateAssetMenu(fileName = "NewDialogue", menuName = "Dialogue/Data")]
public class DialogueData : ScriptableObject
{
    public Color nameColor = Color.cyan;

    [Header("Content")]
    [TextArea(3, 10)]
    public string[] sentences;

    [Header("Settings")]
    public float typeSpeed = 0.05f;
}