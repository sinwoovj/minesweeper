using System.Collections;
using TMPro; // TextMeshPro를 사용할 경우 필수
using UnityEngine;
using UnityEngine.UI;

public class LoadingSceneController : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private TextMeshProUGUI matchingText; // "매칭중..." 텍스트
    [SerializeField] private RectTransform leftPanel;     // 왼쪽 슬라이드 패널
    [SerializeField] private RectTransform rightPanel;    // 오른쪽 슬라이드 패널

    [Header("Text Settings")]
    [SerializeField] private float textAnimSpeed = 0.5f;   // 점(.)이 찍히는 속도 (초)
    [SerializeField] private float matchingDuration = 7f;  // 매칭 진행 시간 (3~5초 사이)

    [Header("Slide Settings")]
    [SerializeField] private float slideDuration = 1f;     // 패널이 슬라이드 되는 시간 (초)
    // 인스펙터에서 원하는 최종 목적지 좌표를 설정할 수 있습니다. (예: Center는 0)
    [SerializeField] private Vector2 leftPanelTargetPos = Vector2.zero;
    [SerializeField] private Vector2 rightPanelTargetPos = Vector2.zero;

    private Vector2 leftPanelStartPos;
    private Vector2 rightPanelStartPos;

    private void Start()
    {
        // 시작할 때 패널들의 초기 위치(화면 밖 위치)를 저장합니다.
        if (leftPanel != null) leftPanelStartPos = leftPanel.anchoredPosition;
        if (rightPanel != null) rightPanelStartPos = rightPanel.anchoredPosition;

        // 전체 시퀀스를 관리할 코루틴 시작
        StartCoroutine(LoadingSequenceRoutine());
    }

    private IEnumerator LoadingSequenceRoutine()
    {
        // --- 1. 매칭중... 애니메이션 진행 (설정한 시간 동안) ---
        float timer = 0f;
        int dotCount = 0;

        while (timer < matchingDuration)
        {
            // 점 개수를 0 -> 1 -> 2 -> 3 -> 0 순으로 반복 계산
            string dots = new string('.', dotCount);
            matchingText.text = "매칭중" + dots;

            dotCount = (dotCount + 1) % 4;

            // 다음 점이 찍힐 때까지 대기 (예: 0.5초)
            yield return new WaitForSeconds(textAnimSpeed);
            timer += textAnimSpeed;
        }

        // --- 2. 매칭 완료 처리 ---
        matchingText.text = "매칭 완료!";
        yield return new WaitForSeconds(1.5f); // 매칭 완료 글자를 잠깐 보여주는 여유 시간
        matchingText.text = "";
        // --- 3. 이미지 좌우 Lerp 슬라이드 연출 ---
        float slideTimer = 0f;

        while (slideTimer < slideDuration)
        {
            slideTimer += Time.deltaTime;
            float t = slideTimer / slideDuration;

            // 부드러운 가속/감속 효과(Ease-Out 느낌)를 주기 위해 역삼각함수나 수학적 보정을 살짝 섞어주면 좋습니다.
            // 여기서는 깔끔하게 기본 Lerp를 사용하되, 원하시면 SmoothStep을 쓸 수도 있습니다.
            t = Mathf.SmoothStep(0f, 1f, t); // 부드러운 시작과 끝을 위한 유니티 내장 함수

            // UI(RectTransform)의 위치를 Lerp로 이동
            if (leftPanel != null)
                leftPanel.anchoredPosition = Vector2.Lerp(leftPanelStartPos, leftPanelTargetPos, t);

            if (rightPanel != null)
                rightPanel.anchoredPosition = Vector2.Lerp(rightPanelStartPos, rightPanelTargetPos, t);

            yield return null; // 매 프레임 대기
        }

        // 확실하게 최종 목적지 좌표 고정
        if (leftPanel != null) leftPanel.anchoredPosition = leftPanelTargetPos;
        if (rightPanel != null) rightPanel.anchoredPosition = rightPanelTargetPos;
        yield return new WaitForSeconds(2f);
        FadeManager.Instance.LoadSceneWithFade("InGame");
    }
}