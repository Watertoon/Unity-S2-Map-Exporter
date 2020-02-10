using UnityEngine;
using UnityEditor;

[CustomPropertyDrawer(typeof(IntParameters))]
[CustomPropertyDrawer(typeof(BoolParameters))]
[CustomPropertyDrawer(typeof(StringParameters))]
[CustomPropertyDrawer(typeof(FloatParameters))]
public class TypeParameterDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);

        position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);

        var indent = EditorGUI.indentLevel;
        EditorGUI.indentLevel = 0;

        var nameField = new Rect(position.x + 105, position.y, 60, position.height);
        var paramField = new Rect(position.x, position.y, 100, position.height);

        EditorGUI.PropertyField(paramField, property.FindPropertyRelative("value"), GUIContent.none);
        EditorGUI.PropertyField(nameField, property.FindPropertyRelative("param_name"), GUIContent.none);

        EditorGUI.indentLevel = indent;

        EditorGUI.EndProperty();
    }
}
