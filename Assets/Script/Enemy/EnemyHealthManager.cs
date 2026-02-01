using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro; 

public class Bot : MonoBehaviour, IDamageable
{
    public int maxHealth = 100;
    private int currentHealth;
    public int damageOnTouch;
    public bool willDie = true;

    [Header("UI Feedback")]
    public GameObject damageTextObject;

    void Start()
    {
        currentHealth = maxHealth;
        if (damageTextObject != null) damageTextObject.SetActive(false);
    }

    public void TakeDamage(int damageAmount)
    {
        currentHealth -= damageAmount;

        ShowDamageText(damageAmount);

        if (currentHealth <= 0 && willDie)
        {
            Die();
        }
    }

    public void ShowDamageText(int amount)
    {
        if (damageTextObject != null)
        {
            StopAllCoroutines();

            damageTextObject.transform.localPosition = new Vector3(0, 2.5f, 0);

            damageTextObject.SetActive(true);

            TMP_Text text = damageTextObject.GetComponentInChildren<TMP_Text>();
            if (text != null) text.text = amount.ToString();

            StartCoroutine(HideDamageText());
        }
    }

    private IEnumerator HideDamageText()
    {
        yield return new WaitForSeconds(0.5f);
        if (damageTextObject != null) damageTextObject.SetActive(false);
    }

    private void Die()
    {
        Destroy(transform.gameObject);
    }

    public int getHealth()
    {
        return this.currentHealth;
    }
}