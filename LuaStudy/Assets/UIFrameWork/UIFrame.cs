using System;
using UnityEngine;
using UnityEngine.UI;

namespace UIFramework
{
    public class UIFrame : MonoBehaviour
    {
        [Tooltip("如果您想手动初始化此UI框架，请将其设置为false")]
        [SerializeField] private bool initalizeOnGameStart = true;

        /// <summary>
        /// 面板层
        /// </summary>
        private PanelUILayer panelLayer;
        /// <summary>
        /// 窗口层
        /// </summary>
        private WindowUILayer windowLayer;

        private Canvas mainCanvas;
        private GraphicRaycaster graphicRaycaster;

        public Canvas MainCanvas
        {
            get
            {
                if (mainCanvas == null)
                    mainCanvas = GetComponent<Canvas>();
                return mainCanvas;
            }
        }
        /// <summary>
        /// 初始化ui框架
        /// </summary>
        private void Initialize()
        {
            if (panelLayer == null)
            {
                panelLayer = gameObject.GetComponentInChildren<PanelUILayer>(true);
                if (panelLayer == null)
                {
                    Debug.LogError("[UI Frame] UI Frame lacks Panel Layer!");
                }
                else
                {
                    panelLayer.Initialize();
                }
            }
            if (windowLayer == null)
            {
                windowLayer = gameObject.GetComponentInChildren<WindowUILayer>(true);
                if (windowLayer == null)
                {
                    Debug.LogError("[UI Frame] UI Frame lacks Window Layer!");
                }
                else
                {
                    windowLayer.Initialize();
                    windowLayer.RequestScreenBlock += OnRequestScreenBlock;
                    windowLayer.RequestScreenUnblock += OnRequestScreenUnblock;
                }
            }

            graphicRaycaster = MainCanvas.GetComponent<GraphicRaycaster>();
        }

        /// <summary>
        /// 仅通过id显示一个面板
        /// </summary>
        /// <param name="screenId"></param>
        public void ShowPanel(string screenId)
        {
            panelLayer.ShowScreenById(screenId);
        }
        /// <summary>
        /// 通过id和属性显示面板
        /// </summary>
        /// <param name="screenId"></param>
        /// <param name="properties"></param>
        /// <typeparam name="T"></typeparam>
        public void ShowPanel<T>(string screenId, T properties) where T : IPanelProperties
        {
            panelLayer.ShowScreenById<T>(screenId, properties);
        }

        /// <summary>
        /// 仅通过id隐藏面板
        /// </summary>
        /// <param name="screenId"></param>
        public void HidePanel(string screenId)
        {
            panelLayer.HideScreenById(screenId);
        }

        /// <summary>
        /// 仅通过id打开窗口
        /// </summary>
        /// <param name="screenId"></param>
        public void OpenWindow(string screenId)
        {
            windowLayer.ShowScreenById(screenId);
        }
        /// <summary>
        /// 仅通过id关闭窗口
        /// </summary>
        /// <param name="screenId"></param>
        public void CloseWindow(string screenId)
        {
            windowLayer.HideScreenById(screenId);
        }
        /// <summary>
        /// 关闭当前窗口
        /// </summary>
        public void CloseCurrentWindow()
        {
            if (windowLayer.CurrentWindow != null)
            {
                CloseWindow(windowLayer.CurrentWindow.ScreenId);
            }
        }
        /// <summary>
        /// 根据id打开窗口并且传递一些参数
        /// </summary>
        /// <param name="screenId"></param>
        /// <param name="properties"></param>
        /// <typeparam name="T"></typeparam>
        public void OpenWindow<T>(string screenId, T properties) where T : IWindowProperties
        {
            windowLayer.ShowScreenById<T>(screenId, properties);
        }
        /// <summary>
        /// 二次包装方法，根据id直接显示对应的界面
        /// </summary>
        /// <param name="screenId"></param>
        public void ShowScreen(string screenId)
        {
            Type type;
            if (IsScreenRegistered(screenId, out type))
            {
                if (type == typeof(IWindowController))
                    OpenWindow(screenId);
                else if (type == typeof(IPanelController))
                    ShowPanel(screenId);
            }
            else
            {
                Debug.LogError(string.Format("Tried to open Screen id {0} but it's not registered as Window or Panel!",
                    screenId));
            }
        }

