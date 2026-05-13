using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class GameManager : MonoBehaviour
{
    [Header("거리 설정")]
    public float totalDistance = 500f; 
    private float traveledDistance = 0f;
    
    [Header("연결")]
    public HeroAI hero;
    public TextMeshProUGUI distanceText;     
    public GameObject demonKing; 

    [Header("엔딩 연출")]
    public Image fadePanel;         
    public float fadeDuration = 2f; 
    public float appearDistance = 10f; // 마왕이 보이기 시작할 남은 거리

    private bool isEndingStarted = false;
    private bool isDemonKingActive = false;

    void Update()
    {
        if (isEndingStarted || hero.isDead) return;

        float currentSpeedMultiplier = hero.GetSpeedMultiplier();
        float currentSpeed = 2f * currentSpeedMultiplier; 
        traveledDistance += currentSpeed * Time.deltaTime;

        float remainingDistance = Mathf.Max(0, totalDistance - traveledDistance);
        
        if (distanceText != null)
            distanceText.text = "Distance: " + Mathf.FloorToInt(remainingDistance) + "m";

        // ⭐️ 1. 마왕 등장 예고 (10m 남았을 때)
        if (!isDemonKingActive && remainingDistance <= appearDistance)
        {
            isDemonKingActive = true;
            demonKing.SetActive(true);
            // 마왕의 초기 위치를 화면 오른쪽 끝 바깥으로 설정
            demonKing.transform.position = new Vector3(hero.transform.position.x + 15f, demonKing.transform.position.y, 0);
        }

        // ⭐️ 2. 마왕이 활성화된 상태라면, 용사가 다가가는 느낌을 위해 마왕을 왼쪽으로 이동시킴
        if (isDemonKingActive && !isEndingStarted)
        {
            // 배경과 똑같은 속도로 왼쪽으로 이동
            demonKing.transform.Translate(Vector3.left * currentSpeed * Time.deltaTime);
        }

        // ⭐️ 3. 완전히 도달 (0m)
        if (remainingDistance <= 0)
        {
            remainingDistance = 0;
            StartEndingSequence();
        }
    }

    // GameManager.cs의 StartEndingSequence 수정
void StartEndingSequence()
{
    isEndingStarted = true;
    
    // 1. 용사의 상태를 강제로 고정 (Run 상태 탈출)
    hero.ForceStopForEnding();

    if (demonKing != null)
    {
        // 2. 마왕을 용사의 코앞(x+1.5f 정도)으로 순간이동 시켜 위치 오차 해결
        // 1.5f는 캐릭터들의 콜라이더 크기에 따라 1.2f ~ 1.8f 사이로 조절해보세요.
        demonKing.transform.position = new Vector3(hero.transform.position.x + 1.5f, demonKing.transform.position.y, 0);
        
        demonKing.SetActive(true);
        // 3. 마왕사망 및 페이드아웃 시퀀스 시작
        demonKing.GetComponent<DemonKing>().StartFinalConfrontation(hero);
    }
}

    public void StartFadeOut()
    {
        StartCoroutine(FadeOutRoutine());
    }

    IEnumerator FadeOutRoutine()
    {
        yield return new WaitForSeconds(1.0f);
        if (fadePanel != null)
        {
            Color panelColor = fadePanel.color;
            float elapsedTime = 0f;
            while (elapsedTime < fadeDuration)
            {
                elapsedTime += Time.deltaTime;
                panelColor.a = Mathf.Clamp01(elapsedTime / fadeDuration);
                fadePanel.color = panelColor;
                yield return null;
            }
        }
    }
}