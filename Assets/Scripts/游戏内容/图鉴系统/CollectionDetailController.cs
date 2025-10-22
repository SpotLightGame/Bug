using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CollectionDetailController : MonoBehaviour
{
    [Header("UI组件")]
    public GameObject detailPanel;
    public Image iconImage;
    public TMP_Text nameText;
    public TMP_Text descriptionText;
    public Button closeButton;

    private void Start()
    {
        // 初始化关闭按钮
        closeButton.onClick.AddListener(HideDetail);
        // 默认隐藏详情面板
        detailPanel.SetActive(false);
    }

    /// <summary>
    /// 显示收集物详情
    /// </summary>
    public void ShowDetail(CollectionSO collection)
    {
        if (collection == null) return;

        iconImage.sprite = collection.unlockedIcon;
        nameText.text = collection.collectionName;
        descriptionText.text = collection.description;

        detailPanel.SetActive(true);
    }

    /// <summary>
    /// 隐藏详情面板
    /// </summary>
    public void HideDetail()
    {
        detailPanel.SetActive(false);
    }
}