        /// <summary>
        /// 注册一个界面，如果传了screenTransform，就相当于制定了父节点
        /// </summary>
        /// <param name="screenId"></param>
        /// <param name="controller"></param>
        /// <param name="screenTransform"></param>
        public void RegisterScreen(string screenId, IScreenController controller, Transform screenTransform)
        {
            IWindowController window = controller as IWindowController;
            if (window != null)
            {
                windowLayer.RegisterScreen(screenId, window);
                if (screenTransform != null)
                {
                    windowLayer.ReparentScreen(controller, screenTransform);
                }
                return;
            }

            IPanelController panel = controller as IPanelController;
            if (panel != null)
            {
                panelLayer.RegisterScreen(screenId, panel);
                if (screenTransform != null)
                {
                    panelLayer.ReparentScreen(controller, screenTransform);
                }
            }
        }

        /// <summary>
        /// 注册一个面板
        /// </summary>
        /// <param name="screenId"></param>
        /// <param name="controller"></param>
        /// <typeparam name="TPanel"></typeparam>
        public void RegisterPanel<TPanel>(string screenId, TPanel controller) where TPanel : IPanelController
        {
            panelLayer.RegisterScreen(screenId, controller);
        }

        /// <summary>
        /// 注销一个面板
        /// </summary>
        /// <param name="screenId"></param>
        /// <param name="controller"></param>
        /// <typeparam name="TPanel"></typeparam>
        public void UnregisterPanel<TPanel>(string screenId, TPanel controller) where TPanel : IPanelController
        {
            panelLayer.UnregisterScreen(screenId, controller);
        }

        /// <summary>
        /// 注册一个窗口
        /// </summary>
        /// <param name="screenId"></param>
        /// <param name="controller"></param>
        /// <typeparam name="TWindow"></typeparam>
        public void RegisterWindow<TWindow>(string screenId, TWindow controller) where TWindow : IWindowController
        {
            windowLayer.RegisterScreen(screenId, controller);
        }

        /// <summary>
        /// 注销窗口
        /// </summary>
        /// <param name="screenId"></param>
        /// <param name="controller"></param>
        /// <typeparam name="TWindow"></typeparam>
        public void UnregisterWindow<TWindow>(string screenId, TWindow controller) where TWindow : IWindowController
        {
            windowLayer.UnregisterScreen(screenId, controller);
        }

        /// <summary>
        /// 根据面板id检测是否开启中
        /// </summary>
        /// <param name="panelId"></param>
        /// <returns></returns>
        public bool IsPanelOpen(string panelId)
        {
            return panelLayer.IsPanelVisible(panelId);
        }

        /// <summary>
        /// 隐藏所有界面
        /// </summary>
        /// <param name="animate"></param>
        public void HideAll(bool animate = true)
        {
            HideAllPanel(animate);
            HideAllWindows(animate);
        }

        /// <summary>
        /// 隐藏所有面板层界面
        /// </summary>
        /// <param name="animate"></param>
        public void HideAllPanel(bool animate = true)
        {
            panelLayer.HideAll(animate);
        }
        /// <summary>
        /// 隐藏所有窗口层的窗口
        /// </summary>
        /// <param name="animate"></param>
        public void HideAllWindows(bool animate = true)
        {
            windowLayer.HideAll(animate);
        }

        /// <summary>
        /// 检查界面是否被注册过了
        /// </summary>
        /// <param name="screenId"></param>
        /// <returns></returns>
        public bool IsScreenRegistered(string screenId)
        {
            if (windowLayer.IsScreenRegistered(screenId))
            {
                return true;
            }
            if (panelLayer.IsScreenRegistered(screenId))
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// 跟上面一样，只不过多了个类型的返回
        /// </summary>
        /// <param name="screenId"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public bool IsScreenRegistered(string screenId, out Type type)
        {
            if (windowLayer.IsScreenRegistered(screenId))
            {
                type = typeof(IWindowController);
                return true;
            }
            if (panelLayer.IsScreenRegistered(screenId))
            {
                type = typeof(IPanelController);
                return true;
            }
            type = null;
            return false;
        }

        private void OnRequestScreenBlock()
        {
            if (graphicRaycaster != null)
                graphicRaycaster.enabled = false;
        }

        private void OnRequestScreenUnblock()
        {
            if (graphicRaycaster != null)
                graphicRaycaster.enabled = true;
        }

        void Awake()
        {
            if (initalizeOnGameStart)
            {
                Initialize();
            }
        }
    }
}
