using System;
using UnityEngine;
using UnityEngine.UI;

public class UIFrame : MonoBehaviour, IController
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
    /// <param name="screeId"></param>
    public void ShowScreen(string screeId)
    {
        Type type;
    }
    private void OnRequestScreenUnblock()
    {

    }

    private void OnRequestScreenBlock()
    {

    }

    public void OnGameStart()
    {
        if (initalizeOnGameStart)
        {
            Initialize();
        }
    }

    public void OnLogin(Action onComplete)
    {

    }

    public void OnLoginOut()
    {

    }
}
