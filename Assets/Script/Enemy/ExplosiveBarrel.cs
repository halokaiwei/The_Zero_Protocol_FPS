using UnityEngine;

public class ExplosiveBarrel : DestructibleObject, IDamageable
{
    [Header("Explosion Settings")]
    public float explosionRadius = 5f;
    public float explosionForce = 700f;
    public int explosionDamage = 100;
    public GameObject explosionEffect;

    [HideInInspector] 
    public bool isExploded = false;
    protected override void Die()
    {
        isExploded = true; 

        if (explosionEffect != null)
        {
            Instantiate(explosionEffect, transform.position, Quaternion.identity);
        }

        Collider[] colliders = Physics.OverlapSphere(transform.position, explosionRadius);
        foreach (Collider hit in colliders)
        {
            IDamageable dmg = hit.GetComponentInParent<IDamageable>();
            if (dmg != null && hit.gameObject != this.gameObject)
            {
                dmg.TakeDamage(explosionDamage);
                Debug.Log("damaged barrel");
            }

            Rigidbody rb = hit.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.AddExplosionForce(explosionForce, transform.position, explosionRadius);
            }
        }

        base.Die();
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, explosionRadius);
    }
}