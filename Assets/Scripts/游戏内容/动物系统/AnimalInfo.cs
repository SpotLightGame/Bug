using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class AnimalInfo : MonoBehaviour, IPointerClickHandler
{
    public bool isBug;
    [Header("Bug相关")]
    //bug贴图
    public float num;

    [Header("动态数据")]
    [SerializeField] private float closeness_EXP;

    [Header("每日状态")]
    public int age;
    public float closeness;
    public float mood;
    public bool isHungry;
    public bool isThirsty;
    public bool isPetted;
    private bool isInRightPlace;
    private bool isBugAnimalAround;

    [Header("关联组件")]
    public PlaceManager placeManager;
    public ResourcesManager resourcesManager;
    [SerializeField] private AnimalPlace place;

    [Header("数值")]
    [SerializeField] private float closeness_EXP_Add = 10f;
    [SerializeField] private float showValue = 100f;

    // Start is called before the first frame update
    void Start()
    {
        placeManager = GameObject.FindGameObjectWithTag("Place").GetComponent<PlaceManager>();
        resourcesManager = GameObject.FindGameObjectWithTag("ResourcesManager").GetComponent<ResourcesManager>();
        Refresh();
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnEnable()
    {
        TimeManager.Instance.OnNewDay += Refresh;
        TimeManager.Instance.ShowDay += Show;
    }

    private void OnDisable()
    {
        TimeManager.Instance.OnNewDay -= Refresh;
        TimeManager.Instance.ShowDay -= Show;
    }

    // 每日刷新状态
    public void Refresh()
    {
        mood = 50f;
        isHungry = true;
        isThirsty = true;

        //是否适配场地
        if (placeManager.gameObject.tag == place.ToString())
        {
            isInRightPlace = true;
            mood += 10f;
        }
        else
        {
            isInRightPlace = false;
            mood -= 10f;
        }

        //是否有bug动物
        if (placeManager.hasBugAnimal)
        {
            isBugAnimalAround = true;
            mood -= 10f;
        }
        else
        {
            isBugAnimalAround = false;
            mood += 10f;
        }

        //抚摸
        isPetted = false;

        BugChange();
        

    }



    // 点击事件，触摸优先，触摸完后再显示信息面板
    public void OnPointerClick(PointerEventData _)
    {
        if (!isPetted)
        {
            Debug.Log("You petted the animal!");
            isPetted = true;
            closeness_EXP += 10f;

        }
        else
        {
            AnimalInspectorUI.I.Show(this);
        }

    }

    public void BugChange()
    {
        // bug化
        if (!isBug)
        {
            num = (int)UnityEngine.Random.Range(0f, 100f);
            if (num % 10 == 0)
            {
                isBug = true;
                gameObject.GetComponent<SpriteRenderer>().color = new Color(255F, 0F, 255F);
            }
            else
                return;
        }
    }
    
    public void Show()
    {
        resourcesManager.gold += showValue;
    }
}
