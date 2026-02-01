using UnityEngine;
using System.Collections;

public class HazardArea : MonoBehaviour
{
    public int damageAmount = 10;
    public float damageInterval = 2f;
    private Coroutine damageCoroutine;
    void Start()
    {

    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            HealthStaminaSystem health = other.GetComponent<HealthStaminaSystem>();

            if (health != null)
            {
                damageCoroutine = StartCoroutine(ApplyContinuousDamage(health));
             
            }

        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (damageCoroutine != null)
            {
                StopCoroutine(damageCoroutine);
                damageCoroutine = null;
            }
        }
    }

    private IEnumerator ApplyContinuousDamage(HealthStaminaSystem health)
    {
        health.TakeDamage(damageAmount);
        Debug.Log("In Hazard, -10");

        while (true)
        {
            yield return new WaitForSeconds(damageInterval);
            health.TakeDamage(damageAmount);
            Debug.Log("In Hazard continuously, -10");
        }
    }
}