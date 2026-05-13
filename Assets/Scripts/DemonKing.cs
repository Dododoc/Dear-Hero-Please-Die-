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
        // 1. 용사에게 3단 콤보 공격 명령
        hero.StartCoroutine("Execute3HitCombo");

        // 2. 용사가 칼을 휘두르며 마왕을 베는 타이밍 (약 0.6초 뒤)
        yield return new WaitForSeconds(0.6f); 
        if (anim != null)
        {
            anim.SetTrigger("doDie"); // 마왕 사망
        }

        // ⭐️ 3. 공격이 완전히 끝나는 시간을 기다립니다.
        // 용사의 3단 콤보 애니메이션 길이에 맞춰 약 1.5초~2초 정도 대기합니다.
        yield return new WaitForSeconds(1.5f);

        // ⭐️ 4. 공격이 끝난 용사를 Idle(대기) 상태로 강제 고정!
        // HeroAI의 애니메이터 컨트롤러에서 State 0이 보통 Idle/Walk입니다.
        hero.GetComponent<Animator>().SetInteger("State", 0); 
        hero.GetComponent<Animator>().SetTrigger("doIdle"); // doIdle 트리거가 있다면 실행

        // 5. 마왕이 쓰러진 채로 1.5초 더 대기 (총 3초 느낌) 후 페이드 아웃
        yield return new WaitForSeconds(1.5f);
        FindObjectOfType<GameManager>().StartFadeOut();
    }
}