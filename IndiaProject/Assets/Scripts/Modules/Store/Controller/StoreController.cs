using System;
using UnityEngine;

[DependsOn(typeof(PlayerManager))]
public class StoreController : IController
{
    public void OnGameStart()
    {
        Debug.Log("OnGameStart");
    }

    public void OnLogin(Action onComplete)
    {
        Debug.Log("StoreController: Initializing...");

        // 模拟异步初始化
        TimerManager.DelayTimer(0.5f, () =>
        {
            Debug.Log("StoreController: Initialized!");
            onComplete?.Invoke();
        });
    }

    public void OnLoginOut()
    {
        Debug.Log("OnLoginOut");
    }
}
