using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HealthStaminaSystem : MonoBehaviour
{
    [Header("UI References")]
    public UnityEngine.UI.Image healthBar;

    [Header("Health Settings")]
    public float maxHealth = 100f;
    public float currentHealth;

    [Header("System References")]
    public GameOverScreen gameOverScreen;
    public AudioClip hurtSound;
    private HitEffect hitEffect;
    private PlayerMovement playerMovement;

    void Start()
    {
        // Find UI components if not assigned
        if (healthBar == null)
            healthBar = GameObject.Find("HealthBar").GetComponent<UnityEngine.UI.Image>();

        hitEffect = GameObject.FindObjectOfType<HitEffect>();
        playerMovement = GetComponent<PlayerMovement>();

        currentHealth = maxHealth;
    }

    void Update()
    {
        // Update Health Bar UI
        if (healthBar != null)
            healthBar.fillAmount = Mathf.Clamp(currentHealth / maxHealth, 0, 1);

        // Check Death Condition
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        gameObject.SetActive(false);
        if (gameOverScreen != null)
            gameOverScreen.GameOver();

        Debug.Log("System Critical: Process Terminated (Player Dead)");
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            Bot bot = collision.gameObject.GetComponent<Bot>();
            if (bot != null)
            {
                TakeDamage(20f);
            }
        }
    }

    public void TakeDamage(float damage)
    {
        currentHealth -= damage;

        if (hurtSound != null)
            SoundManager.Instance.PlaySoundByInterval(hurtSound, 0.1f);

        if (hitEffect != null)
            hitEffect.ShowFlash();

        if (currentHealth <= 0f)
        {
            currentHealth = 0f;
        }
    }

    public void RecoverHealth(float healing)
    {
        currentHealth += healing;
        if (currentHealth > maxHealth)
        {
            currentHealth = maxHealth;
        }
        Debug.Log("Integrity Restored. Current Health: " + currentHealth);
    }
}