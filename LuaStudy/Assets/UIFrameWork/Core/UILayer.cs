using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 基础的UI layer层
/// </summary>
/// <typeparam name="TScreen"></typeparam>
public abstract class UILayer<TScreen> : MonoBehaviour where TScreen : IScreenController
{
    /// <summary>
    /// 已经注册的界面
    /// </summary>
    protected Dictionary<string, TScreen> registeredScreens;

    /// <summary>
    /// 显示界面
    /// </summary>
    /// <param name="screen"></param>
    public abstract void ShowScreen(TScreen screen);

    /// <summary>
    /// 显示界面，带点参数
    /// </summary>
    /// <param name="screen"></param>
    /// <param name="props"></param>
    /// <typeparam name="TProps"></typeparam>
    public abstract void ShowScreen<TProps>(TScreen screen, TProps props) where TProps : IScreenProperties;

    /// <summary>
    /// 隐藏界面
    /// </summary>
    /// <param name="screen">界面类型参数</param>
    public abstract void HideScreen(TScreen screen);

    /// <summary>
    /// 初始化lay层
    /// </summary>
    public virtual void Initalize()
    {
        registeredScreens = new();
    }

    public virtual void ReparentScreen(IScreenController controller, Transform screenTransform)
    {

    }
}
