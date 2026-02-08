using UnityEngine;
using System.Collections;

public class CameraGuide : MonoBehaviour
{
    public Transform playerCamera;  
    public Transform targetPoint;   
    public float flySpeed = 1f;    
    public float waitTime = 10f;

    public GameObject gunHolder;
    public GameObject crosshair;

    private Transform originalParent;
    private Vector3 originalLocalPos;
    private Quaternion originalLocalRot;
    private bool isFlying = false;

    public void StartGuide()
    {
        if (!isFlying) StartCoroutine(FlySequence());
    }

    private IEnumerator FlySequence()
    {
        isFlying = true;

        if (gunHolder != null) gunHolder.SetActive(false);
        if (crosshair != null) crosshair.SetActive(false);

        var playerLook = playerCamera.GetComponent<PlayerLook>();
        if (playerLook != null) playerLook.isLocked = true;

        Vector3 startWorldPos = playerCamera.position;
        Quaternion startWorldRot = playerCamera.rotation;

        Vector3 targetWorldPos = targetPoint.position;
        Quaternion targetWorldRot = targetPoint.rotation;

        float t = 0;
        while (t < 1f)
        {
            t += Time.deltaTime * flySpeed;
            playerCamera.position = Vector3.Lerp(startWorldPos, targetWorldPos, t);
            playerCamera.rotation = Quaternion.Slerp(startWorldRot, targetWorldRot, t);
            yield return null;
        }

        playerCamera.position = targetWorldPos;
        playerCamera.rotation = targetWorldRot;

        float timer = 0;
        while (timer < waitTime)
        {
            timer += Time.deltaTime;
            playerCamera.position = targetWorldPos;
            playerCamera.rotation = targetWorldRot;
            yield return null;
        }
        t = 0;
        while (t < 1f)
        {
            t += Time.deltaTime * flySpeed;
            playerCamera.position = Vector3.Lerp(targetWorldPos, startWorldPos, t);
            playerCamera.rotation = Quaternion.Slerp(targetWorldRot, startWorldRot, t);
            yield return null;
        }

        playerCamera.position = startWorldPos;
        playerCamera.rotation = startWorldRot;

        if (playerLook != null) playerLook.isLocked = false;
        if (gunHolder != null) gunHolder.SetActive(true);
        if (crosshair != null) crosshair.SetActive(true);

        isFlying = false;
    }

    private IEnumerator MoveCameraLocal(Vector3 fromPos, Quaternion fromRot, Vector3 toPos, Quaternion toRot)
    {
        float t = 0;
        while (t < 1f)
        {
            t += Time.deltaTime * flySpeed;
            playerCamera.localPosition = Vector3.Lerp(fromPos, toPos, t);
            playerCamera.localRotation = Quaternion.Slerp(fromRot, toRot, t);
            yield return null;
        }
    }

    private IEnumerator MoveCamera(Vector3 targetPos, Quaternion targetRot)
    {
        float t = 0;
        Vector3 startPos = playerCamera.position;
        Quaternion startRot = playerCamera.rotation;

        while (t < 1f)
        {
            t += Time.deltaTime * flySpeed;
            playerCamera.position = Vector3.Lerp(startPos, targetPos, t);
            playerCamera.rotation = Quaternion.Lerp(startRot, targetRot, t);
            yield return null;
        }
    }
    private void OnDrawGizmos()
    {
        if (targetPoint == null) return;

        Gizmos.color = Color.cyan;

        Gizmos.DrawLine(transform.position, targetPoint.position);

        Gizmos.DrawIcon(targetPoint.position, "Camera Gizmo.png", true);

        Gizmos.DrawWireSphere(targetPoint.position, 0.5f);

        Gizmos.color = Color.red; 
        Gizmos.DrawRay(targetPoint.position, targetPoint.forward * 2f);

#if UNITY_EDITOR
        UnityEditor.Handles.Label(targetPoint.position + Vector3.up, "Camera Destination: " + gameObject.name);
#endif
    }
}