using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using XLua;

#region 核心系统
/// <summary>
/// 游戏管理器
/// </summary>
public class GameManager : MonoBehaviour
{
    #region 单例模式
    private static GameManager instance;
    public static GameManager Instance => instance ??= FindObjectOfType<GameManager>() ?? new GameObject("GameManager").
    AddComponent<GameManager>();
    #endregion

    #region 控制器管理
    /// <summary>
    /// 所有注册的控制器
    /// </summary>
    /// <returns></returns>
    private readonly List<IController> _controllers = new();
    /// <summary>
    /// 控制器初始化状态
    /// </summary>
    /// <returns></returns>
    private readonly Dictionary<Type, ControllerState> _controllerStates = new();
    /// <summary>
    /// 初始化队列
    /// </summary>
    /// <returns></returns>
    private readonly Queue<IController> _initQueue = new();
    /// <summary>
    /// 每一帧最大控制器加载数量
    /// </summary>
    private const int MAX_INIT_PER_FRAME = 5;
    private bool _isInitializing = false;
    private Action _onAllInitialized;
    /// <summary>
    /// lua控制器
    /// </summary>
    private LuaController _luaController;
    #endregion

    #region 生命周期
    void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
        DontDestroyOnLoad(gameObject);
        //加载计时器
        if (!gameObject.GetComponent<TimerUpdater>())
        {
            gameObject.AddComponent<TimerUpdater>();
        }
        //创建lua控制器
        _luaController = new LuaController();
    }
    #endregion

    #region 公共API
    /// <summary>
    /// 注册控制器
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public void RegisterController<T>() where T : IController, new()
    {
        var controller = new T();
        _controllers.Add(controller);
        _controllerStates[typeof(T)] = ControllerState.UnInitialized;
    }

    /// <summary>
    /// 初始化所有控制器
    /// </summary>
    /// <param name="onComplete"></param>
    public void InitializeControllers(Action onComplete)
    {
        if (_isInitializing)
        {
            Debug.LogWarning("正在初始化控制器");
            return;
        }
        _isInitializing = true;
        _onAllInitialized = onComplete;
        //重置加载状态
        _initQueue.Clear();
        foreach (var key in _controllerStates.Keys.ToList())
        {
            _controllerStates[key] = ControllerState.UnInitialized;
        }
        foreach (var controller in _controllers)
        {
            //没有控制器依赖的先加载
            if (!HasDependencies(controller))
            {
                _initQueue.Enqueue(controller);
                _controllerStates[controller.GetType()] = ControllerState.Queued;
            }
        }
        //分帧初始化
        StartCoroutine(InitializeCoroutine());
    }
    /// <summary>
    /// 获取控制器实例
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public T GetController<T>() where T : class, IController
    {
        return _controllers.FirstOrDefault(c => c is T) as T;
    }
    /// <summary>
    /// 是否正在初始化
    /// </summary>
    public bool IsInitializing => _isInitializing;
    /// <summary>
    /// 所有控制器是否已初始化完成
    /// </summary>
    /// <returns></returns>
    public bool AreAllControllersInitialized => _controllerStates.Values.All(s => s == ControllerState.Initialized);
    /// <summary>
    /// 控制器总数
    /// </summary>
    public int TotalControllerCount => _controllers.Count;
    /// <summary>
    /// 已初始化控制器数量
    /// </summary>
    /// <returns></returns>
    public int InitializedControllerCount => _controllerStates.Count(s => s.Value == ControllerState.Initialized);
    /// <summary>
    /// 获取控制器状态
    /// </summary>
    /// <param name="controllerType"></param>
    /// <returns></returns>
    public ControllerState GetControllerState(Type controllerType)
    {
        return _controllerStates.TryGetValue(controllerType, out var data) ? data : ControllerState.UnInitialized;
    }
    /// <summary>
    /// 获得所有控制器
    /// </summary>
    public List<IController> Controllers => _controllers;

    #region 生命周期管理
    /// <summary>
    /// 游戏启动时调用
    /// </summary>
    public void OnGameStart()
    {
        //先初始化lua环境
        _luaController.InitLuaEnv();
        foreach (var controller in _controllers)
        {
            controller.OnGameStart();
        }
        //调用lua中的GameStart
        _luaController.CallLuaFunction("OnGameStart");
    }
    /// <summary>
    /// 重连时调用
    /// </summary>
    /// <param name="onComplete"></param>
    public void onReconnect(Action onComplete)
    {
        InitializeControllers(() =>
        {
            _luaController.CallLuaFunction("OnReconnect");
            onComplete?.Invoke();
        });
    }
    /// <summary>
    /// 退出登录的时候调用
    /// </summary>
    public void OnLogout()
    {
        foreach (var controller in _controllers)
        {
            controller.OnLoginOut();
        }
        //重置所有控制器状态
        foreach (var key in _controllerStates.Keys.ToList())
        {
            _controllerStates[key] = ControllerState.UnInitialized;
        }
        //调用Lua的Logout
        _luaController.CallLuaFunction("OnLogout");
    }
    #endregion
    #region Lua系统

    /// <summary>
    /// Lua是否已初始化
    /// </summary>
    public bool IsLuaInitialized => _luaController?.Initialized ?? false;
    /// <summary>
    /// 执行lua脚本
    /// </summary>
    /// <param name="script"></param>
    public void DoLuaScript(string script)
    {
        _luaController.Dostring(script);
    }
    /// <summary>
    /// 调用Lua函数
    /// </summary>
    /// <param name="funcName"></param>
    /// <param name="args"></param>
    public void CallLuaFunction(string funcName, params object[] args)
    {
        _luaController.CallLuaFunction(funcName, args);
    }
    /// <summary>
    /// 注册C#方法到lua
    /// </summary>
    /// <param name="luaFuncName"></param>
    /// <param name="callBack"></param>
    public void RegisterLuaFunction(string luaFuncName, Delegate callBack)
    {
        _luaController.RegisterFunction(luaFuncName, callBack);
    }
    /// <summary>
    /// 获取Lua全局变量
    /// </summary>
    /// <param name="globalName"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public T GetLuaGlobal<T>(string globalName)
    {
        return _luaController.GetGlobal<T>(globalName);
    }
    /// <summary>
    /// 重新加载lua环境
    /// </summary>
    public void ReloadLuaEnvironment()
    {
        if (_luaController != null) _luaController.Reload();
    }

    /// <summary>
    /// 获取已加载的Lua文件
    /// </summary>
    public List<string> GetLoadedLuaFiles()
    {
        return _luaController?.GetLoadedFiles() ?? new List<string>();
    }

    /// <summary>
    /// 获取Lua全局变量
    /// </summary>
    public Dictionary<string, object> GetLuaGlobals()
    {
        return _luaController?.GetGlobalVariables() ?? new Dictionary<string, object>();
    }

    #endregion
    #endregion

    #region 初始化系统
    private IEnumerator InitializeCoroutine()
    {
        while (_initQueue.Count > 0 || _controllerStates.Values.Any(s => s == ControllerState.Initializing))
        {
            int processedThisFrame = 0;
            while (processedThisFrame < MAX_INIT_PER_FRAME && _initQueue.Count > 0)
            {
                var controller = _initQueue.Dequeue();
                var type = controller.GetType();
                if (_controllerStates[type] != ControllerState.Queued)
                {
                    Debug.LogWarning($"{type.Name}{_controllerStates[type]}");
                    continue;
                }
                _controllerStates[type] = ControllerState.Initializing;
                processedThisFrame++;

                //开始初始化
                StartControllerInitialization(controller, type);
            }
            //判断是否全部完成加载
            if (_controllerStates.Values.All(s => s == ControllerState.Initialized))
            {
                FinalizeInitialization();
                yield break;
            }
            //下一帧执行
            yield return null;
        }
        if (_controllerStates.Values.Any(s => s != ControllerState.Initialized))
        {
            Debug.LogError("Controller initialization incomplete! " +
              $"Initialized: {_controllerStates.Count(s => s.Value == ControllerState.Initialized)}/" +
              $"{_controllers.Count}");
        }
        FinalizeInitialization();
    }



    /// <summary>
    /// 开始初始化
    /// </summary>
    /// <param name="controller"></param>
    /// <param name="type"></param>
    private void StartControllerInitialization(IController controller, Type type)
    {
        // 设置超时检测
        var timerId = TimerManager.DelayTimer(10f, () =>
        {
            Debug.LogError($"Controller {type.Name} initialization timed out!");
            CompleteInitialization(controller, type, false);
        });

        try
        {
            // 执行初始化
            controller.OnLogin(() =>
            {
                TimerManager.RemoveTimer(timerId);
                CompleteInitialization(controller, type, true);
            });
        }
        catch (Exception ex)
        {
            TimerManager.RemoveTimer(timerId);
            Debug.LogError($"Controller {type.Name} initialization failed: {ex}");
            CompleteInitialization(controller, type, false);
        }
    }

    /// <summary>
    /// 完成初始化
    /// </summary>
    /// <param name="controller"></param>
    /// <param name="type"></param>
    /// <param name="v"></param>
    private void CompleteInitialization(IController controller, Type type, bool success)
    {
        if (success)
        {
            _controllerStates[type] = ControllerState.Initialized;
            Debug.Log($"Controller {type.Name} initialized successfully.");

            // 检查依赖此控制器的其他控制器
            CheckDependentControllers(controller);
        }
        else
        {
            _controllerStates[type] = ControllerState.Failed;
            Debug.LogError($"Controller {type.Name} initialization failed.");
        }
    }
    /// <summary>
    /// 检查依赖检查
    /// </summary>
    /// <param name="initializedController"></param>
    private void CheckDependentControllers(IController initializedController)
    {
        foreach (var controller in _controllers)
        {
            var type = controller.GetType();

            // 跳过已处理或无需处理的控制器
            if (_controllerStates[type] != ControllerState.UnInitialized)
                continue;

            // 检查是否所有依赖都已满足
            if (HasDependencies(controller))
            {
                _controllerStates[type] = ControllerState.Queued;
                _initQueue.Enqueue(controller);
            }
        }
    }

    /// <summary>
    /// 完成初始化
    /// </summary>
    private void FinalizeInitialization()
    {
        _isInitializing = false;
        _onAllInitialized?.Invoke();
        _onAllInitialized = null;

        //通知Lua初始化完成
        _luaController.CallLuaFunction("OnControllerInitialized");
    }
    #endregion
    #region 依赖检查
    /// <summary>
    /// 检查controller依赖
    /// </summary>
    /// <param name="controller"></param>
    /// <returns></returns>
    private bool HasDependencies(IController controller)
    {
        //使用dependsOn特性检查特效
        var dependsOnAttributes = controller.GetType().GetCustomAttributes(typeof(DependsOnAttribute), true).Cast<DependsOnAttribute>();
        foreach (var attr in dependsOnAttributes)
        {
            foreach (var dependencyType in attr.DependencyTypes)
            {
                if (!_controllerStates.ContainsKey(dependencyType)
                || _controllerStates[dependencyType] != ControllerState.Initialized)
                {
                    return false;
                }
            }
        }
        return true;
    }
    #endregion
}
#endregion