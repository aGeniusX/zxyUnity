using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.UI;
using UnityEngine.UI;
using Object = UnityEngine.Object;

namespace UIFramework
{
    public static class UIFrameWorkTools
    {
        [MenuItem("Assets/Create/UI/UI Frame in Scene", priority = 2)]
        public static void CreateUIFrameInScene()
        {
            CreateUIFrame();
        }
        [MenuItem("Assets/Create/UI/UI Frame Prefab", priority = 1)]
        public static void CreateUIFramePrefab()
        {
            var frame = CreateUIFrame();
            string prefabPath = GetCurrentPath();
            prefabPath = EditorUtility.SaveFilePanel("UI Frame Prefab", prefabPath, "UIFrame", "prefab");

            if (prefabPath.StartsWith(Application.dataPath))
                prefabPath = "Assets" + prefabPath.Substring(Application.dataPath.Length);
            if (!string.IsNullOrEmpty(prefabPath))
                CreateNewPrefab(frame, prefabPath);
            Object.DestroyImmediate(frame);
        }

        private static GameObject CreateUIFrame()
        {
            var uiLayer = LayerMask.NameToLayer("UI");
            var root = new GameObject("UIFrame");
            var camera = new GameObject("UICamera");

            var cam = camera.AddComponent<Camera>();
            cam.clearFlags = CameraClearFlags.Depth;
            cam.cullingMask = LayerMask.GetMask("UI");
            cam.orthographic = true;
            cam.farClipPlane = 25;

            root.AddComponent<UIFrame>();
            var canvas = root.AddComponent<Canvas>();
            root.layer = uiLayer;

            //ScreenSpaceCamera 允许使用3D模型，粒子效果
            canvas.renderMode = RenderMode.ScreenSpaceCamera;
            canvas.worldCamera = cam;

            cam.transform.SetParent(root.transform, false);
            cam.transform.localPosition = new Vector3(0f, 0f, -1500f);

            var screenScaler = root.AddComponent<CanvasScaler>();
            screenScaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            screenScaler.referenceResolution = new Vector2(1920, 1080);

            root.AddComponent<GraphicRaycaster>();

            var eventSystem = new GameObject("EventSystem");
            eventSystem.transform.SetParent(root.transform, false);
            eventSystem.AddComponent<EventSystem>();
            eventSystem.AddComponent<InputSystemUIInputModule>();

            //创建层
            //panel层
            var panelLayGO = CreateRect("PanelLayer", root, uiLayer);
            var panelLayer = panelLayGO.AddComponent<PanelUILayer>();

            //window层
            var windowLayerGO = CreateRect("WindowLayer", root, uiLayer);
            var windowLayer = windowLayerGO.AddComponent<WindowUILayer>();

            var prioPanelLayer = CreateRect("PriorityPanelLayer", root, uiLayer);


            //window辅助层
            var windowParaLayerGO = CreateRect("PriorityWindowLayer", root, uiLayer);
            var windowParaLayer = windowParaLayerGO.AddComponent<WindowParaLayer>();
            //通过反射来设置参数
            SetPrivateField(windowLayer, windowParaLayer, "priorityParaLayer");

            //黑色遮罩
            var drakenGO = CreateRect("DarkenBG", windowParaLayer.gameObject, uiLayer);
            var darkenImage = drakenGO.AddComponent<Image>();
            darkenImage.color = new Color(0f, 0f, 0f, 0.75f);

            //通过反射来设置蒙黑
            SetPrivateField(windowParaLayer, drakenGO, "darkenBgObject");
            drakenGO.SetActive(false);

            //引导层
            var tutorialPanelLayer = CreateRect("TutorialPanelLayer", root, uiLayer);

            //在面板层装配参数
            var prioList = new List<PanelPriorityLayerListEntry>()
            {
                new PanelPriorityLayerListEntry(PanelPriority.None,panelLayer.transform),
                new PanelPriorityLayerListEntry(PanelPriority.Prioritary,prioPanelLayer.transform),
                new PanelPriorityLayerListEntry(PanelPriority.Tutorial,tutorialPanelLayer.transform)
            };
            var panelPrios = new PanelPriorityLayerList(prioList);

            SetPrivateField(panelLayer, panelPrios, "priorityLayers");

            return root;
        }

        public static string GetCurrentPath()
        {
            string path = AssetDatabase.GetAssetPath(Selection.activeObject);
            if (path == "")
                path = "Assets";
            else if (Path.GetExtension(path) != "")
                path = path.Replace(Path.GetFileName(AssetDatabase.GetAssetPath(Selection.activeObject)), "");

            return path;
        }

        private static void SetPrivateField(object target, object element, string fieldName)
        {
            var test = target.GetType();
            var prop = target.GetType().
                GetField(fieldName, System.Reflection.BindingFlags.NonPublic 
                | System.Reflection.BindingFlags.Instance);
            prop.SetValue(target, element);
        }

        private static GameObject CreateRect(string name, GameObject parentGO, int uiLayer)
        {
            var parent = parentGO.GetComponent<RectTransform>();
            var newRect = new GameObject(name, typeof(RectTransform));
            newRect.layer = uiLayer;
            var rt = newRect.GetComponent<RectTransform>();

            rt.anchoredPosition = parent.position;
            rt.anchorMax = new Vector2(0, 0);
            rt.anchorMin = new Vector2(1, 1);
            rt.pivot = new Vector2(.5f, .5f);
            rt.transform.SetParent(parent, false);
            rt.sizeDelta = Vector3.zero;

            return newRect;
        }

        private static void CreateNewPrefab(GameObject obj, string localPath)
        {
            PrefabUtility.SaveAsPrefabAsset(obj, localPath);
        }
    }
}
