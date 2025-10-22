using UnityEngine;
using TMPro;
using System;
using UnityEngine.UI;

public class GoOutManager : MonoBehaviour
{
    [Header("����")]
    public TimeManager timeManager;
    public ResourcesManager resourcesManager;
    public GameObject dogSprite; // ������ʾ sprite
    public GameObject controlPanel; // �������UI
    public GameObject rewardPanel; // ����չʾ���
    public TMP_Text rewardText; // ������������
    public Image rewardIconImage;
    public Sprite goldIcon; // ���ͼ��
    public Sprite collectibleIcon; // �ռ���ͼ��
    public TMP_Text rewardIconText; // ͼ���Աߵ�����
    public GameObject sheepPrefab; // ���Ԥ����
    public Sprite sheepIcon;
    public Transform spawnPosition; // �������λ��
    public Button treasureButton; // Ѱ����ť
    

    [Header("״̬")]
    public bool isOut = false; // �Ƿ������
    public bool canGoOutToday = true; // �����Ƿ������
    private RewardType currentRewardType;
    private int currentGoldAmount;
    private bool hasGeneratedReward = false;

    [Header("ͼ��ϵͳ")]
    public GameObject albumManagerObject; // ͼ����������Ϸ����
    private AlbumManager albumManager; // ͼ�����������

    private CollectionSO currentCollectionReward; // ��ǰ�ռ��ｱ��

    // ��������ö��
    private enum RewardType
    {
        Gold,
        Collectible,
        Animal
    }

    private void Awake()
    {
        // �Զ���ȡ����
        if (timeManager == null)
            timeManager = FindObjectOfType<TimeManager>();

        if (resourcesManager == null)
            resourcesManager = FindObjectOfType<ResourcesManager>();

 
    
}

private void Start()
    {
        InitializeState();
        //ʱ���¼����µ�һ��/���紥��
        if (timeManager != null)
        {
            timeManager.OnNewDay += OnNewDay;
            timeManager.OnMidDay += OnMidDay;
            CheckIfPastMidDay();//����Ƿ��ѹ�����
        }
    }

    private void InitializeState()
    {
        // ��ʼ��������ʾ״̬�����ʱ���أ��ڼ�ʱ��ʾ��
        if (dogSprite != null)
        {
            dogSprite.SetActive(!isOut);
        }

        // ���ؿ������ͽ������
        if (controlPanel != null) controlPanel.SetActive(false);
        if (rewardPanel != null) rewardPanel.SetActive(false);

        UpdateTreasureButtonState(); // ����Ѱ����ť״̬
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
        // ��ť����״̬����������������δ���ʱ�ɵ��
        if (treasureButton != null)
        {
            treasureButton.interactable = canGoOutToday && !isOut;
            
            // ���°�ť���֣�����״̬��ʾ��ͬ�ı���
            TMP_Text buttonText = treasureButton.GetComponentInChildren<TMP_Text>();
            if (buttonText != null)
            {
                if (!canGoOutToday)
                    buttonText.text = "�����ѹ����ʱ��";
                else if (isOut)
                    buttonText.text = "�����";
                else
                    buttonText.text = "����Ѱ��";
            }
        }
    }

    private void OnNewDay()
    {
        // �µ�һ������״̬�����������������С�δ���ɽ���
        isOut = false;
        hasGeneratedReward = false;
        canGoOutToday = true;
        // ����UI����ʾ�����������
        if (dogSprite != null) dogSprite.SetActive(true);
        if (controlPanel != null) controlPanel.SetActive(false);
        if (rewardPanel != null) rewardPanel.SetActive(false);

        UpdateTreasureButtonState(); // ���°�ť
    }

    private void OnMidDay()
    {

        // ������������δ���ɽ��������ɽ�������ʾ
        if (isOut && !hasGeneratedReward)
        {
            if (dogSprite != null) dogSprite.SetActive(true);

            GenerateReward();
            ShowRewardPanel();//��ʾ�������
                             
            // ����״̬���ѹ��������������
            hasGeneratedReward = true;
            isOut = false;
            canGoOutToday = false;
            UpdateTreasureButtonState();
        }
        // ���δ������ѹ����磺��ֹ�������
        else if (!isOut)
        {
            canGoOutToday = false;
            UpdateTreasureButtonState();
        }
    }
    //����߼�
    private void OnMouseDown()
    {
        // ����Ƿ��ѹ����磨ʱ��>=300�룩����δ������ֹ�������
        if (!isOut && controlPanel != null)
        {
            if (!canGoOutToday) return;
            controlPanel.SetActive(true);
        }
    }
    //��ʼѰ��
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
    //��������
    private void GenerateReward()
    {
        float random = UnityEngine.Random.value;

        if (random < 0.6f)
        {
            currentRewardType = RewardType.Gold;
            GenerateGoldReward();
        }
        // 30% �ռ��� - ����Ƿ������ռ��ﶼ�ѽ���
        else if (random < 0.9f)
        {
            if (albumManager != null && !albumManager.AreAllCollectionsUnlocked())
            {
                currentRewardType = RewardType.Collectible;
                // ��ȡһ�����δ�������ռ���
                currentCollectionReward = albumManager.GetRandomLockedCollection();
                if (currentCollectionReward == null)
                {
                    // ���û��δ�������ռ�����˵����
                    currentRewardType = RewardType.Gold;
                    GenerateGoldReward();
                }
            }
          
        }
        else
        {
            currentRewardType = RewardType.Animal;
        }

        Debug.Log($"���ɵĽ�������: {currentRewardType}");
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

        rewardPanel.SetActive(true);//��ʾ�������

        // ����ͼ��״̬
        rewardIconImage.color = Color.white;
        rewardIconImage.enabled = true;

        // ���ͼ����ʾ�߼�
        Image rewardIcon = rewardPanel.GetComponentInChildren<Image>();
        if (rewardIcon == null)
        {
            // ���û���ҵ����������Ӷ����в���
            rewardIcon = rewardPanel.transform.Find("RewardIcon")?.GetComponent<Image>();
        }
        // ���ݽ������͸����������
        switch (currentRewardType)
        {
            case RewardType.Gold:
                rewardText.text = $"���������˽�ң�";
                rewardIconText.text = $"{currentGoldAmount}g";
                if (goldIcon != null)
                {
                    rewardIconImage.sprite = goldIcon;
                    Debug.Log("���ý��ͼ��");
                }
                break;
            case RewardType.Collectible:
                if (currentCollectionReward != null)
                {
                    rewardText.text = $"�����������µ���Ʒ��";
                    rewardIconText.text = currentCollectionReward.collectionName;
                    if (currentCollectionReward.unlockedIcon != null)
                    {
                        rewardIconImage.sprite = currentCollectionReward.unlockedIcon;
                        Debug.Log("�����ռ���ͼ��");
                    }
                }
                break;
            case RewardType.Animal:
                rewardText.text = $"����������һֻ��";
                rewardIconText.text = "�»�飡";
                if (sheepIcon != null) 
                {
                    rewardIconImage.sprite = sheepIcon;
                    Debug.Log("������ͼ��");
                }
                break;
        }
        Debug.Log($"��������: {currentRewardType}, ͼ��Sprite: {rewardIconImage.sprite}");
    }

