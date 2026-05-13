using UnityEngine;
using UnityEngine.EventSystems; 
using UnityEngine.UI;

public class CardUI : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    private GameObject myTrapPrefab; 
    private DeckManager deckManager; 
    private int mySlotIndex;         
    private int myManaCost; // 이 카드의 마나 비용
    [Header("카드 UI 설정")]
    // ⭐️ 이제 카드 전체(배경)가 아니라, '자식으로 있는 아이콘 이미지'만 바꿀 겁니다!
    public Image iconImage; 

    // ⭐️ SetupCard 함수 부분을 아래 내용으로 완전히 덮어씌워 주세요! (여기서 5개의 정보를 받습니다)
    public void SetupCard(GameObject trap, Sprite icon, int cost, DeckManager manager, int slotIndex)
    {
        myTrapPrefab = trap;
        
        // iconImage는 현재 설정하신 Image 변수 이름에 맞게 쓰시면 됩니다. (예: cardImage)
        iconImage.sprite = icon; 
        
        myManaCost = cost; // 에러가 났던 cost 부분을 이렇게 연결합니다!
        deckManager = manager;
        mySlotIndex = slotIndex;
    }
    public void OnPointerDown(PointerEventData eventData)
    {
        transform.localScale = new Vector3(0.9f, 0.9f, 1f); 
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        transform.localScale = Vector3.one;
        HeroAI hero = FindObjectOfType<HeroAI>();

        if (hero != null)
        {
            // ⭐️ 핵심: 용사의 현재 마나가 카드 비용보다 많거나 같을 때만 발동!
            if (hero.currentMana >= myManaCost)
            {
                hero.currentMana -= myManaCost; // 마나 깎기!
                
                float spawnX = Camera.main.ViewportToWorldPoint(new Vector3(1.1f, 0, 0)).x;
                float spawnY = myTrapPrefab.transform.position.y;
                Instantiate(myTrapPrefab, new Vector3(spawnX, spawnY, 0f), Quaternion.identity);
                
                deckManager.DrawCard(mySlotIndex); // 카드 뽑기
            }
            else
            {
                Debug.Log("마나가 부족합니다! 필요 마나: " + myManaCost);
                // (여기에 카드가 빨갛게 번쩍이거나 '삑!' 소리 나는 효과를 나중에 추가하세요)
            }
        }
    }
}