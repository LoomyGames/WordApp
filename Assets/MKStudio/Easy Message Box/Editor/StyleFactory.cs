using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace MKStudio.UI.MessageBox
{
    public static class StyleFactory
    {
        static GUIStyle titleStyle;

        public static void DrawTitle(string text)
        {
            if (titleStyle == null)
            {
                titleStyle = new GUIStyle(EditorStyles.helpBox);
                titleStyle.fontSize = 16;
                titleStyle.fontStyle = FontStyle.Bold;
            }

            Color original = GUI.backgroundColor;
            GUI.backgroundColor = new Color(0.090f, 0.851f, 1.000f);
            EditorGUILayout.LabelField(text, titleStyle);
            GUI.backgroundColor = original;
        }
    }
}
