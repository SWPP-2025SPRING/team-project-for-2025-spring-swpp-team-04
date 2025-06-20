using UnityEngine;

public class PlayerController : MonoBehaviour
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

    [Header("사운드 설정")]
    public AudioSource engineAudioSource;
    public AudioClip engineClip;
    public float minPitch = 0.8f;
    public float maxPitch = 2.0f;
    public float volumeMultiplier = 1.0f;

    // 내부 변수
    private float currentSteerAngle = 0f;
    private float currentMotorTorque = 0f;
    private float currentBrakeTorque = 0f;
    private Rigidbody rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.centerOfMass = new Vector3(0, centerOfMassHeight, 0);

        if (engineAudioSource != null && engineClip != null)
        {
            engineAudioSource.clip = engineClip;
            engineAudioSource.loop = true;
            engineAudioSource.volume = 0.5f * volumeMultiplier;
            engineAudioSource.pitch = minPitch;
            engineAudioSource.Play();
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

        // Update the engine sound
        UpdateEngineSound();
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
    }

    private void UpdateEngineSound()
  {
      if (engineAudioSource == null || engineClip == null) return;

      float speedPercent = Mathf.Clamp01(rb.velocity.magnitude / 50f); // normalize speed
      engineAudioSource.pitch = Mathf.Lerp(minPitch, maxPitch, speedPercent);
      engineAudioSource.volume = Mathf.Lerp(0.2f, 1.0f, speedPercent) * volumeMultiplier;

      if (rb.velocity.magnitude < 0.5f)
      {
          if (engineAudioSource.isPlaying)
              engineAudioSource.Pause();
      }
      else
      {
          if (!engineAudioSource.isPlaying)
              engineAudioSource.UnPause();
      }
  }
}
