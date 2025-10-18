#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(SceneField))]
public class SceneFieldPropertyDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);

        SerializedProperty assetProp = property.FindPropertyRelative("sceneAsset");
        Object obj = EditorGUI.ObjectField(position, label, assetProp.objectReferenceValue, typeof(SceneAsset), false);
        assetProp.objectReferenceValue = obj;

        EditorGUI.EndProperty();
    }
}
#endif