using UnityEngine;

public class BGMManager : MonoBehaviour
{
    public AudioClip clip;
    void Start()
    {
        SoundManager.Instance.PlayBGM(clip);   
    }
}