    // ȷ��������ť 
    public void ConfirmReward()
    {
        Debug.Log($"ȷ�Ͻ���: {currentRewardType}");

        switch (currentRewardType)
        {
            case RewardType.Gold:
                if (resourcesManager != null)
                {
                    resourcesManager.AddGold(currentGoldAmount);
                    Debug.Log($"�������: {currentGoldAmount}");
                }
                break;
            case RewardType.Collectible:
                if (albumManager != null && currentCollectionReward != null)
                {
                    albumManager.UnlockCollection(currentCollectionReward.collectionID);
                    Debug.Log($"�����ռ���: {currentCollectionReward.collectionName}");
                }
                break;
            case RewardType.Animal:
                SpawnSheep();//������
                break;
        }

        if (rewardPanel != null) rewardPanel.SetActive(false);
    }

    // ר�Ŵ���������ɣ���⣩
    private void SpawnSheep()
    {
        Debug.Log("��ʼ������...");

        // ���Ԥ����
        if (sheepPrefab == null)
        {
            Debug.LogError("���Ԥ����δ��ֵ��");
            return;
        }

        // �������λ��
        if (spawnPosition == null)
        {
            Debug.LogError("�������λ��δ���ã�");
            // ���û����������λ�ã�ʹ�ù���λ��
            spawnPosition = transform;
            Debug.Log($"ʹ�ù���λ����Ϊ����λ��: {spawnPosition.position}");
        }

        // �������λ���Ƿ����������Ұ��
        Camera mainCamera = Camera.main;
        if (mainCamera != null)
        {
            Vector3 viewportPos = mainCamera.WorldToViewportPoint(spawnPosition.position);
            if (viewportPos.x < 0 || viewportPos.x > 1 || viewportPos.y < 0 || viewportPos.y > 1)
            {
                Debug.LogWarning("����λ�ÿ������������Ұ�⣬���Ե�������Ļ���ĸ���");
                // ��������Ļ���ĸ���
                Vector3 screenCenter = mainCamera.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, 10f));
                spawnPosition.position = screenCenter;
            }
        }

        try
        {
            // ʵ������
            GameObject newSheep = Instantiate(sheepPrefab, spawnPosition.position, Quaternion.identity);
            Debug.Log($"�����ɳɹ���λ��: {spawnPosition.position}");

            // ȷ�����Ǽ���״̬
            newSheep.SetActive(true);

            // ����һ�����ʵ�����
            newSheep.name = "DogFoundSheep";

            // ���������
            CheckSheepComponents(newSheep);

        }
        catch (System.Exception e)
        {
            Debug.LogError($"������ʱ����: {e.Message}");
        }
    }

    // ���������
    private void CheckSheepComponents(GameObject sheep)
    {
        // �����Ⱦ��
        Renderer renderer = sheep.GetComponent<Renderer>();
        if (renderer == null)
        {
            Debug.LogWarning("��û��Renderer��������ܲ��ɼ�");
            // ���Բ����������е�Renderer
            renderer = sheep.GetComponentInChildren<Renderer>();
            if (renderer != null)
                Debug.Log("�����������ҵ�Renderer���");
        }
        else
        {
            Debug.Log("����Renderer���");
        }

        // �����ײ�������ڵ����
        Collider collider = sheep.GetComponent<Collider>();
        if (collider == null)
            Debug.LogWarning("��û��Collider����������޷������");
        else
            Debug.Log("����Collider���");

        // ��鶯����Ϣ��������ʹ�����Ķ���ϵͳ��
        AnimalInfo animalInfo = sheep.GetComponent<AnimalInfo>();
        if (animalInfo == null)
            Debug.LogWarning("��û��AnimalInfo����������޷���������");
        else
            Debug.Log("����AnimalInfo���");

        Debug.Log($"��Ĳ㼶: {sheep.layer}, ��ǩ: {sheep.tag}");
    }

    // ���Է�����ǿ�����������ڲ��ԣ�
    public void DebugSpawnSheep()
    {
        Debug.Log("ǿ�����ɲ�����");
        currentRewardType = RewardType.Animal;
        SpawnSheep();
    }
}