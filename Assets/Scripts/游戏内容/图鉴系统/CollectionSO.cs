using UnityEngine;

[CreateAssetMenu(fileName = "NewCollection", menuName = "收集物/CollectionSO")]
public class CollectionSO : ScriptableObject
{
    [Header("基础信息")]
    public int collectionID; // 唯一ID
    public string collectionName; // 收集物名称
    public string description; // 收集物描述

    [Header("图标")]
    public Sprite unlockedIcon; // 已解锁图标
    public Sprite lockedIcon; // 未解锁图标（问号图）
}