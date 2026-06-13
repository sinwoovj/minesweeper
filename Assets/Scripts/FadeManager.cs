using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class FadeManager : MonoBehaviour
{
    public static FadeManager Instance { get; private set; }

    [Header("Fade Settings")]
    [Tooltip("앞서 만든 FadeCanvas의 CanvasGroup을 여기에 끌어다 넣으세요.")]
    public CanvasGroup fadeCanvasGroup;
    public float fadeDuration = 1f; // 페이드에 걸리는 시간 (초)

    private void Awake()
    {
        // 싱글톤 세팅 및 씬 전환 시 파괴 방지
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        // 게임 시작 시 화면이 밝아지도록 초기화
        fadeCanvasGroup.alpha = 0f;
        fadeCanvasGroup.blocksRaycasts = false;
    }

    // 외부에서 씬을 전환할 때 이 함수를 부릅니다.
    public void LoadSceneWithFade(string sceneName)
    {
        StartCoroutine(FadeAndLoadRoutine(sceneName));
    }

    private IEnumerator FadeAndLoadRoutine(string sceneName)
    {
        // 1. 페이드 아웃 시작 (화면이 어두워짐)
        fadeCanvasGroup.blocksRaycasts = true; // 전환 중 플레이어가 다른 UI를 클릭하지 못하게 방지
        float timer = 0f;

        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;
            fadeCanvasGroup.alpha = Mathf.Lerp(0f, 1f, timer / fadeDuration);
            yield return null;
        }
        fadeCanvasGroup.alpha = 1f; // 확실하게 1로 고정

        // 2. 비동기로 다음 씬 로드 (로딩 중 게임이 멈추지 않음)
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);
        while (!asyncLoad.isDone)
        {
            yield return null; // 로드가 끝날 때까지 대기
        }

        // 3. 페이드 인 시작 (화면이 다시 밝아짐)
        timer = 0f;
        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;
            fadeCanvasGroup.alpha = Mathf.Lerp(1f, 0f, timer / fadeDuration);
            yield return null;
        }
        fadeCanvasGroup.alpha = 0f; // 확실하게 0으로 고정
        fadeCanvasGroup.blocksRaycasts = false; // 클릭 방지 해제
    }
}