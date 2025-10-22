using UnityEngine;
using TMPro;
using System;
using UnityEngine.UI;

public class GoOutManager : MonoBehaviour
{
    [Header("引用")]
    public TimeManager timeManager;
    public ResourcesManager resourcesManager;
    public GameObject dogSprite; // 狗的显示 sprite
    public GameObject controlPanel; // 控制面板UI
    public GameObject rewardPanel; // 奖励展示面板
    public TMP_Text rewardText; // 奖励文字描述
    public Image rewardIconImage;
    public Sprite goldIcon; // 金币图标
    public Sprite collectibleIcon; // 收集物图标
    public TMP_Text rewardIconText; // 图标旁边的文字
    public GameObject sheepPrefab; // 羊的预制体
    public Sprite sheepIcon;
    public Transform spawnPosition; // 羊的生成位置
    public Button treasureButton; // 寻宝按钮
    

    [Header("状态")]
    public bool isOut = false; // 是否外出中
    public bool canGoOutToday = true; // 今天是否还能外出
    private RewardType currentRewardType;
    private int currentGoldAmount;
    private bool hasGeneratedReward = false;

    [Header("图鉴系统")]
    public GameObject albumManagerObject; // 图鉴管理器游戏对象
    private AlbumManager albumManager; // 图鉴管理器组件

    private CollectionSO currentCollectionReward; // 当前收集物奖励

    // 奖励类型枚举
    private enum RewardType
    {
        Gold,
        Collectible,
        Animal
    }

    private void Awake()
    {
        // 自动获取引用
        if (timeManager == null)
            timeManager = FindObjectOfType<TimeManager>();

        if (resourcesManager == null)
            resourcesManager = FindObjectOfType<ResourcesManager>();

 
    
}

private void Start()
    {
        InitializeState();
        //时间事件，新的一天/中午触发
        if (timeManager != null)
        {
            timeManager.OnNewDay += OnNewDay;
            timeManager.OnMidDay += OnMidDay;
            CheckIfPastMidDay();//检查是否已过中午
        }
    }

    private void InitializeState()
    {
        // 初始化狗的显示状态（外出时隐藏，在家时显示）
        if (dogSprite != null)
        {
            dogSprite.SetActive(!isOut);
        }

        // 隐藏控制面板和奖励面板
        if (controlPanel != null) controlPanel.SetActive(false);
        if (rewardPanel != null) rewardPanel.SetActive(false);

        UpdateTreasureButtonState(); // 更新寻宝按钮状态
    }

    private void OnEnable()
    {
        if (timeManager != null)
        {
           
            timeManager.OnNewDay += OnNewDay;
            timeManager.OnMidDay += OnMidDay;
        }
    }

    private void OnDisable()
    {
        if (timeManager != null)
        {
            timeManager.OnNewDay -= OnNewDay;
            timeManager.OnMidDay -= OnMidDay;
        }
    }

    private void Update()
    {
        CheckIfPastMidDay();
    }

    private void CheckIfPastMidDay()
    {
        if (timeManager != null)
        {
            bool isPastMidDay = timeManager.timer >= 300f;

            if (isPastMidDay && canGoOutToday)
            {
                canGoOutToday = false;
                UpdateTreasureButtonState();
            }
        }
    }

    private void UpdateTreasureButtonState()
    {
        // 按钮交互状态：仅当今天可外出且未外出时可点击
        if (treasureButton != null)
        {
            treasureButton.interactable = canGoOutToday && !isOut;
            
            // 更新按钮文字（根据状态显示不同文本）
            TMP_Text buttonText = treasureButton.GetComponentInChildren<TMP_Text>();
            if (buttonText != null)
            {
                if (!canGoOutToday)
                    buttonText.text = "今日已过外出时间";
                else if (isOut)
                    buttonText.text = "外出中";
                else
                    buttonText.text = "出发寻宝";
            }
        }
    }

    private void OnNewDay()
    {
        // 新的一天重置状态：可外出、不在外出中、未生成奖励
        isOut = false;
        hasGeneratedReward = false;
        canGoOutToday = true;
        // 更新UI：显示狗、隐藏面板
        if (dogSprite != null) dogSprite.SetActive(true);
        if (controlPanel != null) controlPanel.SetActive(false);
        if (rewardPanel != null) rewardPanel.SetActive(false);

        UpdateTreasureButtonState(); // 更新按钮
    }

