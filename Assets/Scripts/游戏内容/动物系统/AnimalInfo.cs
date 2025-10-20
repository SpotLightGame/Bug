using System;
using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.PlayerLoop;
using UnityEngine.SceneManagement;

public class AnimalInfo : MonoBehaviour, IPointerClickHandler
{

    [Header("Bug 相关")]
    public bool isBug;
    public bool showBug;
    public AnimalType type;

    public Material bugMaterial;
    private Material normalMaterial;
    [SerializeField] private BattleInfoRecord_SO bugDB;
    [Range(0f, 1f)]
    [SerializeField] private float bugProbability = 0.1f;

    [Header("动态数据")]
    public float closeness_EXP;

    [Header("每日状态")]
    private int state;

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
    public BattleManager battleManager;
    [SerializeField] private AnimalPlace place;

    [Header("数据")]
    [SerializeField] public string animalID;
    [SerializeField] private float closeness_EXP_Add = 10f;
    [SerializeField] private float showValue = 100f;

    [Header("产品掉落")]
    [SerializeField] private GameObject[] productDropPrefab;
    
    [Header("首次抚摸特效")]
    public GameObject heartPrefab;
    

    void Start()
    {
        normalMaterial = GetComponent<SpriteRenderer>().material;
        placeManager = GameObject.FindGameObjectWithTag("Place")?.GetComponent<PlaceManager>();
        resourcesManager = GameObject.FindGameObjectWithTag("ResourcesManager")?.GetComponent<ResourcesManager>();
        battleManager = GameObject.Find("BattleManager")?.GetComponent<BattleManager>();


        Refresh();
        Debug.Log($"动物初始化: ID={animalID}, Type={type}, Position={transform.position}");
    }

    
    void Update()
    {
        SpriteChange();
    }

    private void OnEnable()
    {
        Debug.Log($"动物激活: ID={animalID}");
        TimeManager.Instance.OnNewDay += Refresh;

        TimeManager.Instance.ShowDay += Show;

        AnimalInfoRecord.Instance.BattleFinish += AfterBattleRefresh;
        
    }

    private void OnDisable()
    {
        TimeManager.Instance.OnNewDay -= Refresh;

        TimeManager.Instance.ShowDay -= Show;

        AnimalInfoRecord.Instance.BattleFinish -= AfterBattleRefresh;
    }

    // 每日刷新状态
    public void Refresh()
    {
        BugChange();
        state = 0;
        

        //若前一天心情较高，则增加亲密值
        if (mood >= 90f)
        {
            closeness_EXP += closeness_EXP_Add;
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
        if (placeManager.placeType.ToString() == place.ToString())
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

        
        ClosenessCheck();
        Produce();
        

    }



    // 点击事件，触摸优先，触摸完后再显示信息面板
    public void OnPointerClick(PointerEventData _)
    {
        if (showBug)
        {
            // 记录进运行时数据
            BattlePre.Instance.SetEnemy(type, bugDB.GetBugSprite(type));
            TimeManager.Instance.Pause(true);

            AnimalInfoRecord.Instance.state = state;
            AnimalInfoRecord.Instance.isBug = isBug;
            AnimalInfoRecord.Instance.showBug = showBug;

            battleManager.battleObj.SetActive(true);
            battleManager.SwitchToBattleCamera();
            
            return;
        }
        if (!isPetted)
        {
            Debug.Log("You petted the animal!");
            isPetted = true;
            closeness_EXP += closeness_EXP_Add;
            StartCoroutine(SpawnHeart());
        }
        else
        {
            AnimalInspectorUI.I.Show(this);
            TimeManager.Instance.Pause(true);
        }

    }

    // bug变化
    public void BugChange()
    {
        // bug化
        if (isBug) return;                    // 已经是 bug 就不用再判
        if (UnityEngine.Random.value < bugProbability)
        {
            isBug = true;
            
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
        if(closenessLevel >= 1f)
        {
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

    


    public void SpriteChange()
    {
        if (!showBug)
        {
            GetComponent<SpriteRenderer>().material = normalMaterial;
            return;
        }
        
        GetComponent<SpriteRenderer>().material = bugMaterial;
    }

    private void AfterBattleRefresh()
    {
        state = AnimalInfoRecord.Instance.state;

        if (state == 0) return;
        if (state == 1)
        {
            this.isBug = false;
            this.showBug = false;
            this.state = 0;
        }
        else if (state == 2)
        {
            Destroy(this.gameObject);
        }

    }
    
    IEnumerator SpawnHeart()
    {
        // 1. 按亲密度给颜色（0-1 范围）
        Color[] colors =
        {
            Color.white,
            Color.green,
            Color.cyan,
            Color.magenta,
            Color.red,
            new Color(1f, 0.84f, 0f) // 金色
        };
        SpriteRenderer sr = heartPrefab.GetComponent<SpriteRenderer>();
        sr.color = colors[Mathf.Clamp((int)closenessLevel, 0, 5)];

        // 2. 生成
        GameObject heart = Instantiate(heartPrefab, transform.position, Quaternion.identity);

        // 3. 保证渲染层与动物一致
        sr.sortingLayerID = GetComponent<SpriteRenderer>().sortingLayerID;
        sr.sortingOrder   = GetComponent<SpriteRenderer>().sortingOrder + 1;

        // 4. 上升 + 渐隐
        float t = 0f;
        Vector3 startPos = heart.transform.position;
        Color startColor = sr.color;

        while (t < 1f)
        {
            t += Time.deltaTime;
            float p = t; // 线性进度

            heart.transform.position = startPos + Vector3.up * (0.5f * p);
            sr.color = new Color(startColor.r, startColor.g, startColor.b, 1f - p);

            yield return null;
        }

        Destroy(heart);
    }
}
