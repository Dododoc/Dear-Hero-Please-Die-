using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class GameManager : MonoBehaviour
{
    [Header("거리 설정")]
    public float totalDistance = 500f;
    private float traveledDistance = 0f;
    private float remainingDistance = 500f; // 매 프레임 갱신, 결과 화면에 전달

    [Header("배경 제어")]
    public ParallaxBackground farBackground;
    public ParallaxBackground nearBackground;
    private int currentPhase = 0;

    [Header("연결")]
    public HeroAI hero;
    public TextMeshProUGUI distanceText;
    public GameObject demonKing;

    [Header("엔딩 연출")]
    public Image fadePanel;
    public float fadeDuration = 2f;
    public float appearDistance = 10f;

    [Header("결과 화면")]
    public ResultManager resultManager;

    private bool isEndingStarted = false;
    private bool isDemonKingActive = false;

    void Start()
    {
        remainingDistance = totalDistance;
    }

    void Update()
    {
        if (isEndingStarted) return;

        // ── 승리: 용사가 죽은 경우 ─────────────────────────────────
        if (hero.isDead)
        {
            isEndingStarted = true;
            // remainingDistance는 이 순간 값을 그대로 사용 (매 프레임 갱신됨)
            StartCoroutine(FadeAndShowResult(isVictory: true));
            return;
        }

        // ── 거리 계산 ──────────────────────────────────────────────
        float speedMult = hero.GetSpeedMultiplier();
        float speed = 2f * speedMult;
        traveledDistance += speed * Time.deltaTime;
        remainingDistance = Mathf.Max(0f, totalDistance - traveledDistance);

        // ── 배경 페이즈 전환 ────────────────────────────────────────
        if (currentPhase == 0 && remainingDistance <= 100f)
        {
            currentPhase = 1;
            farBackground.ChangeBackground(1);
            if (nearBackground != null) nearBackground.ChangeBackground(1);
        }
        else if (currentPhase == 1 && remainingDistance <= 50f)
        {
            currentPhase = 2;
            farBackground.ChangeBackground(2);
            if (nearBackground != null) nearBackground.ChangeBackground(2);
        }

        if (distanceText != null)
            distanceText.text = "Distance: " + Mathf.FloorToInt(remainingDistance) + "m";

        // ── 마왕 등장 ───────────────────────────────────────────────
        if (!isDemonKingActive && remainingDistance <= appearDistance)
        {
            isDemonKingActive = true;
            demonKing.SetActive(true);
            demonKing.transform.position = new Vector3(
                hero.transform.position.x + 15f,
                demonKing.transform.position.y, 0);
        }

        if (isDemonKingActive && !isEndingStarted)
            demonKing.transform.Translate(Vector3.left * speed * Time.deltaTime);

        // ── 패배: 거리가 0이 된 경우 ───────────────────────────────
        if (remainingDistance <= 0f)
        {
            remainingDistance = 0f;
            StartEndingSequence();
        }
    }

    void StartEndingSequence()
    {
        isEndingStarted = true;
        hero.ForceStopForEnding();

        if (demonKing != null)
        {
            demonKing.transform.position = new Vector3(
                hero.transform.position.x + 1.5f,
                demonKing.transform.position.y, 0);
            demonKing.SetActive(true);
            demonKing.GetComponent<DemonKing>().StartFinalConfrontation(hero);
        }

        StartCoroutine(FadeAndShowResult(isVictory: false));
    }

    public void StartFadeOut()
    {
        StartCoroutine(FadeOutOnly());
    }

    IEnumerator FadeOutOnly()
    {
        yield return DoFadeOut();
    }

    IEnumerator FadeAndShowResult(bool isVictory)
    {
        yield return new WaitForSeconds(1.0f);
        yield return DoFadeOut();

        if (resultManager != null)
        {
            float heroHpLeft = (hero != null) ? hero.currentHP : 0f;
            float heroMaxHp  = (hero != null) ? hero.maxHP     : 100f;

            // 남은 거리 기준으로 별 판정
            resultManager.ShowResult(
                isVictory,
                remainingDistance,  // 용사를 잡은 순간의 남은 거리
                totalDistance,      // 총 거리 (500m)
                heroHpLeft,
                heroMaxHp
            );
        }
    }

    IEnumerator DoFadeOut()
    {
        if (fadePanel == null) yield break;

        Color c = fadePanel.color;
        float elapsed = 0f;
        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            c.a = Mathf.Clamp01(elapsed / fadeDuration);
            fadePanel.color = c;
            yield return null;
        }
    }
}
