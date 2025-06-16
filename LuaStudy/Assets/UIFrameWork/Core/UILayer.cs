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
    public virtual void Initialize()
    {
        registeredScreens = new();
    }

    /// <summary>
    /// 传进度的界面当作层的子节点
    /// </summary>
    /// <param name="controller"></param>
    /// <param name="screenTransform"></param>
    public virtual void ReparentScreen(IScreenController controller, Transform screenTransform)
    {
        screenTransform.SetParent(transform, false);
    }
    /// <summary>
    /// 注册界面的controller 带上明确的界面id
    /// </summary>
    /// <param name="screenId"></param>
    /// <param name="controller"></param>
    public void RegisterScreen(string screenId, TScreen controller)
    {
        if (!registeredScreens.ContainsKey(screenId))
        {
            ProcessScreenRegister(screenId, controller);
        }
        else
        {
            Debug.LogError("[界面id已经注册]" + screenId);
        }
    }

    /// <summary>
    /// 根据id取消注册界面controller
    /// </summary>
    /// <param name="screenId"></param>
    /// <param name="controller"></param>
    public void UnregisterScreen(string screenId, TScreen controller)
    {
        if (registeredScreens.ContainsKey(screenId))
        {
            ProcessScreenUnRegister(screenId, controller);
        }
        else
        {
            Debug.LogError("[界面id未注册]" + screenId);
        }
    }

    /// <summary>
    /// 根据id去找界面的controller，并显示出来
    /// </summary>
    /// <param name="screenId"></param>
    public void ShowScreenById(string screenId)
    {
        TScreen ctl;
        if (registeredScreens.TryGetValue(screenId, out ctl))
        {
            ShowScreen(ctl);
        }
        else
        {
            Debug.LogError("[界面id未注册]" + screenId);
        }
    }
    /// <summary>
    /// 根据界面id显示具体的controller,带上具体的属性参数
    /// </summary>
    /// <param name="screenId"></param>
    /// <param name="properties"></param>
    /// <typeparam name="TProp"></typeparam>
    public void ShowScreenById<TProp>(string screenId, TProp properties) where TProp : IScreenProperties
    {
        TScreen ctl;
        if (registeredScreens.TryGetValue(screenId, out ctl))
        {
            ShowScreen(ctl, properties);
        }
        else
        {
            Debug.LogError("[界面id未注册]" + screenId);
        }
    }
    /// <summary>
    /// 根据id隐藏界面
    /// </summary>
    /// <param name="screenId"></param>
    public void HideScreenById(string screenId)
    {
        TScreen ctl;
        if (registeredScreens.TryGetValue(screenId, out ctl))
        {
            HideScreen(ctl);
        }
        else
        {
            Debug.LogError("[界面id未注册]" + screenId);
        }
    }
    /// <summary>
    /// 查看id是否注册了
    /// </summary>
    /// <param name="screenId"></param>
    /// <returns></returns>
    public bool IsScreenRegistered(string screenId)
    {
        return registeredScreens.ContainsKey(screenId);
    }

    /// <summary>
    /// 隐藏所有界面
    /// </summary>
    /// <param name="shouldAnimateWhenHiding"></param>
    public virtual void HideAll(bool shouldAnimateWhenHiding = true)
    {
        foreach (var screen in registeredScreens)
        {
            screen.Value.Hide(shouldAnimateWhenHiding);
        }
    }

    /// <summary>
    /// 注册界面
    /// </summary>
    /// <param name="screenId"></param>
    /// <param name="controller"></param>
    protected virtual void ProcessScreenRegister(string screenId, TScreen controller)
    {
        controller.ScreenId = screenId;
        registeredScreens.Add(screenId, controller);
        controller.ScreenDestroyed += OnScreenDestroy;
    }


    /// <summary>
    /// 注销界面
    /// </summary>
    /// <param name="screenId"></param>
    /// <param name="controller"></param>
    protected virtual void ProcessScreenUnRegister(string screenId, TScreen controller)
    {
        controller.ScreenDestroyed -= OnScreenDestroy;
        registeredScreens.Remove(screenId);
    }

    /// <summary>
    /// 销毁界面时调用
    /// </summary>
    /// <param name="controller"></param>
    private void OnScreenDestroy(IScreenController screen)
    {
        if (!string.IsNullOrEmpty(screen.ScreenId) && IsScreenRegistered(screen.ScreenId))
        {
            UnregisterScreen(screen.ScreenId, (TScreen)screen);
        }
    }
}
