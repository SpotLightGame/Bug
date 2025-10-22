using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class AlbumUIController : MonoBehaviour
{
    [Header("����")]
    public AlbumManager albumManager;
    public GameObject collectionItemPrefab;
    public Transform collectionGrid;
    public CollectionDetailController detailPanel;
    public GameObject albumPanel;
    public Button closeAlbumButton;

    private List<CollectionItemUI> collectionItems = new List<CollectionItemUI>();

    private void Start()
    {
        // ��ʼ��ͼ��UI
        InitializeCollectionGrid();
        // Ĭ������ͼ�����
        albumPanel.SetActive(false);
        // �������󶨹رհ�ť�¼�
        closeAlbumButton.onClick.AddListener(HideAlbum);
    }

    /// <summary>
    /// ��ʼ���ռ�������
    /// </summary>
    private void InitializeCollectionGrid()
    {
        // ���������
        foreach (var item in collectionItems)
        {
            Destroy(item.gameObject);
        }
        collectionItems.Clear();

        // ���������ռ�����
        foreach (var collection in albumManager.allCollections)
        {
            GameObject itemObj = Instantiate(collectionItemPrefab, collectionGrid);
            CollectionItemUI itemUI = itemObj.GetComponent<CollectionItemUI>();

            if (itemUI != null)
            {
                itemUI.Initialize(collection, albumManager.IsUnlocked(collection.collectionID), this);
                collectionItems.Add(itemUI);
            }
            else
            {
                Destroy(itemObj);
                Debug.LogError("�ռ���Ԥ����ȱ��CollectionItemUI���");
            }
        }
    }

    /// <summary>
    /// ��ʾͼ�����
    /// </summary>
    public void ShowAlbum()
    {
        albumPanel.SetActive(true);
        InitializeCollectionGrid(); // ˢ����ʾ
    }

    /// <summary>
    /// ����ͼ�����
    /// </summary>
    public void HideAlbum()
    {
        albumPanel.SetActive(false);
    }

    /// <summary>
    /// ��ʾ�ռ�������
    /// </summary>
    public void ShowCollectionDetail(CollectionSO collection)
    {
        if (detailPanel != null)
        {
            detailPanel.ShowDetail(collection);
            // ȷ�����������ͼ�����֮��
            detailPanel.detailPanel.transform.SetAsLastSibling();
            Debug.Log($"��ʾ�ղ�Ʒ����: {collection.collectionName}");
        }
        else
        {
            Debug.LogError("δ����CollectionDetailController���ã�����Inspector�и�ֵ");
        }
    }
}