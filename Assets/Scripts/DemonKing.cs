using UnityEngine;
using System.Collections;

public class DemonKing : MonoBehaviour
{
    private Animator anim;

    void Start()
    {
        anim = GetComponent<Animator>();
    }

    public void StartFinalConfrontation(HeroAI hero)
    {
        StartCoroutine(DeathSequence(hero));
    }

    IEnumerator DeathSequence(HeroAI hero)
    {
        // 1. 용사에게 3단 콤보 공격 명령 (전체 약 2초 소요)
        hero.StartCoroutine("Execute3HitCombo");

        // 2. 칼이 마왕에게 닿는 타이밍 (약 0.6초 뒤)
        yield return new WaitForSeconds(0.6f); 
        
        if (anim != null)
        {
            anim.SetTrigger("doDie"); // 마왕 사망 애니메이션 실행
        }

        // ⭐️ 3. 불필요한 대기 시간을 없애고, 마왕이 베이는 즉시 페이드 아웃 시작!
        FindObjectOfType<GameManager>().StartFadeOut();

        // ⭐️ 4. 용사 걷기 버그 완벽 차단!
        // 용사의 3단 콤보가 완전히 끝나는 시간(약 1.4~1.5초)을 마저 기다립니다.
        yield return new WaitForSeconds(1.5f);

        // 콤보가 끝난 직후, 용사가 걷기(Walk) 상태로 돌아가기 전에 강제로 영구 정지시킵니다.
        if (hero != null)
        {
            hero.isAttacking = true; // Update 문의 이동 로직 차단
            hero.GetComponent<Animator>().SetInteger("State", 0); // Idle(대기) 자세로 고정
            hero.GetComponent<Animator>().SetTrigger("doIdle"); // 혹시 모를 모션 튀어오름 방지
        }
    }
}