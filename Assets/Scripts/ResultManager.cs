using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections;

public class ResultManager : MonoBehaviour
{
    [Header("결과 패널 루트")]
    public GameObject resultPanel;

    [Header("타이틀")]
    public TextMeshProUGUI titleText;
    public TextMeshProUGUI subtitleText;

    [Header("별 3개 (Image 배열)")]
    public Image[] starImages;
    public Sprite starOn;
    public Sprite starOff;

    [Header("보상 UI")]
    public TextMeshProUGUI coinText;
    public TextMeshProUGUI diamondText;
    public GameObject diamondRow;

    [Header("버튼")]
    public Button retryButton;
    public Button lobbyButton;

    [Header("별 조건 - 남은 거리 비율 (승리 시 기준)")]
    [Tooltip("총 거리 대비 이 비율 이상 남아있으면 ★★ (예: 0.3 = 30% = 150m/500m)")]
    public float star2DistanceRatio = 0.3f;
    [Tooltip("총 거리 대비 이 비율 이상 남아있으면 ★★★ (예: 0.6 = 60% = 300m/500m)")]
    public float star3DistanceRatio = 0.6f;

    [Header("보상 기준값")]
    public int baseVictoryCoin = 100;
    public int baseDiamond = 10;
    public float defeatRewardRatio = 0.3f;

    // 별 TMP 색상 (스프라이트 없을 때)
    static readonly Color StarOnColor  = new Color(1.0f, 0.82f, 0.08f, 1f);
    static readonly Color StarOffColor = new Color(0.28f, 0.24f, 0.18f, 1f);

    private int _earnedStars;

    void Start()
    {
        if (resultPanel != null) resultPanel.SetActive(false);
        if (retryButton != null) retryButton.onClick.AddListener(OnRetry);
        if (lobbyButton != null) lobbyButton.onClick.AddListener(OnLobby);
    }

    // ── 외부 진입점 ──────────────────────────────────────────────────
    /// <param name="remainingDistance">용사를 잡은 순간 남은 거리 (m)</param>
    /// <param name="totalDistance">스테이지 총 거리 (m)</param>
    public void ShowResult(bool isVictory,
                           float remainingDistance, float totalDistance,
                           float heroHpLeft, float heroMaxHp)
    {
        StartCoroutine(ShowRoutine(isVictory, remainingDistance, totalDistance, heroHpLeft, heroMaxHp));
    }

    IEnumerator ShowRoutine(bool isVictory,
                             float remainingDistance, float totalDistance,
                             float heroHpLeft, float heroMaxHp)
    {
        yield return new WaitForSeconds(0.3f);

        if (resultPanel != null) resultPanel.SetActive(true);

        // ── 타이틀 & 부제 ──────────────────────────────────────────
        if (isVictory)
        {
            int remainM = Mathf.RoundToInt(remainingDistance);
            SetText(titleText,    "용사 퇴근 완료! ⚰️");
            SetText(subtitleText, $"{remainM}m 전방에서 처치!");
            titleText.color = new Color(1f, 0.92f, 0.28f, 1f);
        }
        else
        {
            int pct = Mathf.RoundToInt((1f - Mathf.Clamp01(heroHpLeft / heroMaxHp)) * 100f);
            SetText(titleText,    "마왕님, 도망치세요! 💀");
            SetText(subtitleText, $"용사의 HP를 {pct}% 깎았습니다");
            titleText.color = new Color(0.9f, 0.3f, 0.3f, 1f);
        }

        // ── 별 계산 & 초기화 ────────────────────────────────────────
        _earnedStars = CalcStars(isVictory, remainingDistance, totalDistance);
        SetAllStars(false);

        // ── 별 하나씩 팡 연출 ────────────────────────────────────────
        for (int i = 0; i < _earnedStars; i++)
        {
            yield return new WaitForSeconds(0.35f);
            if (i < starImages.Length && starImages[i] != null)
            {
                SetStar(starImages[i], true);
                StartCoroutine(StarPopAnim(starImages[i].transform));
            }
        }

        yield return new WaitForSeconds(0.2f);
        ShowRewards(isVictory, heroHpLeft, heroMaxHp);
    }

    // ── 별 계산 ─────────────────────────────────────────────────────
    // 총 거리 500m 기준 예시:
    //   ★     : 승리 (거리 무관)
    //   ★★   : 남은 거리 30% 이상 (≥150m)
    //   ★★★ : 남은 거리 60% 이상 (≥300m)
    int CalcStars(bool isVictory, float remainingDistance, float totalDistance)
    {
        if (!isVictory) return 0;

        float ratio = (totalDistance > 0f) ? remainingDistance / totalDistance : 0f;

        if (ratio >= star3DistanceRatio) return 3;
        if (ratio >= star2DistanceRatio) return 2;
        return 1;
    }

    // ── 별 On/Off ────────────────────────────────────────────────────
    void SetAllStars(bool on)
    {
        for (int i = 0; i < starImages.Length; i++)
            if (starImages[i] != null) SetStar(starImages[i], on);
    }

    void SetStar(Image img, bool on)
    {
        if (starOn != null && starOff != null)
        {
            img.sprite = on ? starOn : starOff;
        }
        else
        {
            var lbl = img.GetComponentInChildren<TextMeshProUGUI>();
            if (lbl != null) lbl.color = on ? StarOnColor : StarOffColor;
            img.color = on
                ? new Color(1f,   0.82f, 0.08f, 0.15f)
                : new Color(0.1f, 0.08f, 0.05f, 0.4f);
        }
    }

    // ── 보상 ─────────────────────────────────────────────────────────
    void ShowRewards(bool isVictory, float heroHpLeft, float heroMaxHp)
    {
        if (isVictory)
        {
            int coin    = baseVictoryCoin + (_earnedStars - 1) * 30;
            int diamond = (_earnedStars == 3) ? baseDiamond : 0;
            SetText(coinText, $"+{coin}");
            if (diamondRow != null) diamondRow.SetActive(diamond > 0);
            SetText(diamondText, $"+{diamond}");
        }
        else
        {
            float dmgRatio  = 1f - Mathf.Clamp01(heroHpLeft / heroMaxHp);
            int consoleCoin = Mathf.Max(5, Mathf.RoundToInt(baseVictoryCoin * dmgRatio * defeatRewardRatio));
            SetText(coinText, $"+{consoleCoin}");
            if (diamondRow != null) diamondRow.SetActive(false);
        }
    }

    // ── 별 팡 애니메이션 ─────────────────────────────────────────────
    IEnumerator StarPopAnim(Transform star)
    {
        Vector3 orig = star.localScale;
        float elapsed = 0f, duration = 0.28f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            float s = t < 0.5f
                ? Mathf.Lerp(1f, 1.5f, t * 2f)
                : Mathf.Lerp(1.5f, 1f, (t - 0.5f) * 2f);
            star.localScale = orig * s;
            yield return null;
        }
        star.localScale = orig;
    }

    void SetText(TextMeshProUGUI tmp, string text)
    {
        if (tmp != null) tmp.text = text;
    }

    // ── 버튼 ─────────────────────────────────────────────────────────
    void OnRetry()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    void OnLobby()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("Lobby");
    }
}
