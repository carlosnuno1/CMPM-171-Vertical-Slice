using UnityEngine;

public class ShockwaveGrenade : MonoBehaviour
{
    public float fuseDelay = 2.5f;
    public float blastRadius = 8f;
    public float launchPower = 25f;
    public GameObject explosionEffectPrefab;

    private bool thrown;

    private Rigidbody rb;
    private bool hasStuck = false;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.useGravity = true;
    }

    public void Thrown()
    {
        thrown = true;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player")) return;

        if (!hasStuck && thrown)
        {
            hasStuck = true;

            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            rb.isKinematic = true;

            Invoke(nameof(Explode), fuseDelay);
        }
    }

    void Explode()
    {
        if (explosionEffectPrefab != null)
        {
            GameObject fx = Instantiate(explosionEffectPrefab, transform.position, Quaternion.identity);
            Destroy(fx, 1f);
        }

        Collider[] victims = Physics.OverlapSphere(transform.position, blastRadius);

        foreach (Collider hit in victims)
        {
            Rigidbody victimRb = hit.GetComponentInParent<Rigidbody>();
            if (victimRb == null) continue;
            if (hit.CompareTag("Player"))
            {
                Debug.Log("launch player");
                Vector3 dir = (hit.transform.position - transform.position).normalized;

                dir.y += 0.75f;

                victimRb.AddForce(dir * launchPower, ForceMode.Impulse);

                Debug.Log("Launch Direction: " + dir);
            }
        }

        Destroy(gameObject);
    }
}