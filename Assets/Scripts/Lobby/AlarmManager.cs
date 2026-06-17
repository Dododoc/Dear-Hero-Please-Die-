using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class AlarmManager : MonoBehaviour
{
    [Header("평상시 UI")]
    public GameObject screen_Static;   // 지직거리는 화면
    public GameObject cristal_Static;  // 꺼진 크리스탈

    [Header("경보 발동 시 UI")]
    public GameObject cctv_Background; // 레이더+용사 화면
    public GameObject cristal_Active;  // 빛나는 크리스탈

    [Header("레이더 스윕 (Canvas 직속 분리됨)")]
    public GameObject radarSweep;      // Rader 오브젝트 (Canvas 직속)

    [Header("시간 설정 (초 단위)")]
    public float minSpawnTime = 120f;
    public float maxSpawnTime = 300f;
    public float alarmDuration = 60f;

    private float timer = 0f;
    private bool isAlarmActive = false;

    void Start()
    {
        SetAlarmMode(false);
        SetNextSpawnTime();
    }

    void Update()
    {
        timer -= Time.deltaTime;

        if (!isAlarmActive)
        {
            if (timer <= 0f)
                TriggerAlarm();
        }
        else
        {
            if (timer <= 0f)
                HandleTimeOutFailure();
        }
    }

    void SetNextSpawnTime()
    {
        timer = Random.Range(minSpawnTime, maxSpawnTime);
        Debug.Log($"[시스템] 다음 용사 등장까지 {Mathf.Round(timer)}초 남았습니다.");
    }

    void TriggerAlarm()
    {
        isAlarmActive = true;
        timer = alarmDuration;
        SetAlarmMode(true);
        Debug.Log("<color=red>🚨 삐용삐용! 용사 포착! 1분 안에 클릭하세요!</color>");
    }

    void HandleTimeOutFailure()
    {
        isAlarmActive = false;
        SetAlarmMode(false);
        SetNextSpawnTime();
        Debug.Log("<color=orange>💀 타임오버! 방치 페널티 적용 (재화 깎임 로직 추가 예정)</color>");
    }

    public void OnRadarClicked()
    {
        if (isAlarmActive)
        {
            Debug.Log("⚔️ 전투 돌입! Ingame 씬을 로드합니다.");
            SceneManager.LoadScene("Ingame");
        }
    }

    void SetAlarmMode(bool isActive)
    {
        screen_Static.SetActive(!isActive);
        cristal_Static.SetActive(!isActive);

        cctv_Background.SetActive(isActive);
        cristal_Active.SetActive(isActive);

        // Canvas 직속으로 분리된 레이더 스윕도 같이 On/Off
        if (radarSweep != null)
            radarSweep.SetActive(isActive);
    }
}
