using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class AnimalInstance : MonoBehaviour, IPointerClickHandler
{
    public AnimalData data;          // 拖对应资产
    [SerializeField] SpriteRenderer body;

    void Start() => body.sprite = data.icon;

    /* --------------- 关键：把点击事件抛给中枢 --------------- */
    public void OnPointerClick(PointerEventData _)
    {
        AnimalInspectorUI.I.Show(data);   // 单例面板
    }
}
