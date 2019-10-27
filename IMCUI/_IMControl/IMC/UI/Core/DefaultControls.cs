using UnityEngine;

namespace IMCUI.UI
{
    public static class DefaultControls
    {
        public struct Resources
        {
            public Sprite standard;
            public Sprite background;
            public Sprite inputField;
            public Sprite knob;
            public Sprite checkmark;
            public Sprite dropdown;
            public Sprite mask;
        }

        private const float kWidth = 160f;
        private const float kThickHeight = 30f;
        private const float kThinHeight = 20f;
        private static Vector2 s_ThickElementSize = new Vector2(kWidth, kThickHeight);
        private static Vector2 s_ThinElementSize = new Vector2(kWidth, kThinHeight);
        private static Vector2 s_ImageElementSize = new Vector2(100f, 100f);
        private static Color s_DefaultSelectableColor = new Color(1f, 1f, 1f, 1f);
        private static Color s_PanelColor = new Color(1f, 1f, 1f, 0.392f);
        private static Color s_TextColor = new Color(50f / 255f, 50f / 255f, 50f / 255f, 1f);

        // Helper methods at top

        private static GameObject CreateUIElementRoot(string name, Vector2 size)
        {
            GameObject child = new GameObject(name);
            RectTransform rectTransform = child.AddComponent<RectTransform>();
            rectTransform.sizeDelta = size;
            rectTransform.anchoredPosition3D = Vector3.zero;
            return child;
        }

        static GameObject CreateUIObject(string name, GameObject parent)
        {
            GameObject go = new GameObject(name);
            go.AddComponent<RectTransform>();
            SetParentAndAlign(go, parent);
            return go;
        }

        private static void SetDefaultTextValues(IMCText lbl)
        {
            // Set text values we want across UI elements in default controls.
            // Don't set values which are the same as the default values for the Text component,
            // since there's no point in that, and it's good to keep them as consistent as possible.
            lbl.color = s_TextColor;
        }

        private static void SetDefaultColorTransitionValues(IMCSelectable slider)
        {
            ColorBlock colors = slider.colors;
            colors.highlightedColor = new Color(0.882f, 0.882f, 0.882f);
            colors.pressedColor = new Color(0.698f, 0.698f, 0.698f);
            colors.disabledColor = new Color(0.521f, 0.521f, 0.521f);
        }

        private static void SetParentAndAlign(GameObject child, GameObject parent)
        {
            if (parent == null)
                return;

            child.transform.SetParent(parent.transform, false);
            SetLayerRecursively(child, parent.layer);
        }

        private static void SetLayerRecursively(GameObject go, int layer)
        {
            go.layer = layer;
            Transform t = go.transform;
            for (int i = 0; i < t.childCount; i++)
                SetLayerRecursively(t.GetChild(i).gameObject, layer);
        }

        // Actual controls

        public static GameObject CreateForm(Resources resources)
        {
            GameObject go = CreateUIElementRoot("IMC Form", s_ThickElementSize);

            RectTransform rectTransform = go.GetComponent<RectTransform>();
            rectTransform.anchorMin = Vector2.zero;
            rectTransform.anchorMax = Vector2.one;
            rectTransform.anchoredPosition = Vector2.zero;
            rectTransform.sizeDelta = Vector2.zero;

            IMCForm form = go.AddComponent<IMCForm>();

            IMCImage image = go.AddComponent<IMCImage>();

            image.sprite = resources.background;
            image.type = IMCImage.Type.Sliced;
            image.color = Color.white;
            go.AddComponent<CanvasGroup>();
            return go;
        }

        public static GameObject CreateGroup(Resources resources)
        {
            GameObject go = CreateUIElementRoot("IMC Group", s_ThickElementSize);

            RectTransform rectTransform = go.GetComponent<RectTransform>();
            rectTransform.anchorMin = Vector2.zero;
            rectTransform.anchorMax = Vector2.one;
            rectTransform.anchoredPosition = Vector2.zero;
            rectTransform.sizeDelta = Vector2.zero;

            go.AddComponent<IMCGroup>();
            IMCImage image = go.AddComponent<IMCImage>();
            image.sprite = resources.background;
            image.type = IMCImage.Type.Sliced;
            image.color = new Color(0, 0, 0, 0);
            go.AddComponent<CanvasGroup>();
            return go;
        }

        public static GameObject CreatePanel(Resources resources)
        {
            GameObject panelRoot = CreateUIElementRoot("IMC Panel", s_ThickElementSize);

            // Set RectTransform to stretch
            RectTransform rectTransform = panelRoot.GetComponent<RectTransform>();
            rectTransform.anchorMin = Vector2.zero;
            rectTransform.anchorMax = Vector2.one;
            rectTransform.anchoredPosition = Vector2.zero;
            rectTransform.sizeDelta = Vector2.zero;

            IMCImage image = panelRoot.AddComponent<IMCImage>();

            image.sprite = resources.background;
            image.type = IMCImage.Type.Sliced;
            image.color = s_PanelColor;

            return panelRoot;
        }

