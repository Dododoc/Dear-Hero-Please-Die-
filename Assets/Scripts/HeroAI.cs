using UnityEngine;
using System.Collections; 

public class HeroAI : MonoBehaviour
{
    private Animator anim;
    
    public enum HeroState { Walk = 0, Stop = 1, Run = 2 }
    public HeroState currentState = HeroState.Walk;

    private float stateTimer = 0f;
    private float stopDuration = 0f; 
    private float nextStateTime; 
    private bool isRestingAfterRun = false; 
    public bool isJumping = false; // ⭐️ 지금 공중에 있는지 확인하는 변수
    [Header("Raycast 설정")]
    public Transform rayStart; 
    public float attackRayDistance = 2.5f; 
    public float defendRayDistance = 0.5f; 
    public LayerMask trapLayer; 
    public float jumpRayDistance = 1.2f; // ⭐️ 추가: 하단 레이캐스트 전용 거리 (원하는 만큼 줄이세요!)
    // ⭐️ 1. 맨 위 변수 모여있는 곳에 점프력 조절 변수와 물리엔진(Rigidbody) 추가
    private Rigidbody2D rb;
    [Header("자동 3단 콤보 설정")]
    public float attackInterval = 0.4f;  
    public float afterComboDelay = 1.2f; 
    
    public bool isAttacking = false;
    private float generalCooldown = 0f;  
    [Header("점프 설정 (Kinematic 전용)")]
    public float jumpHeight = 2.5f; // 위로 얼마나 올라갈지 (머리 높이)
    public float jumpDuration = 0.8f; // 총 체공 시간 (올라갔다 내려오는 데 걸리는 시간)
    private GameObject lastBlockedTrap = null; 
    // ⭐️ 추가할 변수 (맨 위 변수 모음 쪽에 넣어주세요)
    public bool isPetrified = false; 
    [Header("마나 설정 ⭐️")]
    public float maxMana = 150f;
    public float currentMana = 150f;
    public float manaRecoverSpeed = 5f; // 초당 5씩 회복

    // ⭐️ 석화 실행 함수 (아무 곳이나 빈 공간에 추가)
    public void BecomePetrified(float duration)
    {
        if (!isPetrified)
        {
            StartCoroutine(PetrifyRoutine(duration));
        }
    }

    private System.Collections.IEnumerator PetrifyRoutine(float duration)
    {
        isPetrified = true;
        
        // 1. Hurt 애니메이션 재생
        anim.SetTrigger("doHurt");
        // 2. 색상을 회색으로 변경
        GetComponent<SpriteRenderer>().color = Color.gray; 

        // 3. ⭐️ 핵심: 애니메이션 속도를 0으로 만들어 그 자리에 굳게 만듭니다.
        // Hurt 애니메이션이 살짝 재생된 직후에 멈추길 원하면 0.1~0.2초 정도 기다린 후 멈춰도 좋습니다.
        float originalSpeed = anim.speed;
        anim.speed = 0; 

        // 4. 석화 시간 동안 대기 (입력이나 이동은 DetectTraps의 return 덕분에 차단됨)
        yield return new WaitForSeconds(duration);

        // 5. 석화 해제: 애니메이션 속도 복구 및 색상 복구
        anim.speed = originalSpeed;
        isPetrified = false;
        GetComponent<SpriteRenderer>().color = Color.white; 
        
        // 다시 Idle 상태로 자연스럽게 돌아가도록 트리거 하나 더 줄 수 있습니다.
        anim.SetTrigger("doIdle"); 
    }

    [Header("체력 설정 ⭐️")]
    public int maxHP = 100;      // 최대 체력
    public int currentHP;        // 현재 체력
    public bool isDead = false;  // 죽었는지 확인하는 자물쇠
    [Header("시각 효과")]
    public SpriteRenderer spriteRenderer; // 용사의 몸통 이미지 (빨갛게 물들일 대상)

