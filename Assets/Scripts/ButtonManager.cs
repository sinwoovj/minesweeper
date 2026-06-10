using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ButtonManager : MonoBehaviour
{
    private Color selectedColor = Color.gray;
    private Color defaultColor = Color.white;

    public void ActiveUI(GameObject obj, bool isActive)
    {
        obj.SetActive(isActive);
    }

    public void SelectUI(List<Button> objs, Button obj)
    {
        foreach (Button o in objs)
        {
            o.Select();
            //o.Equals(obj) ? true : false
        }
    }

    public void MoveScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

    public void GameExit()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