        public static GameObject CreateButton(Resources resources)
        {
            GameObject buttonRoot = CreateUIElementRoot("IMC Button", s_ThickElementSize);

            GameObject childText = new GameObject("Text");
            SetParentAndAlign(childText, buttonRoot);

            IMCImage image = buttonRoot.AddComponent<IMCImage>();
            image.sprite = resources.standard;
            image.type = IMCImage.Type.Sliced;
            image.color = s_DefaultSelectableColor;

            IMCButton bt = buttonRoot.AddComponent<IMCButton>();

            SetDefaultColorTransitionValues(bt);

            IMCText text = childText.AddComponent<IMCText>();
            text.raycastTarget = false;

            text.text = "IMC Button";
            text.alignment = TextAnchor.MiddleCenter;
            SetDefaultTextValues(text);

            RectTransform textRectTransform = childText.GetComponent<RectTransform>();
            textRectTransform.anchorMin = Vector2.zero;
            textRectTransform.anchorMax = Vector2.one;
            textRectTransform.sizeDelta = Vector2.zero;

            return buttonRoot;
        }

        public static GameObject CreateText(Resources resources)
        {
            GameObject go = CreateUIElementRoot("IMC Text", s_ThickElementSize);

            IMCText lbl = go.AddComponent<IMCText>();
            lbl.text = "IMCUI Text";
            lbl.supportRichText = false;
            lbl.raycastTarget = false;
            SetDefaultTextValues(lbl);

            return go;
        }
        public static GameObject CreateHyperlinksLabel(Resources resources)
        {
            GameObject go = CreateUIElementRoot("IMC Hyperlinks Label", s_ThickElementSize);
            IMCHyperlinksLabel hyp = go.AddComponent<IMCHyperlinksLabel>();
            hyp.text = "IMC Hyperlinks Label";
            SetDefaultTextValues(hyp);
            return go;
        }
        public static GameObject CreateImage(Resources resources)
        {
            GameObject go = CreateUIElementRoot("IMC Image", s_ImageElementSize);
            IMCImage image = go.AddComponent<IMCImage>();
            image.raycastTarget = false;
            return go;
        }

        public static GameObject CreateRawImage(Resources resources)
        {
            GameObject go = CreateUIElementRoot("IMC RawImage", s_ImageElementSize);
            IMCRawImage rawImage = go.AddComponent<IMCRawImage>();
            rawImage.raycastTarget = false;
            return go;
        }

        public static GameObject CreateSlider(Resources resources)
        {
            // Create GOs Hierarchy
            GameObject root = CreateUIElementRoot("IMC Slider", s_ThinElementSize);

            GameObject background = CreateUIObject("Background", root);
            GameObject fillArea = CreateUIObject("Fill Area", root);
            GameObject fill = CreateUIObject("Fill", fillArea);
            GameObject handleArea = CreateUIObject("Handle Slide Area", root);
            GameObject handle = CreateUIObject("Handle", handleArea);

            // Background
            IMCImage backgroundImage = background.AddComponent<IMCImage>();

            backgroundImage.sprite = resources.background;
            backgroundImage.type = IMCImage.Type.Sliced;
            backgroundImage.color = s_DefaultSelectableColor;
            RectTransform backgroundRect = background.GetComponent<RectTransform>();
            backgroundRect.anchorMin = new Vector2(0, 0.25f);
            backgroundRect.anchorMax = new Vector2(1, 0.75f);
            backgroundRect.sizeDelta = new Vector2(0, 0);

            // Fill Area
            RectTransform fillAreaRect = fillArea.GetComponent<RectTransform>();
            fillAreaRect.anchorMin = new Vector2(0, 0.25f);
            fillAreaRect.anchorMax = new Vector2(1, 0.75f);
            fillAreaRect.anchoredPosition = new Vector2(-5, 0);
            fillAreaRect.sizeDelta = new Vector2(-20, 0);

            // Fill
            IMCImage fillImage = fill.AddComponent<IMCImage>();
            fillImage.sprite = resources.standard;
            fillImage.type = IMCImage.Type.Sliced;
            fillImage.color = s_DefaultSelectableColor;

            RectTransform fillRect = fill.GetComponent<RectTransform>();
            fillRect.sizeDelta = new Vector2(10, 0);

            // Handle Area
            RectTransform handleAreaRect = handleArea.GetComponent<RectTransform>();
            handleAreaRect.sizeDelta = new Vector2(-20, 0);
            handleAreaRect.anchorMin = new Vector2(0, 0);
            handleAreaRect.anchorMax = new Vector2(1, 1);

            // Handle
            IMCImage handleImage = handle.AddComponent<IMCImage>();
            handleImage.sprite = resources.knob;
            handleImage.color = s_DefaultSelectableColor;

            RectTransform handleRect = handle.GetComponent<RectTransform>();
            handleRect.sizeDelta = new Vector2(20, 0);

            // Setup slider component
            IMCSlider slider = root.AddComponent<IMCSlider>();
            slider.fillRect = fill.GetComponent<RectTransform>();
            slider.handleRect = handle.GetComponent<RectTransform>();
            slider.targetGraphic = handleImage;
            slider.direction = IMCSlider.Direction.LeftToRight;
            SetDefaultColorTransitionValues(slider);

            return root;
        }

        public static GameObject CreateScrollbar(Resources resources)
        {
            // Create GOs Hierarchy
            GameObject scrollbarRoot = CreateUIElementRoot("IMC Scrollbar", s_ThinElementSize);

            GameObject sliderArea = CreateUIObject("Sliding Area", scrollbarRoot);
            GameObject handle = CreateUIObject("Handle", sliderArea);

            IMCImage bgImage = scrollbarRoot.AddComponent<IMCImage>();
            bgImage.sprite = resources.background;
            bgImage.type = IMCImage.Type.Sliced;
            bgImage.color = s_DefaultSelectableColor;

            IMCImage handleImage = handle.AddComponent<IMCImage>();
            handleImage.sprite = resources.standard;
            handleImage.type = IMCImage.Type.Sliced;
            handleImage.color = s_DefaultSelectableColor;

            RectTransform sliderAreaRect = sliderArea.GetComponent<RectTransform>();
            sliderAreaRect.sizeDelta = new Vector2(-20, -20);
            sliderAreaRect.anchorMin = Vector2.zero;
            sliderAreaRect.anchorMax = Vector2.one;

            RectTransform handleRect = handle.GetComponent<RectTransform>();
            handleRect.sizeDelta = new Vector2(20, 20);

            IMCScrollbar scrollbar = scrollbarRoot.AddComponent<IMCScrollbar>();
            scrollbar.handleRect = handleRect;
            scrollbar.targetGraphic = handleImage;
            SetDefaultColorTransitionValues(scrollbar);

            return scrollbarRoot;
        }

