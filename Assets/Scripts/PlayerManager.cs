using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    public static PlayerManager Instance { get; private set; }

    [Header("Player Stats")]
    public string playerName = "player 1";
    public const int playerCharacterCount = 2;
    public int playerCharacter = 0; // 0 : male, 1 : female
    [Header("플레이어 상태")]
    public int maxLives = 2; // 최대 목숨 개수
    public int maxItem = 1;
    private int currentItem;
    private int currentLives;
    public float progress; // 0~1;

    private void Awake()
    {
        // 2. 싱글톤 중복 생성 방지 및 초기화
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); 
        }
        else if (Instance != this)
        {
            Debug.LogWarning("PlayerManager가 이미 존재합니다. 중복된 오브젝트를 파괴합니다.");
            Destroy(gameObject);
        }
    }

    public void GameStart()
    {
        // 게임 시작 시 목숨 꽉 채우기
        currentLives = maxLives;
        currentItem = maxItem;
        // 시작할 때 UI 업데이트 (초기화)
        UIManager.Instance.UpdateLivesUI(currentLives);
    }

    public void LoseLife()
    {
        --currentLives;
        currentLives = Mathf.Max(currentLives, 0); // 0 밑으로 떨어지지 않게 고정
        // 목숨이 남았다면 부활 연출이나 무적 시간 등 추가
        Debug.Log($"앗! 맞았다. 남은 목숨: {currentLives}");
        // UI에 남은 목숨 반영
        UIManager.Instance.UpdateLivesUI(currentLives);
    }
    
    public void RecoveryLife()
    {
        ++currentLives;
        UIManager.Instance.UpdateLivesUI(currentLives);
    }

    public bool UsableItem() => currentItem > 0;

    public void GetItem()
    {
        currentItem--;
    }

    /// <summary>
    /// 아이템을 사용했을 때 (연출 확인용)
    /// </summary>
    public void UseItem(int num)
    {
        switch(num)
        {
            case 0:  // 회복 아이템
                RecoveryLife();
                SFXManager.Instance.PlayRecoverySFX();
                break;
        }
        UIManager.Instance.TriggerItemUseEffect();
    }
    public int CurrentLives => currentLives;
}