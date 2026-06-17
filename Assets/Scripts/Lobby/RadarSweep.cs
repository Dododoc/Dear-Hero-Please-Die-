using UnityEngine;

public class RadarSweep : MonoBehaviour
{
    [Header("레이더 회전 속도 (마이너스는 시계방향)")]
    public float rotationSpeed = -150f;

    void Update()
    {
        // eulerAngles.z 만 직접 증가 → X/Y 축 틀어짐 없이 순수 2D 평면 회전
        float newZ = transform.eulerAngles.z + rotationSpeed * Time.deltaTime;
        transform.eulerAngles = new Vector3(0f, 0f, newZ);
    }
}
