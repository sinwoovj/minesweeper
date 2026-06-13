using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour
{
    public List<GameObject> profiles;
    public List<GameObject> characterViews;
    public List<GameObject> selectedCharacters;
    public List<GameObject> selectedCharacterProfiles;
    public List<GameObject> selectedCharacterViews;

    public void OnChangedValue()
    {
        PlayerManager pm = PlayerManager.Instance;
        int cn = pm.playerCharacter;

        for (int i = 0; i < PlayerManager.playerCharacterCount; i++)
        {
            profiles[i].SetActive(i == cn);
            characterViews[i].SetActive(i == cn);
            selectedCharacters[i].GetComponent<Image>().color = 
                (i == cn ? Color.white : Color.gray);
            selectedCharacterProfiles[i].GetComponent<Image>().color = 
                (i == cn ? Color.white : Color.gray);
            selectedCharacterViews[i].SetActive(i == cn);
        }
    }

    public void SelectCharacterFunc(int num)
    {
        PlayerManager pm = PlayerManager.Instance;
        pm.playerCharacter = num;
        OnChangedValue();
    }
}
