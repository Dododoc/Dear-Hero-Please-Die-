using UnityEngine;
using System.Collections.Generic;

public class DeckManager : MonoBehaviour
{
    [Header("프로토타입 덱 설정 (정해진 8장)")]
    public GameObject[] deckSequence; 
    public Sprite[] cardIcons;        
    public int[] manaCosts; // ⭐️ 추가: 8장 카드의 마나 비용을 적어둘 배열!
    [Header("UI 연결")]
    public CardUI[] handSlots;        

    private Queue<int> drawQueue = new Queue<int>(); 
    
    // ⭐️ 핵심 추가: 현재 4개의 슬롯(손패)에 각각 몇 번 카드가 있는지 기억하는 공간
    private int[] currentHand; 

    void Start()
    {
        // 손패 배열 크기 초기화 (슬롯이 4개면 4칸짜리 배열 생성)
        currentHand = new int[handSlots.Length]; 

        // 1. 게임 시작 시 0번부터 7번 카드까지 대기열에 세웁니다.
        for (int i = 0; i < deckSequence.Length; i++)
        {
            drawQueue.Enqueue(i);
        }

        // 2. 처음 4장을 뽑아서 손패에 쥐어줍니다.
        for (int i = 0; i < handSlots.Length; i++)
        {
            int firstDrawIndex = drawQueue.Dequeue();
            currentHand[i] = firstDrawIndex; 
            
            // ⭐️ 여기를 수정했습니다! 시작할 때도 마나 비용(cost)을 찾아서 같이 넘겨줍니다!
            int firstCost = manaCosts[firstDrawIndex]; 
            handSlots[i].SetupCard(deckSequence[firstDrawIndex], cardIcons[firstDrawIndex], firstCost, this, i);
        }
    }

    public void DrawCard(int slotIndex)
    {
        if (drawQueue.Count > 0)
        {
            int usedCardIndex = currentHand[slotIndex];
            drawQueue.Enqueue(usedCardIndex);

            int nextIndex = drawQueue.Dequeue();
            currentHand[slotIndex] = nextIndex;

            GameObject nextTrap = deckSequence[nextIndex];
            Sprite nextIcon = cardIcons[nextIndex];
            
            // ⭐️ 여기가 중요합니다! 배열에서 마나 비용을 꺼내옵니다.
            int nextCost = manaCosts[nextIndex]; 

            // ⭐️ 카드에게 5가지 정보(함정 원본, 그림, 마나, 매니저, 번호)를 정확히 던져줍니다!
            handSlots[slotIndex].SetupCard(nextTrap, nextIcon, nextCost, this, slotIndex);
        }
    }
}