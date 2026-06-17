using UnityEngine;

public class GroundTreadmill : MonoBehaviour
{
    [Header("연결 설정")]
    public HeroAI heroLogic;
    public float baseSpeed = 2f; // ⭐️ 함정(SpikeTrap)의 baseSpeed와 무조건 똑같이 맞춰주세요!

    [Header("타일맵 덩어리 2개")]
    public Transform ground1;
    public Transform ground2;

    [Header("바닥 1개의 길이")]
    public float groundWidth = 18f; // ⭐️ 아까 ground2를 오른쪽으로 밀어준 X좌표 값을 적습니다!

    void Update()
    {
        if (heroLogic == null) return;

        // 용사의 상태(걷기, 멈춤, 뛰기)에 따라 바닥이 뒤로 밀리는 속도 결정
        float currentSpeed = baseSpeed * heroLogic.GetSpeedMultiplier();

        // 두 바닥을 동시에 왼쪽으로 이동!
        ground1.Translate(Vector3.left * currentSpeed * Time.deltaTime, Space.World);
        ground2.Translate(Vector3.left * currentSpeed * Time.deltaTime, Space.World);

        // ⭐️ 핵심: 화면 왼쪽 밖으로 완전히 나간 바닥을 다시 맨 오른쪽 꼬리로 순간이동!
        if (ground1.position.x <= -groundWidth)
        {
            ground1.position += new Vector3(groundWidth * 2f, 0, 0);
        }
        
        if (ground2.position.x <= -groundWidth)
        {
            ground2.position += new Vector3(groundWidth * 2f, 0, 0);
        }
    }
}