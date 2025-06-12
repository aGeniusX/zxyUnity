using System;
using System.Collections.Generic;
using UnityEngine;


public class GameStarter : MonoBehaviour
{
    void Start()
    {
        //注册控制器
        GameManager.Instance.RegisterController<PlayerManager>();
        GameManager.Instance.RegisterController<StoreController>();
        GameManager.Instance.RegisterController<AchievementController>();
        GameManager.Instance.RegisterController<LuaController>();


        //启动游戏
        GameManager.Instance.OnGameStart();

        //初始化控制器
        GameManager.Instance.InitializeControllers(() =>
        {
            Debug.Log("All System Ready");
        });

    }

    void OnApplicationQuit()
    {
        GameManager.Instance.OnLogout();
    }
}