        public static GameObject CreateToggle(Resources resources)
        {
            // Set up hierarchy
            GameObject toggleRoot = CreateUIElementRoot("IMC Toggle", s_ThinElementSize);

            GameObject background = CreateUIObject("Background", toggleRoot);
            GameObject checkmark = CreateUIObject("Checkmark", background);
            GameObject childLabel = CreateUIObject("Label", toggleRoot);

            // Set up components
            IMCToggle toggle = toggleRoot.AddComponent<IMCToggle>();
            toggle.isOn = true;

            IMCImage bgImage = background.AddComponent<IMCImage>();
            bgImage.sprite = resources.standard;
            bgImage.type = IMCImage.Type.Sliced;
            bgImage.color = s_DefaultSelectableColor;

            IMCImage checkmarkImage = checkmark.AddComponent<IMCImage>();
            checkmarkImage.sprite = resources.checkmark;

            IMCText label = childLabel.AddComponent<IMCText>();
            label.text = "Toggle";
            label.raycast = false;
            SetDefaultTextValues(label);

            //toggle.backGround = bgImage;
            toggle.graphic = checkmarkImage;
            toggle.targetGraphic = bgImage;
            SetDefaultColorTransitionValues(toggle);

            RectTransform bgRect = background.GetComponent<RectTransform>();
            bgRect.anchorMin = new Vector2(0f, 1f);
            bgRect.anchorMax = new Vector2(0f, 1f);
            bgRect.anchoredPosition = new Vector2(10f, -10f);
            bgRect.sizeDelta = new Vector2(kThinHeight, kThinHeight);

            RectTransform checkmarkRect = checkmark.GetComponent<RectTransform>();
            checkmarkRect.anchorMin = new Vector2(0.5f, 0.5f);
            checkmarkRect.anchorMax = new Vector2(0.5f, 0.5f);
            checkmarkRect.anchoredPosition = Vector2.zero;
            checkmarkRect.sizeDelta = new Vector2(20f, 20f);

            RectTransform labelRect = childLabel.GetComponent<RectTransform>();
            labelRect.anchorMin = new Vector2(0f, 0f);
            labelRect.anchorMax = new Vector2(1f, 1f);
            labelRect.offsetMin = new Vector2(23f, 1f);
            labelRect.offsetMax = new Vector2(-5f, -2f);

            return toggleRoot;
        }

        public static GameObject CreateInputField(Resources resources)
        {
            GameObject root = CreateUIElementRoot("IMC InputField", s_ThickElementSize);

            GameObject childPlaceholder = CreateUIObject("Placeholder", root);
            GameObject childText = CreateUIObject("Text", root);

            IMCImage image = root.AddComponent<IMCImage>();
            image.sprite = resources.inputField;
            image.type = IMCImage.Type.Sliced;
            image.color = s_DefaultSelectableColor;

            IMCInputField inputField = root.AddComponent<IMCInputField>();
            SetDefaultColorTransitionValues(inputField);

            IMCText text = childText.AddComponent<IMCText>();
            text.text = "";
            text.supportRichText = false;
            SetDefaultTextValues(text);

            IMCText placeholder = childPlaceholder.AddComponent<IMCText>();
            placeholder.text = "Enter text...";
            placeholder.fontStyle = FontStyle.Italic;
            // Make placeholder color half as opaque as normal text color.
            Color placeholderColor = text.color;
            placeholderColor.a *= 0.5f;
            placeholder.color = placeholderColor;

            RectTransform textRectTransform = childText.GetComponent<RectTransform>();
            textRectTransform.anchorMin = Vector2.zero;
            textRectTransform.anchorMax = Vector2.one;
            textRectTransform.sizeDelta = Vector2.zero;
            textRectTransform.offsetMin = new Vector2(10, 6);
            textRectTransform.offsetMax = new Vector2(-10, -7);

            RectTransform placeholderRectTransform = childPlaceholder.GetComponent<RectTransform>();
            placeholderRectTransform.anchorMin = Vector2.zero;
            placeholderRectTransform.anchorMax = Vector2.one;
            placeholderRectTransform.sizeDelta = Vector2.zero;
            placeholderRectTransform.offsetMin = new Vector2(10, 6);
            placeholderRectTransform.offsetMax = new Vector2(-10, -7);

            inputField.textComponent = text;
            inputField.placeholder = placeholder;

            return root;
        }

