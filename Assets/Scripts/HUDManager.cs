using UnityEngine;
using UnityEngine.UI; 
using TMPro; // ⭐️ 1. 텍스트 프로 사용을 위해 상단에 꼭 추가!

public class HUDManager : MonoBehaviour
{
    [Header("연결할 데이터")]
    public HeroAI hero;           

    [Header("UI 연결 칸")]
    public Image hpFill;          
    public Image manaFill;        
    
    // ⭐️ 2. 유니티에서 글자를 띄워줄 TMPro 텍스트 칸을 만듭니다.
    public TextMeshProUGUI manaText; 

    void Update()
    {
        if (hero != null)
        {
            if (hpFill != null)
            {
                hpFill.fillAmount = (float)hero.currentHP / hero.maxHP;
            }

            if (manaFill != null)
            {
                manaFill.fillAmount = hero.currentMana / hero.maxMana;
            }

            // ⭐️ 3. 소수점은 버리고 정수 형태로 "150 / 150" 문자를 UI에 표시합니다!
            if (manaText != null)
            {
                manaText.text = $"{Mathf.FloorToInt(hero.currentMana)} / {Mathf.FloorToInt(hero.maxMana)}";
            }
        }
    }
}