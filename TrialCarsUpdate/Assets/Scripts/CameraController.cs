using UnityEngine;

public class CameraController : MonoBehaviour
{
    [Header("카메라 설정")]
    public Transform target; // 플레이어 차량
    public float smoothSpeed = 1000.0f; // 카메라 이동 부드러움 (값이 클수록 빠름)
    public float rotationSmoothSpeed = 1000.0f; // 카메라 회전 부드러움 (값이 클수록 빠름)

    [Header("카메라 시점")]
    public Vector3[] cameraPositions; // 다양한 카메라 위치 (로컬 오프셋)
    public Vector3[] cameraRotations; // 다양한 카메라 회전값 (오일러 각도)
    private int currentCameraView = 0; // 현재 카메라 시점

    [Header("카메라 효과")]
    public float tiltMax = 5f; // 최대 기울기 각도 (좌우 입력에 따른)
    public float tiltSpeed = 2f; // 기울기 변화 속도
    public float jumpReactionStrength = 0.5f; // 착지 시 카메라 반응 강도

    // 내부 변수
    private Vector3 desiredPosition;
    private Quaternion desiredRotation;
    private Vector3 velocity = Vector3.zero; // SmoothDamp용 속도 변수
    private PlayerController playerController; // 타겟의 PlayerController 컴포넌트
    private float currentTilt = 0f; // 현재 카메라 틸트 값
    private bool wasGrounded = true; // 이전 프레임의 접지 상태

    private void Start()
    {
        // 타겟 설정 확인
        if (target == null)
        {
            Debug.LogError("카메라 타겟(target)이 설정되지 않았습니다! CameraController를 비활성화합니다.");
            this.enabled = false; // 타겟 없으면 컴포넌트 비활성화
            return;
        }

        // PlayerController 컴포넌트 가져오기 (점프/착지 효과용)
        playerController = target.GetComponent<PlayerController>();
        if (playerController == null)
        {
            Debug.LogWarning($"타겟 '{target.name}'에 PlayerController 컴포넌트가 없습니다. 점프/착지 효과가 작동하지 않습니다.");
        }

        // 기본 카메라 위치/회전 설정 (Inspector에서 설정되지 않았을 경우 대비)
        if (cameraPositions == null || cameraPositions.Length == 0)
        {
            Debug.LogWarning("cameraPositions 배열이 Inspector에서 설정되지 않아 기본값으로 초기화합니다.");
            cameraPositions = new Vector3[3];
            cameraPositions[0] = new Vector3(0, 3.0f, -6.0f); // 예시: 3인칭 뒤 (Y=높이, Z=거리)
            cameraPositions[1] = new Vector3(3f, 2.6f, 3f);  // 예시: 높은 시점 (오른쪽 위 뒤)
            cameraPositions[2] = new Vector3(-3f, 2.6f, 3f); // 예시: 낮은 시점 (왼쪽 아래 뒤)
        }
        if (cameraRotations == null || cameraRotations.Length == 0)
        {
            Debug.LogWarning("cameraRotations 배열이 Inspector에서 설정되지 않아 기본값으로 초기화합니다.");
            cameraRotations = new Vector3[3];
            cameraRotations[0] = new Vector3(15, 0, 0);   // 예시: 3인칭 뒤 (살짝 아래로)
            cameraRotations[1] = new Vector3(30, 230, 0); // 예시: 높은 시점 (오른쪽 위에서 바라봄, Roll=0)
            cameraRotations[2] = new Vector3(30, 140, 0);  // 예시: 낮은 시점 (왼쪽 아래에서 바라봄, Roll=0)
        }

        // 초기 카메라 위치 및 회전 즉시 설정 (게임 시작 시 카메라가 제자리 찾아가는 과정 스킵)
        HandleCameraViewInput(); // 현재 카메라 뷰 인덱스 확정
        Vector3 initialLocalPosition = cameraPositions[currentCameraView];
        Quaternion initialTargetYawRotation = Quaternion.Euler(0, target.eulerAngles.y, 0); // 타겟의 초기 Yaw 회전
        // 효과 적용 전의 순수 로컬 위치로 초기 위치 계산
        transform.position = target.position + initialTargetYawRotation * cameraPositions[currentCameraView];
        transform.rotation = initialTargetYawRotation * Quaternion.Euler(cameraRotations[currentCameraView]); // 초기 회전 설정

        // 초기 접지 상태 기록
        wasGrounded = playerController != null ? playerController.IsGrounded() : true;
    }