        public static GameObject CreateOrderShowButtons(Resources resources)
        {
            GameObject go = CreateImage(resources);
            go.name = "IMC Order Show Buttons";
            go.AddComponent<CanvasGroup>();
            IMCImage tempIma = go.GetComponent<IMCImage>();
            IMCOrderShowButtons orderShowButtons = go.AddComponent<IMCOrderShowButtons>();
            GameObject leftBtn = CreateButton(resources);
            leftBtn.name = "LeftButton";
            IMCButton tempLeftBtn = leftBtn.GetComponent<IMCButton>();

            tempLeftBtn.anchorMin = new Vector2(0, 0.5f);
            tempLeftBtn.anchorMax = new Vector2(0, 0.5f);
            tempLeftBtn.pivot = new Vector2(0, 0.5f);
            tempLeftBtn.Text.text = "LeftButton";
            GameObject rightBtn = CreateButton(resources);
            rightBtn.name = "RightButton";
            IMCButton tempRightBtn = rightBtn.GetComponent<IMCButton>();

            tempRightBtn.anchorMin = new Vector2(1, 0.5f);
            tempRightBtn.anchorMax = new Vector2(1, 0.5f);
            tempRightBtn.pivot = new Vector2(1, 0.5f);
            tempRightBtn.Text.text = "RightButton";

            tempIma.color = new Color(0, 0, 0, 0);
            tempIma.sizeDelta = new Vector2(tempLeftBtn.sizeDelta.x * 3, tempLeftBtn.sizeDelta.y);
            leftBtn.transform.SetParent(orderShowButtons.transform);
            tempLeftBtn.anchoredPosition3D = Vector3.zero;
            rightBtn.transform.SetParent(orderShowButtons.transform);
            tempRightBtn.anchoredPosition3D = Vector3.zero;
            orderShowButtons.leftBtn = leftBtn.GetComponent<IMCButton>();
            orderShowButtons.rightBtn = rightBtn.GetComponent<IMCButton>();

            return go;
        }

        public static GameObject CreateDropdown(Resources resources)
        {
            GameObject root = CreateUIElementRoot("IMC Dropdown", s_ThickElementSize);

            GameObject label = CreateUIObject("Label", root);
            GameObject arrow = CreateUIObject("Arrow", root);
            GameObject template = CreateUIObject("Template", root);
            GameObject viewport = CreateUIObject("Viewport", template);
            GameObject content = CreateUIObject("Content", viewport);
            GameObject item = CreateUIObject("Item", content);
            GameObject itemBackground = CreateUIObject("Item Background", item);
            GameObject itemCheckmark = CreateUIObject("Item Checkmark", item);
            GameObject itemLabel = CreateUIObject("Item Label", item);

            // Sub controls.

            GameObject scrollbar = CreateScrollbar(resources);
            scrollbar.name = "Scrollbar";
            SetParentAndAlign(scrollbar, template);

            IMCScrollbar scrollbarScrollbar = scrollbar.GetComponent<IMCScrollbar>();
            scrollbarScrollbar.SetDirection(IMCScrollbar.Direction.BottomToTop, true);

            RectTransform vScrollbarRT = scrollbar.GetComponent<RectTransform>();
            vScrollbarRT.anchorMin = Vector2.right;
            vScrollbarRT.anchorMax = Vector2.one;
            vScrollbarRT.pivot = Vector2.one;
            vScrollbarRT.sizeDelta = new Vector2(vScrollbarRT.sizeDelta.x, 0);

            // Setup item UI components.

            IMCText itemLabelText = itemLabel.AddComponent<IMCText>();
            itemLabelText.raycast = false;
            SetDefaultTextValues(itemLabelText);
            itemLabelText.alignment = TextAnchor.MiddleLeft;

            IMCImage itemBackgroundImage = itemBackground.AddComponent<IMCImage>();
            itemBackgroundImage.color = new Color32(245, 245, 245, 255);

            IMCImage itemCheckmarkImage = itemCheckmark.AddComponent<IMCImage>();
            itemCheckmarkImage.sprite = resources.checkmark;

            IMCToggle itemToggle = item.AddComponent<IMCToggle>();
            itemToggle.targetGraphic = itemBackgroundImage;
            itemToggle.graphic = itemCheckmarkImage;
            itemToggle.isOn = true;

            // Setup template UI components.

            IMCImage templateImage = template.AddComponent<IMCImage>();
            templateImage.sprite = resources.standard;
            templateImage.type = IMCImage.Type.Sliced;

            IMCScrollRect templateScrollRect = template.AddComponent<IMCScrollRect>();
            templateScrollRect.content = (RectTransform)content.transform;
            templateScrollRect.viewport = (RectTransform)viewport.transform;
            templateScrollRect.horizontal = false;
            templateScrollRect.movementType = IMCScrollRect.MovementType.Clamped;
            templateScrollRect.verticalScrollbar = scrollbarScrollbar;
            templateScrollRect.verticalScrollbarVisibility = IMCScrollRect.ScrollbarVisibility.AutoHideAndExpandViewport;
            templateScrollRect.verticalScrollbarSpacing = -3;

            Mask scrollRectMask = viewport.AddComponent<Mask>();
            scrollRectMask.showMaskGraphic = false;

            IMCImage viewportImage = viewport.AddComponent<IMCImage>();
            viewportImage.sprite = resources.mask;
            viewportImage.type = IMCImage.Type.Sliced;

            // Setup dropdown UI components.

            IMCText labelText = label.AddComponent<IMCText>();
            SetDefaultTextValues(labelText);
            labelText.text = "Option A";
            labelText.raycast = false;
            labelText.alignment = TextAnchor.MiddleLeft;

            IMCImage arrowImage = arrow.AddComponent<IMCImage>();
            arrowImage.sprite = resources.dropdown;

            IMCImage backgroundImage = root.AddComponent<IMCImage>();
            backgroundImage.sprite = resources.standard;
            backgroundImage.color = s_DefaultSelectableColor;
            backgroundImage.type = IMCImage.Type.Sliced;

            IMCDropdown dropdown = root.AddComponent<IMCDropdown>();
            dropdown.targetGraphic = backgroundImage;
            SetDefaultColorTransitionValues(dropdown);
            dropdown.template = template.GetComponent<RectTransform>();
            dropdown.captionText = labelText;
            dropdown.itemText = itemLabelText;

            // Setting default Item list.
            itemLabelText.text = "Option A";
            dropdown.options.Add(new IMCDropdown.OptionData { text = "Option A" });
            dropdown.options.Add(new IMCDropdown.OptionData { text = "Option B" });
            dropdown.options.Add(new IMCDropdown.OptionData { text = "Option C" });

            // Set up RectTransforms.

            RectTransform labelRT = label.GetComponent<RectTransform>();
            labelRT.anchorMin = Vector2.zero;
            labelRT.anchorMax = Vector2.one;
            labelRT.offsetMin = new Vector2(10, 6);
            labelRT.offsetMax = new Vector2(-25, -7);

            RectTransform arrowRT = arrow.GetComponent<RectTransform>();
            arrowRT.anchorMin = new Vector2(1, 0.5f);
            arrowRT.anchorMax = new Vector2(1, 0.5f);
            arrowRT.sizeDelta = new Vector2(20, 20);
            arrowRT.anchoredPosition = new Vector2(-15, 0);

            RectTransform templateRT = template.GetComponent<RectTransform>();
            templateRT.anchorMin = new Vector2(0, 0);
            templateRT.anchorMax = new Vector2(1, 0);
            templateRT.pivot = new Vector2(0.5f, 1);
            templateRT.anchoredPosition = new Vector2(0, 2);
            templateRT.sizeDelta = new Vector2(0, 150);

            RectTransform viewportRT = viewport.GetComponent<RectTransform>();
            viewportRT.anchorMin = new Vector2(0, 0);
            viewportRT.anchorMax = new Vector2(1, 1);
            viewportRT.sizeDelta = new Vector2(-18, 0);
            viewportRT.pivot = new Vector2(0, 1);

            RectTransform contentRT = content.GetComponent<RectTransform>();
            contentRT.anchorMin = new Vector2(0f, 1);
            contentRT.anchorMax = new Vector2(1f, 1);
            contentRT.pivot = new Vector2(0.5f, 1);
            contentRT.anchoredPosition = new Vector2(0, 0);
            contentRT.sizeDelta = new Vector2(0, 28);

            RectTransform itemRT = item.GetComponent<RectTransform>();
            itemRT.anchorMin = new Vector2(0, 0.5f);
            itemRT.anchorMax = new Vector2(1, 0.5f);
            itemRT.sizeDelta = new Vector2(0, 20);

            RectTransform itemBackgroundRT = itemBackground.GetComponent<RectTransform>();
            itemBackgroundRT.anchorMin = Vector2.zero;
            itemBackgroundRT.anchorMax = Vector2.one;
            itemBackgroundRT.sizeDelta = Vector2.zero;

            RectTransform itemCheckmarkRT = itemCheckmark.GetComponent<RectTransform>();
            itemCheckmarkRT.anchorMin = new Vector2(0, 0.5f);
            itemCheckmarkRT.anchorMax = new Vector2(0, 0.5f);
            itemCheckmarkRT.sizeDelta = new Vector2(20, 20);
            itemCheckmarkRT.anchoredPosition = new Vector2(10, 0);

            RectTransform itemLabelRT = itemLabel.GetComponent<RectTransform>();
            itemLabelRT.anchorMin = Vector2.zero;
            itemLabelRT.anchorMax = Vector2.one;
            itemLabelRT.offsetMin = new Vector2(20, 1);
            itemLabelRT.offsetMax = new Vector2(-10, -2);

            template.SetActive(false);

            return root;
        }

