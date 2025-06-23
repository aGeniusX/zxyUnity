using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 玩家controller
/// </summary>
public class PlayerManager : IController
{
    public void OnGameStart()
    {
        Debug.Log("PlayerController: Game Started");
        
    }

    public void OnLogin(Action onComplete)
    {
        Debug.Log("PlayerController: Initializing...");

        // 模拟异步初始化
        TimerManager.DelayTimer(1f, () =>
        {
            Debug.Log("PlayerController: Initialized!");
            onComplete?.Invoke();
        });
    }

    public void OnLoginOut()
    {
        Debug.Log("PlayerController: Logged out");
    }
}
