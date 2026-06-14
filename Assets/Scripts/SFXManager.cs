using UnityEngine;

public class SFXManager : MonoBehaviour
{
    [Header("Audio Clips")]
    public AudioClip explodeClip;
    public AudioClip revealClip;
    public AudioClip recoveryClip;
    public AudioClip getItemClip;

    public static SFXManager Instance;

    private void Awake()
    {
        if(SFXManager.Instance == null)
            Instance = this;
    }

    public void PlayExplodeSFX()
    {
        SoundManager.Instance.PlaySFX(explodeClip);
    }

    public void PlayRevealSFX()
    {
        SoundManager.Instance.PlaySFX(revealClip);
    }

    public void PlayRecoverySFX()
    {
        SoundManager.Instance.PlaySFX(recoveryClip);
    }

    public void PlayGetItemSFX()
    {
        SoundManager.Instance.PlaySFX(getItemClip);
    }
}
