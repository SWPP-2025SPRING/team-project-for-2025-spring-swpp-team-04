using UnityEngine;

public class LapManager : MonoBehaviour
{
    private bool hasStarted = false;
    private float startTime;
    private float endTime;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (!hasStarted)
            {
                // 게임 시작
                startTime = Time.time;
                hasStarted = true;
                Debug.Log("게임 시작!");
            }
            else
            {
                // 게임 종료
                endTime = Time.time;
                float totalTime = endTime - startTime;
                Debug.Log("게임 종료! 총 시간: " + totalTime + "초");
            }
        }
    }
}
