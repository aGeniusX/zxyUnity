using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

[DependsOn(typeof(PlayerController))]
public class AchievementController : IController
{
    public void OnGameStart()
    {
        Debug.Log("Achievement system ready");
    }

    public void OnLogin(Action onComplete)
    {
        Debug.Log("Loading achievements...");

        // 模拟异步加载
        TimerManager.DelayTimer(0.7f, () =>
        {
            Debug.Log("Achievements loaded!");
            onComplete?.Invoke();
        });
    }

    public void OnLoginOut()
    {
        Debug.Log("Clearing achievement data");
    }
}
