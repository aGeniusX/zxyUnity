using System;

public interface IScreenController
{
    /// <summary>
    /// ui ID
    /// </summary>
    /// <value></value>
    public string ScreenId { get; set; }
    /// <summary>
    /// 显隐
    /// </summary>
    /// <value></value>
    public bool IsVisible { get; }
    /// <summary>
    /// 显示ui
    /// </summary>
    /// <param name="props"></param>
    void Show(IScreenProperties props = null);
    /// <summary>
    /// 隐藏ui
    /// </summary>
    /// <param name="animate"></param>
    void Hide(bool animate = true);

    Action<IScreenController> InTransitionFinished { get; set; }
    Action<IScreenController> OutTransitionFinished { get; set; }
    Action<IScreenController> CloseRequest { get; set; }
    Action<IScreenController> ScreenDestroyed { get; set; }
}

/// <summary>
/// 所有的窗口必须实现的接口
/// </summary>
public interface IWindowController : IScreenController
{
    bool HideOnForegroundLost { get; }
    bool IsPopup { get; }
    WindowPriority WindowPriority { get; }
}

/// <summary>
/// 所有的面板必须实现的接口
/// </summary>
public interface IPanelController : IScreenController
{
    PanelPriority Priority { get; }
}