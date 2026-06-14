using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    // 어디서든 접근 가능하도록 싱글톤 구성
    public static GameManager Instance;

    // 1. 게임 상태를 정의하는 enum
    public enum GameState
    {
        Ready,      // 시작 전 (카운트다운 등)
        Playing,    // 게임 진행 중 (플레이 타임 흐름)
        GameOver    // 게임 종료
    }

    [SerializeField] private Game our_game;
    [SerializeField] private Game enemy_game;
    [Header("게임 상태")]
    public GameState currentState = GameState.Ready;
    public TMP_Text timeText;
    [Header("플레이 타임 설정")]
    [SerializeField] private float maxGameTime = 180f; // 최대 게임 시간 (예: 60초)
    private float currentPlayTime = 0f;
    private bool isTimerRunning = false;

    public int revealTiles, rightFlags, explodeCount;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }

    private void Start()
    {
        // 게임 시작 시 초기 상태 설정
        ChangeState(GameState.Ready);
        // 여기에 이전 질문에서 만든 카운트다운을 넣으면 좋습니다.
        // 예: CountdownController.Instance.StartCountdown();
    }


    private void Update()
    {
        // 2. Playing 상태일 때만 플레이 타임이 흐르도록 설정
        if (isTimerRunning && currentState == GameState.Playing)
        {
            currentPlayTime += Time.deltaTime;
            int minutes = Mathf.FloorToInt((maxGameTime - currentPlayTime) / 60f);
            int seconds = Mathf.FloorToInt((maxGameTime - currentPlayTime) % 60f);

            // 2. "00:00" 형식의 문자열로 변환
            string s_timeText = $"{minutes:00}:{seconds:00}";
            timeText.text = s_timeText;
            // UI에 시간을 표시하고 싶다면 여기에 연결 (예: UIManager.UpdateTimer(currentPlayTime);)

            // 타임아웃 조건 체크 (시간 제한이 있는 게임인 경우)
            if (currentPlayTime >= maxGameTime)
            {
                // 시간이 다 되면 무승부 혹은 패배 처리
                EndGame(judgeWinner());
            }
        }
    }

    // 진행도로 승패 비교 후 결과 도출
    private bool judgeWinner()
    {
        bool win = false;
        return win;
    }

    /// <summary>
    /// 게임 상태를 안전하게 변경하는 메서드
    /// </summary>
    public void ChangeState(GameState newState)
    {
        currentState = newState;

        // 상태가 변경될 때 각각 필요한 초기화 로직 실행
        switch (currentState)
        {
            case GameState.Ready:
                UIManager.Instance.UIReset();
                currentPlayTime = 0f;
                isTimerRunning = false;
                our_game.NewGame();
                enemy_game.NewGame();
                timeText.text = "03:00";
                PlayerManager.Instance.GameStart();
                Debug.Log("게임 준비 상태");
                CountdownManager.Instance.StartCountdown();
                break;

            case GameState.Playing:
                isTimerRunning = true;
                Debug.Log("게임 시작! 타이머 작동");
                break;

            case GameState.GameOver:
                isTimerRunning = false;
                Debug.Log("게임 오버");
                break;
        }
    }

    public void RestartGame()
    {
        ChangeState(GameState.Ready);
        CountdownManager.Instance.StartCountdown();
    }

    /// <summary>
    /// 게임을 실제로 시작하는 메서드 (카운트다운이 끝나는 시점 등에 호출)
    /// </summary>
    public void StartGame()
    {
        ChangeState(GameState.Playing);
    }

    /// <summary>
    /// 3. 특정 조건을 만족했을 때 게임을 종료하고 승패를 판정하는 메서드
    /// </summary>
    /// <param name="isWin">true면 승리, false면 패배</param>
    public void EndGame(bool isWin)
    {
        // 이미 게임 오버 상태라면 중복 실행 방지
        if (currentState == GameState.GameOver) return;

        ChangeState(GameState.GameOver);

        if (isWin)
        {
            Debug.Log($"승리! 플레이 타임: {currentPlayTime:F2}초");
            // 승리 UI 띄우기 등의 로직 추가
        }
        else
        {
            Debug.Log($"패배! 플레이 타임: {currentPlayTime:F2}초");
            // 패배 UI 띄우기 등의 로직 추가
        }
        UIManager.Instance.SetResultPanelTexts(isWin, currentPlayTime, revealTiles, rightFlags, explodeCount);
        UIManager.Instance.ControlResultWindow(true);
    }

    // 외부에서 현재 플레이 타임을 읽어갈 수 있는 프로퍼티
    public float CurrentPlayTime => currentPlayTime;
}