    // 물리 업데이트 이후에 실행되어 타겟의 최종 위치/회전을 반영
    private void LateUpdate()
    {
        // 타겟이 사라졌을 경우 대비
        if (target == null)
        {
            Debug.LogError("카메라 타겟이 사라졌습니다! CameraController를 비활성화합니다.");
            this.enabled = false;
            return;
        }

        // --- 입력 처리 ---
        HandleCameraViewInput(); // 1, 2, 3 키 등으로 카메라 시점 변경

        // --- 위치 계산 ---
        // 1. 현재 시점의 기본 로컬 오프셋 가져오기
        Vector3 localPosition = cameraPositions[currentCameraView];

        // 2. 카메라 효과 적용 (로컬 오프셋 수정)
        ApplyCameraTilt(ref localPosition);       // 좌우 회전 시 틸트 효과
        ApplyJumpLandingEffect(ref localPosition); // 착지 시 효과

        // 3. 최종 원하는 월드 위치 계산 (타겟의 Yaw 회전만 반영)
        Quaternion targetYawRotation = Quaternion.Euler(0, target.eulerAngles.y, 0);
        desiredPosition = target.position + targetYawRotation * localPosition;

        // --- 회전 계산 ---
        // 최종 원하는 월드 회전 계산 (타겟 Yaw 회전 + 카메라 오프셋 각도)
        Vector3 eulerAngles = cameraRotations[currentCameraView]; // 현재 시점의 기본 각도
        desiredRotation = targetYawRotation * Quaternion.Euler(eulerAngles);

        // --- 카메라 적용 ---
        // 1. 부드러운 위치 이동 (Vector3.SmoothDamp 사용)
        // smoothTime은 (1 / smoothSpeed) 입니다. smoothSpeed가 클수록 smoothTime은 작아져 더 빠르게 따라갑니다.
        transform.position = Vector3.SmoothDamp(transform.position, desiredPosition, ref velocity, 1 / smoothSpeed);

        // 2. 부드러운 회전 이동 (Quaternion.Slerp 사용)
        // Slerp의 세 번째 인자는 보간 계수(0~1)입니다. Time.deltaTime * speed 형태로 사용하면 프레임률에 독립적인 속도 조절 가능.
        transform.rotation = Quaternion.Slerp(transform.rotation, desiredRotation, Time.deltaTime * rotationSmoothSpeed);
    }

    // 카메라 시점 변경 입력 처리 함수
    private void HandleCameraViewInput()
    {
        int previousCameraView = currentCameraView;

        // 숫자 키 입력 확인 (배열 범위 고려)
        if (Input.GetKeyDown(KeyCode.Alpha1) && cameraPositions.Length > 0)
            currentCameraView = 0;
        else if (Input.GetKeyDown(KeyCode.Alpha2) && cameraPositions.Length > 1)
            currentCameraView = 1;
        else if (Input.GetKeyDown(KeyCode.Alpha3) && cameraPositions.Length > 2)
            currentCameraView = 2;
        // 필요하다면 KeyCode.Alpha4, Alpha5 등 추가

        // Clamp 함수로 안전하게 인덱스 범위 제한
        currentCameraView = Mathf.Clamp(currentCameraView, 0, cameraPositions.Length - 1);

        // 카메라 시점이 실제로 변경되었을 때
        if (previousCameraView != currentCameraView)
        {
            // 위치 이동이 갑자기 튀는 것을 방지하기 위해 SmoothDamp의 velocity를 초기화
            velocity = Vector3.zero;
            // 틸트 값도 리셋할지 여부는 선택 (주석 처리 시 유지)
            // currentTilt = 0f;
        }
    }

    // 좌우 입력에 따른 카메라 틸트 효과 적용 함수
    private void ApplyCameraTilt(ref Vector3 localPosition)
    {
        // "Horizontal" 축 입력 값 가져오기 (-1 ~ 1)
        // 프로젝트 설정 > Input Manager 에서 축 이름 확인 필요
        float horizontalInput = Input.GetAxis("Horizontal");

        // 목표 틸트 각도 계산 (입력 반대 방향으로 기울기)
        float targetTilt = -horizontalInput * tiltMax;

        // 현재 틸트 값에서 목표 틸트 값으로 부드럽게 변경 (Lerp 사용)
        currentTilt = Mathf.Lerp(currentTilt, targetTilt, Time.deltaTime * tiltSpeed);

        // 로컬 위치 오프셋 벡터를 Z축 기준으로 회전시켜 틸트 효과 적용
        // 카메라 자체가 회전하는 것이 아니라, 타겟 기준 로컬 오프셋 위치를 변경하여 기울어 보이게 함
        localPosition = Quaternion.Euler(0, 0, currentTilt) * localPosition;
    }

    // 착지 효과 적용 함수
    private void ApplyJumpLandingEffect(ref Vector3 localPosition)
    {
        // PlayerController가 있어야 작동
        if (playerController != null)
        {
            bool isGrounded = playerController.IsGrounded(); // 현재 접지 상태 확인

            // 착지 감지: 이전 프레임에는 공중(not wasGrounded) & 현재 프레임에는 땅(isGrounded)
            if (!wasGrounded && isGrounded)
            {
                // 착지 시 카메라 로컬 Y 위치를 순간적으로 낮춤
                localPosition.y -= jumpReactionStrength;
            }

            // 현재 접지 상태를 다음 프레임 비교를 위해 저장
            wasGrounded = isGrounded;
        }
    }
}
