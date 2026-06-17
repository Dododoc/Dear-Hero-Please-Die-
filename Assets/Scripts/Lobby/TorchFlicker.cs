using UnityEngine;

/// <summary>
/// Animator 루프 애니메이션을 랜덤 오프셋에서 시작시켜
/// 여러 횃불이 제각각 자연스럽게 일렁이도록 합니다.
/// </summary>
public class TorchFlicker : MonoBehaviour
{
    [Range(0f, 1f)]
    [Tooltip("직접 고정 오프셋을 줄 경우 사용 (0이면 완전 랜덤)")]
    public float fixedOffset = 0f;

    void Start()
    {
        var anim = GetComponent<Animator>();
        if (anim == null) return;

        float offset = fixedOffset > 0f ? fixedOffset : Random.value;

        // 현재 재생 중인 스테이트를 offset 위치부터 시작
        var stateInfo = anim.GetCurrentAnimatorStateInfo(0);
        anim.Play(stateInfo.shortNameHash, 0, offset);
    }
}