    void Start()
    {
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>(); // ⭐️ 2. Start 함수 안에 추가
        // 자식 오브젝트에 있는 SpriteRenderer를 자동으로 찾아옵니다.
        spriteRenderer = GetComponentInChildren<SpriteRenderer>(); 
        
        currentHP = maxHP; 
        SetState(HeroState.Walk);
    }
    // HeroAI.cs 파일 안에 추가
public void ForceStopForEnding()
{
    // 모든 상태 변화를 막고 걷기(애니메이션상 기본 자세)로 고정
    isAttacking = true; // Update 로직 진입 방지
    currentState = HeroState.Walk;
    anim.SetInteger("State", 0); // Walk 상태로 애니메이션 고정
    anim.SetTrigger("doIdle");   // 혹은 Idle 트리거가 있다면 실행
}
    void Update()
    {
        if (!isDead && currentMana < maxMana)
        {
            currentMana += manaRecoverSpeed * Time.deltaTime;
            if (currentMana > maxMana) currentMana = maxMana; // 최대치를 넘지 않게 고정
        }
        // ⭐️ 죽었거나, 공격 중이면 다른 행동을 못하게 막음
        if (isDead || isAttacking) return;

        stateTimer += Time.deltaTime;
        if (generalCooldown > 0) generalCooldown -= Time.deltaTime;

        switch (currentState)
        {
            case HeroState.Walk:
                if (stateTimer >= nextStateTime) SetState(HeroState.Stop);
                break;
            case HeroState.Stop:
                if (!isRestingAfterRun) stopDuration += Time.deltaTime; 
                if (stateTimer >= nextStateTime)
                {
                    if (isRestingAfterRun) 
                    {
                        SetState(HeroState.Walk);
                        isRestingAfterRun = false; 
                    }
                    else 
                    {
                        float runTime = stopDuration * 1.5f; 
                        SetState(HeroState.Run, runTime);
                        stopDuration = 0f; 
                    }
                }
                break;
            case HeroState.Run:
                if (stateTimer >= nextStateTime)
                {
                    SetState(HeroState.Stop, 2f); 
                    isRestingAfterRun = true; 
                }
                break;
        }

        DetectTraps(); 
    }

    void SetState(HeroState newState, float customTime = 0f)
    {
        currentState = newState;
        stateTimer = 0f;
        anim.SetInteger("State", (int)currentState);

        lastBlockedTrap = null; 

        if (customTime > 0) nextStateTime = customTime;
        else 
        {
            if (newState == HeroState.Walk) nextStateTime = Random.Range(3f, 6f);
            else if (newState == HeroState.Stop) nextStateTime = Random.Range(2f, 4f);
        }
    }

    void DetectTraps()
{
    if (isPetrified) return;

    RaycastHit2D hitAttack = Physics2D.Raycast(rayStart.position, Vector2.right, attackRayDistance, trapLayer);
    RaycastHit2D hitDefend = Physics2D.Raycast(rayStart.position, Vector2.right, defendRayDistance, trapLayer);
    
    Vector3 footPos = rayStart.position + Vector3.down * 1.3f; 
    
    // ⭐️ 수정: attackRayDistance 대신 jumpRayDistance를 사용합니다.
    RaycastHit2D hitFoot = Physics2D.Raycast(footPos, Vector2.right, jumpRayDistance, trapLayer);
    
    Debug.DrawRay(rayStart.position, Vector2.right * attackRayDistance, Color.blue);
    Debug.DrawRay(rayStart.position + Vector3.down * 0.1f, Vector2.right * defendRayDistance, Color.red);
    
    // ⭐️ 수정: 디버그용 선 길이도 똑같이 jumpRayDistance로 맞춥니다.
    Debug.DrawRay(footPos, Vector2.right * jumpRayDistance, Color.green);
        // 걷거나 뛸 때
        if (currentState == HeroState.Walk || currentState == HeroState.Run)
        {
            // 상단 레이저: 공격 함정 감지
            if (hitAttack.collider != null)
            {
                string trapTag = hitAttack.collider.tag;
                if (trapTag == "Trap_Attack" && !isAttacking && currentState == HeroState.Walk) 
                {
                    StartCoroutine(Execute3HitCombo()); 
                }
            }

            // DetectTraps() 함수 내부의 하단 레이저 체크 부분
        if (hitFoot.collider != null)
        {
            string trapTag = hitFoot.collider.tag;
            
            // ⭐️ 수정: 'Run이 아닐 때' 대신 '확실히 Walk일 때만' 점프하고, 
            // 이미 점프 중(!isJumping)인지도 체크합니다.
            if (trapTag == "Trap_Jump" && currentState == HeroState.Walk && !isJumping && generalCooldown <= 0) 
            {
                anim.SetTrigger("doJump");
                generalCooldown = 1.0f; 
                isJumping = true; 

                // ⭐️ 걷기 속도에서 확실히 넘어갈 수 있도록 jumpHeight를 조금 높였습니다.
                StartCoroutine(KinematicJumpRoutine());
            }
        }
        }
        // 멈춰있을 때 (방패로 막기)
        else if (currentState == HeroState.Stop && hitDefend.collider != null)
        {
            GameObject currentTrap = hitDefend.collider.gameObject;
            string trapTag = currentTrap.tag;

            if (trapTag == "Trap_Unblockable" && generalCooldown <= 0) 
            {
                TakeDamage(10); 
                generalCooldown = 1.0f;
            }
            else if (trapTag != "Trap_Unblockable") 
            {
                if (currentTrap != lastBlockedTrap)
                {
                    anim.SetTrigger("doDefend"); 
                    lastBlockedTrap = currentTrap; 
                }
            }
        }
    }

