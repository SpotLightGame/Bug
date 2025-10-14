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

    [Header("每日状态")]
    private bool isInRightPlace;
    private bool isBugAnimalAround;

    [Header("关联组件")]
    public PlaceManager placeManager;

    [Header("数据")]
    [SerializeField] private float closeness_EXP_Add = 10f;

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
        TimeManager.Instance.OnNewDay +=  Refresh;
    }

    private void OnDisable()
    {
        TimeManager.Instance.OnNewDay -= Refresh;
    }

    // 每日刷新状态
    public void Refresh()
    {
        placeManager = gameObject.GetComponentInParent<PlaceManager>();
        data.mood = 50f;
        if (data != null)
        {
            data.isHungry = true;
            data.isThirsty = true;
        }

        if (placeManager.gameObject.tag == place.ToString())
        {
            isInRightPlace = true;
            data.mood += 10f;
        }
        else
        {
            isInRightPlace = false;
            data.mood -= 10f;
        }


        if (placeManager.hasBugAnimal)
        {
            isBugAnimalAround = true;
            data.mood -= 10f;
        }
        else
        {
            isBugAnimalAround = false;
            data.mood += 10f;
        }

        data.isPetted = false;
        


    }



    // 点击事件，触摸优先，触摸完后再显示信息面板
    public void OnPointerClick(PointerEventData _)
    {
        Debug.Log($"Click detected on {data.animalName}");
        if (!data.isPetted)
        {
            Debug.Log("You petted the animal!");
            data.isPetted = true;
            closeness_EXP += 10f;
            
        }
        else
        {
            AnimalInspectorUI.I.Show(data);
        }

    }
    
}
