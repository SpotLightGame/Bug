using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class AlbumUIController : MonoBehaviour
{
    [Header("引用")]
    public AlbumManager albumManager;
    public GameObject collectionItemPrefab;
    public Transform collectionGrid;
    public CollectionDetailController detailPanel;
    public GameObject albumPanel;
    public Button closeAlbumButton;

    private List<CollectionItemUI> collectionItems = new List<CollectionItemUI>();

    private void Start()
    {
        // 初始化图鉴UI
        InitializeCollectionGrid();
        // 默认隐藏图鉴面板
        albumPanel.SetActive(false);
        // 新增：绑定关闭按钮事件
        closeAlbumButton.onClick.AddListener(HideAlbum);
    }

    /// <summary>
    /// 初始化收集物网格
    /// </summary>
    private void InitializeCollectionGrid()
    {
        // 清空现有项
        foreach (var item in collectionItems)
        {
            Destroy(item.gameObject);
        }
        collectionItems.Clear();

        // 创建所有收集物项
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
                Debug.LogError("收集物预制体缺少CollectionItemUI组件");
            }
        }
    }

    /// <summary>
    /// 显示图鉴面板
    /// </summary>
    public void ShowAlbum()
    {
        albumPanel.SetActive(true);
        InitializeCollectionGrid(); // 刷新显示
    }

    /// <summary>
    /// 隐藏图鉴面板
    /// </summary>
    public void HideAlbum()
    {
        albumPanel.SetActive(false);
    }

    /// <summary>
    /// 显示收集物详情
    /// </summary>
    public void ShowCollectionDetail(CollectionSO collection)
    {
        if (detailPanel != null)
        {
            detailPanel.ShowDetail(collection);
            // 确保详情面板在图鉴面板之上
            detailPanel.detailPanel.transform.SetAsLastSibling();
            Debug.Log($"显示收藏品详情: {collection.collectionName}");
        }
        else
        {
            Debug.LogError("未设置CollectionDetailController引用！请在Inspector中赋值");
        }
    }
}