using UnityEngine;

public class SpearSkill : MonoBehaviour
{
    [Header("창 투척 설정")]
    public float speedX = 7f;         
    public float initialSpeedY = 8f;  
    public float gravity = 15f;       
    public int damage = 40;           
    
    // ⭐️ 추가: 이미지 각도를 맞추기 위한 보정값 (유니티에서 수정 가능!)
    public float rotationOffset = 45f; 

    private float currentSpeedY;
    private bool isHit = false;

    void Start()
    {
        currentSpeedY = initialSpeedY; 
    }

    void Update()
    {
        if (!isHit)
        {
            currentSpeedY -= gravity * Time.deltaTime;
            transform.Translate(new Vector3(-speedX, currentSpeedY, 0) * Time.deltaTime, Space.World);

            // ⭐️ 기존 계산식 끝에 '+ rotationOffset'을 추가했습니다.
            float angle = Mathf.Atan2(currentSpeedY, -speedX) * Mathf.Rad2Deg - 180f + rotationOffset;
            transform.rotation = Quaternion.Euler(0, 0, angle);
        }
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.CompareTag("Player") && !isHit)
        {
            HeroAI hero = col.GetComponent<HeroAI>();
            if (hero != null)
            {
                // ⭐️ 주의: 대공 창이니까 용사가 '점프 중'일 때만 데미지를 입고 사라져야 합니다!
                if (hero.isJumping)
                {
                    isHit = true; 
                    hero.TakeDamage(damage); 
                    Destroy(gameObject); 
                }
                // 점프 중이 아니라면(걸어가는 중이면) if문을 무시하고 용사를 그냥 통과합니다.
            }
        }
    }

    void OnBecameInvisible()
    {
        Destroy(gameObject);
    }
}