using UnityEngine;
using UnityEngine.UI; // UI(Image, Text 등)를 제어하기 위해 반드시 필요합니다!

public class HUDManager : MonoBehaviour
{
    [Header("연결할 데이터")]
    public HeroAI hero;           // 체력과 마나 정보를 빼올 용사 스크립트

    [Header("UI 연결 칸")]
    public Image hpFill;          // 빨간색 피 채워지는 이미지
    public Image manaFill;        // 파란색 마나 채워지는 이미지

    void Update()
    {
        if (hero != null)
        {
            // 1. HP바 업데이트
            // 현재 체력을 최대 체력으로 나눠서 0.0 ~ 1.0 사이의 비율로 만듭니다.
            if (hpFill != null)
            {
                hpFill.fillAmount = (float)hero.currentHP / hero.maxHP;
            }

            // 2. 마나바 업데이트
            if (manaFill != null)
            {
                manaFill.fillAmount = hero.currentMana / hero.maxMana;
            }
        }
    }
}