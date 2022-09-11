using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using System.Linq;

namespace MKStudio.UI.MessageBox
{
    public class PrefabManager : EditorWindow
    {
        Vector2 scrollPos;

        GlobalSettings currentSettings;

        [MenuItem("Window/Easy Message Box/Prefab Manager", false, 300)]
        [MenuItem("GameObject/Easy Message Box/Prefab Manager...", false, 10)]
        public static void Initiate()
        {
            var window = EditorWindow.GetWindow<PrefabManager>(true, string.Empty);
            window.minSize = new Vector2(420f, 240f);
        }

        List<string> fileResources;
        List<string> filePrefabs;

        private void OnFocus()
        {
            Refresh();
        }

        void Refresh()
        {
            fileResources = GetPrefabListFromPath("Assets/MKStudio/Easy Message Box/Resources");

            filePrefabs = GetPrefabListFromPath("Assets/MKStudio/Easy Message Box/Prefabs");

            string pathSettings = "Assets/MKStudio/Easy Message Box/Resources/Global Settings.asset";

            var loadedSettings = AssetDatabase.LoadAssetAtPath<GlobalSettings>(pathSettings);
            if (loadedSettings == null)
            {
                if (File.Exists(pathSettings))
                {
                    File.Delete(pathSettings);
                }
                var created = ScriptableObject.CreateInstance<GlobalSettings>();
                AssetDatabase.CreateAsset(created, pathSettings);
                loadedSettings = created;
                AssetDatabase.SaveAssets();
            }
            if (loadedSettings.DefaultPrefab == null)
            {
                loadedSettings.DefaultPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/MKStudio/Easy Message Box/Resources/Default Message Box.prefab");
                if (loadedSettings.DefaultPrefab == null)
                {
                    Debug.LogError("Default Message Box prefab is missing. Make sure it's in 'Assets/MKStudio/Easy Message Box/Resources/' and its file name is 'Default Message Box.prefab'");
                }
            }
            currentSettings = loadedSettings;
        }

        public static List<string> GetPrefabListFromPath(string path)
        {
            try
            {
                return Directory.GetFiles(path, "*.*", SearchOption.AllDirectories).Where(s => s.EndsWith(".prefab")).ToList();
            }
            catch
            {
                return null;
            }
        }

        private void OnGUI()
        {
            StyleFactory.DrawTitle("Message Box Prefab Manager");

            EditorGUILayout.LabelField(string.Format("Place your message box prefabs in 'Assets/MKStudio/Easy Message Box/Prefabs'{0}To add a message box to the scene, select a Canvas and click the 'Add' button.{1}The 'Default' button sets the template to use when there is no template in the scene.",
                                                     System.Environment.NewLine, System.Environment.NewLine),
                                      EditorStyles.centeredGreyMiniLabel, GUILayout.Height(EditorGUIUtility.singleLineHeight * 2 + 5f));

            if (fileResources == null)
            {
                EditorGUILayout.HelpBox("Cannot find the Resource folder, make sure the folder 'Assets/MKStudio/Easy Message Box/Resources' exists", MessageType.Error);
            } 
            else if (fileResources.Count == 0)
            {
                EditorGUILayout.HelpBox("Default Message Box prefab is missing", MessageType.Error);
            }

            if (filePrefabs == null)
            {
                EditorGUILayout.HelpBox("Cannot locate prefabs folder, make sure the folder 'Assets/MKStudio/Easy Message Box/Prefabs' exists", MessageType.Error);
            } 
            else if (filePrefabs.Count == 0)
            {
                EditorGUILayout.HelpBox("No message box prefab found in 'Prefabs' folder. Make sure you place the prefabs under 'Assets/Xen Studio/Easy Message Box/Prefabs'", MessageType.Info);
            }
            scrollPos = EditorGUILayout.BeginScrollView(scrollPos); 
            Rect lastRct = new Rect();
            EditorGUILayout.LabelField("Prefab Name", EditorStyles.miniBoldLabel);
            int total = (fileResources == null ? 0 : fileResources.Count) + (filePrefabs == null ? 0 : filePrefabs.Count);
            if (Event.current.type == EventType.Repaint)
            {
                lastRct = GUILayoutUtility.GetLastRect();
            }
            for (int i = 0; i < total; i++)
            {
                EditorGUILayout.BeginHorizontal();
                if (fileResources != null && i < fileResources.Count)
                {
                    if (i % 2 == 0)
                    {
                        EditorGUI.DrawRect(new Rect(lastRct.x, lastRct.y + (i+1) * (EditorGUIUtility.singleLineHeight + 2f), lastRct.width, EditorGUIUtility.singleLineHeight), new Color(1f,1f,1f,0.1f));
                    }
                    DrawItem(fileResources[i]);
                }
                int totalResource = (fileResources == null ? 0 : fileResources.Count);
                if (filePrefabs != null && i >= totalResource)
                {
                    if (i % 2 == 0)
                    {
                        EditorGUI.DrawRect(new Rect(lastRct.x, lastRct.y + (i + 1) * (EditorGUIUtility.singleLineHeight + 2f), lastRct.width, EditorGUIUtility.singleLineHeight), new Color(1f, 1f, 1f, 0.1f));
                    }
                    DrawItem(filePrefabs[i - totalResource]);
                }
                EditorGUILayout.EndHorizontal();
            }
            EditorGUILayout.EndScrollView();

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("");
            EditorGUILayout.LabelField(string.Format("MK Studio - Easy Message Box {0}", EasyMessageBox.VersionString), EditorStyles.miniLabel, GUILayout.Width(175f));
            EditorGUILayout.EndHorizontal();
        }

