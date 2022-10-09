using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace ShipGameEditor.CustomFields
{
    public class MultiRangeSlider
    {
        #region Variables
        private static RangeSliderStyles _styles;
        private static RangeSliderStyles Styles
        {
            get
            {
                if (_styles == null) 
                    _styles = new RangeSliderStyles();
                return _styles;
            }
        }

        private static readonly int _sliderId = "LODSliderIDHash".GetHashCode();
        private static readonly Color[] ItemsColors = {
            new Color(0.4831376f, 0.6211768f, 0.0219608f, 1.0f),
            new Color(0.2792160f, 0.4078432f, 0.5835296f, 1.0f),
            new Color(0.2070592f, 0.5333336f, 0.6556864f, 1.0f),
            new Color(0.5333336f, 0.1600000f, 0.0282352f, 1.0f),
            new Color(0.3827448f, 0.2886272f, 0.5239216f, 1.0f),
            new Color(0.8000000f, 0.4423528f, 0.0000000f, 1.0f),
            new Color(0.4486272f, 0.4078432f, 0.0501960f, 1.0f),
            new Color(0.7749016f, 0.6368624f, 0.0250984f, 1.0f)
        };

        private Func<int, string> getItemLabel;
        private MultiRangeInfo selected;
        #endregion

        public void SetCallbacks(Func<int, string> getItemLabel) => this.getItemLabel = getItemLabel;
        public void Draw(string label, ref List<float> percents)
        {
            GUILayout.BeginVertical();

            GUILayout.Space(5);
            if (!string.IsNullOrEmpty(label))
                GUILayout.Label(label);

            var sliderBarPosition = GUILayoutUtility.GetRect(EditorGUIUtility.currentViewWidth, 30);
            sliderBarPosition.width = EditorGUIUtility.currentViewWidth - (sliderBarPosition.x + 8);

            float processed = 0;
            List<MultiRangeInfo> list = new List<MultiRangeInfo>();
            for (int i = 0; i < percents.Count; i++)
            {
                var info = NewMultiRangeInfo(sliderBarPosition, i, percents[i], processed);
                processed += info.percent;
                list.Add(info);
            }

            DrawLevelSlider(sliderBarPosition, list, percents);
            GUILayout.EndVertical();

            for (int i = 0; i < list.Count; i++)
                percents[i] = list[i].percent;
        }

        #region Calculate Rects
        private Rect CalcButton(Rect rect) => new Rect(rect.x + rect.width - 5, rect.y, 10, rect.height);
        private Rect CalcRange(Rect totalRect, float startPercent, float endPercent)
        {
            var startX = Mathf.Round(totalRect.width * startPercent);
            var endX = Mathf.Round(totalRect.width * endPercent);

            return new Rect(totalRect.x + startX, totalRect.y, endX, totalRect.height);
        }
        #endregion

        #region Draw
        private void DrawLevelSlider(Rect sliderPosition, List<MultiRangeInfo> list, List<float> percents)
        {
            int sliderId = GUIUtility.GetControlID(_sliderId, FocusType.Passive);
            Event evt = Event.current;

            switch (evt.GetTypeForControl(sliderId))
            {
                case EventType.Repaint:
                    DrawSlider(sliderPosition, list, percents);
                    break;

                case EventType.MouseDown:
                    var barPosition = sliderPosition;
                    barPosition.x -= 5;
                    barPosition.width += 10;

                    if (!barPosition.Contains(evt.mousePosition))
                        break;

                    evt.Use();
                    foreach (var item in list)
                    {
                        if (!item.buttonPosition.Contains(evt.mousePosition))
                            continue;

                        GUIUtility.hotControl = sliderId;
                        selected = item;
                        GUI.changed = true;
                        break;
                    }
                    break;

                case EventType.MouseUp:
                    if (GUIUtility.hotControl == sliderId)
                    {
                        GUIUtility.hotControl = 0;
                        evt.Use();
                    }
                    break;

                case EventType.MouseDrag:
                    if (GUIUtility.hotControl == sliderId && selected != null)
                    {
                        var delta = evt.delta.x * .003f;
                        var id = selected.id;

                        list[id].percent += delta;
                        if (id + 1 < list.Count)
                            list[id + 1].percent -= delta;

                        GUI.changed = true;
                    }
                    break;
            }
        }

        private void DrawSlider(Rect area, List<MultiRangeInfo> list, List<float> percents)
        {
            DrawGUI(Styles.SliderBG, area, GUIContent.none);

            for (int i = 0; i < list.Count; i++)
            {
                var isSelected = selected != null && i == selected.id;

                DrawRange(list[i], percents[i], isSelected);
                EditorGUIUtility.AddCursorRect(list[i].buttonPosition, MouseCursor.ResizeHorizontal);
            }
        }

        private void DrawRange(MultiRangeInfo info, float percentage, bool isSelected)
        {
            var tempColor = GUI.backgroundColor;
            var value = $"{percentage * 100:F}%";
            var label = $"{value}\n{info.name}";
            int colorId = info.id % (ItemsColors.Length - 1);
            var foreground = info.rangePosition;

            GUI.backgroundColor = ItemsColors[colorId];
            if (isSelected)
            {
                foreground.width -= 6;
                foreground.height -= 6;
                foreground.center += new Vector2(3, 3);
                DrawGUI(Styles.SliderRangeSelected, info.rangePosition, GUIContent.none);
            }
            else GUI.backgroundColor *= 0.6f;

            if (foreground.width > 0)
                DrawGUI(Styles.SliderRange, foreground, GUIContent.none);
            DrawGUI(Styles.SliderText, info.rangePosition, label);

            GUI.backgroundColor = tempColor;
        }

        private MultiRangeInfo NewMultiRangeInfo(Rect rect, int id, float percent, float processed)
        {
            Rect range = CalcRange(rect, processed, percent);
            return new MultiRangeInfo(id, getItemLabel(id), percent)
            {
                rangePosition = range,
                buttonPosition = CalcButton(range)
            };
        }

        private void DrawGUI(GUIStyle gui, Rect position, string content) => DrawGUI(gui, position, new GUIContent(content));
        private void DrawGUI(GUIStyle gui, Rect position, GUIContent content)
        {
            gui.Draw(position, content, false, false, false, false);
        }
        #endregion

        #region Auxiliar Classes
        public class MultiRangeInfo
        {
            public Rect buttonPosition;
            public Rect rangePosition;

            public MultiRangeInfo(int id, string name, float percent)
            {
                this.id = id;
                this.name = name;
                this.percent = percent;
            }

            public int id;
            public string name;
            public float percent;
        }
        private class RangeSliderStyles
        {
            public readonly GUIStyle SliderBG = "LODSliderBG";
            public readonly GUIStyle SliderText = "LODSliderText";
            public readonly GUIStyle SliderRange = "LODSliderRange";
            public readonly GUIStyle SliderRangeSelected = "LODSliderRangeSelected";
        }
        #endregion
    }
}
