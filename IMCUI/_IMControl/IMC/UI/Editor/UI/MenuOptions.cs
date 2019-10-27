
//using IMCUI;
//
using IMCUI.UI;
using UnityEditor;
using UnityEngine;
namespace IMCUIEditor.UI
{
    /// <summary>
    /// This script adds the UI menu options to the Unity Editor.
    /// </summary>

    static internal class MenuOptions
    {
        private const string kUILayerName = "IMCUI";

        private const string kStandardSpritePath = "UI/Skin/UISprite.psd";
        private const string kBackgroundSpritePath = "UI/Skin/Background.psd";
        private const string kInputFieldBackgroundPath = "UI/Skin/InputFieldBackground.psd";
        private const string kKnobPath = "UI/Skin/Knob.psd";
        private const string kCheckmarkPath = "UI/Skin/Checkmark.psd";
        private const string kDropdownArrowPath = "UI/Skin/DropdownArrow.psd";
        private const string kMaskPath = "UI/Skin/UIMask.psd";

        static private DefaultControls.Resources s_StandardResources;

        static private DefaultControls.Resources GetStandardResources()
        {
            if (s_StandardResources.standard == null)
            {
                s_StandardResources.standard = AssetDatabase.GetBuiltinExtraResource<Sprite>(kStandardSpritePath);
                s_StandardResources.background = AssetDatabase.GetBuiltinExtraResource<Sprite>(kBackgroundSpritePath);
                s_StandardResources.inputField = AssetDatabase.GetBuiltinExtraResource<Sprite>(kInputFieldBackgroundPath);
                s_StandardResources.knob = AssetDatabase.GetBuiltinExtraResource<Sprite>(kKnobPath);
                s_StandardResources.checkmark = AssetDatabase.GetBuiltinExtraResource<Sprite>(kCheckmarkPath);
                s_StandardResources.dropdown = AssetDatabase.GetBuiltinExtraResource<Sprite>(kDropdownArrowPath);
                s_StandardResources.mask = AssetDatabase.GetBuiltinExtraResource<Sprite>(kMaskPath);
            }
            return s_StandardResources;
        }

        private static void SetPositionVisibleinSceneView(RectTransform canvasRTransform, RectTransform itemTransform)
        {
            // Find the best scene view
            SceneView sceneView = SceneView.lastActiveSceneView;
            if (sceneView == null && SceneView.sceneViews.Count > 0)
                sceneView = SceneView.sceneViews[0] as SceneView;

            // Couldn't find a SceneView. Don't set position.
            if (sceneView == null || sceneView.camera == null)
                return;

            // Create world space Plane from canvas position.
            Vector2 localPlanePosition;
            Camera camera = sceneView.camera;
            Vector3 position = Vector3.zero;
            if (RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRTransform, new Vector2(camera.pixelWidth / 2, camera.pixelHeight / 2), camera, out localPlanePosition))
            {
                // Adjust for canvas pivot
                localPlanePosition.x = localPlanePosition.x + canvasRTransform.sizeDelta.x * canvasRTransform.pivot.x;
                localPlanePosition.y = localPlanePosition.y + canvasRTransform.sizeDelta.y * canvasRTransform.pivot.y;

                localPlanePosition.x = Mathf.Clamp(localPlanePosition.x, 0, canvasRTransform.sizeDelta.x);
                localPlanePosition.y = Mathf.Clamp(localPlanePosition.y, 0, canvasRTransform.sizeDelta.y);

                // Adjust for anchoring
                position.x = localPlanePosition.x - canvasRTransform.sizeDelta.x * itemTransform.anchorMin.x;
                position.y = localPlanePosition.y - canvasRTransform.sizeDelta.y * itemTransform.anchorMin.y;

                Vector3 minLocalPosition;
                minLocalPosition.x = canvasRTransform.sizeDelta.x * (0 - canvasRTransform.pivot.x) + itemTransform.sizeDelta.x * itemTransform.pivot.x;
                minLocalPosition.y = canvasRTransform.sizeDelta.y * (0 - canvasRTransform.pivot.y) + itemTransform.sizeDelta.y * itemTransform.pivot.y;

                Vector3 maxLocalPosition;
                maxLocalPosition.x = canvasRTransform.sizeDelta.x * (1 - canvasRTransform.pivot.x) - itemTransform.sizeDelta.x * itemTransform.pivot.x;
                maxLocalPosition.y = canvasRTransform.sizeDelta.y * (1 - canvasRTransform.pivot.y) - itemTransform.sizeDelta.y * itemTransform.pivot.y;

                position.x = Mathf.Clamp(position.x, minLocalPosition.x, maxLocalPosition.x);
                position.y = Mathf.Clamp(position.y, minLocalPosition.y, maxLocalPosition.y);
            }