    private void OnMidDay()
    {

        // 如果在外出中且未生成奖励：生成奖励并显示
        if (isOut && !hasGeneratedReward)
        {
            if (dogSprite != null) dogSprite.SetActive(true);

            GenerateReward();
            ShowRewardPanel();//显示奖励面板
                             
            // 更新状态：已归来、不可再外出
            hasGeneratedReward = true;
            isOut = false;
            canGoOutToday = false;
            UpdateTreasureButtonState();
        }
        // 如果未外出但已过中午：禁止当天外出
        else if (!isOut)
        {
            canGoOutToday = false;
            UpdateTreasureButtonState();
        }
    }
    //外出逻辑
    private void OnMouseDown()
    {
        // 检查是否已过中午（时间>=300秒），若未外出则禁止当天外出
        if (!isOut && controlPanel != null)
        {
            if (!canGoOutToday) return;
            controlPanel.SetActive(true);
        }
    }
    //开始寻宝
    public void StartTreasureHunt()
    {
        if (!canGoOutToday) return;

        if (!isOut)
        {
            isOut = true;
            hasGeneratedReward = false;

            if (dogSprite != null) dogSprite.SetActive(false);
            if (controlPanel != null) controlPanel.SetActive(false);

            UpdateTreasureButtonState();
        }
    }

    public void Cancel()
    {
        if (controlPanel != null) controlPanel.SetActive(false);
    }
    //奖励生成
    private void GenerateReward()
    {
        float random = UnityEngine.Random.value;

        if (random < 0.6f)
        {
            currentRewardType = RewardType.Gold;
            GenerateGoldReward();
        }
        // 30% 收集物 - 检查是否所有收集物都已解锁
        else if (random < 0.9f)
        {
            if (albumManager != null && !albumManager.AreAllCollectionsUnlocked())
            {
                currentRewardType = RewardType.Collectible;
                // 获取一个随机未解锁的收集物
                currentCollectionReward = albumManager.GetRandomLockedCollection();
                if (currentCollectionReward == null)
                {
                    // 如果没有未解锁的收集物，回退到金币
                    currentRewardType = RewardType.Gold;
                    GenerateGoldReward();
                }
            }
          
        }
        else
        {
            currentRewardType = RewardType.Animal;
        }

        Debug.Log($"生成的奖励类型: {currentRewardType}");
    }

    private void GenerateGoldReward()
    {
        float goldRandom = UnityEngine.Random.value;

        if (goldRandom < 0.5f) currentGoldAmount = 500;
        else if (goldRandom < 0.8f) currentGoldAmount = 1000;
        else if (goldRandom < 0.95f) currentGoldAmount = 3000;
        else currentGoldAmount = 5000;
    }

    private void ShowRewardPanel()
    {

        rewardPanel.SetActive(true);//显示奖励面板

        // 重置图标状态
        rewardIconImage.color = Color.white;
        rewardIconImage.enabled = true;

        // 添加图标显示逻辑
        Image rewardIcon = rewardPanel.GetComponentInChildren<Image>();
        if (rewardIcon == null)
        {
            // 如果没有找到，尝试在子对象中查找
            rewardIcon = rewardPanel.transform.Find("RewardIcon")?.GetComponent<Image>();
        }
        // 根据奖励类型更新面板内容
        switch (currentRewardType)
        {
            case RewardType.Gold:
                rewardText.text = $"狗狗带回了金币！";
                rewardIconText.text = $"{currentGoldAmount}g";
                if (goldIcon != null)
                {
                    rewardIconImage.sprite = goldIcon;
                    Debug.Log("设置金币图标");
                }
                break;
            case RewardType.Collectible:
                if (currentCollectionReward != null)
                {
                    rewardText.text = $"狗狗带回了新的物品！";
                    rewardIconText.text = currentCollectionReward.collectionName;
                    if (currentCollectionReward.unlockedIcon != null)
                    {
                        rewardIconImage.sprite = currentCollectionReward.unlockedIcon;
                        Debug.Log("设置收集物图标");
                    }
                }
                break;
            case RewardType.Animal:
                rewardText.text = $"狗狗带回了一只羊！";
                rewardIconText.text = "新伙伴！";
                if (sheepIcon != null) 
                {
                    rewardIconImage.sprite = sheepIcon;
                    Debug.Log("设置羊图标");
                }
                break;
        }
        Debug.Log($"奖励类型: {currentRewardType}, 图标Sprite: {rewardIconImage.sprite}");
    }

