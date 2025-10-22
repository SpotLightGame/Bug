using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class CollectionItemUI : MonoBehaviour
{
    [Header("UI���")]
    public Image iconImage;
    public Button itemButton;

    private CollectionSO collectionSO;
    private AlbumUIController albumController;

    /// <summary>
    /// ��ʼ���ռ�����
    /// </summary>
    // �޸�CollectionItemUI.cs��Initialize����
    // �޸�CollectionItemUI.cs��Initialize����
    public void Initialize(CollectionSO collection, bool isUnlocked, AlbumUIController controller)
    {
        collectionSO = collection;
        albumController = controller;

        // ����ͼ��
        iconImage.sprite = isUnlocked ? collection.unlockedIcon : collection.lockedIcon;
        iconImage.color = isUnlocked ? Color.white : Color.gray;

        // ���ð�ť״̬
        itemButton.onClick.RemoveAllListeners();

        // ֻ���ѽ��������ӵ���¼�������Ϊ�ɽ���
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
    /// ����ռ�����
    /// </summary>
    private void OnItemClicked()
    {
        if (collectionSO != null && albumController != null)
        {
            albumController.ShowCollectionDetail(collectionSO);
        }
    }
}