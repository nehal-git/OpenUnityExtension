using UnityEditor;
using UnityEditor.Overlays;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.SceneManagement;
using System.Collections.Generic;
using UnityEditor.UIElements;
using UnityEditor.Toolbars;
using UnityEngine.SceneManagement;
using System.Linq;

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
            sceneListDropDown.style.width = 110;
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
    class SceneListDropDown : EditorToolbarDropdownToggle
    {

        private Dictionary<string, string> sceneViews = new Dictionary<string, string>();
        public const string id = "SceneSwitch";
        public static string dropChoice = null;

        public SceneListDropDown()
        {
            RefreshSceneList();

            icon = (Texture2D)EditorGUIUtility.IconContent("d_UnityLogo").image;

            dropdownClicked += ShowDropdown;


        }

        void ShowDropdown()
        {

            var menu = new GenericMenu();
            menu.allowDuplicateNames = true;
            foreach (var scene in sceneViews)
            {

                menu.AddItem(new GUIContent($"{AssetDatabase.GUIDToAssetPath(scene.Key).Substring(7, AssetDatabase.GUIDToAssetPath(scene.Key).Length - 7)}"), dropChoice == scene.Key, () =>
                {
                    LoadSelectedScene(scene.Key);
                    text = SceneManager.GetActiveScene().name.Substring(0, SceneManager.GetActiveScene().name.Length > 10 ? 10 : SceneManager.GetActiveScene().name.Length);

#if UNITY_2022_2_OR_NEWER
                    text = $"{scene.Value} : {AssetDatabase.GUIDToAssetPath(scene.Key).Substring(7, AssetDatabase.GUIDToAssetPath(scene.Key).Length-7)}";
#endif
                    tooltip = $"{scene.Value} : {AssetDatabase.GUIDToAssetPath(scene.Key).Substring(7, AssetDatabase.GUIDToAssetPath(scene.Key).Length - 7)}";
                    dropChoice = scene.Key;
                });

            }



            menu.ShowAsContext();
            sceneViews.Clear();
            RefreshSceneList();
        }


        private void RefreshSceneList()
        {


            string[] sceneGuids = AssetDatabase.FindAssets("t:Scene", new string[] { "Assets" });

            foreach (string sceneGuid in sceneGuids)
            {
                string scenePath = AssetDatabase.GUIDToAssetPath(sceneGuid);
                string sceneName = System.IO.Path.GetFileNameWithoutExtension(scenePath);
                sceneViews.Add(sceneGuid, sceneName);

            }
        }
        private void LoadSelectedScene(string sceneID)
        {

            EditorSceneManager.OpenScene(AssetDatabase.GUIDToAssetPath(sceneID));

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

            Object sceneAsset = AssetDatabase.LoadAssetAtPath<Object>(AssetDatabase.GUIDToAssetPath(SceneListDropDown.dropChoice));
            Selection.activeObject = sceneAsset;
            EditorGUIUtility.PingObject(sceneAsset);
        }

    }
}

