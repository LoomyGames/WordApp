using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.Build;
namespace MKStudio.UI.MessageBox
{
    [CustomEditor(typeof(EasyMessageBox))]
    public class EasyMessageBoxEditor : Editor
    {
        SerializedProperty mPrefabList;
        GUILayoutOption wId = GUILayout.Width(20f);
        GUILayoutOption wPrefab = GUILayout.Width(250f);
        GUILayoutOption wDelBtn = GUILayout.Width(50f);
        Vector2 scrollPos;

        [MenuItem("GameObject/Easy Message Box/Default Message Box", false, 10)]
        static void AddDefaultMessageBoxTemplate(MenuCommand menuCommand)
        {
            if (Selection.activeGameObject == null || Selection.activeGameObject.GetComponent<Canvas>()==null)
            {
                EditorUtility.DisplayDialog("Select Canvas", "Select the Canvas on which you want to show the message box and retry this command.", "OK");
                return;
            }

            if (Selection.activeGameObject.GetComponent<EasyMessageBox>() == null)
            {
                Undo.RecordObject(Selection.activeObject, "Add Component");
                Selection.activeGameObject.AddComponent<EasyMessageBox>();
                Selection.activeGameObject.GetComponent<EasyMessageBox>().Templates = new List<BoxController>();
            }

            var boxPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Xen Studio/Easy Message Box/Resources/Default Message Box.prefab");
            if (boxPrefab != null)
            {
                var defaultMessageBox = Instantiate(boxPrefab);
                defaultMessageBox.name = "Default Message Box";
                GameObjectUtility.SetParentAndAlign(defaultMessageBox, menuCommand.context as GameObject);
                Undo.RegisterCreatedObjectUndo(defaultMessageBox, "Add Message Box Template");
                var currentTemplates = Selection.activeGameObject.GetComponent<EasyMessageBox>().Templates;
                //if (currentTemplates == null)
                //{
                //    currentTemplates = new List<BoxController>();
                //}
                currentTemplates.Add(defaultMessageBox.GetComponent<BoxController>());
                EditorUtility.SetDirty(defaultMessageBox.GetComponent<BoxController>());
                Selection.activeGameObject = defaultMessageBox;
            }
        }

        private void OnEnable()
        {
            mPrefabList = serializedObject.FindProperty("Templates");
            int size = mPrefabList.arraySize;
            int delCount = 0;
            for (int i = size - 1; i >= 0; i--)
            {
                if (mPrefabList.GetArrayElementAtIndex(i).objectReferenceValue == null)
                {
                    mPrefabList.GetArrayElementAtIndex(i).objectReferenceValue = null;
                    mPrefabList.DeleteArrayElementAtIndex(i);
                    delCount++;
                }
                else if ((mPrefabList.GetArrayElementAtIndex(i).objectReferenceValue as BoxController) == null)
                {
                    mPrefabList.GetArrayElementAtIndex(i).objectReferenceValue = null;
                    mPrefabList.DeleteArrayElementAtIndex(i);
                    delCount++;
                }
            }
            serializedObject.ApplyModifiedPropertiesWithoutUndo();
            if (delCount > 0)
            {
                Debug.Log("Easy Message Box: Removed " + delCount + " empty templates");
            }
        }

