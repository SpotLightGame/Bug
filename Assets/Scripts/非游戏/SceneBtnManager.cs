using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneBtnManager : MonoBehaviour
{
    [SerializeField] private FadeManager fadeManager;

    public void LoadScene(String sceneName)
    {
        fadeManager.FadeOut();
        StartCoroutine(Load(sceneName));
    }

    public void EnterScene(String sceneName)
    {
        fadeManager.FadeIn();
    }

    private IEnumerator Load(String sceneName)
    {

        yield return new WaitForSeconds(fadeManager.fadeTime); //等待淡化
        
        if (sceneName == "0")
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
                Application.Quit();
#endif
        }
        else
        {
            SceneManager.LoadScene(sceneName);
        }
    }
}
