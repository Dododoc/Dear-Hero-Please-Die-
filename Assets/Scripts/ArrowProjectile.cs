using UnityEngine;

public class ArrowProjectile : MonoBehaviour
{
    [Header("화살 설정")]
    public float speed = 15f; // ⭐️ 속도 조절 가능! (기본값을 15로 아주 빠르게 설정)
    public int damage = 10;   // 화살 1발당 데미지 (3발 다 맞으면 30!)

    private bool isHit = false;

    void Update()
    {
        if (!isHit)
        {
            // 화살은 꺾이지 않고 무조건 왼쪽으로 광속 직진!
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
                // ⭐️ 1순위: 용사가 맹렬하게 칼을 휘두르는 중이면 화살이 튕겨나감(파괴)!
                if (hero.isAttacking)
                {
                    isHit = true;
                    Destroy(gameObject);
                    return; 
                }

                // 2순위: 방패로 막고 있을 때
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