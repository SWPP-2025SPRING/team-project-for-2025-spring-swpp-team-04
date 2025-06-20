using UnityEngine;

public class WheelAnimator : MonoBehaviour
{
    [Header("바퀴 애니메이션 설정")]
    public WheelCollider[] wheelColliders = new WheelCollider[4]; // 휠 콜라이더 참조
    public Transform[] wheelMeshes = new Transform[4]; // 바퀴 메시 참조
    public float rotationSpeed = 10f; // 바퀴 회전 속도
    public float maxSteerAngle = 30f; // 최대 조향 각도
    public float steerSpeed = 5f; // 조향 속도
    public float returnSpeed = 3f; // 중앙 복귀 속도
    
    // 내부 변수
    private float[] currentSteerAngles = new float[4]; // 현재 각 바퀴의 조향 각도
    
    private void Update()
    {
        // 입력 처리
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");
        
        // 목표 조향 각도 계산
        float targetSteerAngle = horizontalInput * maxSteerAngle;
        
        // 각 바퀴 업데이트
        for (int i = 0; i < wheelColliders.Length; i++)
        {
            // 바퀴 회전 애니메이션
            UpdateWheelRotation(i, verticalInput);
            
            // 바퀴 조향 애니메이션 (앞바퀴만 - 인덱스 0, 1)
            if (i < 2)
            {
                UpdateWheelSteering(i, targetSteerAngle, horizontalInput);
            }
        }
    }
    
    private void UpdateWheelRotation(int wheelIndex, float verticalInput)
    {
        if (wheelMeshes[wheelIndex] == null) return;
        
        // 바퀴 회전 속도 계산 (차량 속도에 비례)
        float rpm = wheelColliders[wheelIndex].rpm;
        float rotationAngle = rpm * 6 * Time.deltaTime; // rpm을 각도로 변환 (6 = 360도/60초)
        
        // 바퀴 회전 애니메이션 적용
        wheelMeshes[wheelIndex].Rotate(rotationAngle, 0, 0);
    }
    
    private void UpdateWheelSteering(int wheelIndex, float targetSteerAngle, float horizontalInput)
    {
        if (wheelMeshes[wheelIndex] == null) return;
        
        // 조향 각도 부드럽게 변경
        if (Mathf.Abs(horizontalInput) > 0.1f)
        {
            // 입력이 있을 때 목표 각도로 부드럽게 변경
            currentSteerAngles[wheelIndex] = Mathf.Lerp(currentSteerAngles[wheelIndex], targetSteerAngle, Time.deltaTime * steerSpeed);
        }
        else
        {
            // 입력이 없을 때 중앙으로 부드럽게 복귀
            currentSteerAngles[wheelIndex] = Mathf.Lerp(currentSteerAngles[wheelIndex], 0f, Time.deltaTime * returnSpeed);
        }
        
        // 바퀴 메시에 조향 각도 적용
        wheelMeshes[wheelIndex].localRotation = Quaternion.Euler(wheelMeshes[wheelIndex].localRotation.eulerAngles.x, 
                                                                currentSteerAngles[wheelIndex], 
                                                                wheelMeshes[wheelIndex].localRotation.eulerAngles.z);
    }
}
