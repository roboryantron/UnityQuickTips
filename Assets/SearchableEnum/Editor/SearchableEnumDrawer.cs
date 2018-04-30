using UnityEngine;
using UnityEditor;

[CustomPropertyDrawer(typeof(SearchableEnum))]
public class SearchableEnumDrawer : UnityEditor.PropertyDrawer
{

    // TODO: display perfect matches first, making it easier to find things like Keycode.A
    // TODO: shorten outer rect based on filter
    // TODO: focus text editor on open
    // TODO: arrow controls


    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        label = EditorGUI.BeginProperty(position, label, property);
        position = EditorGUI.PrefixLabel(position, label);

        if (GUI.Button(position, property.enumDisplayNames[property.enumValueIndex], EditorStyles.popup))
        {
            PopupWindow.Show(position, new EnumSelectorWindow(property));
        }
        EditorGUI.EndProperty();
    }

    private class EnumSelectorWindow : PopupWindowContent
    {
        private string filterText = "";
        private SerializedProperty property;
        private Vector2 scroll;
        private string[] enumNames;
        
        public EnumSelectorWindow(SerializedProperty property)
        {
            this.property = property;
            enumNames = property.enumDisplayNames;
        }

        public override void OnOpen()
        {
            base.OnOpen();
            EditorApplication.update += Repaint;
        }

        public override void OnClose()
        {
            base.OnClose();
            EditorApplication.update -= Repaint;
        }

        private void Repaint()
        {
            PopupWindow.focusedWindow.Repaint();
        }

        public override Vector2 GetWindowSize()
        {
            return new Vector2(base.GetWindowSize().x,
                Mathf.Min(600, enumNames.Length * 16));
        }

        public override void OnGUI(Rect rect)
        {
            GUIStyle style = "ToolbarSeachTextField";
            GUIStyle cancel = "ToolbarSeachCancelButton";

            Rect searchRect = new Rect(rect);
            searchRect.height = 16;
            searchRect.width -= cancel.fixedWidth;
            filterText = GUI.TextField(searchRect, filterText, style);
            searchRect.x = searchRect.xMax;
            searchRect.width = cancel.fixedWidth;
            if (GUI.Button(searchRect, "x", cancel))
            {
                filterText = "";
            }
            
            Rect innerRect = new Rect(rect);
            innerRect.height = enumNames.Length * 16;
            innerRect.x = 0;
            innerRect.y = 0;
            rect.y += 16;
            scroll = GUI.BeginScrollView(rect, scroll, innerRect);

            rect.y = 0;
            rect.height = 16;
            
            for (int i = 0; i < enumNames.Length; i++)
            {
                if (string.IsNullOrEmpty(filterText) || enumNames[i].ToLower().Contains(filterText.ToLower()))
                {
                    if (rect.Contains(Event.current.mousePosition))
                    {
                        GUI.Box(rect, "");
                        if (Event.current.type == EventType.MouseDown)
                        {
                            property.enumValueIndex = i;
                            property.serializedObject.ApplyModifiedProperties();
                            PopupWindow.focusedWindow.Close();
                        }
                    }
                    GUI.Label(rect, enumNames[i]);
                    rect.y = rect.yMax;
                }
            }
            GUI.EndScrollView();
        }
    }
}
