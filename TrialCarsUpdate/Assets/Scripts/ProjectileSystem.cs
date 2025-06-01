using UnityEngine;
using System.Collections;

public class ProjectileSystem : MonoBehaviour
{
    [Header("발사체 설정")]
    public GameObject projectilePrefab; // 발사체 프리팹
    public Transform firePoint; // 발사 위치
    public float projectileSpeed = 50f; // 발사체 속도
    public float fireRate = 0.5f; // 발사 간격 (초)
    public float projectileLifetime = 3f; // 발사체 수명 (초)

    // 내부 변수
    private float nextFireTime = 0f;

    private void Start()
    {
        // 발사 위치가 설정되지 않았으면 현재 오브젝트 위치 사용
        if (firePoint == null)
        {
            firePoint = transform;
        }
    }

    private void Update()
    {
        // 스페이스바 입력 감지 및 발사
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
            Debug.LogError("발사체 프리팹이 설정되지 않았습니다!");
            return;
        }
        //Quaternion zRotation = Quaternion.AngleAxis(90f, Vector3.forward);
        //Quaternion yRotation = Quaternion.AngleAxis(90f, Vector3.right);
        //Quaternion finalRotation = firePoint.rotation * zRotation * yRotation;

        //Vector3 offsetPositon = new Vector3(0, 1, 4);
        // 발사체 생성
        GameObject projectile = Instantiate(projectilePrefab, firePoint.position, firePoint.rotation);

        // 발사체에 속도 적용
        Rigidbody rb = projectile.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.velocity = firePoint.up * projectileSpeed;
        }

        // 발사체 스크립트 설정
        Projectile projectileScript = projectile.GetComponent<Projectile>();
        if (projectileScript == null)
        {
            projectileScript = projectile.AddComponent<Projectile>();
        }
        projectileScript.lifetime = projectileLifetime;
    }
}

// 발사체 클래스
public class Projectile : MonoBehaviour
{
    [Header("발사체 속성")]
    public float lifetime = 3f; // 수명 (초)
    public float damage = 10f; // 데미지
    public GameObject hitEffectPrefab; // 충돌 이펙트

    private void Start()
    {
        // 수명 후 자동 제거
        Destroy(gameObject, lifetime);
    }

    private void OnCollisionEnter(Collision collision)
    {
        // 적과 충돌 시 처리
        if (collision.gameObject.CompareTag("Enemy"))
        {
            // 적 제거
            //Destroy(collision.gameObject);
            collision.gameObject.SetActive(false);
            this.gameObject.GetComponent<ScoreManager>().scoreUpdate(200);

            // 발사체 제거
            Destroy(gameObject);
        }
        // 장애물과 충돌 시 처리
        else if (!collision.gameObject.CompareTag("Player"))
        {
            // 발사체 제거
            Destroy(gameObject);
        }
    }
}
