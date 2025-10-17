using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattlePre : MonoBehaviour
{
    public static BattlePre Instance { get; private set; }

    public AnimalType   lastSelectedType; // 点击动物时写入
    public Sprite       lastBugSprite;    // 进入战斗前读取

    void Awake()
    {
        if (Instance && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    // 点击动物时调用
    public void SetEnemy(AnimalType type, BattleInfoRecord_SO db)
    {
        lastSelectedType = type;
        lastBugSprite    = db.GetBugSprite(type);
    }
}
