using System;
using TMPro;
using UnityEngine;

public class TimeManager : MonoBehaviour
{
    public static TimeManager Instance { get; private set; }

    [SerializeField] private TMP_Text timeTxt;
    [SerializeField] private float dayLength = 60f;   // 一天多少秒
    public float timer;
    public int year = 1;
    public int season = 1;
    public int day = 1;

    /* 事件：天刚增加时广播 */
    public event Action OnNewDay;
    public event Action OnMidDay;      // 日中（300秒）
    public event Action ShowDay;
    [Header("状态")]
    public bool isPause;

    private void Awake()
    {
        if (Instance == null) { Instance = this; }
        else { Destroy(gameObject); return; }
    }

    private void Start()
    {
        TimeManager.Instance.OnNewDay += Save;   // 每天自动存
        Load();                                  // 启动时读档
        RefreshUI();
    }

    private void Update()
    {
        UpdateDay();
    }

    public void RefreshUI() => timeTxt.text = $"{year} 年 {season} 季 \n {day} 日";

    // 暂停功能
    public void Pause(bool comfirm)
    {
        isPause = comfirm;
    }

    public void UpdateDay()
    {
        if (isPause) return;
        timer += Time.deltaTime;

        //检测日中
        if (timer >= dayLength / 2 && timer - Time.deltaTime < dayLength / 2)
        {
            OnMidDay?.Invoke();
        }//检测

        if (timer >= dayLength)
        {
            timer = 0;

            if (day < 28)
            {
                day++;
            }
            else
            {
                day = 1;
                season++;
                if (season > 4)
                {
                    season = 1;
                    year++;
                }
            }

            RefreshUI();
            OnNewDay?.Invoke();   // 广播
            if (day % 7 == 0)
            {
                ShowDay?.Invoke();
            }
        }
    }

    public void SetDate(int y, int s, int d)
    {
        year = y; season = s; day = d;
        RefreshUI();
    }
    
    public void Save() => SaveManager.Instance.Save();
    public void Load() => SaveManager.Instance.Load();

    public void TriggerNewDay() => OnNewDay?.Invoke();
    public void TriggerShowDay() => ShowDay?.Invoke();
}