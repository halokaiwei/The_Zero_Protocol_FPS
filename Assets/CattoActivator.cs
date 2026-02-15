using UnityEngine;
using System.Collections;

public class CattoActivator : MonoBehaviour
{
    public GameObject cattoObject;
    public float delayAfterDialogue = 1.0f; 

    public void ActivateCatto()
    {
        StartCoroutine(AppearRoutine());
    }

    IEnumerator AppearRoutine()
    {
        yield return new WaitForSeconds(delayAfterDialogue);

        if (cattoObject != null)
        {
            cattoObject.SetActive(true);
            Debug.Log("System warn: Catto existed.");
        }
    }
}