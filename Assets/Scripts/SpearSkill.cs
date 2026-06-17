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
                // 공중에서 맞췄을 때!
                if (hero.isJumping)
                {
                    isHit = true; 
                    hero.TakeDamage(damage); 
                    
                    // ⭐️ 추가: 창이 몸에 꽂히는 순간 화면이 슬로우 모션으로 느려집니다!
                    hero.TriggerSlowMotion(); 
                    // ⭐️ 추가: 카메라를 0.2초 동안 0.3의 강도로 강하게 흔듭니다!
                    if (CameraShake.instance != null)
                    {
                        CameraShake.instance.ShakeCamera(0.2f, 0.3f);
                    }
                    Destroy(gameObject); 
                }
            }
        }
    }

    void OnBecameInvisible()
    {
        Destroy(gameObject);
    }
}