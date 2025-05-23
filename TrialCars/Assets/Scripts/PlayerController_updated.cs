using UnityEngine;

public class PlayerController_updated : MonoBehaviour
{
    [Header("차량 설정")]
    public float motorForce = 1500f;
    public float brakeForce = 3000f;
    public float maxSteerAngle = 30f;
    public float centerOfMassHeight = -0.5f;


    [Header("바퀴 설정")]
    public WheelCollider[] wheelColliders = new WheelCollider[4]; // 앞바퀴 2개, 뒷바퀴 2개
    public Transform[] wheelMeshes = new Transform[4]; // 바퀴 메시

    [Header("입력 설정")]
    public float inputSensitivity = 1.0f;
    public float steeringResetSpeed = 5.0f;

    [Header("물리 설정")]
    public float downforceValue = 100f;

    [Header("점프 설정")]
    public float jumpForce = 7000f; // 점프 힘
    public float jumpCooldown = 1.5f; // 점프 쿨다운 (초)

    // 내부 변수
    private float currentSteerAngle = 0f;
    private float currentMotorTorque = 0f;
    private float currentBrakeTorque = 0f;
    private Rigidbody rb;
    private float lastJumpTime = -10f; // 마지막 점프 시간 (쿨다운 계산용)

    // Score
    int score;

    private void Start()
    {
        score = 0;
        rb = GetComponent<Rigidbody>();
        rb.centerOfMass = new Vector3(0, centerOfMassHeight, 0);
    }

    private void Update()
    {
        // 점프 입력 처리 (Update에서 처리하여 입력 누락 방지)
        if (Input.GetKeyDown(KeyCode.Z))
        {
            TryJump();
        }
    }

    private void FixedUpdate()
    {
        // 입력 처리
        float verticalInput = Input.GetAxis("Vertical");
        float horizontalInput = Input.GetAxis("Horizontal");

        // 모터 토크 계산 (가속/감속)
        currentMotorTorque = verticalInput * motorForce * inputSensitivity;

        // 브레이크 처리 (후진 입력 시 전진 중이면 브레이크 적용)
        currentBrakeTorque = 0f;
        if (verticalInput < 0 && rb.velocity.magnitude > 1f && Vector3.Dot(rb.velocity, transform.forward) > 0)
        {
            currentBrakeTorque = brakeForce;
            currentMotorTorque = 0f;
        }
        // 전진 입력이 들어오면 브레이크 즉시 해제 (해결 방법 2)
        else if (verticalInput > 0)
        {
            currentBrakeTorque = 0f;
            currentMotorTorque = verticalInput * motorForce * inputSensitivity;
        }
        else if (verticalInput < 0)
        {
            currentMotorTorque = verticalInput * motorForce * inputSensitivity;
        }
        else
        {
            currentMotorTorque = 0f;
        }

        // 조향각 계산 (좌/우 회전)
        float targetSteerAngle = horizontalInput * maxSteerAngle;

        // 조향각 부드럽게 변경
        currentSteerAngle = Mathf.Lerp(currentSteerAngle, targetSteerAngle, Time.fixedDeltaTime * 5f);

        // 조향각 자동 복귀 (입력이 없을 때)
        if (Mathf.Abs(horizontalInput) < 0.1f)
        {
            currentSteerAngle = Mathf.Lerp(currentSteerAngle, 0f, Time.fixedDeltaTime * steeringResetSpeed);
        }

        // 바퀴에 물리 적용
        ApplyWheelPhysics();

        // 다운포스 적용 (안정성 향상)
        ApplyDownforce();

        // 바퀴 메시 업데이트
        UpdateWheelMeshes();

        StabilizeInAir();
    }

