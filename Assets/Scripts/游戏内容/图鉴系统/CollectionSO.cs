using UnityEngine;

[CreateAssetMenu(fileName = "NewCollection", menuName = "�ռ���/CollectionSO")]
public class CollectionSO : ScriptableObject
{
    [Header("������Ϣ")]
    public int collectionID; // ΨһID
    public string collectionName; // �ռ�������
    public string description; // �ռ�������

    [Header("ͼ��")]
    public Sprite unlockedIcon; // �ѽ���ͼ��
    public Sprite lockedIcon; // δ����ͼ�꣨�ʺ�ͼ��
}