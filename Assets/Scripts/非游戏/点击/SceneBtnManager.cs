using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneBtnManager : MonoBehaviour
{
    [Header("各类界面")]
    public GameObject shopPanel;
    public GameObject[] shopPages;
    private int currentShopPage = 0;

    [SerializeField] private FadeManager fadeManager;

    private void Start()
    {
        InitializeShop();
    }
    
    // 初始化商店状态
    private void InitializeShop()
    {
        if (shopPanel != null)
        {
            shopPanel.SetActive(false);
        }
        
        // 确保所有页面都被隐藏
        if (shopPages != null)
        {
            foreach (GameObject page in shopPages)
            {
                if (page != null)
                {
                    page.SetActive(false);
                }
            }
        }
        
        // 重置到第一页
        currentShopPage = 0;
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
        if (shopPanel == null) return;
        
        bool isOpening = !shopPanel.activeSelf;
        shopPanel.SetActive(isOpening);
        
        if (isOpening)
        {
            // 打开商店时，确保从第0页开始
            ShowShopPage(0);
        }
        else
        {
            // 关闭商店时，隐藏所有页面
            HideAllShopPages();
        }
    }

    // 显示指定页面的辅助方法
    private void ShowShopPage(int pageIndex)
    {
        if (shopPages == null || shopPages.Length == 0) return;
        
        // 确保索引在有效范围内
        pageIndex = Mathf.Clamp(pageIndex, 0, shopPages.Length - 1);
        
        // 隐藏所有页面
        HideAllShopPages();
        
        // 显示指定页面
        if (shopPages[pageIndex] != null)
        {
            shopPages[pageIndex].SetActive(true);
            currentShopPage = pageIndex;
        }
    }
    
    // 隐藏所有页面的辅助方法
    private void HideAllShopPages()
    {
        if (shopPages == null) return;
        
        foreach (GameObject page in shopPages)
        {
            if (page != null)
            {
                page.SetActive(false);
            }
        }
    }

    public void NextShopPage()
    {
        if (shopPages == null || shopPages.Length == 0) return;
        
        int nextPage = (currentShopPage + 1) % shopPages.Length;
        ShowShopPage(nextPage);
    }

    public void LastShopPage()
    {
        if (shopPages == null || shopPages.Length == 0) return;
        
        int prevPage = (currentShopPage - 1 + shopPages.Length) % shopPages.Length;
        ShowShopPage(prevPage);
    }

    public void ShowBug()
    {
        foreach (GameObject animal in GameObject.FindGameObjectsWithTag("Animal"))
        {
            AnimalInfo animalInfo = animal.GetComponent<AnimalInfo>();
            if (animalInfo != null && animalInfo.isBug)
            {
                animalInfo.showBug = true;
            }
        }
    }

    //暂停或继续游戏
    public void TogglePause()
    {
        bool isPaused = TimeManager.Instance.isPause;   // 当前状态
        TimeManager.Instance.Pause(!isPaused);          // 取反
        Debug.Log(isPaused ? "继续游戏" : "已暂停");
    }
    
    // 直接跳到下一天
    public void SkipToNextDay()
    {
        TimeManager tm = TimeManager.Instance;
        tm.Pause(false);
        tm.timer = 0f;

        tm.day++;
        if (tm.day > 28)
        {
            tm.day = 1;
            tm.season++;
            if (tm.season > 4)
            {
                tm.season = 1;
                tm.year++;
            }
        }

        tm.RefreshUI();
        tm.TriggerNewDay();   // ⭐ 用公共方法
        if (tm.day % 7 == 0) tm.TriggerShowDay(); // ⭐ 用公共方法

        Debug.Log($"已跳过至 {tm.year} 年 {tm.season} 季 {tm.day} 日");
    }
}