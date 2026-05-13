using UnityEngine;

public class SpikeTrap : MonoBehaviour
{
    [Header("이동 및 연동 설정")]
    public float baseSpeed = 2f; // ⭐️ 배경(ParallaxBackground)과 똑같은 기본 속도로 맞춰주세요!
    private HeroAI heroLogic;    // 용사 스크립트 (속도를 훔쳐올 대상)

    [Header("함정 작동 설정")]
    public float triggerDistance = 4f; // 용사가 이 거리만큼 가까워지면 가시가 튀어나옴
    public int damage = 20;            // 밟았을 때 데미지

    private Animator anim;
    private bool isTriggered = false; // 가시가 한 번만 튀어나오게 하는 자물쇠
    private bool hasDamaged = false; // ⭐️ 추가: 데미지를 한 번만 주게 하는 자물쇠!

    void Start()
    {
        anim = GetComponent<Animator>();
        
        // 씬 뷰에 용사가 있다면 자동으로 찾아서 연결해 주는 해커톤용 꿀팁 코드입니다!
        heroLogic = FindObjectOfType<HeroAI>();
    }

    void Update()
    {
        MoveLikeBackground();
        CheckProximity();
    }

    // 1. 배경처럼 용사 속도에 맞춰 움직이는(혹은 멈추는) 함수
    void MoveLikeBackground()
    {
        if (heroLogic != null)
        {
            // 용사가 뛰면 3배, 멈추면 0.1배, 공격 중엔 0배가 됩니다.
            float speedMultiplier = heroLogic.GetSpeedMultiplier();
            float currentSpeed = baseSpeed * speedMultiplier;

            // Space.World 기준으로 왼쪽으로 이동
            transform.Translate(Vector3.left * currentSpeed * Time.deltaTime, Space.World);
        }
    }

    // 2. 용사가 가까이 왔는지 거리를 재고 애니메이션을 트는 함수
    void CheckProximity()
    {
        if (!isTriggered && heroLogic != null)
        {
            // 용사와 함정 사이의 X축 거리 계산
            float distance = Mathf.Abs(heroLogic.transform.position.x - transform.position.x);

            // 거리가 설정값(4f) 안으로 좁혀지면 가시 발동!
            if (distance <= triggerDistance)
            {
                isTriggered = true;
                anim.SetTrigger("doSpike"); // 가시가 촥! 솟아오르는 애니메이션
            }
        }
    }

    // 3. 용사가 함정을 밟았을 때 데미지 주기 (사라지지 않음!)
    // OnTriggerEnter2D() 함수 수정
void OnTriggerEnter2D(Collider2D col)
{
    // ⭐️ 수정: 용사가 점프 중이 아닐 때(!heroLogic.isJumping)만 데미지를 입힙니다.
    if (col.CompareTag("Player") && !hasDamaged)
    {
        if (heroLogic != null && !heroLogic.isJumping)
        {
            hasDamaged = true; 
            heroLogic.TakeDamage(damage);
        }
        // 점프 중이라면 hasDamaged를 true로 만들지 않아, 
        // 혹시나 착지 지점이 겹쳤을 때만 데미지를 입게 할 수도 있습니다.
    }
}

    // 4. 카메라(화면) 밖으로 완전히 나가면 스스로 삭제하는 유니티 내장 함수
    void OnBecameInvisible()
    {
        Destroy(gameObject);
        Debug.Log("화면 밖으로 나간 가시 함정 삭제 완료!");
    }
}