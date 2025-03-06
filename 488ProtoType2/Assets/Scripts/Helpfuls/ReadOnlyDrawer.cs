using UnityEditor;
using UnityEngine;

//Create a ReadOnly Attribute
public class ReadOnlyAttribute : PropertyAttribute { }


//Create a Property Drawer for the ReadOnly Attribute
#if UNITY_EDITOR
[CustomPropertyDrawer(typeof(ReadOnlyAttribute))]
public class ReadOnlyDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        GUI.enabled = false;
        EditorGUI.PropertyField(position, property, label);
        GUI.enabled = true;
    }
}
#endif