using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UI;

namespace MKStudio.UI.MessageBox
{
    [CustomEditor(typeof(BoxController))]
    public class BoxControllerEditor : Editor
    {
        SerializedProperty pDisableSound;
        SerializedProperty pInStartSound;
        SerializedProperty pInEndSound;
        SerializedProperty pOutStartSound;
        GUIStyle style_Title;
        bool styleSetFlag;

        private void OnEnable()
        {
            pDisableSound = serializedObject.FindProperty("disableSoundEffects");
            pInStartSound = serializedObject.FindProperty("defaultSound_InAnimationStart");
            pInEndSound = serializedObject.FindProperty("defaultSound_InAnimationEnd");
            pOutStartSound = serializedObject.FindProperty("defaultSound_OutAnimationStart");


        }

        public override void OnInspectorGUI()
        {
            if (!styleSetFlag)
            {
                style_Title = new GUIStyle(EditorStyles.label);
                style_Title.fontStyle = FontStyle.Bold;
                style_Title.fontSize = 15;
                style_Title.fixedHeight = 30;
                styleSetFlag = true;
            }


            Color original = GUI.backgroundColor;
            GUI.backgroundColor = new Color(0.722f, 1.000f, 0.839f);
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            EditorGUILayout.LabelField("Message Box Settings", style_Title);
            EditorGUILayout.Space();

            var box = (target as BoxController);

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Message Box Scaler", EditorStyles.boldLabel);
            if (!(Mathf.Approximately(box.transform.localScale.x, 1f)))
            {
                if (GUILayout.Button("Reset", EditorStyles.miniButton, GUILayout.Width(45f)))
                {
                    Undo.RecordObject(box.transform, "Reset Message Box Scale");
                    box.transform.localScale = Vector3.one;
                    if (box.Background != null)
                    {
                        Undo.RecordObject(box.Background.transform, "Reset Background Scale");
                        box.Background.transform.localScale = Vector3.one;
                    }
                }
            }
            EditorGUILayout.EndHorizontal();
            if (!(Mathf.Approximately(box.transform.localScale.x, box.transform.localScale.y) &&
                  Mathf.Approximately(box.transform.localScale.y, box.transform.localScale.z)))
            {
                box.transform.localScale = Vector3.one * box.transform.localScale.x;
            }
            float current = EditorGUILayout.Slider(box.transform.localScale.x, 0.1f, 10f);
            if (!Mathf.Approximately(current, box.transform.localScale.x))
            {
                Undo.RecordObject(box.transform, "Change Message Box Scale");
                box.transform.localScale = Vector3.one * current;
                if (box.Background != null && current >= 0.05f)
                {
                    Undo.RecordObject(box.Background.transform, "Change Background Scale");
                    box.Background.transform.localScale = Vector3.one / current;
                }
            }

            EditorGUILayout.LabelField("Sound Effects", EditorStyles.boldLabel);

            EditorGUILayout.PropertyField(serializedObject.FindProperty("disableSoundEffects"), new GUIContent("Disable Sound"));
            EditorGUILayout.LabelField(new GUIContent("Default Animation SFX", "Sets the default sound effects. These sound will be played when no sound clip is set through the script."));
            if (pDisableSound.boolValue)
            {
                EditorGUILayout.HelpBox("Sound effects is disabled for this message box. No sound will be played even when you set a clip from the script.", MessageType.Warning);
            }
            GUI.enabled = !pDisableSound.boolValue;
            EditorGUI.indentLevel++;
            EditorGUILayout.PropertyField(pInStartSound, new GUIContent("In Start"));
            EditorGUILayout.PropertyField(pInEndSound, new GUIContent("In End"));
            EditorGUILayout.PropertyField(pOutStartSound, new GUIContent("Out Start"));
            EditorGUI.indentLevel--;
            GUI.enabled = true;
            EditorGUILayout.LabelField("Drag", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("dragBoundLimit"), new GUIContent("Drag Clamp Value", "Controls the distance used to keep the message box from dragging out of the screen."));
            GUI.backgroundColor = original;
            EditorGUILayout.EndVertical();

            serializedObject.ApplyModifiedProperties();
            EditorGUILayout.Space();
            DrawDefaultInspector();

        }
    }
}