            itemTransform.anchoredPosition = position;
            itemTransform.localRotation = Quaternion.identity;
            itemTransform.localScale = Vector3.one;

        }

        private static void PlaceUIElementRoot(GameObject element, MenuCommand menuCommand) //2017年12月20日14:14:02 周俊优化代码逻辑
        {
            GameObject canvas = null;
            GameObject parent = menuCommand.context as GameObject;
            if (parent == null || parent.GetComponentInParent<Canvas>() == null)//不存在焦点,并且上面不存在Canvas组件，创建并获取canvas
            {
                canvas = GetOrCreateCanvasGameObject();//create canvas
            }
            else //存在焦点,并且上面存在Canvas组件，获取canvas
            {
                canvas = parent.GetComponentInParent<Canvas>().gameObject;//get canvas
            }
            if (element.GetComponent<IMCForm>() != null)//创建的物体是IMCForm组件
            {
                element.transform.name = GameObjectUtility.GetUniqueNameForSibling(canvas.transform, element.name);
                GameObjectUtility.SetParentAndAlign(element.gameObject, canvas.gameObject);
            }
            else//创建的物体不是IMCForm组件
            {
                if (parent != null && parent.GetComponent<Canvas>() == null)//存在焦点并且焦点不在Canvas
                {
                    GameObjectUtility.SetParentAndAlign(element, parent);
                }
                else//不存在焦点
                {
                    GameObject go = DefaultControls.CreateForm(GetStandardResources());
                    PlaceUIElementRoot(go, menuCommand);
                    RectTransform rect = go.GetComponent<RectTransform>();
                    rect.anchoredPosition = Vector2.zero;
                    rect.sizeDelta = Vector2.zero;
                    GameObjectUtility.SetParentAndAlign(element, go);
                }
            }
            Undo.RegisterCreatedObjectUndo(element, "Create " + element.name);
            if (parent != menuCommand.context) // not a context click, so center in sceneview
                SetPositionVisibleinSceneView(parent.GetComponent<RectTransform>(), element.GetComponent<RectTransform>());
            element.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
            Selection.activeGameObject = element;
        }

