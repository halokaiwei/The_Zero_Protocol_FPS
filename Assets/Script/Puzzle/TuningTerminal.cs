using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class TuningTerminal : MonoBehaviour
{
    public GameObject tuningCanvas;
    public float interactDistance = 3f;
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
            if (!hasSpokenHint && !tuningCanvas.activeSelf)
            {
                Debug.Log("[TuningTerminal] Artemis dialogue triggered");                
                if (BiometricScanner.IsScanCompleted)
                {
                    dialogueManager.GlobalShowMessage("Artemis: You're at the gate node. Press [E] to synchronize the frequency to open Sector C.");
                }
                else
                {
                    dialogueManager.GlobalShowMessage("Artemis: Access denied. Zero, you must complete the Biometric Scan before tuning this terminal.");
                }
                hasSpokenHint = true;
            }

            if (Input.GetKeyDown(KeyCode.E))
            {
                if (BiometricScanner.IsScanCompleted)
                {
                    OpenTuningInterface();
                }
                else
                {
                    dialogueManager.GlobalShowMessage("Artemis: Biometric data missing. Please return to the scanning station.");
                }
            }
        }
    }

    void OpenTuningInterface()
    {
        if (tuningCanvas != null)
        {
            tuningCanvas.SetActive(true);

            FrequencyTuningUI uiScript = tuningCanvas.GetComponent<FrequencyTuningUI>();
            if (uiScript != null)
            {
                uiScript.Setup(this);
            }

            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }
        else
        {
            Debug.LogError("[TuningTerminal] Error: No object assigned in tuningCanvas.");
        }
    }

    public void OnTaskComplete()
    {
        isCompleted = true;
        GetComponent<Renderer>().material.color = Color.green;

        Invoke("ArtemisPanicDialogue", 0.5f);
    }

    void ArtemisPanicDialogue()
    {
        dialogueManager.GlobalShowMessage("Artemis: Zero. That was a neural-link virus from the D-Sector insurgents. Do not trust the data leakage.");
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
            hasSpokenHint = false;
        }
    }
}