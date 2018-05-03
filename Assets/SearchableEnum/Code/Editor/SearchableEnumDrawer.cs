// ---------------------------------------------------------------------------- 
// Author: Ryan Hipple
// Date:   05/01/2018
// ----------------------------------------------------------------------------

using System;
using UnityEditor;
using UnityEngine;

namespace RoboRyanTron.SearchableEnum.Editor
{
    /// <summary>
    /// Draws the custom enum selector popup for enum fileds using the
    /// SearchableEnumAttribute.
    /// </summary>
    [CustomPropertyDrawer(typeof(SearchableEnumAttribute))]
    public class SearchableEnumDrawer : PropertyDrawer
    {
        // TODO: for keycode, add a button to listen for next keycode
        
        private const string TYPE_ERROR =
            "SearchableEnum can only be used on enum fields.";
        
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (property.type != "Enum")
            {
                GUIStyle errorStyle = "CN EntryErrorIconSmall";
                Rect r = new Rect(position);
                r.width = errorStyle.fixedWidth;
                position.xMin = r.xMax;
                GUI.Label(r, "", errorStyle);
                GUI.Label(position, TYPE_ERROR);
                return;
            }
            
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
        
        /// <summary>
        /// A custom button drawer that allows for a controlID so that we can
        /// sync the button ID and the label ID to allow for keyboard
        /// navigation like the built-in enum drawers.
        /// </summary>
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
