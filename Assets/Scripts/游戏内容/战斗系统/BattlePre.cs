using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattlePre : MonoBehaviour
{
    public static BattlePre Instance { get; private set; }

    public AnimalType lastSelectedType;
    public Sprite lastBugSprite;

    void Awake()
    {
        if (Instance && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void SetEnemy(AnimalType type, Sprite bugSprite)
    {
        lastSelectedType = type;
        lastBugSprite = bugSprite;
    }
}
