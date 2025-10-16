using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
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
    public float closenessLevel;
    public float mood;
    public bool isHungry;
    public bool isThirsty;
    public bool isPetted;
    private bool isInRightPlace;
    private bool isBugAnimalAround;
    [SerializeField] private AnimalCloseness closeness;

    [Header("关联组件")]
    public PlaceManager placeManager;
    public ResourcesManager resourcesManager;
    [SerializeField] private AnimalPlace place;
    [SerializeField] private AnimalType animalType;

    [Header("数值")]
    [SerializeField] private float closeness_EXP_Add = 10f;
    [SerializeField] private float showValue = 100f;

    [Header("产品掉落")]
    [SerializeField]private GameObject[] productDropPrefab;

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
        //若前一天心情较高，则增加亲密值
        if (mood >= 90f)
        {
            closeness_EXP += 10f;
        }
        
        //根据前一天状况决定新一天心情
        if(!isHungry && !isThirsty)
        {
            mood = 60f;
        }
        else
        {
            mood = 50f;
        }
        
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
        ClosenessCheck();
        Produce();
        

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

    // bug变化
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

    // 产出产品
    public void Produce()
    {
        if (productDropPrefab == null || productDropPrefab.Length == 0) return;
        int num = (int)UnityEngine.Random.Range(0f, 100f);
        ProductDrop drop = null;

        if (num < 70)
        {
            drop = Instantiate(productDropPrefab[0], transform.position, Quaternion.identity).GetComponent<ProductDrop>();
        }
        else if (num < 95 && productDropPrefab.Length > 1)
        {
            if (productDropPrefab[1] == null) return;
            drop = Instantiate(productDropPrefab[1], transform.position, Quaternion.identity).GetComponent<ProductDrop>(); ;
        }
        else if (productDropPrefab.Length > 2)
        {
            if (productDropPrefab[2] == null) return;
            drop = Instantiate(productDropPrefab[2], transform.position, Quaternion.identity).GetComponent<ProductDrop>(); ;
        }

        if (isPetted)
        {
            drop.productValue *= 2;
        }

    }

    // 计算亲密度
    public void ClosenessCheck()
    {
        if (closeness_EXP >= 0f && closeness_EXP < 100f)
        {
            closenessLevel = 0f;
        }
        else if (closeness_EXP >= 100f && closeness_EXP < 300f)
        {
            closenessLevel = 1f;
        }
        else if (closeness_EXP >= 300f && closeness_EXP < 600f)
        {
            closenessLevel = 2f;
        }
        else if (closeness_EXP >= 600f && closeness_EXP < 1000f)
        {
            closenessLevel = 3f;
        }
        else if (closeness_EXP >= 1000f && closeness_EXP < 1500f)
        {
            closenessLevel = 4f;
        }
        else
        {
            closenessLevel = 5f;
        }
    }
    

}
