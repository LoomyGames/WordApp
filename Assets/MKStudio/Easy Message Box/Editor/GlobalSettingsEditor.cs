using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace MKStudio.UI.MessageBox
{
    [CustomEditor(typeof(GlobalSettings))]
    public class GlobalSettingsEditor : Editor
    {
        SerializedProperty pPrefab;


        private void OnEnable()
        {
            pPrefab = serializedObject.FindProperty("DefaultPrefab");
        }

        public override void OnInspectorGUI()
        {
            StyleFactory.DrawTitle("Easy Message Box - Global Settings");
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(new GUIContent("Default Message Box", "Select a message box prefab to be used as the default message box when trying to show a message box without setting up a template in the scene."), GUILayout.Width(120f));
            GUI.enabled = false;
            EditorGUILayout.ObjectField(pPrefab.objectReferenceValue, typeof(GameObject), false);
            GUI.enabled = true;
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.HelpBox("Change the default message box prefab in the Prefab Manager. ('Window->Easy Message Box->Prefab Manager)", MessageType.Info);
        }
    }
}