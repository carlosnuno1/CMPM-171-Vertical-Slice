using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float lifetime = 3f;

    void Start()
    {
        Invoke(nameof(DestroyByLifetime), lifetime);
    }

    void DestroyByLifetime()
    {
        Debug.Log("Bullet destroyed: Lifetime");
        Destroy(gameObject);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerHealth playerHealth = other.GetComponent<PlayerHealth>();
            if (playerHealth == null)
                playerHealth = other.GetComponentInParent<PlayerHealth>();

            if (playerHealth != null)
                playerHealth.TakeDamage(playerHealth.bulletDamage);

            Debug.Log("Bullet destroyed: Player");
            Destroy(gameObject);
        }
        else if (!other.CompareTag("Enemy") && !other.isTrigger)
        {
            Debug.Log("Bullet destroyed: Environment");
            Destroy(gameObject);
        }
    }
}