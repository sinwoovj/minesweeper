
using System.Collections.Generic;
using UnityEngine;

public class LoadingManager : MonoBehaviour
{
    public List<GameObject> ourCharacters;
    public List<GameObject> ourProfiles;
    void Start()
    {
        PlayerManager pm = PlayerManager.Instance;
        int pc = pm.playerCharacter;

        for (int i = 0; i < PlayerManager.playerCharacterCount; i++)
        {
            ourCharacters[i].SetActive(pc == i);
            ourProfiles[i].SetActive(pc == i);
        }
    }
}
