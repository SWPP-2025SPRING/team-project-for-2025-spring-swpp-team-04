using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EnemyController : MonoBehaviour
{
    [Header("적 설정")]
    public float moveSpeed = 5f;
    public float rotationSpeed = 3f;
    public float detectionRange = 15f;
    public float patrolRadius = 10f;
    public Transform patrolCenter;
    
    [Header("웨이포인트 설정")]
    public Transform[] waypoints;
    public float waypointStopDistance = 1.5f;
    private int currentWaypoint = 0;
    
    [Header("물리 설정")]
    public float gravity = 100f;
    
    // 내부 변수
    private Transform player;
    private Vector3 moveDirection;
    private CharacterController controller;
    private Vector3 randomPatrolPoint;
    private bool isPatrolPointSet = false;
    private float patrolTimer = 0f;
    
    private enum EnemyState { Patrol, Chase, Return }
    private EnemyState currentState = EnemyState.Patrol;
    
    private void Start()
    {
        controller = GetComponent<CharacterController>();
        
        // 플레이어 찾기
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
        
        // 패트롤 중심점이 설정되지 않았으면 현재 위치로 설정
        if (patrolCenter == null)
        {
            patrolCenter = new GameObject("PatrolCenter_" + gameObject.name).transform;
            patrolCenter.position = transform.position;
        }
        
        // 웨이포인트가 없으면 랜덤 패트롤 사용
        if (waypoints == null || waypoints.Length == 0)
        {
            SetRandomPatrolPoint();
        }
    }
    
    private void Update()
    {
        // 플레이어가 없으면 패트롤만 수행
        if (player == null)
        {
            Patrol();
            return;
        }
        
        // 플레이어와의 거리 계산
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);
        
        // 상태 업데이트
        UpdateState(distanceToPlayer);
        
        // 현재 상태에 따른 행동 수행
        switch (currentState)
        {
            case EnemyState.Patrol:
                Patrol();
                break;
            case EnemyState.Chase:
                ChasePlayer();
                break;
            case EnemyState.Return:
                ReturnToPatrol();
                break;
        }
        
        // 중력 적용
        ApplyGravity();
        
        // 이동 적용
        controller.Move(moveDirection * Time.deltaTime);
    }
    
    private void UpdateState(float distanceToPlayer)
    {
        // 플레이어가 감지 범위 내에 있으면 추적
        if (distanceToPlayer <= detectionRange)
        {
            currentState = EnemyState.Chase;
        }
        // 플레이어가 감지 범위를 벗어나면 패트롤 지점으로 복귀
        else if (currentState == EnemyState.Chase)
        {
            currentState = EnemyState.Return;
        }
        // 패트롤 중심 근처로 돌아왔으면 패트롤 상태로 전환
        else if (currentState == EnemyState.Return && 
                 Vector3.Distance(transform.position, patrolCenter.position) < patrolRadius)
        {
            currentState = EnemyState.Patrol;
            SetRandomPatrolPoint();
        }
    }
    
    private void Patrol()
    {
        if (waypoints != null && waypoints.Length > 0)
        {
            PatrolWaypoints();
        }
        else
        {
            PatrolRandom();
        }
    }
    
    private void PatrolWaypoints()
    {
        if (currentWaypoint >= waypoints.Length) currentWaypoint = 0;
        
        Vector3 targetPosition = waypoints[currentWaypoint].position;
        Vector3 directionToWaypoint = targetPosition - transform.position;
        directionToWaypoint.y = 0; // 수평 이동만 고려
        
        // 웨이포인트에 도착했는지 확인
        if (directionToWaypoint.magnitude < waypointStopDistance)
        {
            currentWaypoint = (currentWaypoint + 1) % waypoints.Length;
            return;
        }
        
        // 웨이포인트 방향으로 회전 및 이동
        RotateTowards(targetPosition);
        moveDirection = transform.forward * moveSpeed;
    }
    
    private void PatrolRandom()
    {
        // 랜덤 패트롤 포인트 설정
        if (!isPatrolPointSet)
        {
            SetRandomPatrolPoint();
        }
        
        // 목표 지점으로 이동
        Vector3 directionToTarget = randomPatrolPoint - transform.position;
        directionToTarget.y = 0; // 수평 이동만 고려
        
        // 목표 지점에 도착했는지 확인
        if (directionToTarget.magnitude < waypointStopDistance)
        {
            // 잠시 대기 후 새 지점 설정
            patrolTimer += Time.deltaTime;
            if (patrolTimer >= 2f)
            {
                SetRandomPatrolPoint();
                patrolTimer = 0f;
            }
            moveDirection = Vector3.zero;
            return;
        }
        
        // 목표 지점 방향으로 회전 및 이동
        RotateTowards(randomPatrolPoint);
        moveDirection = transform.forward * moveSpeed;
    }
    
    private void SetRandomPatrolPoint()
    {
        // 패트롤 중심점 주변의 랜덤한 위치 설정
        Vector2 randomCircle = Random.insideUnitCircle * patrolRadius;
        randomPatrolPoint = patrolCenter.position + new Vector3(randomCircle.x, 0, randomCircle.y);
        isPatrolPointSet = true;
    }
    
    private void ChasePlayer()
    {
        // 플레이어 방향으로 회전 및 이동
        RotateTowards(player.position);
        moveDirection = transform.forward * moveSpeed;
    }
    
    private void ReturnToPatrol()
    {
        // 패트롤 중심점 방향으로 회전 및 이동
        RotateTowards(patrolCenter.position);
        moveDirection = transform.forward * moveSpeed;
    }
    
    private void RotateTowards(Vector3 targetPosition)
    {
        // 타겟 방향 계산 (y축 제외)
        Vector3 direction = targetPosition - transform.position;
        direction.y = 0;
        
        // 회전해야 할 방향이 있는 경우에만 회전
        if (direction != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }
    }
    
    private void ApplyGravity()
    {
        // 중력 적용
        if (controller.isGrounded)
        {
            moveDirection.y = -0.5f; // 접지 상태 유지를 위한 약간의 하향력
        }
        else
        {
            moveDirection.y -= gravity * Time.deltaTime;
        }
    }
    
    // 충돌 처리 (발사체와의 충돌은 Projectile 스크립트에서 처리)
    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        // 플레이어와 충돌 시 데미지 처리 등 추가 가능
        if (hit.gameObject.CompareTag("Player"))
        {
            // 플레이어에게 데미지 주는 로직 (필요시 구현)
        }
    }
}
