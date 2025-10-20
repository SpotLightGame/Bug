using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class SaveManager : MonoBehaviour
{
    public static SaveManager Instance { get; private set; }

    private string SavePath => Path.Combine(Application.persistentDataPath, "farmSave.json");

    // 添加对 AnimalIDGeneratorSO 的引用
    [Header("引用")]
    public AnimalIDGeneratorSO idGeneratorSO;

    private void Awake()
    {
        if (Instance && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        // 添加这行来显示存档路径
        Debug.Log("存档路径: " + SavePath);
    }
    
    void Start()
    {
        // 直接使用已分配的引用，不再使用 FindObjectOfType
        if (idGeneratorSO != null)
        {
            idGeneratorSO.ValidateConfiguration();
        }
        else
        {
            Debug.LogError("SaveManager: idGeneratorSO 未分配！请在 Inspector 中分配 AnimalIDGeneratorSO。");
        }
    }

    /* 自动存档入口：TimeManager 的 OnNewDay 会调它 */
    public void Save()
    {
        SaveData data = new SaveData
        {
            year   = TimeManager.Instance.year,
            season = TimeManager.Instance.season,
            day    = TimeManager.Instance.day,
            gold   = FindObjectOfType<ResourcesManager>().gold
        };

        /* 把场景里所有 AnimalInfo 拍个快照 */
        foreach (var ai in FindObjectsOfType<AnimalInfo>())
        {
            data.animals.Add(new AnimalSnapshot
            {
                animalID    = ai.animalID,
                type        = ai.type,
                isBug       = ai.isBug,
                showBug     = ai.showBug,
                closenessEXP= ai.closeness_EXP,
                age         = ai.age,
                mood        = ai.mood,
                pos         = ai.transform.position
            });
        }

        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(SavePath, json);
        Debug.Log("[Save] 已存档 " + SavePath);
    }

    /* 启动时读档 */
    public void Load()
    {
        if (!File.Exists(SavePath)) { Debug.Log("无存档"); return; }

        string json = File.ReadAllText(SavePath);
        SaveData data = JsonUtility.FromJson<SaveData>(json);

        // 添加存档数据调试
        Debug.Log($"[Load] 开始读档，动物数量: {data.animals.Count}");

        /* 时间 */
        TimeManager.Instance.SetDate(data.year, data.season, data.day);

        /* 金钱 */
        FindObjectOfType<ResourcesManager>().gold = data.gold;

        /* 动物：先清场，再重建 */
        var existingAnimals = FindObjectsOfType<AnimalInfo>();
        Debug.Log($"[Load] 清空现有动物: {existingAnimals.Length} 只");
        foreach (var old in existingAnimals)
            Destroy(old.gameObject);

        // 检查 ID 生成器是否存在
        if (idGeneratorSO == null)
        {
            Debug.LogError("AnimalIDGeneratorSO 未分配！");
            return;
        }

        // 验证预制体表
        idGeneratorSO.ValidatePrefabTable();

        int maxID = 0;
        int successCount = 0;
        
        foreach (var snap in data.animals)
        {
            // 添加每个动物的调试信息
            Debug.Log($"[Load] 正在加载动物: ID={snap.animalID}, Type={snap.type}, Pos={snap.pos}");

            // 关键修改：根据动物类型获取对应的预制体
            GameObject prefab = idGeneratorSO.GetPrefab(snap.type);
            
            // 检查预制体是否存在
            if (prefab == null)
            {
                Debug.LogError($"找不到类型 {snap.type} 的预制体！");
                continue;
            }

            GameObject go = Instantiate(prefab, snap.pos, Quaternion.identity);
            AnimalInfo ai = go.GetComponent<AnimalInfo>();
            
            // 检查 AnimalInfo 组件
            if (ai == null)
            {
                Debug.LogError($"预制体 {prefab.name} 没有 AnimalInfo 组件！");
                Destroy(go);
                continue;
            }

            // 设置动物属性
            ai.animalID = snap.animalID;
            ai.type = snap.type;
            ai.isBug = snap.isBug;
            ai.showBug = snap.showBug;
            ai.closeness_EXP = snap.closenessEXP;
            ai.age = snap.age;
            ai.mood = snap.mood;

            // 解析ID获取最大ID值
            string[] seg = snap.animalID.Split('_');
            if (seg.Length == 2 && int.TryParse(seg[1], out int num))
            {
                maxID = Mathf.Max(maxID, num);
                Debug.Log($"[Load] 解析ID: {snap.animalID} -> 数字部分: {num}");
            }
            else
            {
                Debug.LogWarning($"[Load] 无法解析动物ID格式: {snap.animalID}");
            }
                
            successCount++;
            Debug.Log($"[Load] 成功加载动物: {snap.animalID}");
        }
        
        idGeneratorSO.SetCurrentID(maxID);
        Debug.Log($"[Load] 读档完成，成功加载 {successCount}/{data.animals.Count} 只动物，设置当前最大ID为: {maxID}");
    }
}