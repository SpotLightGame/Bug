using System;
using TMPro;
using UnityEngine;

public class TimeManager : MonoBehaviour
{
    public static TimeManager Instance { get; private set; }

    [SerializeField] private TMP_Text timeTxt;
    [SerializeField] private float dayLength = 60f;   // 一天多少秒
    private float timer;
    private int year = 1;
    private int season = 1;
    private int day = 1;

    /* 事件：天刚增加时广播 */
    public event Action OnNewDay;
    public event Action ShowDay;
    [Header("状态")]
    private bool isPause;

    private void Awake()
    {
        if (Instance == null) { Instance = this; }
        else { Destroy(gameObject); return; }
    }

    private void Start() => RefreshUI();

    private void Update()
    {
        UpdateDay();
    }

    private void RefreshUI() => timeTxt.text = $"{year} 年 {season} 季 \n {day} 日";

    // 暂停功能
    public void Pause(bool comfirm)
    {
        isPause = comfirm;
    }

    public void UpdateDay()
    {
        if (isPause) return;
        timer += Time.deltaTime;
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
            if(day % 7 == 0)
            {
                ShowDay?.Invoke();
            }
        }
    }
}