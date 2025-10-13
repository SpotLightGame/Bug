using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class AnimalInfo : MonoBehaviour, IPointerClickHandler
{
    public AnimalData data;
    [SerializeField] private AnimalPlace place;
    [Header("动态数据")]
    public bool isBug;
    [SerializeField] private float closeness_EXP;
    [SerializeField] private float mood_EXP;
    [Header("每日状态")]
    private bool isHungry;
    private bool isThirsty;
    private bool isInRightPlace;
    private bool isBugAnimalAround;
    private bool isPetted;

    [Header("关联组件")]
    public PlaceManager placeManager;
    // Start is called before the first frame update
    void Start()
    {
        Refresh();
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnEnable()
    {
        TimeManager.Instance.OnNewDay += _ => Refresh();
    }

    private void OnDisable()
    {
        TimeManager.Instance.OnNewDay -= _ => Refresh();
    }

    public void Refresh()
    {
        placeManager = gameObject.GetComponentInParent<PlaceManager>();
        isHungry = true;
        isThirsty = true;

        if (placeManager.gameObject.tag == place.ToString())
            isInRightPlace = true;
        else
            isInRightPlace = false;

        if (placeManager.hasBugAnimal)
            isBugAnimalAround = true;
        else
            isBugAnimalAround = false;


    }
    
    public void OnPointerClick(PointerEventData _)
    {
        AnimalInspectorUI.I.Show(data);   // 单例面板
    }
}
