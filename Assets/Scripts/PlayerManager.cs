using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    public static PlayerManager Instance { get; private set; }

    [Header("Player Stats")]
    public string playerName = "player 1";
    public const int playerCharacterCount = 2;
    public int playerCharacter = 0; // 0 : male, 1 : female
    public int currentHealth = 3;
    public int maxHealth = 3;
    public int playerScore = 0;

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

    // --- 아래부터는 플레이어 관련 기능 예시입니다 ---

    public void AddScore(int points)
    {
        playerScore += points;
        Debug.Log($"점수 획득! 현재 점수: {playerScore}");
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        Debug.Log($"플레이어 피격! 남은 체력: {currentHealth}");

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        Debug.Log("플레이어 사망 처리!");
        // 여기에 게임 오버 처리나 리스폰 로직을 추가하세요.
    }
}