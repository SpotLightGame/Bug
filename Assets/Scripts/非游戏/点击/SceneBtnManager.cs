using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneBtnManager : MonoBehaviour
{
    [Header("各类界面")]
    public GameObject shopPanel;


    [SerializeField] private FadeManager fadeManager;

    private void Start()
    {
        shopPanel.SetActive(false);
    }
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

    public void ControlShopPanel()
    {
        shopPanel.SetActive(!shopPanel.activeSelf);
    }

    public void ShowBug()
    {
        foreach (GameObject animal in GameObject.FindGameObjectsWithTag("Animal"))
        {
            if (animal.GetComponent<AnimalInfo>().isBug)
            {
                animal.GetComponent<AnimalInfo>().showBug = true;
            }
        }
    }
}
