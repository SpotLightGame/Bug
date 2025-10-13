using System;
using TMPro;
using UnityEngine;

public class TimeManager : MonoBehaviour
{
    public static TimeManager Instance { get; private set; }

    [SerializeField] private TMP_Text timeTxt;
    [SerializeField] private float dayLength = 60f;   // 一天多少秒
    private float timer;
    private int day = 1;

    /* 事件：天刚增加时广播 */
    public event Action<int> OnNewDay;

    private void Awake()
    {
        if (Instance == null) { Instance = this; }
        else { Destroy(gameObject); return; }
    }

    private void Start() => RefreshUI();

    private void Update()
    {
        timer += Time.deltaTime;
        if (timer >= dayLength)
        {
            timer = 0;
            day++;
            RefreshUI();
            OnNewDay?.Invoke(day);   // 广播
        }
    }

    private void RefreshUI() => timeTxt.text = $"Day {day}";
}