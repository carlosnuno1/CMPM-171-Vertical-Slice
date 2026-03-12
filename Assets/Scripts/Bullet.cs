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
            Debug.Log("hit player");
            PlayerHealth playerHealth = other.GetComponent<PlayerHealth>();
            if (playerHealth == null)
                Debug.Log("finding component in parent");
                playerHealth = other.GetComponentInParent<PlayerHealth>();

            if (playerHealth != null)
                Debug.Log("Doing damage");
                playerHealth.TakeDamage(playerHealth.bulletDamage);

            Destroy(gameObject);
        }
        else if (!other.CompareTag("Enemy") && !other.isTrigger)
        {
            Destroy(gameObject);
        }
    }
}