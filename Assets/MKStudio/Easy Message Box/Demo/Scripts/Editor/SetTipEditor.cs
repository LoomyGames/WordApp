using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace MKStudio.UI.MessageBox
{
    [CustomEditor(typeof(SetTip))]
    public class SetTipEditor : Editor
    {
        bool styleSet;
        GUIStyle style;
        SerializedProperty tip;

        private void OnEnable()
        {
            tip = serializedObject.FindProperty("Tip");
        }


        public override void OnInspectorGUI()
        {
            if (!styleSet)
            {
                style = new GUIStyle(EditorStyles.textArea);
                style.wordWrap = true;
                styleSet = false;
            }

            EditorGUILayout.PrefixLabel("Tip");
            tip.stringValue = EditorGUILayout.TextArea(tip.stringValue, style,GUILayout.MinHeight(100f));
            serializedObject.ApplyModifiedProperties();
        }
    }
}