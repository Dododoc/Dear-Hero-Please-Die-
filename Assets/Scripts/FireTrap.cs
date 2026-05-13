using UnityEngine;

public class FireTrap : MonoBehaviour
{
    [Header("불꽃 설정")]
    public float speed = 3f; // 다가오는 속도
    public int damage = 15;  // 화상 데미지

    private bool isHit = false;

    void Update()
    {
        if (!isHit)
        {
            // 불꽃이 서서히 다가옵니다.
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
                // 1. 용사가 기를 모으며 방패로 막고 있을 때 (Stop 상태) -> 완벽히 방어!
                if (hero.currentState == HeroAI.HeroState.Stop)
                {
                    Destroy(gameObject); // 불꽃이 막혀서 꺼짐 (데미지 X)
                }
                // 2. 걷거나, 뛰거나, 심지어 '공격(칼질) 중'일 때 -> 칼로 안 잘리므로 데미지!
                else
                {
                    isHit = true;
                    hero.TakeDamage(damage); // 화상 데미지
                    Destroy(gameObject);     // 타격 후 소멸
                }
            }
        }
    }
}