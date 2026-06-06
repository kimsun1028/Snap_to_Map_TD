using UnityEditor;
using UnityEngine;
using SnapToMapTD.Game;

[CustomPropertyDrawer(typeof(Wave))]
public class WaveDrawer : PropertyDrawer
{
    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        => EditorGUI.GetPropertyHeight(property, label, true);

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        int index = ParseArrayIndex(property.propertyPath);
        EditorGUI.PropertyField(position, property, index >= 0 ? new GUIContent($"Wave {index + 1}") : label, true);
    }

    internal static int ParseArrayIndex(string path)
    {
        int open = path.LastIndexOf('[');
        int close = path.LastIndexOf(']');
        if (open >= 0 && close > open && int.TryParse(path.Substring(open + 1, close - open - 1), out int i))
            return i;
        return -1;
    }
}

[CustomPropertyDrawer(typeof(WaveEntry))]
public class WaveEntryDrawer : PropertyDrawer
{
    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        => EditorGUI.GetPropertyHeight(property, label, true);

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        int index = WaveDrawer.ParseArrayIndex(property.propertyPath);
        EditorGUI.PropertyField(position, property, index >= 0 ? new GUIContent($"Entry {index + 1}") : label, true);
    }
}

[CustomPropertyDrawer(typeof(EnemySpawn))]
public class EnemySpawnDrawer : PropertyDrawer
{
    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        => EditorGUI.GetPropertyHeight(property, label, true);

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        int index = WaveDrawer.ParseArrayIndex(property.propertyPath);
        EditorGUI.PropertyField(position, property, index >= 0 ? new GUIContent($"Enemy {index + 1}") : label, true);
    }
}