        public static GameObject CreateScrollView(Resources resources)
        {
            GameObject root = CreateUIElementRoot("IMC Scroll View", new Vector2(200, 200));
            root.AddComponent<CanvasGroup>();

            GameObject viewport = CreateUIObject("Viewport", root);
            GameObject content = CreateUIObject("Content", viewport);

            // Sub controls.

            GameObject hScrollbar = CreateScrollbar(resources);
            hScrollbar.name = "Scrollbar Horizontal";
            SetParentAndAlign(hScrollbar, root);
            RectTransform hScrollbarRT = hScrollbar.GetComponent<RectTransform>();
            hScrollbarRT.anchorMin = Vector2.zero;
            hScrollbarRT.anchorMax = Vector2.right;
            hScrollbarRT.pivot = Vector2.zero;
            hScrollbarRT.sizeDelta = new Vector2(0, hScrollbarRT.sizeDelta.y);

            GameObject vScrollbar = CreateScrollbar(resources);
            vScrollbar.name = "Scrollbar Vertical";
            SetParentAndAlign(vScrollbar, root);
            vScrollbar.GetComponent<IMCScrollbar>().SetDirection(IMCScrollbar.Direction.BottomToTop, true);
            RectTransform vScrollbarRT = vScrollbar.GetComponent<RectTransform>();
            vScrollbarRT.anchorMin = Vector2.right;
            vScrollbarRT.anchorMax = Vector2.one;
            vScrollbarRT.pivot = Vector2.one;
            vScrollbarRT.sizeDelta = new Vector2(vScrollbarRT.sizeDelta.x, 0);

            // Setup RectTransforms.

            // Make viewport fill entire scroll view.
            RectTransform viewportRT = viewport.GetComponent<RectTransform>();
            viewportRT.anchorMin = Vector2.zero;
            viewportRT.anchorMax = Vector2.one;
            viewportRT.sizeDelta = Vector2.zero;
            viewportRT.pivot = Vector2.up;

            // Make context match viewpoprt width and be somewhat taller.
            // This will show the vertical scrollbar and not the horizontal one.
            RectTransform contentRT = content.GetComponent<RectTransform>();
            contentRT.anchorMin = Vector2.up;
            contentRT.anchorMax = Vector2.one;
            contentRT.sizeDelta = new Vector2(0, 300);
            contentRT.pivot = Vector2.up;

            // Setup UI components.

            IMCScrollRect scrollRect = root.AddComponent<IMCScrollRect>();
            scrollRect.content = contentRT;
            scrollRect.viewport = viewportRT;
            scrollRect.horizontalScrollbar = hScrollbar.GetComponent<IMCScrollbar>();
            scrollRect.verticalScrollbar = vScrollbar.GetComponent<IMCScrollbar>();
            scrollRect.horizontalScrollbarVisibility = IMCScrollRect.ScrollbarVisibility.AutoHideAndExpandViewport;
            scrollRect.verticalScrollbarVisibility = IMCScrollRect.ScrollbarVisibility.AutoHideAndExpandViewport;
            scrollRect.horizontalScrollbarSpacing = -3;
            scrollRect.verticalScrollbarSpacing = -3;

            IMCImage rootImage = root.AddComponent<IMCImage>();
            rootImage.sprite = resources.background;
            rootImage.type = IMCImage.Type.Sliced;
            rootImage.color = s_PanelColor;

            Mask viewportMask = viewport.AddComponent<Mask>();
            viewportMask.showMaskGraphic = false;

            IMCImage viewportImage = viewport.AddComponent<IMCImage>();
            viewportImage.sprite = resources.mask;
            viewportImage.type = IMCImage.Type.Sliced;

            return root;
        }
        public static GameObject CreateTabView(Resources resources)
        {
            GameObject go = CreateUIElementRoot("IMC Tab View", s_ImageElementSize);
            go.AddComponent<CanvasGroup>();
            IMCTabView tabView = go.AddComponent<IMCTabView>();

            IMCImage image = go.AddComponent<IMCImage>();
            image.sprite = resources.background;
            image.type = IMCImage.Type.Sliced;
            image.color = s_PanelColor;

            RectTransform tabViewRT = go.GetComponent<RectTransform>();
            tabViewRT.sizeDelta = new Vector2(600, 250);
            tabViewRT.pivot = new Vector2(0.5f, 1);

            GameObject toggleContainer = CreateUIElementRoot("ToggleContainer", s_ImageElementSize);
            RectTransform toggleContainerRT = toggleContainer.GetComponent<RectTransform>();
            toggleContainerRT.SetParent(tabViewRT);
            toggleContainerRT.anchorMin = new Vector2(0.5f, 1);
            toggleContainerRT.anchorMax = new Vector2(0.5f, 1);
            toggleContainerRT.pivot = new Vector2(0.5f, 0);
            toggleContainerRT.anchoredPosition = Vector2.zero;
            toggleContainerRT.sizeDelta = new Vector2(600, 50);
            ContentSizeFitter contentSizeFitter = toggleContainer.AddComponent<ContentSizeFitter>();
            contentSizeFitter.horizontalFit = IMCUI.UI.ContentSizeFitter.FitMode.PreferredSize;
            toggleContainer.AddComponent<IMCHorizontalLayoutGroup>();
            ToggleGroup toggleGroup = toggleContainer.AddComponent<ToggleGroup>();

            for (int i = 0; i <= 2; i++)
            {
                GameObject toggle = CreateToggle(resources);
                toggle.transform.FindChild("Label").GetComponent<IMCText>().raycast = false;//if don't shut down IMCText is raycast attribute  ,occur BUG.

                IMCTabViewToggle tabViewToggle = toggle.AddComponent<IMCTabViewToggle>();
                tabViewToggle.tabView = tabViewRT.GetComponent<IMCTabView>();

                tabViewRT.GetComponent<IMCTabView>().toggles.Add(tabViewToggle);
                // in editor script , add event invalidity.
                //tabViewToggle.toggle.onValueChanged.AddListener(tabViewToggle.parent.TabViewControlShowOrClose);

                IMCToggle toggleT = toggle.GetComponent<IMCToggle>();
                toggleT.name = toggleT.name + i.ToString();
                toggleT.transform.SetParent(toggleContainerRT);

                toggleT.group = toggleGroup;

                // because toggleContainer is  ToggleGroup component,ToggleGroup is Execution order impact Toggle,so annotation this code .
                //if (i == 0) 
                //    toggleT.isOn = true;
                //else
                //    toggleT.isOn = false;

                RectTransform backGround = toggle.transform.GetChild(0).GetComponent<RectTransform>();
                backGround.anchorMin = Vector2.zero;
                backGround.anchorMax = Vector2.one;
                backGround.pivot = Vector2.zero;
                backGround.anchoredPosition = Vector2.zero;
                backGround.sizeDelta = Vector2.zero;

                RectTransform checkmark = backGround.GetChild(0).GetComponent<RectTransform>();
                checkmark.anchorMin = Vector2.zero;
                checkmark.anchorMax = Vector2.one;
                checkmark.anchoredPosition = Vector2.zero;
                checkmark.sizeDelta = Vector2.zero;

                RectTransform labelRT = toggle.transform.GetChild(1).GetComponent<RectTransform>();
                labelRT.anchoredPosition = Vector2.zero;
                labelRT.sizeDelta = Vector2.zero;
                labelRT.GetComponent<IMCText>().alignment = TextAnchor.MiddleCenter;

                LayoutElement layoutElement = toggle.AddComponent<LayoutElement>();
                layoutElement.preferredWidth = 200;
                layoutElement.preferredHeight = 20;
            }

            GameObject contentContainer = CreateUIElementRoot("ContentContainer", s_ImageElementSize);
            RectTransform contentContainerRT = contentContainer.GetComponent<RectTransform>();
            contentContainerRT.sizeDelta = tabViewRT.sizeDelta;
            contentContainerRT.SetParent(tabViewRT);
            contentContainerRT.anchoredPosition = Vector2.zero;
            for (int i = 0; i <= 2; i++)
            {
                GameObject toggleContent = CreateUIElementRoot("TabContent" + i.ToString(), s_ImageElementSize);
                IMCImage toggleContentImage = toggleContent.AddComponent<IMCImage>();
                toggleContentImage.sprite = resources.background;
                toggleContentImage.type = IMCImage.Type.Sliced;
                toggleContentImage.color = s_PanelColor;

                tabViewRT.GetComponent<IMCTabView>().contents.Add(toggleContentImage);

                RectTransform toggleContentRT = toggleContent.GetComponent<RectTransform>();
                toggleContentRT.sizeDelta = tabViewRT.sizeDelta;
                toggleContentRT.SetParent(contentContainerRT);
                toggleContentRT.anchoredPosition = Vector2.zero;
            }
            return go;
        }
        public static GameObject CreateMessageBox(Resources resources)
        {
            GameObject go = CreateImage(resources);
            go.name = "IMCMessageBox";
            go.AddComponent<CanvasGroup>();
            IMCMessageBox mesBox = go.AddComponent<IMCMessageBox>();

            mesBox.background = go.GetComponent<IMCImage>();

            RectTransform goRT = go.GetComponent<RectTransform>();
            goRT.anchorMin = new Vector2(0.5f, 0.5f);
            goRT.anchorMax = new Vector2(0.5f, 0.5f);
            goRT.pivot = new Vector2(0.5f, 0.5f);
            goRT.sizeDelta = new Vector2(400, 250);

            GameObject title = CreateText(resources);
            title.name = "Title";
            IMCText titleIMC = title.GetComponent<IMCText>();
            titleIMC.transform.SetParent(goRT);
            titleIMC.anchorMin = new Vector2(0.5f, 1);
            titleIMC.anchorMax = new Vector2(0.5f, 1);
            titleIMC.pivot = new Vector2(0.5f, 1);
            titleIMC.sizeDelta = new Vector2(400, 30);
            titleIMC.anchoredPosition3D = Vector3.zero;
            titleIMC.text = "Title";
            titleIMC.fontSize = 14;
            titleIMC.alignment = TextAnchor.MiddleCenter;
            mesBox.title = titleIMC;

            GameObject content = CreateText(resources);
            content.name = "Content";
            IMCText contentIMC = content.GetComponent<IMCText>();
            contentIMC.transform.SetParent(goRT);
            contentIMC.anchorMin = new Vector2(0.5f, 0.5f);
            contentIMC.anchorMax = new Vector2(0.5f, 0.5f);
            contentIMC.pivot = new Vector2(0.5f, 0.5f);
            contentIMC.sizeDelta = new Vector2(400, 120);
            contentIMC.anchoredPosition3D = Vector3.zero;
            contentIMC.text = "Content";
            contentIMC.fontSize = 14;
            contentIMC.alignment = TextAnchor.MiddleCenter;
            mesBox.content = contentIMC;

            GameObject btns = new GameObject("ButtonList");
            RectTransform btnsRT = btns.AddComponent<RectTransform>();
            btnsRT.SetParent(goRT);
            btnsRT.anchorMin = new Vector2(0.5f, 0);
            btnsRT.anchorMax = new Vector2(0.5f, 0);
            btnsRT.pivot = new Vector2(0.5f, 0);
            btnsRT.sizeDelta = new Vector2(380, 40);
            btnsRT.anchoredPosition3D = new Vector3(0, 10, 0);
            ContentSizeFitter btnsCSF = btns.AddComponent<ContentSizeFitter>();
            btnsCSF.horizontalFit = ContentSizeFitter.FitMode.PreferredSize;
            btnsCSF.verticalFit = ContentSizeFitter.FitMode.Unconstrained;
            IMCHorizontalLayoutGroup btnsHLG = btns.AddComponent<IMCHorizontalLayoutGroup>();
            btnsHLG.spacing = 5;
            mesBox.buttonParent = btns;
            for (int i = 0; i < 3; i++)
            {
                GameObject tempBtn = CreateButton(resources);
                LayoutElement tempBtnLE = tempBtn.AddComponent<LayoutElement>();
                tempBtn.transform.SetParent(btnsRT);
                tempBtnLE.preferredHeight = 35.9f;
                tempBtnLE.preferredWidth = 124.4f;
                mesBox.btns.Add(tempBtn.GetComponent<IMCButton>());
            }

            GameObject closeBtn = CreateButton(resources);
            closeBtn.name = "closeBtn";
            RectTransform closeBtnRT = closeBtn.GetComponent<RectTransform>();
            closeBtnRT.SetParent(goRT);
            closeBtnRT.anchorMin = Vector2.one;
            closeBtnRT.anchorMax = Vector2.one;
            closeBtnRT.pivot = Vector2.one;
            closeBtnRT.sizeDelta = new Vector2(35, 30);
            closeBtnRT.anchoredPosition3D = Vector3.zero;
            IMCText closeText = closeBtn.GetComponent<IMCButton>().Text;
            closeText.text = "X";
            closeText.fontSize = 14;
            closeText.alignment = TextAnchor.MiddleCenter;
            mesBox.closeButton = closeBtn.GetComponent<IMCButton>();

            return go;
        }
        public static GameObject CreateCascadeDropDown(Resources resoures)
        {
            GameObject go = new GameObject("IMC Cascade Dropdown");
            go.AddComponent<IMCCascadeDropdown>();
            RectTransform goRT = go.AddComponent<RectTransform>();
            goRT.anchoredPosition3D = Vector3.zero;
            go.AddComponent<CanvasGroup>();
            IMCDropdown[] dropdowns = new IMCDropdown[3];
            for (int i = 0; i < dropdowns.Length; i++)
            {
                dropdowns[i] = CreateDropdown(resoures).GetComponent<IMCDropdown>();
                RectTransform tempRT = dropdowns[i].GetComponent<RectTransform>();

                tempRT.pivot = new Vector2(0, 1);
                tempRT.anchorMin = new Vector2(0, 1);
                tempRT.anchorMax = new Vector2(0, 1);
            }
            goRT.sizeDelta = new Vector2(dropdowns[0].sizeDelta.x * 3, dropdowns[0].sizeDelta.y);
            for (int i = 0; i < dropdowns.Length; i++)
            {
                dropdowns[i].transform.SetParent(goRT);
                dropdowns[i].GetComponent<RectTransform>().anchoredPosition3D = new Vector3(goRT.anchoredPosition3D.x + i * dropdowns[i].GetComponent<RectTransform>().sizeDelta.x, goRT.anchoredPosition3D.y, goRT.anchoredPosition3D.z);
            }

            return go;
        }

