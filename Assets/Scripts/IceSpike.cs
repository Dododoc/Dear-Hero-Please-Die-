using UnityEngine;

public class IceSpike : MonoBehaviour
{
    public float speed = 5f; 
    
    [Header("데미지 설정")]
    public int normalDamage = 10;  // 무방비로 몸에 맞았을 때의 기본 딜
    public int counterDamage = 30; // 칼로 쳤을 때 터지는 빙정의 강력한 딜

    private Animator anim;
    private bool isHit = false; 

    void Start()
    {
        anim = GetComponent<Animator>();
    }

    void Update()
    {
        if (!isHit)
        {
            transform.Translate(Vector3.left * speed * Time.deltaTime);
        }
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.CompareTag("Player") && !isHit) 
        {
            HeroAI hero = col.GetComponent<HeroAI>();
            if (hero != null)
            {
                if (hero.isAttacking)
                {
                    CounterExplode(hero);
                }
                else if (hero.currentState == HeroAI.HeroState.Stop)
                {
                    Destroy(gameObject); 
                }
                else
                {
                    NormalHit(hero);
                }
            }
        }
    }

    void CounterExplode(HeroAI hero)
    {
        isHit = true;           
        anim.SetTrigger("doHit"); 
        
        // ⭐️ 용사에게 '카운터 데미지(30)'를 정확히 전달!
        hero.TakeDamage(counterDamage); 
        
        Destroy(gameObject, 0.5f); 
    }

    void NormalHit(HeroAI hero)
    {
        isHit = true;
        
        // ⭐️ 용사에게 '일반 데미지(10)'를 정확히 전달!
        hero.TakeDamage(normalDamage); 
        
        Destroy(gameObject); 
    }
}