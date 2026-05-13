using UnityEngine;

public class BatSkill : MonoBehaviour
{
    [Header("방망이 설정")]
    public float speed = 6f;        // 날아가는 속도 (도끼보다 살짝 빠르거나 다르게 조절해 보세요!)
    public float spinSpeed = 500f;  // 빙글빙글 도는 속도
    public int damage = 25;         // 출혈이 없는 대신 한 방 데미지를 묵직하게!

    private bool isHit = false;

    void Update()
    {
        if (!isHit)
        {
            // ⭐️ 도끼와 마찬가지로 Space.World를 써야 위아래로 꺾이지 않고 곧게 왼쪽으로 날아갑니다.
            transform.Translate(Vector3.left * speed * Time.deltaTime, Space.World);
            
            // Z축 기준으로 빙글빙글 회전
            transform.Rotate(0, 0, spinSpeed * Time.deltaTime);
        }
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.CompareTag("Player") && !isHit)
        {
            HeroAI hero = col.GetComponent<HeroAI>();
            if (hero != null)
            {
                // ⭐️ 1순위: 용사가 공격 중일 때 (데미지 없이 파괴!)
                if (hero.isAttacking)
                {
                    isHit = true;
                    Destroy(gameObject);
                    return; // ⭐️ return을 쓰면 데미지 코드까지 안 내려가고 여기서 끝납니다!
                }

                // 2순위: 용사가 방패로 막고 있을 때
                if (hero.currentState == HeroAI.HeroState.Stop)
                {
                    Destroy(gameObject);
                    return;
                }
                
                // 3순위: 무방비하게 맞았을 때
                isHit = true;
                hero.TakeDamage(damage); 
                Destroy(gameObject);
            }
        }
    }
}