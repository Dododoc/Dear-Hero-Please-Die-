using UnityEngine;

public class ParallaxBackground : MonoBehaviour
{
    public HeroAI heroLogic; // 용사 스크립트 연결
    
    [Header("배경 레이어 설정")]
    public Transform farBackground; // 맨 뒤 (하늘, 산 등)
    public Transform midBackground; // 중간 (바위, 나무 등)
    
    public float baseSpeed = 2f; // 기본 스크롤 속도

    void Update()
    {
        // 용사의 상태(걷기, 멈춤, 뛰기)에 따라 속도 배율 가져오기
        float speedMultiplier = heroLogic.GetSpeedMultiplier();
        float currentSpeed = baseSpeed * speedMultiplier;

        // 거리에 따라 다르게 움직임 (앞에 있는게 더 빨리 지나감)
        farBackground.Translate(Vector3.left * currentSpeed * 0.5f * Time.deltaTime);
        midBackground.Translate(Vector3.left * currentSpeed * 1.0f * Time.deltaTime);

        // 배경이 일정 범위를 벗어나면 다시 오른쪽으로 보내는 무한 스크롤 로직은 
        // 배경 이미지의 크기에 맞춰서 추가해주시면 됩니다!
    }
}