        static void AddTag(string tag)
        {
            if (!isHasTag(tag))
            {
                SerializedObject tagManager = new SerializedObject(AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/TagManager.asset")[0]);
                SerializedProperty it = tagManager.GetIterator();
                while (it.NextVisible(true))
                {
                    if (it.name == "tags")
                    {
                        for (int i = 0; i < it.arraySize; i++)
                        {
                            SerializedProperty dataPoint = it.GetArrayElementAtIndex(i);
                            if (string.IsNullOrEmpty(dataPoint.stringValue))
                            {
                                dataPoint.stringValue = tag;
                                tagManager.ApplyModifiedProperties();
                                return;
                            }
                        }
                    }
                }
            }
        }

        static void AddLayer(string layer)
        {
            if (!isHasLayer(layer))
            {
                SerializedObject tagManager = new SerializedObject(AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/TagManager.asset")[0]);
                SerializedProperty it = tagManager.GetIterator();
                while (it.NextVisible(true))
                {
                    //#if UNITY_5_0
                    if (it.name == "layers")
                    {
                        for (int i = 8; i < it.arraySize; i++)
                        {
                            SerializedProperty dataPoint = it.GetArrayElementAtIndex(i);
                            if (string.IsNullOrEmpty(dataPoint.stringValue))
                            {
                                dataPoint.stringValue = layer;
                                tagManager.ApplyModifiedProperties();
                                return;
                            }
                        }
                    }
                    //#else
                    //                    if (it.name.StartsWith("User Layer"))
                    //                    {
                    //                        if (it.type == "string")
                    //                        {
                    //                            if (string.IsNullOrEmpty(it.stringValue))
                    //                            {
                    //                                it.stringValue = layer;
                    //                                tagManager.ApplyModifiedProperties();
                    //                                return;
                    //                            }
                    //                        }
                    //                    }
                    //#endif
                }
            }
        }

        static bool isHasTag(string tag)
        {
            for (int i = 0; i < UnityEditorInternal.InternalEditorUtility.tags.Length; i++)
            {
                if (UnityEditorInternal.InternalEditorUtility.tags[i].Contains(tag))
                    return true;
            }
            return false;
        }

        static bool isHasLayer(string layer)
        {
            for (int i = 0; i < UnityEditorInternal.InternalEditorUtility.layers.Length; i++)
            {
                if (UnityEditorInternal.InternalEditorUtility.layers[i].Contains(layer))
                    return true;
            }
            return false;
        }

        // Graphic elements

        [MenuItem("GameObject/IMCUI/IMC Text", false, 0)]
        static public void AddText(MenuCommand menuCommand)
        {
            GameObject go = DefaultControls.CreateText(GetStandardResources());
            PlaceUIElementRoot(go, menuCommand);
        }

        [MenuItem("GameObject/IMCUI/IMC Hyperlinks Label", false, 0)]
        static public void AddHyperlinksLabel(MenuCommand menuCommand)
        {
            GameObject go = DefaultControls.CreateHyperlinksLabel(GetStandardResources());
            PlaceUIElementRoot(go, menuCommand);
        }
        [MenuItem("GameObject/IMCUI/IMC Image", false, 0)]
        static public void AddImage(MenuCommand menuCommand)
        {
            GameObject go = DefaultControls.CreateImage(GetStandardResources());
            PlaceUIElementRoot(go, menuCommand);
        }

        [MenuItem("GameObject/IMCUI/IMC Raw Image", false, 0)]
        static public void AddRawImage(MenuCommand menuCommand)
        {
            GameObject go = DefaultControls.CreateRawImage(GetStandardResources());
            PlaceUIElementRoot(go, menuCommand);
        }

        // Controls

        // Button and toggle are controls you just click on.

        [MenuItem("GameObject/IMCUI/IMC Button", false, 0)]
        static public void AddButton(MenuCommand menuCommand)
        {
            GameObject go = DefaultControls.CreateButton(GetStandardResources());
            PlaceUIElementRoot(go, menuCommand);
        }

        [MenuItem("GameObject/IMCUI/IMC Toggle", false, 0)]
        static public void AddToggle(MenuCommand menuCommand)
        {
            GameObject go = DefaultControls.CreateToggle(GetStandardResources());
            PlaceUIElementRoot(go, menuCommand);
        }

        // Slider and Scrollbar modify a number

        [MenuItem("GameObject/IMCUI/IMC Slider", false, 0)]
        static public void AddSlider(MenuCommand menuCommand)
        {
            GameObject go = DefaultControls.CreateSlider(GetStandardResources());
            PlaceUIElementRoot(go, menuCommand);
        }

        [MenuItem("GameObject/IMCUI/IMC Scrollbar", false, 0)]
        static public void AddScrollbar(MenuCommand menuCommand)
        {
            GameObject go = DefaultControls.CreateScrollbar(GetStandardResources());
            PlaceUIElementRoot(go, menuCommand);
        }

        // More advanced controls below

        [MenuItem("GameObject/IMCUI/IMC Dropdown", false, 0)]
        static public void AddDropdown(MenuCommand menuCommand)
        {
            GameObject go = DefaultControls.CreateDropdown(GetStandardResources());
            PlaceUIElementRoot(go, menuCommand);
        }

        [MenuItem("GameObject/IMCUI/IMC Input Field", false, 0)]
        public static void AddInputField(MenuCommand menuCommand)
        {
            GameObject go = DefaultControls.CreateInputField(GetStandardResources());
            PlaceUIElementRoot(go, menuCommand);
        }

        // Containers

        [MenuItem("GameObject/IMCUI/IMC Canvas", false, 0)]
        static public void AddCanvas(MenuCommand menuCommand)
        {
            var go = CreateNewUI();
            GameObjectUtility.SetParentAndAlign(go, menuCommand.context as GameObject);
            if (go.transform.parent as RectTransform)
            {
                RectTransform rect = go.transform as RectTransform;
                rect.anchorMin = Vector2.zero;
                rect.anchorMax = Vector2.one;
                rect.anchoredPosition = Vector2.zero;
                rect.sizeDelta = Vector2.zero;
            }
            Selection.activeGameObject = go;
        }

        [MenuItem("GameObject/IMCUI/IMC Panel", false, 0)]
        static public void AddPanel(MenuCommand menuCommand)
        {
            GameObject go = DefaultControls.CreatePanel(GetStandardResources());
            PlaceUIElementRoot(go, menuCommand);

            // Panel is special, we need to ensure there's no padding after repositioning.
            RectTransform rect = go.GetComponent<RectTransform>();
            rect.anchoredPosition = Vector2.zero;
            rect.sizeDelta = Vector2.zero;
        }

        [MenuItem("GameObject/IMCUI/Container/IMC Scroll View", false, 0)]
        static public void AddScrollView(MenuCommand menuCommand)
        {
            GameObject go = DefaultControls.CreateScrollView(GetStandardResources());
            PlaceUIElementRoot(go, menuCommand);
        }


        [MenuItem("GameObject/IMCUI/Container/IMC Form", false, 0)]
        public static void CreateForm(MenuCommand menuCommand)
        {
            GameObject go = DefaultControls.CreateForm(GetStandardResources());
            PlaceUIElementRoot(go, menuCommand);

            RectTransform rect = go.GetComponent<RectTransform>();
            rect.anchoredPosition = Vector2.zero;
            rect.sizeDelta = Vector2.zero;
        }

        [MenuItem("GameObject/IMCUI/Container/IMC Group", false, 0)]
        public static void CreateGroup(MenuCommand menuCommand)
        {
            GameObject go = DefaultControls.CreateGroup(GetStandardResources());
            PlaceUIElementRoot(go, menuCommand);

            RectTransform rect = go.GetComponent<RectTransform>();
            rect.anchoredPosition = Vector2.zero;
            rect.sizeDelta = Vector2.zero;
        }

        [MenuItem("GameObject/IMCUI/Container/IMC Tab View", false, 0)]
        public static void CreatTabView(MenuCommand menuCommand)
        {
            GameObject go = DefaultControls.CreateTabView(GetStandardResources());
            PlaceUIElementRoot(go, menuCommand);

            RectTransform rect = go.GetComponent<RectTransform>();
            rect.anchoredPosition = new Vector2(0, 10);
        }

        [MenuItem("GameObject/IMCUI/IMC Message Box", false, 0)]
        public static void CreateMessageBox(MenuCommand menuCommand)
        {
            GameObject go = DefaultControls.CreateMessageBox(GetStandardResources());
            PlaceUIElementRoot(go, menuCommand);
        }

        [MenuItem("GameObject/IMCUI/IMC Cascade Dropdown", false, 0)]
        public static void CreateCascadeDropdown(MenuCommand menuCommand)
        {
            GameObject go = DefaultControls.CreateCascadeDropDown(GetStandardResources());
            PlaceUIElementRoot(go, menuCommand);
        }

        [MenuItem("GameObject/IMCUI/IMC Switch Button", false, 0)]
        public static void CreateSwitchButton(MenuCommand menuCommand)
        {
            GameObject go = DefaultControls.CreateSwitchButton(GetStandardResources());
            PlaceUIElementRoot(go, menuCommand);
        }
        [MenuItem("GameObject/IMCUI/IMC Order Show Buttons", false, 0)]
        public static void CreateOrderShowButtons(MenuCommand menuCommand)
        {
            GameObject go = DefaultControls.CreateOrderShowButtons(GetStandardResources());
            PlaceUIElementRoot(go, menuCommand);
        }
        static public GameObject CreateNewUI()
        {
            AddLayer(kUILayerName);
            // Root for the UI
            var root = new GameObject("IMC Canvas");
            root.AddComponent<IMCCanvas>();
            root.AddComponent<AudioSource>();

            root.layer = LayerMask.NameToLayer(kUILayerName);
            Canvas canvas = root.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            root.AddComponent<IMCCanvasScaler>();
            IMCGraphicRaycaster gr = root.AddComponent<IMCGraphicRaycaster>();
            //gr.ControlType = ControlType.None;
            //gr.ContainerType = ContainerType.Other;
            Undo.RegisterCreatedObjectUndo(root, "Create " + root.name);

            // if there is no event system add one...
            CreateEventSystem(false);
            return root;
        }

        [MenuItem("GameObject/IMCUI/IMC Event System", false, 0)]
        public static void CreateEventSystem(MenuCommand menuCommand)
        {
            GameObject parent = menuCommand.context as GameObject;
            CreateEventSystem(true, parent);
        }

        private static void CreateEventSystem(bool select)
        {
            CreateEventSystem(select, null);
        }

        private static void CreateEventSystem(bool select, GameObject parent)
        {
            var esys = Object.FindObjectOfType<EventSystem>();
            if (esys == null)
            {
                var eventSystem = new GameObject("IMC EventSystem");
                GameObjectUtility.SetParentAndAlign(eventSystem, parent);
                esys = eventSystem.AddComponent<EventSystem>();
                //esys.ControlType = ControlType.None;
                //esys.ContainerType = ContainerType.Other;

                IMCStandaloneInputModule tempStandaloneInputModule = eventSystem.AddComponent<IMCStandaloneInputModule>();
                //tempStandaloneInputModule.ControlType = ControlType.None;
                //tempStandaloneInputModule.ContainerType = ContainerType.Other;

                IMCTouchInputModule tempTouchInputModule = eventSystem.AddComponent<IMCTouchInputModule>();
                //tempTouchInputModule.ControlType = ControlType.None;
                //tempTouchInputModule.ContainerType = ContainerType.Other;

                Undo.RegisterCreatedObjectUndo(eventSystem, "Create " + eventSystem.name);
            }

            if (select && esys != null)
            {
                Selection.activeGameObject = esys.gameObject;
            }
        }

        // Helper function that returns a Canvas GameObject; preferably a parent of the selection, or other existing Canvas.
        static public GameObject GetOrCreateCanvasGameObject()
        {
            GameObject selectedGo = Selection.activeGameObject;

            // Try to find a gameobject that is the selected GO or one if its parents.
            IMCCanvasScaler canvas = (selectedGo != null) ? selectedGo.GetComponentInParent<IMCCanvasScaler>() : null;
            if (canvas != null && canvas.gameObject.activeInHierarchy)
                return canvas.gameObject;

            // No canvas in selection or its parents? Then use just any canvas..
            canvas = Object.FindObjectOfType(typeof(IMCCanvasScaler)) as IMCCanvasScaler;
            if (canvas != null && canvas.gameObject.activeInHierarchy)
                return canvas.gameObject;

            // No canvas in the scene at all? Then create a new one.
            return CreateNewUI();
        }
    }
}
