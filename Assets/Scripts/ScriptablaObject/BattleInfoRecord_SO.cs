using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Animal/BattleInfoDB")]
public class BattleInfoRecord_SO : ScriptableObject
{
    [System.Serializable]
    private struct BugInfo
    {
        public AnimalType type;
        public Sprite sprite;
    }

    [SerializeField] private List<BugInfo> bugs;

    // 启动时一次性转字典
    private Dictionary<AnimalType, Sprite> dict;

    private void OnEnable()   // SO 一加载就执行
    {
        dict = new Dictionary<AnimalType, Sprite>();
        foreach (var b in bugs)
            dict[b.type] = b.sprite;
    }

    // 对外接口：O(1) 查找
    public Sprite GetBugSprite(AnimalType type) =>
        dict.TryGetValue(type, out var sp) ? sp : null;
}