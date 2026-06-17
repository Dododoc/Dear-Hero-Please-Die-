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
                // 1순위: 가만히 있는 상태(Stop)일 때 -> 방패로 완벽 방어!
                if (hero.currentState == HeroAI.HeroState.Stop)
                {
                    hero.GetComponent<Animator>().SetTrigger("doDefend"); 
                    Destroy(gameObject); 
                    return; 
                }

                // ⭐️ 2순위: 걷는 중(Walk)일 때 -> 반사적으로 칼을 휘두르며 카운터 폭발!
                if (hero.currentState == HeroAI.HeroState.Walk)
                {
                    isHit = true; // ⭐️ 얼음 가시가 더 이상 다가오지 않고 즉시 그 자리에 멈추게 합니다!

                    // 레이저가 놓쳤더라도, 얼음에 닿는 순간 즉시 3단 베기를 시전합니다!
                    if (!hero.isAttacking)
                    {
                        hero.StartCoroutine("Execute3HitCombo");
                    }
                    
                    // ⭐️ 수정: 즉시 터지지 않고, 0.15초 뒤에 터지는 '코루틴'을 실행합니다.
                    StartCoroutine(DelayedCounterExplode(hero)); 
                    return; 
                }

                // 3순위: 뛰는 중(Run)이거나 무방비 상태일 때 -> 안 터지고 그냥 맞음!
                NormalHit(hero);
            }
        }
    }

    // ⭐️ 기존의 CounterExplode 함수를 코루틴으로 바꿨습니다!
    System.Collections.IEnumerator DelayedCounterExplode(HeroAI hero)
    {
        // 용사가 칼을 뻗는 모션을 보여줄 때까지 0.15초 동안 대기합니다. (타이밍에 맞게 조절 가능)
        yield return new WaitForSeconds(0.15f); 
        
        anim.SetTrigger("doHit"); // 빙정 폭발 애니메이션
        
        // ⭐️ 추가: 얼음 가시가 카운터로 터지는 이 순간에만 화면을 강하게 흔듭니다!
        if (CameraShake.instance != null)
        {
            // 빙정이 터지는 것이니 창(0.3f)보다 살짝 더 강한 진동(0.4f)을 줘도 좋습니다.
            CameraShake.instance.ShakeCamera(0.2f, 0.4f);
        }
        
        // 칼에 닿은 직후 30의 카운터 데미지를 주며 튕겨나감(Hurt)
        if (hero != null)
        {
            hero.TakeDamage(counterDamage); 
        }
        
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