using UnityEngine;
using System.Collections.Generic;

public class PuzzleTwoManager : MonoBehaviour
{
    public List<PuzzleNode> nodes = new List<PuzzleNode>();
    [Header("Reward Settings")]
    public RotatingHazard spinner;
    public DialogueManager dialogueManager;
    public DoorController areaBDoor1;
    public void OnNodeHit(int index)
    {
        nodes[index].Toggle();

        if (index > 0)
            nodes[index - 1].Toggle();

        if (index < nodes.Count - 1)
            nodes[index + 1].Toggle();

        CheckWinCondition();
    }

    void CheckWinCondition()
    {
        bool allRed = true;
        foreach (var node in nodes)
        {
            if (!node.isOn) 
            {
                allRed = false;
                break;
            }
        }

        if (allRed)
        {
            Debug.Log("Solved Successfully");
            dialogueManager.GlobalShowMessage("Well done Zero. The frequency had been balanced. The access had been granted. Now we can proceed to sector C.");
            HandleWin();
        }
    }

    void HandleWin()
    {
        if (spinner != null)
        {
            spinner.DeactivateHazard();
            Debug.Log("Spin Stopped");
            areaBDoor1.isAccessGranted = true;
            areaBDoor1.TryOpenDoor();
            foreach (var node in nodes)
            {
                node.LockNode();
                Renderer r = node.GetComponent<Renderer>();
                if (r != null) r.material.EnableKeyword("_EMISSION");
            }
        }
    }
}