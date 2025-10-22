using System.Collections.Generic;
using UnityEngine;

public class AlbumManager : MonoBehaviour
{
    [Header("�ռ����б�")]
    public List<CollectionSO> allCollections = new List<CollectionSO>();

    private HashSet<int> unlockedCollectionIDs = new HashSet<int>();

    private void Awake()
    {
        // ��ʼ����ʵ����Ŀ��Ӧ�Ӵ浵���أ�
        LoadUnlockedData();
        // ���ԣ�ȷ����ʼʱ�����ռ��ﶼ��δ����״̬
        Debug.Log($"��Ϸ����ʱ�������ռ�������: {unlockedCollectionIDs.Count}");
        foreach (var collection in allCollections)
        {
            Debug.Log($"�ռ��� {collection.collectionName} (ID: {collection.collectionID}) ����״̬: {IsUnlocked(collection.collectionID)}");
        }

    }

    /// <summary>
    /// �����ռ���
    /// </summary>
    public void UnlockCollection(int id)
    {
        if (!unlockedCollectionIDs.Contains(id))
        {
            unlockedCollectionIDs.Add(id);
            SaveUnlockedData();
            Debug.Log($"�����ռ���: {id}");
        }
    }

    /// <summary>
    /// ����ռ����Ƿ��ѽ���
    /// </summary>
    public bool IsUnlocked(int id)
    {
        return unlockedCollectionIDs.Contains(id);
    }

    /// <summary>
    /// ��ȡ���δ�������ռ���
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
    /// ����Ƿ������ռ��ﶼ�ѽ���
    /// </summary>
    public bool AreAllCollectionsUnlocked()
    {
        return unlockedCollectionIDs.Count >= allCollections.Count;
    }

    // ����������ݣ�ʵ����Ŀ��Ӧ����SaveManager��
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
        Debug.Log($"�����������: {string.Join(",", unlockedCollectionIDs)}");
    }

    // �޸�AlbumManager.cs��LoadUnlockedData����
    private void LoadUnlockedData()
    {
        unlockedCollectionIDs.Clear();

        // ֻ���б�������ʱ�ż��أ����򱣳ֿռ��ϣ�ȫ��δ����״̬��
        if (PlayerPrefs.HasKey("UnlockedCollections"))
        {
            string unlockedData = PlayerPrefs.GetString("UnlockedCollections");
            Debug.Log($"�Ӵ浵���ؽ�������: {unlockedData}");

            if (!string.IsNullOrEmpty(unlockedData))
            {
                string[] ids = unlockedData.Split(',');
                foreach (var idStr in ids)
                {
                    if (int.TryParse(idStr.Trim(), out int id))
                    {
                        // ��֤ID�Ƿ�����ڼ����У���ֹ��ЧID���µ�����
                        bool idExists = allCollections.Exists(c => c.collectionID == id);
                        if (idExists)
                        {
                            unlockedCollectionIDs.Add(id);
                            Debug.Log($"���ؽ������ղ�ƷID: {id}");
                        }
                        else
                        {
                            Debug.LogWarning($"��Ч���ղ�ƷID: {id}���Ѻ���");
                        }
                    }
                }
            }
        }
        else
        {
            Debug.Log("δ�ҵ��浵���ݣ���ʼ��Ϊȫ��δ����״̬");
        }

        Debug.Log($"��ǰ�ѽ����ղ�Ʒ����: {unlockedCollectionIDs.Count}");
    }

    /// <summary>
    /// �������н���״̬�����ڲ��ԣ�
    /// </summary>
    public void ResetAllCollections()
    {
        unlockedCollectionIDs.Clear();
        PlayerPrefs.DeleteKey("UnlockedCollections");
        PlayerPrefs.Save();
        Debug.Log("���������ռ������״̬");
    }
}