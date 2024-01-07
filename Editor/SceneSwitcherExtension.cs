using UnityEditor;
using UnityEditor.Overlays;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.SceneManagement;
using System.Collections.Generic;
using UnityEditor.UIElements;
using UnityEditor.Toolbars;
using UnityEngine.SceneManagement;

namespace OpenUnityExtensions
{
#if UNITY_2022_2_OR_NEWER
    [Overlay(typeof(SceneView), "Scene Switch Overlay", defaultDockPosition = DockPosition.Top, defaultDockZone = DockZone.TopToolbar, defaultDockIndex = 0)]
#elif UNITY_2021_2 || UNITY_2021_3
    [Overlay(typeof(SceneView), "Scene Switch Overlay", false)]
#endif
    //[Icon("Assets/OpenUnityExtensions/Texture/unity.png")]
    public class SceneSwitcherExtension : ToolbarOverlay
    {
        SceneSwitcherExtension() : base(
             SceneListDropDown.id,
             SceneLocator.id
            )
        { }

#if UNITY_2021_2 || UNITY_2021_3


        public override VisualElement CreatePanelContent()
        {
            var root = new VisualElement() { name = "Scene Switch Overlay" };
          

            root.style.flexDirection = FlexDirection.Row;
            root.style.alignItems = Align.Center;
            root.style.alignContent = Align.Stretch;
            root.style.justifyContent = Justify.SpaceBetween;

            SceneListDropDown sceneListDropDown = new SceneListDropDown();
            sceneListDropDown.style.width = 100;
            sceneListDropDown.style.height = 30;

            sceneListDropDown.style.marginRight = 0;
            sceneListDropDown.style.flexDirection = FlexDirection.Row;
            SceneLocator sceneLocator = new SceneLocator();
            sceneLocator.style.width = 30;
            sceneLocator.style.height = 30;


            root.Add(sceneListDropDown);
            root.Add(sceneLocator);


            float totalWidth = sceneListDropDown.resolvedStyle.width + sceneLocator.resolvedStyle.width;
            float totalHeight = Mathf.Max(sceneListDropDown.resolvedStyle.height, sceneLocator.resolvedStyle.height);


            root.style.width = totalWidth;
            root.style.height = totalHeight;

            return root;

        }

       
#endif
    }



    [EditorToolbarElement(id, typeof(SceneView))]
    class SceneListDropDown : EditorToolbarDropdown
    {
        private List<string> sceneNames;
        public const string id = "SceneSwitch";
        static string dropChoice = null;

        public SceneListDropDown()
        {
            RefreshSceneList();
#if UNITY_2022_2_OR_NEWER
            text = " " + SceneManager.GetActiveScene().name.Substring(0, SceneManager.GetActiveScene().name.Length > 10 ? 10 : SceneManager.GetActiveScene().name.Length);
            tooltip = "Current Scene";
#elif UNITY_2021_2 || UNITY_2021_3
            text =  SceneManager.GetActiveScene().name.Substring(0, SceneManager.GetActiveScene().name.Length > 10 ? 10 : SceneManager.GetActiveScene().name.Length);
tooltip = SceneManager.GetActiveScene().name.Substring(0, SceneManager.GetActiveScene().name.Length > 10 ? 10 : SceneManager.GetActiveScene().name.Length);
#endif
            icon = (Texture2D)EditorGUIUtility.IconContent("d_UnityLogo").image;

            clicked += ShowDropdown;

        }

        void ShowDropdown()
        {





            var menu = new GenericMenu();
            foreach (var name in sceneNames)
            {

                menu.AddItem(new GUIContent(name), dropChoice == name, () => { LoadSelectedScene(name); text = SceneManager.GetActiveScene().name.Substring(0, SceneManager.GetActiveScene().name.Length > 10 ? 10 : SceneManager.GetActiveScene().name.Length); });
            }

            menu.ShowAsContext();
            RefreshSceneList();
        }


        private void RefreshSceneList()
        {
            sceneNames = new List<string>();
            string[] sceneGuids = AssetDatabase.FindAssets("t:Scene");

            foreach (string sceneGuid in sceneGuids)
            {
                string scenePath = AssetDatabase.GUIDToAssetPath(sceneGuid);
                string sceneName = System.IO.Path.GetFileNameWithoutExtension(scenePath);
                sceneNames.Add(sceneName);
            }
        }
        private void LoadSelectedScene(string name)
        {

            string scenePath = AssetDatabase.FindAssets(sceneNames.Find(n => n == name) + " t:Scene")[0];
            EditorSceneManager.OpenScene(AssetDatabase.GUIDToAssetPath(scenePath));

        }
    }

    [EditorToolbarElement(id, typeof(SceneView))]
    class SceneLocator : EditorToolbarButton
    {


        public const string id = "SceneLocator";
        public SceneLocator()
        {

            icon = (Texture2D)EditorGUIUtility.IconContent("FolderOpened On Icon").image;

            tooltip = "Locate Current Scene";
            clicked += OnClick;
        }

        void OnClick()
        {
            string scenePath = AssetDatabase.FindAssets(SceneManager.GetActiveScene().name)[0];
            Object sceneAsset = AssetDatabase.LoadAssetAtPath<Object>(AssetDatabase.GUIDToAssetPath(scenePath));
            Selection.activeObject = sceneAsset;
            EditorGUIUtility.PingObject(sceneAsset);
        }

    }
}

