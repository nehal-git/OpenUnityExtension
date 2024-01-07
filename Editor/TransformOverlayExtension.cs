using UnityEditor;
using UnityEditor.Overlays;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.UIElements;

namespace OpenUnityExtensions
{
#if UNITY_2022_2_OR_NEWER
    [Overlay(typeof(SceneView), "Transform Overlay", false, defaultLayout = Layout.Panel)]
#else
  [Overlay(typeof(SceneView), "Transform Overlay", false)]
#endif
    
    public class TransformOverlayExtension : Overlay
    {
        // Fields for user input
        private Toggle toggle;
        private Vector3Field positionField;
        private Vector3Field rotationField;
        private Vector3Field scaleField;
        float width = 350;

        public override VisualElement CreatePanelContent()
        {
           
            var root = new VisualElement() { name = "Transform Overlay" };
            this.displayedChanged += OnDisplayChanged;
            this.Undock();
            this.collapsed = false;
 

            toggle = new Toggle("Is Active?");
            positionField = new Vector3Field("Position");
            rotationField = new Vector3Field("Rotation");
            scaleField = new Vector3Field("Scale");

            positionField.style.width = width;
            rotationField.style.width = width;
            scaleField.style.width = width;

            toggle.RegisterValueChangedCallback(b =>
            {
                if (Selection.activeGameObject != null)
                    Selection.activeGameObject.SetActive(b.newValue);
            });



            positionField.RegisterValueChangedCallback(position =>
            {
                if (Selection.activeGameObject != null)
                    Selection.activeGameObject.transform.position = positionField.value;

            });
            rotationField.RegisterValueChangedCallback(rotation =>
            {
                if (Selection.activeGameObject != null)
                {

                   
                    UnityEditor.TransformUtils.SetInspectorRotation(Selection.activeGameObject.transform, rotationField.value);
                }

            });
            scaleField.RegisterValueChangedCallback(scale =>
            {
                if (Selection.activeGameObject != null)
                    Selection.activeGameObject.transform.localScale = scaleField.value;


            });

           // toggle.value = Selection.activeGameObject.activeSelf;

            Selection.selectionChanged += OnSelectionChanged;
            EditorApplication.update += OnSelectedObjectTransformationChanged;
            root.Add(toggle);
            root.Add(positionField);
            root.Add(rotationField);
            root.Add(scaleField);
            return root;
        }

        private void OnDisplayChanged(bool obj)
        {

            if (!obj)
            {
                Selection.selectionChanged -= OnSelectionChanged;
                EditorApplication.update -= OnSelectedObjectTransformationChanged;
                this.displayedChanged -= OnDisplayChanged;
            }
          
        }

        private void OnSelectedObjectTransformationChanged()
        {
            if (Selection.activeGameObject != null)
            {
               


                if (Selection.activeGameObject.transform.position != positionField.value)
                {
                    positionField.value = Selection.activeGameObject.transform.position;

                }
                if (Selection.activeGameObject.transform.rotation.eulerAngles != rotationField.value)
                {
                    rotationField.value = UnityEditor.TransformUtils.GetInspectorRotation(Selection.activeGameObject.transform);

                }
                if (Selection.activeGameObject.transform.localScale != scaleField.value)
                {
                    scaleField.value = Selection.activeGameObject.transform.localScale;

                }

            }

        }



        private void OnSelectionChanged()
        {
            // Debug.Log(Selection.GetFiltered(typeof(GameObject), SelectionMode.ExcludePrefab)[0].name);
            if (Selection.activeGameObject != null)
            {
                //this.displayed = true;
                this.collapsed = false;
                this.floatingPosition = new Vector2(0, SceneView.focusedWindow.position.height);
                toggle.value = Selection.activeGameObject.activeSelf;
                positionField.value = Selection.activeGameObject.transform.position;
                rotationField.value = UnityEditor.TransformUtils.GetInspectorRotation(Selection.activeGameObject.transform);
                scaleField.value = Selection.activeGameObject.transform.localScale;
            }
            else
            {
                toggle.value = false;
                positionField.value = Vector3.zero;
                rotationField.value = Vector3.zero;
                scaleField.value = Vector3.zero;
                //this.displayed = false;
            }

        }
    }
}