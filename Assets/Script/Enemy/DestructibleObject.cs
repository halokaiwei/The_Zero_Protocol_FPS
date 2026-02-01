using UnityEngine;

public class DestructibleObject : MonoBehaviour, IDamageable
{
    public int health = 50;
    public GameObject destroyedVersion; 

    public virtual void TakeDamage(int amount)
    {
        health -= amount;
        if (health <= 0) Die();
    }

    protected virtual void Die()
    {
        if (destroyedVersion != null)
        {
            Instantiate(destroyedVersion, transform.position, transform.rotation);
        }
        Destroy(gameObject);
    }
}