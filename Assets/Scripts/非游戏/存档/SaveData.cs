using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SaveData
{
    public int year, season, day;      // 时间
    public float gold;                   // 金钱
    public List<AnimalSnapshot> animals = new List<AnimalSnapshot>();
}

[System.Serializable]
public class AnimalSnapshot
{
    public string animalID;   // 唯一标识
    public AnimalType type;
    public bool   isBug;
    public bool   showBug;
    public float  closenessEXP;
    public int    age;
    public float  mood;
    public Vector3 pos;       // 场景坐标（可选）
    // 需要啥再加
}