        public override void OnInspectorGUI()
        {
            StyleFactory.DrawTitle("Easy Message Box - Template List");
            //EditorGUILayout.LabelField("Template List", EditorStyles.boldLabel);
            EditorGUILayout.HelpBox("Add a prefab as a template with the 'Prefab Manager', or manually place your a prefab as the child of this GameObject and include it in the list by pressing the 'Auto Detect' button.", MessageType.Info);

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("ID", EditorStyles.miniLabel, wId);
            EditorGUILayout.LabelField("Template", EditorStyles.miniLabel, wPrefab);
            EditorGUILayout.EndHorizontal();

            scrollPos = EditorGUILayout.BeginScrollView(scrollPos, GUILayout.MaxHeight(mPrefabList.arraySize * (EditorGUIUtility.singleLineHeight + 2f) + EditorGUIUtility.singleLineHeight));

            if (mPrefabList.arraySize == 0)
            {
                EditorGUILayout.HelpBox("No template in the list, will use the 'Default' prefab set in Prefab Manager as the template. Add a message box prefab using Prefab Manager.", MessageType.Info);
            }

            for (int i = 0; i < mPrefabList.arraySize; i++)
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField(i.ToString(), wId);
                EditorGUILayout.PropertyField(mPrefabList.GetArrayElementAtIndex(i), GUIContent.none);
                if (GUILayout.Button("Remove", EditorStyles.miniButton, wDelBtn))
                {
                    mPrefabList.GetArrayElementAtIndex(i).objectReferenceValue = null;
                    mPrefabList.DeleteArrayElementAtIndex(i);
                }

                EditorGUILayout.EndHorizontal();
            }
            EditorGUILayout.EndScrollView();
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("");
            Color original = GUI.backgroundColor;
            GUI.backgroundColor = new Color(0.000f, 0.745f, 0.439f);
            if (GUILayout.Button("Prefab Manager...", GUILayout.Width(125f)))
            {
                PrefabManager.Initiate();
            }
            GUI.backgroundColor = new Color(0.058f, 0.585f, 0.745f);
            if (GUILayout.Button(new GUIContent("Auto Detect", "Auto detects message boxes in the children of this GameObject and sets the templates correctly. Will also remove empty items in the list."), GUILayout.Width(80f)))
            {
                var childMessageBoxes = Selection.activeGameObject.GetComponentsInChildren<BoxController>(true);
                List<GameObject> templates = new List<GameObject>();
                for (int i = 0; i < childMessageBoxes.Length; i++)
                {
                    if (!templates.Contains(childMessageBoxes[i].gameObject))
                    {
                        templates.Add(childMessageBoxes[i].gameObject);
                    }
                }
                List<int> idToRemove = new List<int>();
                for (int i = 0; i < mPrefabList.arraySize; i++)
                {
                    if (mPrefabList.GetArrayElementAtIndex(i).objectReferenceValue == null)
                    {
                        idToRemove.Add(i);
                    }
                    else if ((mPrefabList.GetArrayElementAtIndex(i).objectReferenceValue as GameObject) == null)
                    {
                        idToRemove.Add(i);
                    }
                    else if (templates.Contains(mPrefabList.GetArrayElementAtIndex(i).objectReferenceValue as GameObject))
                    {
                        templates.Remove(mPrefabList.GetArrayElementAtIndex(i).objectReferenceValue as GameObject);
                    }
                }
                for (int i = idToRemove.Count - 1; i >= 0; i--)
                {
                    mPrefabList.GetArrayElementAtIndex(idToRemove[i]).objectReferenceValue = null;
                    mPrefabList.DeleteArrayElementAtIndex(idToRemove[i]);
                }
                for (int i = 0; i < templates.Count; i++)
                {
                    mPrefabList.arraySize++;
                    mPrefabList.GetArrayElementAtIndex(mPrefabList.arraySize - 1).objectReferenceValue = templates[i];
                }
            }
            GUI.backgroundColor = original;

            //if (GUILayout.Button("Manually Add", GUILayout.Width(100f)))
            //{
            //    mPrefabList.arraySize++;
            //    mPrefabList.GetArrayElementAtIndex(mPrefabList.arraySize - 1).objectReferenceValue = null;
            //}
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Space();

            serializedObject.ApplyModifiedProperties();
        }

        //public int callbackOrder { get { return 0; } }
        //public void OnPreprocessBuild(BuildTarget target, string path)
        //{
        //    Debug.Log("MyCustomBuildProcessor.OnPreprocessBuild for target " + target + " at path " + path);
        //}
    }    
}