    // 确定奖励按钮 
    public void ConfirmReward()
    {
        Debug.Log($"确认奖励: {currentRewardType}");

        switch (currentRewardType)
        {
            case RewardType.Gold:
                if (resourcesManager != null)
                {
                    resourcesManager.AddGold(currentGoldAmount);
                    Debug.Log($"金币增加: {currentGoldAmount}");
                }
                break;
            case RewardType.Collectible:
                if (albumManager != null && currentCollectionReward != null)
                {
                    albumManager.UnlockCollection(currentCollectionReward.collectionID);
                    Debug.Log($"解锁收集物: {currentCollectionReward.collectionName}");
                }
                break;
            case RewardType.Animal:
                SpawnSheep();//生成羊
                break;
        }

        if (rewardPanel != null) rewardPanel.SetActive(false);
    }

    // 专门处理羊的生成（检测）
    private void SpawnSheep()
    {
        Debug.Log("开始生成羊...");

        // 检查预制体
        if (sheepPrefab == null)
        {
            Debug.LogError("羊的预制体未赋值！");
            return;
        }

        // 检查生成位置
        if (spawnPosition == null)
        {
            Debug.LogError("羊的生成位置未设置！");
            // 如果没有设置生成位置，使用狗的位置
            spawnPosition = transform;
            Debug.Log($"使用狗的位置作为生成位置: {spawnPosition.position}");
        }

        // 检查生成位置是否在摄像机视野内
        Camera mainCamera = Camera.main;
        if (mainCamera != null)
        {
            Vector3 viewportPos = mainCamera.WorldToViewportPoint(spawnPosition.position);
            if (viewportPos.x < 0 || viewportPos.x > 1 || viewportPos.y < 0 || viewportPos.y > 1)
            {
                Debug.LogWarning("生成位置可能在摄像机视野外，尝试调整到屏幕中心附近");
                // 调整到屏幕中心附近
                Vector3 screenCenter = mainCamera.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, 10f));
                spawnPosition.position = screenCenter;
            }
        }

        try
        {
            // 实例化羊
            GameObject newSheep = Instantiate(sheepPrefab, spawnPosition.position, Quaternion.identity);
            Debug.Log($"羊生成成功！位置: {spawnPosition.position}");

            // 确保羊是激活状态
            newSheep.SetActive(true);

            // 给羊一个合适的名字
            newSheep.name = "DogFoundSheep";

            // 检查羊的组件
            CheckSheepComponents(newSheep);

        }
        catch (System.Exception e)
        {
            Debug.LogError($"生成羊时出错: {e.Message}");
        }
    }

    // 检查羊的组件
    private void CheckSheepComponents(GameObject sheep)
    {
        // 检查渲染器
        Renderer renderer = sheep.GetComponent<Renderer>();
        if (renderer == null)
        {
            Debug.LogWarning("羊没有Renderer组件，可能不可见");
            // 尝试查找子物体中的Renderer
            renderer = sheep.GetComponentInChildren<Renderer>();
            if (renderer != null)
                Debug.Log("在子物体中找到Renderer组件");
        }
        else
        {
            Debug.Log("羊有Renderer组件");
        }

        // 检查碰撞器（用于点击）
        Collider collider = sheep.GetComponent<Collider>();
        if (collider == null)
            Debug.LogWarning("羊没有Collider组件，可能无法被点击");
        else
            Debug.Log("羊有Collider组件");

        // 检查动物信息组件（如果使用您的动物系统）
        AnimalInfo animalInfo = sheep.GetComponent<AnimalInfo>();
        if (animalInfo == null)
            Debug.LogWarning("羊没有AnimalInfo组件，可能无法正常运作");
        else
            Debug.Log("羊有AnimalInfo组件");

        Debug.Log($"羊的层级: {sheep.layer}, 标签: {sheep.tag}");
    }

    // 调试方法：强制生成羊（用于测试）
    public void DebugSpawnSheep()
    {
        Debug.Log("强制生成测试羊");
        currentRewardType = RewardType.Animal;
        SpawnSheep();
    }
}