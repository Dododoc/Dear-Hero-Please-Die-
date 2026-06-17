using UnityEngine;

public class ParallaxBackground : MonoBehaviour
{
    public HeroAI heroLogic; 
    public float scrollSpeed = 0.5f; 

    [Header("배경 이미지 설정")]
    public Texture2D grassTexture;  
    public Texture2D desertTexture; 
    public Texture2D hellTexture;   

    [Header("테마별 크기(Scale) 설정 ⭐️")]
    public Vector3 grassScale = new Vector3(32, 18, 1);
    public Vector3 desertScale = new Vector3(32, 12, 1); // 사막만 작게 하고 싶다면 Y값을 줄여보세요
    public Vector3 hellScale = new Vector3(32, 18, 1);

    private Material mat;
    private float offset;

    void Start()
    {
        mat = GetComponent<MeshRenderer>().material;
        
        // 처음 시작할 때 초원 크기로 세팅
        transform.localScale = grassScale;
        if (grassTexture != null) mat.mainTexture = grassTexture; 
    }

    void Update()
    {
        if (heroLogic == null) return;

        float currentSpeed = scrollSpeed * heroLogic.GetSpeedMultiplier();
        offset += currentSpeed * Time.deltaTime;
        mat.mainTextureOffset = new Vector2(offset, 0);
    }

    // ⭐️ GameManager가 호출할 때 크기까지 같이 바꿔줍니다!
    // ⭐️ GameManager가 호출할 때 크기까지 같이 바꿔줍니다!
    public void ChangeBackground(int phase)
    {
        // ⭐️ 1. 배경이 바뀔 때 무조건 offset을 0으로 초기화해서 처음부터 보여줍니다!
        offset = 0f;
        mat.mainTextureOffset = new Vector2(offset, 0);

        if (phase == 1) // 사막 테마
        {
            mat.mainTexture = desertTexture;
            transform.localScale = desertScale; // 사막 전용 크기로 변경!
        }
        else if (phase == 2) // 지옥 테마
        {
            mat.mainTexture = hellTexture;
            transform.localScale = hellScale; // 지옥 전용 크기로 변경!
        }
    }
}