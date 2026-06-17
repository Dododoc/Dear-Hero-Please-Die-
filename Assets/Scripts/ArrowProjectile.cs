using UnityEngine;

public class ArrowProjectile : MonoBehaviour
{
    [Header("화살 설정")]
    public float speed = 15f; 
    public int damage = 10;   

    private bool isHit = false;

    void Update()
    {
        if (!isHit)
        {
            transform.Translate(Vector3.left * speed * Time.deltaTime, Space.World);
        }
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.CompareTag("Player") && !isHit)
        {
            HeroAI hero = col.GetComponent<HeroAI>();
            if (hero != null)
            {
                // 1순위: 이미 맹렬하게 칼을 휘두르고 있다면 화살이 튕겨나감(파괴)!
                if (hero.isAttacking)
                {
                    isHit = true;
                    Destroy(gameObject);
                    return; 
                }

                // ⭐️ 2순위 수정: || hero.currentState == HeroAI.HeroState.Run 을 지웠습니다!
                // 이제 오직 걷고 있을 때(Walk)만 반사적으로 화살을 쳐냅니다.
                if (hero.currentState == HeroAI.HeroState.Walk)
                {
                    hero.StartCoroutine("Execute3HitCombo"); // 3단 베기 실행명령
                    isHit = true;
                    Destroy(gameObject); // 화살은 베어져서 소멸
                    return;
                }

                // 3순위: 멈춰 있을 때는 방패로 막음
                if (hero.currentState == HeroAI.HeroState.Stop)
                {
                    hero.GetComponent<Animator>().SetTrigger("doDefend"); 
                    Destroy(gameObject); 
                    return;
                }

                // 4순위: 무방비하게 맞았을 때 (위 조건에 하나도 안 맞을 때)
                isHit = true;
                hero.TakeDamage(damage); 
                Destroy(gameObject);
            }
        }
    }
}