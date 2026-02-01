using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PressureStabilizationUI : MonoBehaviour
{
    public RectTransform backgroundRect;
    public RectTransform pointer;
    public RectTransform safetyZone;

    [Header("Adjustments")]
    public float upSpeed = 180f;
    public float downSpeed = 80f;

    private float currentY;
    private float minY;
    private float maxY;
    private float progress = 0f;
    private bool isSolved = false;

    [Header("Systems")]
    public TextMeshProUGUI statusText;
    public DoorController targetDoor;
    public DialogueManager dialogueManager;

    private float lastMessageTime = 0f;
    private float messageCooldown = 0.8f; 
    private TubingTube parentTube;

    void Start()
    {
        if (backgroundRect == null) backgroundRect = GetComponent<RectTransform>();
        LayoutRebuilder.ForceRebuildLayoutImmediate(backgroundRect);

        float halfHeight = backgroundRect.rect.height / 2f;
        minY = -halfHeight;
        maxY = halfHeight;

        currentY = minY;
    }

    public void Setup(TubingTube tube)
    {
        parentTube = tube;
        progress = 0f;
        isSolved = false;
        currentY = minY;
    }

    void Update()
    {
        if (isSolved) return;

        bool isPushing = Input.GetKey(KeyCode.Space);

        if (isPushing)
        {
            currentY += upSpeed * Time.deltaTime;
        }
        else
        {
            currentY -= downSpeed * Time.deltaTime;
        }

        currentY = Mathf.Clamp(currentY, minY, maxY);

        pointer.anchoredPosition = new Vector2(0, currentY);
        UpdateProgress();
    }

    void UpdateProgress()
    {
        float zoneY = safetyZone.anchoredPosition.y;
        float zoneHeight = safetyZone.rect.height;

        bool inZone = currentY >= (zoneY - zoneHeight / 2) && currentY <= (zoneY + zoneHeight / 2);

        if (inZone)
        {
            progress += 0.25f * Time.deltaTime;
            pointer.GetComponent<Image>().color = Color.green;
        }
        else
        {
            progress -= 0.15f * Time.deltaTime;
            pointer.GetComponent<Image>().color = Color.red;
        }

        progress = Mathf.Clamp01(progress);

        if (Time.time > lastMessageTime + messageCooldown && !isSolved)
        {
            SendProgressToArtemis(inZone);
            lastMessageTime = Time.time;
        }

        if (progress >= 1f)
        {
            isSolved = true;
            statusText.text = "Stabilization Successful!";
            targetDoor.isAccessGranted = true;

            if (parentTube != null) parentTube.OnTaskComplete();

            Invoke("CloseUI", 1.5f);
        }
    }

    void SendProgressToArtemis(bool inZone)
    {
        int totalBlocks = 10;
        int currentStep = Mathf.FloorToInt(progress * totalBlocks);
        string progressBar = "";

        for (int i = 0; i < totalBlocks; i++)
        {
            progressBar += (i < currentStep) ? "█" : "░";
        }

        string status = inZone ? "STABILIZING: " : "PRESSURE CRITICAL: ";
        dialogueManager.GlobalShowMessage(status + progressBar + " " + (int)(progress * 100) + "%", true);
    }

    void CloseUI()
    {
        gameObject.SetActive(false);
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }
}