using UnityEngine;

[System.Serializable]
public class SceneField
{
    [SerializeField] private Object sceneAsset; // 在编辑器里拖场景文件
    private string sceneName = "";

    public string SceneName
    {
        get
        {
#if UNITY_EDITOR
            // 只在编辑器里解析名字
            if (sceneAsset == null) return "";
            string path = UnityEditor.AssetDatabase.GetAssetPath(sceneAsset);
            return System.IO.Path.GetFileNameWithoutExtension(path);
#else
            return sceneName;   // 运行期用缓存
#endif
        }
    }

#if UNITY_EDITOR
    // 保存时把名字写进序列化字段，运行期不用解析
    public void OnValidate()
    {
        sceneName = SceneName;
    }
#endif
}