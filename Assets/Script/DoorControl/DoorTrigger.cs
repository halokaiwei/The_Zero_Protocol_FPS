using UnityEngine;

public class DoorTrigger : MonoBehaviour
{
    public CatAI theCat; 

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (theCat != null)
            {
                theCat.SwitchToFollowMode();

                Destroy(gameObject); 
            }
        }
    }
}