    IEnumerator Execute3HitCombo()
    {
        isAttacking = true; 

        anim.SetTrigger("doAttack1");
        yield return new WaitForSeconds(attackInterval); 
        if (!isAttacking) yield break; // ⭐️ 추가: 맞아서 취소됐으면 2타 안 나가고 중단!

        anim.SetTrigger("doAttack2");
        yield return new WaitForSeconds(attackInterval); 
        if (!isAttacking) yield break; // ⭐️ 추가: 맞아서 취소됐으면 3타 안 나가고 중단!

        anim.SetTrigger("doAttack3");
        yield return new WaitForSeconds(afterComboDelay); 

        isAttacking = false; 
        yield return new WaitForSeconds(attackInterval);
    
    // 코루틴 맨 마지막에 추가
    isAttacking = true; // 엔딩이므로 다시는 Walk로 돌아가지 못하게 계속 잠금
    anim.SetInteger("State", 0); // Idle 상태 번호
    }

    public float GetSpeedMultiplier()
    {
        if (isDead) return 0f;       // ⭐️ 죽으면 배경 멈춤!
        if (isAttacking) return 0f; 
        
        if (currentState == HeroState.Walk) return 1f;
        if (currentState == HeroState.Stop) return 0.1f; 
        if (currentState == HeroState.Run) return 3f; 
        return 1f;
    }

    // ⭐️ 데미지 처리 함수 (숫자를 받아서 체력을 깎습니다)
    public void TakeDamage(int damage)
    {
        if (isDead) return; // 이미 죽었으면 무시

        currentHP -= damage; // 체력 감소
        Debug.Log("용사가 데미지를 입었습니다! 남은 체력: " + currentHP);

        isAttacking = false;

        // 체력이 0 이하가 되면 사망!
        if (currentHP <= 0)
        {
            isDead = true;
            anim.SetTrigger("doDie"); // 사망 애니메이션 실행
            Debug.Log("용사 사망!");
        }
        else // 아직 살아있으면 아파하기만 함
        {
            anim.SetTrigger("doHurt");
            SetState(HeroState.Walk); 
        }
    }
    // ⭐️ 출혈을 시작하는 함수
    public void StartBleeding(int tickDamage, int tickCount, float tickInterval)
    {
        if (isDead) return;
        StartCoroutine(BleedRoutine(tickDamage, tickCount, tickInterval));
    }

    // 출혈 틱 데미지를 관리하는 코루틴
    IEnumerator BleedRoutine(int tickDamage, int tickCount, float tickInterval)
    {
        for (int i = 0; i < tickCount; i++)
        {
            if (isDead) yield break; 
            
            // ⭐️ 틱 데미지가 들어올 때 잠깐 움찔(Hurt)하게 하고 싶다면 아래 순서가 좋습니다.
            TakeDamage(tickDamage);
            StartCoroutine(FlashRed());
            
            yield return new WaitForSeconds(tickInterval); 
        }
    }
    // ⭐️ 0.2초 동안 빨간색으로 번쩍이는 효과 코루틴
    // 0.2초 동안 빨간색으로 번쩍이는 효과 코루틴
    IEnumerator FlashRed()
    {
        if (spriteRenderer != null)
        {
            spriteRenderer.color = Color.red; 
            yield return new WaitForSeconds(0.2f); 
            
            // ⭐️ 디테일: 석화 중일 때는 흰색이 아니라 회색으로 복구!
            if (isPetrified)
            {
                spriteRenderer.color = Color.gray; 
            }
            else
            {
                spriteRenderer.color = Color.white; 
            }
        }
    }
    // ⭐️ 0.8초 뒤에 자동으로 실행될 함수
    void ResetJump()
    {
        isJumping = false;
    }
    // ⭐️ 중력 없이 코드로만 올라갔다 내려오는 완벽한 점프 곡선!
    IEnumerator KinematicJumpRoutine()
    {
        Vector3 startPos = transform.position; // 점프 시작(원래 바닥) 위치
        Vector3 targetPos = startPos + new Vector3(0, jumpHeight, 0); // 최고점 위치

        float halfDuration = jumpDuration / 2f; // 올라가는 데 절반, 내려오는 데 절반
        float elapsedTime = 0f;

        // 1. 위로 올라가기
        while (elapsedTime < halfDuration)
        {
            // 부드럽게 감속하며 올라감 (Lerp)
            transform.position = Vector3.Lerp(startPos, targetPos, (elapsedTime / halfDuration));
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        transform.position = targetPos; // 최고점 도달 쾅!

        // 2. 아래로 떨어지기
        elapsedTime = 0f;
        while (elapsedTime < halfDuration)
        {
            // 가속하며 내려옴
            transform.position = Vector3.Lerp(targetPos, startPos, (elapsedTime / halfDuration));
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        transform.position = startPos; // 원래 자리 복구 완료!

        isJumping = false; // 점프 끝! (이제 다시 맞거나 뛸 수 있음)
    }
}