        public static GameObject CreateSwitchButton(Resources resources)
        {
            GameObject root = CreateUIElementRoot("IMC Switch Button", s_ThinElementSize);

            GameObject background = CreateUIObject("Background", root);
            GameObject fillArea = CreateUIObject("Fill Area", root);
            GameObject fill = CreateUIObject("Fill", fillArea);
            GameObject handleArea = CreateUIObject("Handle Slide Area", root);
            GameObject handle = CreateUIObject("Handle", handleArea);

            // Background
            IMCImage backgroundImage = background.AddComponent<IMCImage>();
            backgroundImage.sprite = resources.background;
            backgroundImage.type = IMCImage.Type.Sliced;
            backgroundImage.color = s_DefaultSelectableColor;
            RectTransform backgroundRect = background.GetComponent<RectTransform>();
            backgroundRect.anchorMin = Vector2.zero;
            backgroundRect.anchorMax = Vector2.one;
            backgroundRect.anchoredPosition3D = Vector3.zero;
            backgroundRect.sizeDelta = new Vector2(0, 0);

            // Fill Area
            RectTransform fillAreaRect = fillArea.GetComponent<RectTransform>();
            fillAreaRect.anchorMin = new Vector2(0, 0.25f);
            fillAreaRect.anchorMax = new Vector2(1, 0.75f);
            fillAreaRect.anchoredPosition = new Vector2(-5, 0);
            fillAreaRect.sizeDelta = new Vector2(-20, 0);

            // Fill
            IMCImage fillImage = fill.AddComponent<IMCImage>();
            fillImage.sprite = resources.standard;
            fillImage.type = IMCImage.Type.Sliced;
            fillImage.color = new Color(0, 0, 0, 0);

            RectTransform fillRect = fill.GetComponent<RectTransform>();
            fillRect.sizeDelta = new Vector2(10, 0);

            // Handle Area
            RectTransform handleAreaRect = handleArea.GetComponent<RectTransform>();
            handleAreaRect.sizeDelta = new Vector2(-20, 0);
            handleAreaRect.anchorMin = new Vector2(0, 0);
            handleAreaRect.anchorMax = new Vector2(1, 1);

            // Handle
            IMCImage handleImage = handle.AddComponent<IMCImage>();
            handleImage.sprite = resources.knob;
            handleImage.color = s_DefaultSelectableColor;

            RectTransform handleRect = handle.GetComponent<RectTransform>();
            handleRect.anchorMin = Vector2.zero;
            handleRect.anchorMax = new Vector2(0, 1);
            handleRect.sizeDelta = new Vector2(20, 0);

            GameObject content = new GameObject("content");
            content.transform.SetParent(handleRect);
            IMCText text = content.AddComponent<IMCText>();
            text.anchorMin = Vector2.zero;
            text.anchorMax = Vector2.one;
            text.anchoredPosition3D = Vector3.zero;
            text.sizeDelta = new Vector2(0, 0);
            text.fontSize = 10;
            text.raycastTarget = false;
            text.supportRichText = false;
            text.color = Color.black;
            text.alignment = TextAnchor.MiddleCenter;

            // Setup slider component
            IMCSwitchButton switchBtn = root.AddComponent<IMCSwitchButton>();
            root.AddComponent<CanvasGroup>();
            switchBtn.content = text.GetComponent<IMCText>();
            switchBtn.fillRect = fill.GetComponent<RectTransform>();
            switchBtn.handleRect = handle.GetComponent<RectTransform>();

            switchBtn.targetGraphic = handleImage;
            switchBtn.direction = IMCSlider.Direction.LeftToRight;
            SetDefaultColorTransitionValues(switchBtn);

            return root;
        }
    }
}
