using UnityEditor;
using UnityEngine;
using SnapToMapTD.Game;

[CustomPropertyDrawer(typeof(Wave))]
public class WaveDrawer : PropertyDrawer
{
    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        return EditorGUI.GetPropertyHeight(property, label, true);
    }

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        int index = ParseArrayIndex(property.propertyPath);
        GUIContent displayLabel = index >= 0 ? new GUIContent($"Wave {index + 1}") : label;
        EditorGUI.PropertyField(position, property, displayLabel, true);
    }

    internal static int ParseArrayIndex(string propertyPath)
    {
        int open = propertyPath.LastIndexOf('[');
        int close = propertyPath.LastIndexOf(']');
        if (open >= 0 && close > open &&
            int.TryParse(propertyPath.Substring(open + 1, close - open - 1), out int index))
            return index;
        return -1;
    }
}

[CustomPropertyDrawer(typeof(WaveEntry))]
public class WaveEntryDrawer : PropertyDrawer
{
    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        return EditorGUI.GetPropertyHeight(property, label, true);
    }

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        int index = WaveDrawer.ParseArrayIndex(property.propertyPath);
        GUIContent displayLabel = index >= 0 ? new GUIContent($"Entry {index + 1}") : label;
        EditorGUI.PropertyField(position, property, displayLabel, true);
    }
}
