using UnityEngine;

public class SoundManager : MonoBehaviour
{
    // 어디서든 쉽게 접근할 수 있도록 싱글톤으로 구성합니다.
    public static SoundManager Instance;

    public AudioClip buttonAudioClip;

    [Header("오디오 소스 (Audio Sources)")]
    [SerializeField] private AudioSource bgmSource;
    [SerializeField] private AudioSource sfxSource;

    private void Awake()
    {
        // 싱글톤 패턴 초기화 및 씬 전환 시 파괴 방지
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

    /// <summary>
    /// 배경음악(BGM)을 재생합니다.
    /// </summary>
    public void PlayBGM(AudioClip clip)
    {
        if (bgmSource.clip == clip) return; // 이미 같은 곡이 재생 중이면 무시

        bgmSource.clip = clip;
        bgmSource.loop = true; // BGM은 반복 재생
        bgmSource.Play();
    }

    /// <summary>
    /// 효과음(SFX)을 재생합니다.
    /// </summary>
    public void PlaySFX(AudioClip clip)
    {
        // 효과음은 겹쳐서 재생될 수 있도록 PlayOneShot을 사용합니다.
        sfxSource.PlayOneShot(clip);
    }
    public void PlayButtonSFX()
    {
        // 효과음은 겹쳐서 재생될 수 있도록 PlayOneShot을 사용합니다.
        sfxSource.PlayOneShot(buttonAudioClip);
    }

    /// <summary>
    /// BGM 볼륨을 설정합니다. (슬라이더의 OnValueChanged 이벤트에 연결)
    /// </summary>
    public void SetBGMVolume(float volume)
    {
        bgmSource.volume = volume;
    }

    /// <summary>
    /// SFX 볼륨을 설정합니다. (슬라이더의 OnValueChanged 이벤트에 연결)
    /// </summary>
    public void SetSFXVolume(float volume)
    {
        sfxSource.volume = volume;
    }
}