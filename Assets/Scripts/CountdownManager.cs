using System.Collections;
using UnityEngine;
using TMPro; // TextMeshPro 네임스페이스 필수

public class CountdownManager : MonoBehaviour
{
    public static CountdownManager Instance;

    [Header("UI 연결")]
    [Tooltip("화면 중앙에 배치한 TextMeshProUGUI를 연결하세요.")]
    public TextMeshProUGUI countdownText;

    [Header("애니메이션 설정")]
    public float maxScale = 1.5f; // 가장 커졌을 때의 크기
    public float animationDuration = 1f; // 숫자 하나당 걸리는 시간 (1초)

    private void Awake()
    {
        if(CountdownManager.Instance == null)
            Instance = this;        
    }

    /// <summary>
    /// 외부에서 카운트다운을 시작할 때 호출하는 메서드입니다.
    /// </summary>
    public void StartCountdown()
    {
        StartCoroutine(CountdownRoutine());
    }

    private IEnumerator CountdownRoutine()
    {
        yield return new WaitForSeconds(3f);
        // 텍스트 활성화
        countdownText.gameObject.SetActive(true);

        // 3, 2, 1 카운트다운 진행
        for (int i = 3; i > 0; i--)
        {
            countdownText.text = i.ToString();
            yield return StartCoroutine(AnimateTextScale());
        }

        // "시작!" 텍스트 출력
        countdownText.text = "시작!";

        // "시작!" 텍스트도 동일하게 커졌다 작아지는 효과 주기
        yield return StartCoroutine(AnimateTextScale());

        // 카운트다운이 모두 끝나면 텍스트를 화면에서 숨김
        countdownText.gameObject.SetActive(false);

        GameManager.Instance.StartGame();
    }

    /// <summary>
    /// cos 함수를 활용하여 텍스트 크기를 비선형적으로 부드럽게 조절하는 애니메이션
    /// </summary>
    private IEnumerator AnimateTextScale()
    {
        float timer = 0f;

        while (timer < animationDuration)
        {
            timer += Time.deltaTime;

            // 0에서 1까지 증가하는 진행률
            float progress = timer / animationDuration;

            // cos 함수를 이용한 Ease-In-Out 곡선 (0 -> 1 -> 0)
            // progress가 0일 때: (1 - cos(0)) / 2 = 0
            // progress가 0.5일 때: (1 - cos(PI)) / 2 = 1 (최대 크기)
            // progress가 1일 때: (1 - cos(2PI)) / 2 = 0
            float curve = (1f - Mathf.Cos(progress * Mathf.PI * 2f)) / 2f;

            // 곡선 값에 최대 크기를 곱하여 현재 스케일 결정
            float currentScale = maxScale * curve;
            countdownText.transform.localScale = new Vector3(currentScale, currentScale, currentScale);

            yield return null;
        }

        // 애니메이션이 끝나면 혹시 모를 오차를 방지하기 위해 스케일을 0으로 확실히 고정
        countdownText.transform.localScale = Vector3.zero;
    }
}