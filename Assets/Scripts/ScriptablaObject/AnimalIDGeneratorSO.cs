using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Animal/IDGenerator")]
public class AnimalIDGeneratorSO : ScriptableObject
{
    [SerializeField] private int currentID = 0;
    [SerializeField] private List<AnimalPrefabPair> prefabTable = new List<AnimalPrefabPair>();

    [System.Serializable]
    private class AnimalPrefabPair
    {
        public AnimalType type;
        public GameObject prefab;
    }

    // 修改这个方法，使用动物类型作为前缀
    public string GetNextID(AnimalType animalType)
    {
        currentID++;
        return $"{animalType}_{currentID}";
    }

    // 读档时用到 - 根据动物类型获取对应的预制体
    public GameObject GetPrefab(AnimalType type)
    {
        var pair = prefabTable.Find(p => p.type == type);
        if (pair == null)
        {
            Debug.LogError($"在预制体表中找不到类型为 {type} 的动物预制体！");
            return null;
        }
        if (pair.prefab == null)
        {
            Debug.LogError($"类型为 {type} 的动物预制体引用为空！");
            return null;
        }
        return pair.prefab;
    }

    // 让外部能改写 currentID（读档后推最大值）
    public void SetCurrentID(int id) => currentID = id;

    // 添加一个调试方法，检查预制体表配置
    public void ValidatePrefabTable()
    {
        foreach (var pair in prefabTable)
        {
            if (pair.prefab == null)
            {
                Debug.LogError($"预制体表中 {pair.type} 的预制体为空！");
            }
            else if (pair.prefab.GetComponent<AnimalInfo>() == null)
            {
                Debug.LogError($"预制体 {pair.prefab.name} 没有 AnimalInfo 组件！");
            }
        }
    }

    public void ValidateConfiguration()
    {
        if (prefabTable.Count == 0)
        {
            Debug.LogError("AnimalIDGeneratorSO 的 Prefab Table 为空！请在 Inspector 中配置动物类型与预制体的映射。");
            return;
        }
        
        foreach (var pair in prefabTable)
        {
            if (pair.prefab == null)
            {
                Debug.LogError($"AnimalIDGeneratorSO: 类型 {pair.type} 的预制体为空！");
            }
            else if (pair.prefab.GetComponent<AnimalInfo>() == null)
            {
                Debug.LogError($"AnimalIDGeneratorSO: 预制体 {pair.prefab.name} 缺少 AnimalInfo 组件！");
            }
        }
        
        Debug.Log($"AnimalIDGeneratorSO 配置验证完成，共有 {prefabTable.Count} 种动物类型。");
    }
}