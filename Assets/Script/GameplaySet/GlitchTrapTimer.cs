using System.Collections;
using UnityEngine;

public class GlitchTrapTimer : MonoBehaviour
{
    [Header("Timing Settings")]
    public float minActiveTime = 2f;
    public float maxActiveTime = 5f;
    public float minHiddenTime = 1f;
    public float maxHiddenTime = 3f;

    [Header("Warning Settings")]
    public float warningDuration = 1.5f;

    [Header("Damage Settings")]
    public float damagePerSecond = 20f; 

    private MeshRenderer meshRenderer;
    private Collider trapCollider;

    public ParticleSystem glitchParticles;
    private bool isTrapActive = true;

    void Start()
    {
        meshRenderer = GetComponent<MeshRenderer>();
        trapCollider = GetComponent<Collider>();

        if (trapCollider != null) trapCollider.isTrigger = true;

        StartCoroutine(TrapCycle());
    }

    IEnumerator TrapCycle()
    {
        while (true)
        {
            isTrapActive = true;
            SetTrapState(true);
            yield return new WaitForSeconds(Random.Range(minActiveTime, maxActiveTime));

            float elapsed = 0;
            while (elapsed < warningDuration)
            {
                meshRenderer.enabled = !meshRenderer.enabled;
                yield return new WaitForSeconds(0.1f);
                elapsed += 0.1f;
            }

            isTrapActive = false;
            SetTrapState(false);
            yield return new WaitForSeconds(Random.Range(minHiddenTime, maxHiddenTime));
        }
    }

    void SetTrapState(bool state)
    {
        meshRenderer.enabled = state;
        if (trapCollider != null) trapCollider.enabled = state;

        if (glitchParticles != null)
        {
            if (state) glitchParticles.Play();
            else glitchParticles.Stop();
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            HealthStaminaSystem playerHealth = other.GetComponent<HealthStaminaSystem>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(damagePerSecond * Time.deltaTime);

                // Camera.main.transform.localPosition += Random.insideUnitSphere * 0.01f;
            }
        }
    }
}