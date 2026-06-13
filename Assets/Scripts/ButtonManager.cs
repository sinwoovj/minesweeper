using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ButtonManager : MonoBehaviour
{
    private Color selectedColor = Color.gray;
    private Color defaultColor = Color.white;

    public GameObject activeObj0;
    public GameObject activeObj1;
    public GameObject activeObj2;
    public GameObject activeObj3;
    public List<GameObject> selectBtns = new List<GameObject>();
    public GameObject selectBtn;
    public string sceneName;
    public void ActiveUI0(bool isActive)
    {
        activeObj0.SetActive(isActive);
    }
    
    public void ActiveUI1(bool isActive)
    {
        activeObj1.SetActive(isActive);
    }
    
    public void ActiveUI2(bool isActive)
    {
        activeObj2.SetActive(isActive);
    }
    
    public void ActiveUI3(bool isActive)
    {
        activeObj3.SetActive(isActive);
    }

    public void SelectUI()
    {
        foreach (GameObject o in selectBtns)
        {
            o.GetComponent<Image>().color = o.Equals(selectBtn) ? selectedColor : defaultColor;
        }
    }

    public void MoveScene()
    {
        FadeManager.Instance.LoadSceneWithFade(sceneName);
    }

    public void GameExit()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    public void Rematch()
    {
        
    }
}
