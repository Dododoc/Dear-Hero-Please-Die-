using UnityEngine;
using System.Collections;

public class CameraShake : MonoBehaviour
{
    // 어디서든 쉽게 불러다 쓸 수 있게 static으로 만듭니다.
    public static CameraShake instance;
    private Vector3 originalPos;

    void Awake()
    {
        instance = this;
        originalPos = transform.localPosition;
    }

    public void ShakeCamera(float duration, float magnitude)
    {
        StartCoroutine(ShakeRoutine(duration, magnitude));
    }

    IEnumerator ShakeRoutine(float duration, float magnitude)
    {
        float elapsed = 0.0f;

        while (elapsed < duration)
        {
            // 화면을 상하좌우로 무작위로 마구 흔듭니다!
            float x = Random.Range(-1f, 1f) * magnitude;
            float y = Random.Range(-1f, 1f) * magnitude;

            transform.localPosition = new Vector3(originalPos.x + x, originalPos.y + y, originalPos.z);

            // Time.timeScale이 느려진 상태(슬로우 모션)에서도 
            // 카메라는 현실 시간 기준으로 제대로 흔들리게 합니다.
            elapsed += Time.unscaledDeltaTime; 
            yield return null;
        }

        transform.localPosition = originalPos; // 진동이 끝나면 원래 자리로 복구
    }
}