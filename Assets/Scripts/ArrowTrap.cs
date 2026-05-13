using UnityEngine;
using System.Collections;

public class ArrowTrap : MonoBehaviour
{
    [Header("연사 설정")]
    public GameObject arrowPrefab; // 쏠 화살의 원본(프리팹)
    public int fireCount = 3;      // 몇 발 쏠 것인가?
    public float fireDelay = 0.15f;// 타다닥! 쏘는 간격 (0.15초면 아주 빠른 연사입니다)

    void Start()
    {
        // 씬에 이 함정이 등장하자마자 3연사 시작!
        StartCoroutine(FireArrowsRapidly());
    }

    IEnumerator FireArrowsRapidly()
    {
        for (int i = 0; i < fireCount; i++)
        {
            // 화살을 현재 발사기 위치에 복제해서 쏩니다
            Instantiate(arrowPrefab, transform.position, Quaternion.Euler(0, 0, 90));
            
            // 다음 화살을 쏘기 전까지 아주 잠깐(0.15초) 대기
            yield return new WaitForSeconds(fireDelay); 
        }

        // 3발 다 쏘고 나면 빈 껍데기가 된 발사기는 스스로 삭제!
        Destroy(gameObject, 1f); 
    }
}