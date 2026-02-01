using UnityEngine;

public class DoorController : MonoBehaviour
{
    private Animator anim;
    private bool isOpen = false;

    [Header("Door Status")]
    public bool isAccessGranted = false; 

    [Header("Messages")]
    [TextArea(1, 3)]
    public string deniedMessage = "ACCESS_DENIED: Requirement not met.";
    [TextArea(1, 3)]
    public string grantedMessage = "ACCESS_GRANTED: Protocol initialized.";

    void Start()
    {
        anim = GetComponent<Animator>();
    }

    public void TryOpenDoor()
    {
        if (isAccessGranted)
        {
            if (!isOpen)
            {
                anim.SetTrigger("Open");
                isOpen = true;
                FindObjectOfType<DialogueManager>().GlobalShowMessage(grantedMessage);
            }
        }
        else
        {
            FindObjectOfType<DialogueManager>().GlobalShowMessage(deniedMessage);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            TryOpenDoor();
        }
    }
}