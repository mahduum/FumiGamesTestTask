using AbilitySystem.Scripts.Data;
using UnityEditor;
using UnityEngine;

namespace Editor.PropertyDrawers
{
    [CustomPropertyDrawer(typeof(AbilityTagCreator))]
    public class AbilityTagCreatorPropertyDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);
            
            SerializedProperty stringProperty = property.FindPropertyRelative("Name");
            SerializedProperty intProperty = property.FindPropertyRelative("SelectedIndex");

            var names = AbilityTagCreator.GetAbilityTagNames();

            int selectedIndex = EditorGUI.Popup(position, label.text, intProperty.intValue, names);
            
            if (selectedIndex < 0)
            {
                return;
            }
            
            stringProperty.stringValue = names[selectedIndex];
            intProperty.intValue = selectedIndex;
            
            EditorGUI.EndProperty();
        }
    }
}