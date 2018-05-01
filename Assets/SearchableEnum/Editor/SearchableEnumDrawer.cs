using UnityEngine;
using UnityEditor;
using System;
using System.Collections.Generic;

[CustomPropertyDrawer(typeof(SearchableEnum))]
public class SearchableEnumDrawer : PropertyDrawer
{
    
    // TODO: for keycode, add a button to listen for next keycode
    
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        label = EditorGUI.BeginProperty(position, label, property);
        position = EditorGUI.PrefixLabel(position, label);

        if (GUI.Button(position, property.enumDisplayNames[property.enumValueIndex], EditorStyles.popup))
        {
            PopupWindow.Show(position, new EnumSelectorWindow(property.enumDisplayNames, property.enumValueIndex, 
                i=> 
                {
                    property.enumValueIndex = i;
                    property.serializedObject.ApplyModifiedProperties(); 
                } ));
        }
        EditorGUI.EndProperty();
    }

    private class EnumSelectorWindow : PopupWindowContent
    {
        private const float ROW_HEIGHT = 16.0f;
        private const float ROW_INDENT = 8.0f;

        private string filterText = "";
        private Vector2 scroll;
        private string[] enumNames;
        private Action<int> onSelectionMade;
        private int currentIndex;
        private int hoverIndex;

        private int scrollToIndex;
        private float scrollOffset;

        private readonly List<IndexedString> entries = new List<IndexedString>();

        private struct IndexedString
        {
            public int index;
            public string text;
            public GUIContent guiContent;

            public IndexedString(int index, string text)
            {
                this.index = index;
                this.text = text;
                guiContent = new GUIContent(text);
            }
        }

        public EnumSelectorWindow(string[] names, int currentIndex, Action<int> onSelectionMade)
        {
            enumNames = names;
            this.onSelectionMade = onSelectionMade;
            this.currentIndex = currentIndex;
            hoverIndex = currentIndex;
            scrollToIndex = currentIndex;
            OnFilterChanged();
            scrollOffset = GetWindowSize().y - ROW_HEIGHT * 2;
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
            EditorWindow.focusedWindow.Repaint();
        }

        public override Vector2 GetWindowSize()
        {
            return new Vector2(base.GetWindowSize().x,
                Mathf.Min(600, (enumNames.Length * ROW_HEIGHT) + EditorStyles.toolbar.fixedHeight));
        }

        private void DrawBox(Rect r, Color tint)
        {
            Color c = GUI.color;
            GUI.color = tint;
            //Dopesheetkeyframe - very white
            //SelectionRect - blueish
            //LODSliderRange - white, less alpha
            //ProfilerTimelineBar - pure white
            GUI.Box(r, "", "SelectionRect");
            GUI.color = c;
        }

        private void OnFilterChanged()
        {
            entries.Clear();

            for (int i = 0; i < enumNames.Length; i++)
            {
                // Exact matches show up first
                if (String.Equals(enumNames[i], filterText, StringComparison.CurrentCultureIgnoreCase))
                {
                    entries.Insert(0, new IndexedString(i, enumNames[i]));
                }
                else if (string.IsNullOrEmpty(filterText) || enumNames[i].ToLower().Contains(filterText.ToLower()))
                {
                    entries.Add(new IndexedString(i, enumNames[i]));
                }
            }

            scroll = Vector2.zero;
        }
        
        public override void OnGUI(Rect rect)
        {
            Rect searchRect = new Rect(0, 0, rect.width, EditorStyles.toolbar.fixedHeight);
            Rect scrollRect = Rect.MinMaxRect(0, searchRect.yMax, rect.xMax, rect.yMax);
            
            HandleKeyboard();
            DrawSearch(searchRect);
            DrawSelectionArea(scrollRect);
        }

        private void HandleKeyboard()
        {
            if (Event.current.type == EventType.KeyDown)
            {
                if (Event.current.keyCode == KeyCode.DownArrow)
                {
                    hoverIndex = Mathf.Min(entries.Count - 1, hoverIndex + 1);
                    Event.current.Use();
                    scrollToIndex = hoverIndex;
                    scrollOffset = ROW_HEIGHT;
                }

                if (Event.current.keyCode == KeyCode.UpArrow)
                {
                    hoverIndex = Mathf.Max(0, hoverIndex - 1);
                    Event.current.Use();
                    scrollToIndex = hoverIndex;
                    scrollOffset = -ROW_HEIGHT;
                }

                if (Event.current.keyCode == KeyCode.Return)
                {
                    onSelectionMade(entries[hoverIndex].index);
                    EditorWindow.focusedWindow.Close();
                }
                
                if (Event.current.keyCode == KeyCode.Escape)
                {
                    EditorWindow.focusedWindow.Close();
                }
            }
        }

        private void DrawSearch(Rect rect)
        {
            GUIStyle style = "ToolbarSeachTextField";
            GUIStyle cancel = "ToolbarSeachCancelButton";
            GUIStyle cancelEmpty = "ToolbarSeachCancelButtonEmpty";
            //GUIStyle style = "SearchTextField";
            //GUIStyle cancel = "SearchCancelButton";

            if (Event.current.type == EventType.Repaint)
                EditorStyles.toolbar.Draw(rect, false, false, false, false);

            GUI.FocusControl("enumsearchtext");

            Rect searchRect = new Rect(rect);
            searchRect.xMin += 6;
            searchRect.xMax -= 6;
            searchRect.y += 2;
            searchRect.width -= cancel.fixedWidth;
            GUI.SetNextControlName("enumsearchtext");

            string newText = GUI.TextField(searchRect, filterText, style);
            if (newText != filterText)
            {
                filterText = newText;
                OnFilterChanged();
            }

            searchRect.x = searchRect.xMax;
            searchRect.width = cancel.fixedWidth;
            if (string.IsNullOrEmpty(filterText))
                GUI.Box(searchRect, GUIContent.none, cancelEmpty);
            else if (GUI.Button(searchRect, "x", cancel))
            {
                filterText = "";
                OnFilterChanged();
            }
        }

        private void DrawRow(Rect rowRect, int i)
        {
            if (entries[i].index == currentIndex)
                DrawBox(rowRect, Color.cyan);
            else if (i == hoverIndex)
                DrawBox(rowRect, Color.white);
                
            Rect labelRect = new Rect(rowRect);
            labelRect.xMin += ROW_INDENT;
                
            GUI.Label(labelRect, entries[i].guiContent);
        }
        
        private void DrawSelectionArea(Rect scrollRect)
        {
            Rect contentRect = new Rect(0, 0, 
                scrollRect.width - GUI.skin.verticalScrollbar.fixedWidth, 
                entries.Count * ROW_HEIGHT);
            
            scroll = GUI.BeginScrollView(scrollRect, scroll, contentRect);

            Rect rowRect = new Rect(0, 0, scrollRect.width, ROW_HEIGHT);
            
            for (int i = 0; i < entries.Count; i++)
            {
                if (scrollToIndex == i && 
                    (Event.current.type == EventType.Repaint
                     || Event.current.type == EventType.Layout))
                {
                    Rect r = new Rect(rowRect);
                    r.y += scrollOffset;
                    GUI.ScrollTo(r);
                    scrollToIndex = -1;
                    scroll.x = 0;
                }
                
                if (rowRect.Contains(Event.current.mousePosition))
                {
                    if (Event.current.type == EventType.MouseMove || 
                        Event.current.type == EventType.ScrollWheel)
                        hoverIndex = i;   
                    if (Event.current.type == EventType.MouseDown)
                    {
                        onSelectionMade(entries[i].index);
                        EditorWindow.focusedWindow.Close();
                    }
                }

                DrawRow(rowRect, i);
                
                rowRect.y = rowRect.yMax;
            }
            GUI.EndScrollView();
        }
    }
}
