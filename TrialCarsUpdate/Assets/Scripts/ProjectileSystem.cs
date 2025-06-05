using UnityEngine;

public class ProjectileSystem : MonoBehaviour
{
    public GameObject projectilePrefab;
    public Transform firePoint;
    public float projectileSpeed = 50f;
    public float fireRate = 0.5f;
    public float projectileLifetime = 3f;

    private float nextFireTime = 0f;

    void Start()
    {
        if (firePoint == null)
        {
            firePoint = transform;
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) && Time.time >= nextFireTime)
        {
            FireProjectile();
            nextFireTime = Time.time + fireRate;
        }
    }

    public void FireProjectile()
    {
        if (projectilePrefab == null)
        {
            Debug.LogError("Projectile prefab not assigned!");
            return;
        }

        GameObject projectile = Instantiate(projectilePrefab, firePoint.position, firePoint.rotation);

        Rigidbody rb = projectile.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.velocity = firePoint.up * projectileSpeed;
        }

        Projectile projScript = projectile.GetComponent<Projectile>();
        if (projScript == null)
        {
            projScript = projectile.AddComponent<Projectile>();
        }

        projScript.lifetime = projectileLifetime;
    }
}

public class Projectile : MonoBehaviour
{
    public float lifetime = 3f;
    public float damage = 10f;
    public GameObject hitEffectPrefab;

    void Start()
    {
        Destroy(gameObject, lifetime);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            collision.gameObject.SetActive(false);
            ScoreManager playerHP = GameObject.FindWithTag("Player")?.GetComponent<ScoreManager>();
            if (playerHP != null)
            {
                playerHP.scoreUpdate(200); // Gain HP
            }
            Destroy(gameObject);
        }
        else if (!collision.gameObject.CompareTag("Player"))
        {
            Destroy(gameObject);
        }
    }
}
