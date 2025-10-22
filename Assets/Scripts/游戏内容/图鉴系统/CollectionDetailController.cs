using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CollectionDetailController : MonoBehaviour
{
    [Header("UI���")]
    public GameObject detailPanel;
    public Image iconImage;
    public TMP_Text nameText;
    public TMP_Text descriptionText;
    public Button closeButton;

    private void Start()
    {
        // ��ʼ���رհ�ť
        closeButton.onClick.AddListener(HideDetail);
        // Ĭ�������������
        detailPanel.SetActive(false);
    }

    /// <summary>
    /// ��ʾ�ռ�������
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
    /// �����������
    /// </summary>
    public void HideDetail()
    {
        detailPanel.SetActive(false);
    }
}