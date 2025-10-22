using System.Collections.Generic;
using UnityEngine;

public class AlbumManager : MonoBehaviour
{
    [Header("收集物列表")]
    public List<CollectionSO> allCollections = new List<CollectionSO>();

    private HashSet<int> unlockedCollectionIDs = new HashSet<int>();

    private void Awake()
    {
        // 初始化（实际项目中应从存档加载）
        LoadUnlockedData();
        // 调试：确保开始时所有收集物都是未解锁状态
        Debug.Log($"游戏启动时解锁的收集物数量: {unlockedCollectionIDs.Count}");
        foreach (var collection in allCollections)
        {
            Debug.Log($"收集物 {collection.collectionName} (ID: {collection.collectionID}) 解锁状态: {IsUnlocked(collection.collectionID)}");
        }

    }

    /// <summary>
    /// 解锁收集物
    /// </summary>
    public void UnlockCollection(int id)
    {
        if (!unlockedCollectionIDs.Contains(id))
        {
            unlockedCollectionIDs.Add(id);
            SaveUnlockedData();
            Debug.Log($"解锁收集物: {id}");
        }
    }

    /// <summary>
    /// 检查收集物是否已解锁
    /// </summary>
    public bool IsUnlocked(int id)
    {
        return unlockedCollectionIDs.Contains(id);
    }

    /// <summary>
    /// 获取随机未解锁的收集物
    /// </summary>
    public CollectionSO GetRandomLockedCollection()
    {
        List<CollectionSO> lockedCollections = new List<CollectionSO>();

        foreach (var collection in allCollections)
        {
            if (!IsUnlocked(collection.collectionID))
            {
                lockedCollections.Add(collection);
            }
        }

        if (lockedCollections.Count == 0) return null;

        return lockedCollections[Random.Range(0, lockedCollections.Count)];
    }

    /// <summary>
    /// 检查是否所有收集物都已解锁
    /// </summary>
    public bool AreAllCollectionsUnlocked()
    {
        return unlockedCollectionIDs.Count >= allCollections.Count;
    }

    // 保存解锁数据（实际项目中应存入SaveManager）
    private void SaveUnlockedData()
    {
        /* if (unlockedCollectionIDs.Count == 0)
         {
             PlayerPrefs.DeleteKey("UnlockedCollections");
         }
         else
         {
             PlayerPrefs.SetString("UnlockedCollections", string.Join(",", unlockedCollectionIDs));
         }
         PlayerPrefs.Save();*/
        Debug.Log($"保存解锁数据: {string.Join(",", unlockedCollectionIDs)}");
    }

    // 修改AlbumManager.cs的LoadUnlockedData方法
    private void LoadUnlockedData()
    {
        unlockedCollectionIDs.Clear();

        // 只在有保存数据时才加载，否则保持空集合（全部未解锁状态）
        if (PlayerPrefs.HasKey("UnlockedCollections"))
        {
            string unlockedData = PlayerPrefs.GetString("UnlockedCollections");
            Debug.Log($"从存档加载解锁数据: {unlockedData}");

            if (!string.IsNullOrEmpty(unlockedData))
            {
                string[] ids = unlockedData.Split(',');
                foreach (var idStr in ids)
                {
                    if (int.TryParse(idStr.Trim(), out int id))
                    {
                        // 验证ID是否存在于集合中，防止无效ID导致的问题
                        bool idExists = allCollections.Exists(c => c.collectionID == id);
                        if (idExists)
                        {
                            unlockedCollectionIDs.Add(id);
                            Debug.Log($"加载解锁的收藏品ID: {id}");
                        }
                        else
                        {
                            Debug.LogWarning($"无效的收藏品ID: {id}，已忽略");
                        }
                    }
                }
            }
        }
        else
        {
            Debug.Log("未找到存档数据，初始化为全部未解锁状态");
        }

        Debug.Log($"当前已解锁收藏品数量: {unlockedCollectionIDs.Count}");
    }

    /// <summary>
    /// 重置所有解锁状态（用于测试）
    /// </summary>
    public void ResetAllCollections()
    {
        unlockedCollectionIDs.Clear();
        PlayerPrefs.DeleteKey("UnlockedCollections");
        PlayerPrefs.Save();
        Debug.Log("重置所有收集物解锁状态");
    }
}