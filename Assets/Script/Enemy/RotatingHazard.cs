using UnityEngine;
using System.Collections;

public class RotatingHazard : MonoBehaviour
{
    [Header("Rotation Settings")]
    public Vector3 rotationSpeed = new Vector3(0, 100, 0); 
    private bool isPaused = false;
    private bool isFunctional = true;
    [Header("Damage Settings")]
    public float damageAmount = 20f;
    public float knockbackForce = 5f; 

    void Update()
    {
        if (isFunctional && !isPaused)
        {
            transform.Rotate(rotationSpeed * Time.deltaTime);
        }
    }

    public void DeactivateHazard()
    {
        isFunctional = false;
        Debug.Log("Spinner Stopped");
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (!isFunctional) return;
        if (collision.gameObject.name.Contains("Player") || collision.gameObject.CompareTag("Player"))
        {
            HealthStaminaSystem health = collision.gameObject.GetComponent<HealthStaminaSystem>();
            if (health != null)
            {
                health.TakeDamage(damageAmount);
            }

            ApplyKnockback(collision);

            StartCoroutine(PauseRotation());
        }
    }

    private void ApplyKnockback(Collision collision)
    {
        Rigidbody playerRb = collision.gameObject.GetComponent<Rigidbody>();
        if (playerRb != null)
        {
            Vector3 knockbackDir = (collision.gameObject.transform.position - transform.position).normalized;
            knockbackDir.y = 0.2f;

            playerRb.AddForce(knockbackDir * knockbackForce, ForceMode.Impulse);
        }
    }

    private IEnumerator PauseRotation()
    {
        isPaused = true;
        yield return new WaitForSeconds(2f);
        isPaused = false;
    }
}