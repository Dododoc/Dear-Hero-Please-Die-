using UnityEngine;

public class AxeSkill : MonoBehaviour
{
    public float speed = 5f;        // 날아가는 속도
    public float spinSpeed = 720f;  // 회전 속도 (숫자가 클수록 빨리 돕니다)
    
    public int directDamage = 15;   // 맞았을 때 즉시 들어가는 데미지
    public int bleedDamage = 5;     // 출혈 1틱당 데미지
    public int bleedTicks = 3;      // 출혈을 몇 번 발생시킬 것인가? (3번)
    public float bleedInterval = 1f;// 출혈 간격 (1초마다)

    private bool isHit = false;

    void Update()
    {
        if (!isHit)
        {
            // ⭐️ Space.World를 써야 도끼가 회전해도 위아래로 꺾이지 않고 왼쪽으로만 직진합니다!
            transform.Translate(Vector3.left * speed * Time.deltaTime, Space.World);
            
            // 빙글빙글 회전 (Z축 기준)
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
                    return; 
                }

                // 2순위: 방패로 막을 때
                if (hero.currentState == HeroAI.HeroState.Stop)
                {
                    Destroy(gameObject); 
                    return;
                }
                
                // 3순위: 맞았을 때 (출혈 포함)
                isHit = true;
                hero.TakeDamage(directDamage);
                hero.StartBleeding(bleedDamage, bleedTicks, bleedInterval);
                Destroy(gameObject);
            }
        }
    }
}