        void DrawItem(string path)
        {
            string fileName = Path.GetFileNameWithoutExtension(path);
            EditorGUILayout.LabelField(fileName);
            if (fileName == currentSettings.DefaultPrefab.name)
            {
                Color before = GUI.contentColor;
                GUI.contentColor = Color.green;
                EditorGUILayout.LabelField("[Default]", EditorStyles.miniLabel, GUILayout.Width(45f));
                GUI.contentColor = before;
            }

            Color bgBefore = GUI.backgroundColor;
            GUI.backgroundColor = new Color(0.008f, 0.599f, 0.745f);
            if (GUILayout.Button(new GUIContent("Add", "Add this prefab to the selected Canvas as a template."), EditorStyles.miniButtonLeft, GUILayout.Width(45f)))
            {
                if (Selection.activeGameObject == null || Selection.activeGameObject.GetComponent<Canvas>() == null)
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

                var boxPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);
                if (boxPrefab != null)
                {
                    var box = Instantiate(boxPrefab);
                    box.name = Path.GetFileNameWithoutExtension(path);
                    GameObjectUtility.SetParentAndAlign(box, Selection.activeGameObject);
                    Undo.RegisterCreatedObjectUndo(box, "Add Message Box Prefab as Template");
                    var currentTemplates = Selection.activeGameObject.GetComponent<EasyMessageBox>().Templates;

                    currentTemplates.Add(box.GetComponent<BoxController>());
                    EditorUtility.SetDirty(box.GetComponent<BoxController>());
                    Selection.activeGameObject = box;
                }
            }
            GUI.backgroundColor = new Color(0.737f, 0.500f, 0.268f);
            if (GUILayout.Button(new GUIContent("Select", "Locate and select the prefab in Project view."), EditorStyles.miniButtonMid, GUILayout.Width(45f)))
            {
                Object obj = AssetDatabase.LoadAssetAtPath(path, typeof(UnityEngine.Object));

                Selection.activeObject = obj;

                EditorGUIUtility.PingObject(obj);
            }
            GUI.backgroundColor = new Color(0.509f, 0.737f, 0.269f);
            if (GUILayout.Button(new GUIContent("Default", "Set as the prefab to be used when trying to show a message box without setting up a template in the scene. Note: this is a global setting and will affect all scenes."), EditorStyles.miniButtonRight, GUILayout.Width(45f)))
            {
                var loaded = AssetDatabase.LoadAssetAtPath<GameObject>(path);
                if (loaded != null)
                {
                    if (loaded.GetComponent<BoxController>() == null)
                    {
                        EditorUtility.DisplayDialog("Invalid Prefab",
                                                    string.Format("The prefab {0} is not a valid message box prefab, consider removing it from the folder '{1}'.",
                                                                  Path.GetFileNameWithoutExtension(path),
                                                                  Path.GetDirectoryName(path)), "OK");
                    }
                    else
                    {
                        currentSettings.DefaultPrefab = loaded;                   
                    }
                }
            }
            GUI.backgroundColor = bgBefore;
        }
    }
}