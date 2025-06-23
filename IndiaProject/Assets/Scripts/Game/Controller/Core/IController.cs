using System;
using UnityEngine;

/// <summary>
/// 游戏控制器接口哦
/// </summary>
public interface IController
{
    public void OnGameStart();
    public void OnLogin(Action onComplete);
    public void OnLoginOut();
}

/// <summary>
/// 控制器状态
/// </summary>
public enum ControllerState
{
    /// <summary>
    /// 未初始化
    /// </summary>
    UnInitialized,
    /// <summary>
    /// 初始化队列中
    /// </summary>
    Queued,
    /// <summary>
    /// 初始化中
    /// </summary>
    Initializing,
    /// <summary>
    /// 初始化完成
    /// </summary>
    Initialized,
    /// <summary>
    /// 失败
    /// </summary>
    Failed,
}