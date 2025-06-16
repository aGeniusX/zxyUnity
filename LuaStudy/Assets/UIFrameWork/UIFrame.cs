using System;
using UnityEngine;
using UnityEngine.UI;

public class UIFrame : MonoBehaviour, IController
{
    [Tooltip("如果您想手动初始化此UI框架，请将其设置为false")]
    [SerializeField] private bool initalizeOnAwake = true;

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
    public void OnGameStart()
    {
        throw new NotImplementedException();
    }

    public void OnLogin(Action onComplete)
    {
        throw new NotImplementedException();
    }

    public void OnLoginOut()
    {
        throw new NotImplementedException();
    }
}
