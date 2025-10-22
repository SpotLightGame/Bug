using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class CollectionItemUI : MonoBehaviour
{
    [Header("UI组件")]
    public Image iconImage;
    public Button itemButton;

    private CollectionSO collectionSO;
    private AlbumUIController albumController;

    /// <summary>
    /// 初始化收集物项
    /// </summary>
    // 修改CollectionItemUI.cs的Initialize方法
    // 修改CollectionItemUI.cs的Initialize方法
    public void Initialize(CollectionSO collection, bool isUnlocked, AlbumUIController controller)
    {
        collectionSO = collection;
        albumController = controller;

        // 设置图标
        iconImage.sprite = isUnlocked ? collection.unlockedIcon : collection.lockedIcon;
        iconImage.color = isUnlocked ? Color.white : Color.gray;

        // 重置按钮状态
        itemButton.onClick.RemoveAllListeners();

        // 只有已解锁项才添加点击事件并设置为可交互
        if (isUnlocked)
        {
            itemButton.interactable = true;
            itemButton.onClick.AddListener(OnItemClicked);
        }
        else
        {
            itemButton.interactable = false;
        }
    }
    /// <summary>
    /// 点击收集物项
    /// </summary>
    private void OnItemClicked()
    {
        if (collectionSO != null && albumController != null)
        {
            albumController.ShowCollectionDetail(collectionSO);
        }
    }
}