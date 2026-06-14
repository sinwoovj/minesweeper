using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    [Header("목숨(Life) UI")]
    [Tooltip("목숨을 나타내는 하트나 아이콘 UI 오브젝트들을 순서대로 넣으세요.")]
    public GameObject[] lifeIcons;

    public GameObject resultWindow;

    public TMP_Text txt_result;
    public TMP_Text txt_playtime;
    public TMP_Text txt_revealTiles;
    public TMP_Text txt_rightFlags;
    public TMP_Text txt_explodeCount;

    public GameObject ourItemUI;

    public Slider sliderProgress;

    [Header("아이템 사용 연출 UI")]
    public RectTransform playerImageRect;
    public Image playerImage;
    public Sprite[] characterSprites;
    public Image playerProfile;
    public Sprite[] characterProfileSprites;
    public float slideDuration = 0.5f;
    public float showDuration = 1.5f;
    public float offScreenX = -500f;
    public float onScreenX = 50f;

    private bool isAnimating = false;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        if (playerImageRect != null)
        {
            playerImageRect.anchoredPosition = new Vector2(offScreenX, playerImageRect.anchoredPosition.y);
            playerImage.sprite = characterSprites[PlayerManager.Instance.playerCharacter];
            playerProfile.sprite = characterProfileSprites[PlayerManager.Instance.playerCharacter];
        }
    }

    public void UIReset()
    {
        ourItemUI.SetActive(false);
        ControlResultWindow(false);
        foreach (GameObject o in lifeIcons)
        {
            o.SetActive(true);
        }
        SliderProgressUI(0f);
    }

    // ==========================================
    // ❤️ 목숨 UI 연동 메서드
    // ==========================================

    /// <summary>
    /// PlayerManager에서 목숨을 잃거나 얻을 때 호출합니다.
    /// </summary>
    public void UpdateLivesUI(int currentLives)
    {
        // 배열을 순회하면서 현재 목숨 수보다 인덱스가 작으면 켜고, 크거나 같으면 끕니다.
        for (int i = 0; i < lifeIcons.Length; i++)
        {
            if (i < currentLives)
            {
                lifeIcons[i].SetActive(true);  // 목숨 아이콘 켜기
            }
            else
            {
                lifeIcons[i].SetActive(false); // 잃은 목숨 아이콘 끄기
            }
        }
    }

    public void SliderProgressUI(float progress)
    {
        sliderProgress.value = progress;
    }

    public void ActiveItemUI()
    {
        ourItemUI.SetActive(true);
    }

    public void InActiveItemUI()
    {
        ourItemUI.SetActive(false);
    }

    public void ControlResultWindow(bool active)
    {
        resultWindow.SetActive(active);
    }

    // ==========================================
    // 🎭 컷신 연출 메서드 (이전과 동일)
    // ==========================================
    public void TriggerItemUseEffect()
    {
        if (!isAnimating) StartCoroutine(SlidePlayerImageRoutine());
    }

    private IEnumerator SlidePlayerImageRoutine()
    {
        isAnimating = true;
        float timer = 0f;
        float startY = playerImageRect.anchoredPosition.y;

        // 슬라이드 인
        while (timer < slideDuration)
        {
            timer += Time.deltaTime;
            float progress = timer / slideDuration;
            float curve = (1f - Mathf.Cos(progress * Mathf.PI)) / 2f;
            float currentX = Mathf.Lerp(offScreenX, onScreenX, curve);
            playerImageRect.anchoredPosition = new Vector2(currentX, startY);
            yield return null;
        }
        playerImageRect.anchoredPosition = new Vector2(onScreenX, startY);

        yield return new WaitForSeconds(showDuration);

        // 슬라이드 아웃
        timer = 0f;
        while (timer < slideDuration)
        {
            timer += Time.deltaTime;
            float progress = timer / slideDuration;
            float curve = (1f - Mathf.Cos(progress * Mathf.PI)) / 2f;
            float currentX = Mathf.Lerp(onScreenX, offScreenX, curve);
            playerImageRect.anchoredPosition = new Vector2(currentX, startY);
            yield return null;
        }
        playerImageRect.anchoredPosition = new Vector2(offScreenX, startY);
        isAnimating = false;
    }

    public void SetResultPanelTexts(bool isWin, float playTime, int revealTiles, int rightFlags, int explodeCount)
    {
        txt_result.text = isWin ? "승리" : "패배";
        txt_playtime.text = "플레이 시간 - " + $"{Mathf.FloorToInt((playTime) / 60f):00}:{Mathf.FloorToInt((playTime) % 60f):00}";
        txt_revealTiles.text = "개방한 칸 - " + revealTiles + "개";
        txt_rightFlags.text = "올바른 깃발 - " + rightFlags + "개";
        txt_explodeCount.text = "폭발 횟수 - " + explodeCount + "번";
    }
}