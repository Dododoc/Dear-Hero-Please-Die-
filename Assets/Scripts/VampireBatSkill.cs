using UnityEngine;

public class VampireBatSkill : MonoBehaviour
{
    [Header("박쥐 설정")]
    public float speed = 4f;            // 날아가는 속도
    public float petrifyDuration = 2f;  // 용사가 돌이 되어 굳어있는 시간 (2초)

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
                // 1. 용사가 칼로 공격 중일 때 (isAttacking) -> 박쥐가 썰림!
                if (hero.isAttacking)
                {
                    isHit = true;
                    // (여기에 박쥐가 펑! 터지는 이펙트를 나중에 넣으셔도 좋습니다)
                    Destroy(gameObject);
                }
                // 2. 용사가 방패로 막거나(Stop), 걷거나 뛰는 무방비 상태일 때 -> 석화!
                else
                {
                    isHit = true;
                    hero.BecomePetrified(petrifyDuration); // 돌로 만들어버립니다
                    Destroy(gameObject);
                }
            }
        }
    }
}