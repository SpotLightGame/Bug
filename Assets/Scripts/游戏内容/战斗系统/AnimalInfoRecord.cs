using UnityEngine;
using System;

public class AnimalInfoRecord : MonoBehaviour
{
    public static AnimalInfoRecord Instance { get; private set; }
    public int state; // 0正常1赢2输
    public bool isBug;
    public bool showBug;

    public event Action BattleFinish;
    void Awake()
    {
        if (Instance && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void NotifyBattleFinish()
    {
        BattleFinish?.Invoke();
    }
}