    private void ApplyWheelPhysics()
    {
        // 앞바퀴 조향 적용 (0, 1번 인덱스가 앞바퀴)
        wheelColliders[0].steerAngle = currentSteerAngle;
        wheelColliders[1].steerAngle = currentSteerAngle;

        // 뒷바퀴에 모터 토크 적용 (2, 3번 인덱스가 뒷바퀴)
        wheelColliders[2].motorTorque = currentMotorTorque;
        wheelColliders[3].motorTorque = currentMotorTorque;

        // 모든 바퀴에 브레이크 적용
        for (int i = 0; i < wheelColliders.Length; i++)
        {
            wheelColliders[i].brakeTorque = currentBrakeTorque;
        }
    }

    private void ApplyDownforce()
    {
        // 차량 속도에 비례하는 다운포스 적용 (안정성 향상)
        if (rb.velocity.magnitude > 5f)
        {
            rb.AddForce(-transform.up * downforceValue * rb.velocity.magnitude);
        }
    }

    private void UpdateWheelMeshes()
    {
        // 각 바퀴 메시의 위치와 회전 업데이트
        for (int i = 0; i < wheelColliders.Length; i++)
        {
            UpdateWheelMesh(wheelColliders[i], wheelMeshes[i]);
        }
    }

    private void UpdateWheelMesh(WheelCollider collider, Transform mesh)
    {
        // 바퀴 콜라이더의 위치와 회전 정보 가져오기
        Vector3 position;
        Quaternion rotation;
        collider.GetWorldPose(out position, out rotation);

        // 바퀴 메시 업데이트
        mesh.position = position;
        mesh.rotation = rotation;
    }

    // 점프 시도 메서드
    private void TryJump()
    {
        // 쿨다운 확인
        if (Time.time < lastJumpTime + jumpCooldown)
            return;

        // 지면에 닿아있는지 확인
        if (IsGrounded())
        {
            // 점프 힘 적용
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);

            // 마지막 점프 시간 기록
            lastJumpTime = Time.time;

            // 디버그 로그
            Debug.Log("차량 점프!");
        }
    }

    // 차량이 공중에 있는지 확인하는 메서드 (점프 후 안정화에 사용)
    public bool IsGrounded()
    {
        int groundedWheels = 0;
        foreach (WheelCollider wheel in wheelColliders)
        {
            if (wheel.isGrounded)
                groundedWheels++;
        }

        // 최소 2개 이상의 바퀴가 지면에 닿아있으면 접지 상태로 간주
        return groundedWheels >= 2;
    }

    // 충돌 처리
    private void OnCollisionEnter(Collision collision)
    {
        // 충돌 시 물리적 반응 (충돌 지점에 따라 토크 적용)
        if (collision.relativeVelocity.magnitude > 5f)
        {
            Vector3 impactDir = collision.contacts[0].normal;
            rb.AddTorque(Vector3.Cross(impactDir, Vector3.up) * collision.relativeVelocity.magnitude * 0.1f, ForceMode.Impulse);
        }
        // 적과 충돌 시 처리
        if (collision.gameObject.CompareTag("Enemy"))
        {
            // 점수 감소
            this.gameObject.GetComponent<ScoreManager>().scoreUpdate(-100);
            // 적 제거
            //Destroy(collision.gameObject);
            collision.gameObject.SetActive(false);
        }
    }

    // 점프 중 차량 안정화 (선택적 구현)
    private void StabilizeInAir()
    {
        if (!IsGrounded())
        {
            // 공중에서 차량이 너무 기울어지지 않도록 안정화
            //Quaternion targetRotation = Quaternion.Euler(0, transform.eulerAngles.y, 0);
            //rb.angularVelocity = Vector3.Lerp(rb.angularVelocity, Vector3.zero, Time.fixedDeltaTime * 0.5f);
            //transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.fixedDeltaTime * 0.5f);

            // 축 고정
            rb.angularVelocity = Vector3.zero; // 회전 속도 즉시 0으로 설정
            transform.rotation = Quaternion.Euler(0, transform.eulerAngles.y, 0); // Y축 회전만 유지
        }
    }
}
