// ---------------------------------------------------------------------------- 
// Author: Ryan Hipple
// Date:   05/01/2018
// ----------------------------------------------------------------------------

using System;
using UnityEditor;
using UnityEngine;

namespace RoboRyanTron.SearchableEnum.Editor
{
    [CustomPropertyDrawer(typeof(SearchableEnumAttribute))]
    public class SearchableEnumDrawer : PropertyDrawer
    {
        // TODO: for keycode, add a button to listen for next keycode
    
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            int id = GUIUtility.GetControlID("SearchableEnumDrawer".GetHashCode(), FocusType.Keyboard, Rect.zero);
            
            label = EditorGUI.BeginProperty(position, label, property);
            position = EditorGUI.PrefixLabel(position, id, label);
            
            if (DropdownButton(id, position, new GUIContent(property.enumDisplayNames[property.enumValueIndex])))
            {
                Action<int> onSelect = i =>
                {
                    property.enumValueIndex = i;
                    property.serializedObject.ApplyModifiedProperties();
                };
                
                SearchablePopup.Show(position, property.enumDisplayNames,
                    property.enumValueIndex, onSelect);
            }
            EditorGUI.EndProperty();
        }
        
        private static bool DropdownButton(int id, Rect position, GUIContent content)
        {
            Event current = Event.current;
            switch (current.type)
            {
                case EventType.MouseDown:
                    if (position.Contains(current.mousePosition) && current.button == 0)
                    {
                        Event.current.Use();
                        return true;
                    }
                    break;
                case EventType.KeyDown:
                    if (GUIUtility.keyboardControl == id && current.character =='\n')
                    {
                        Event.current.Use();
                        return true;
                    }
                    break;
                case EventType.Repaint:
                    EditorStyles.popup.Draw(position, content, id, false);
                    break;
            }
            return false;
        }
    }
}
