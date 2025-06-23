using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using UnityEngine;
using XLua;
using Debug = UnityEngine.Debug;

/// <summary>
/// Lua控制器
/// </summary>
public class LuaController : IController
{
    /// <summary>
    /// lua环境
    /// </summary>
    private LuaEnv _luaEnv;
    /// <summary>
    /// lua全局表
    /// </summary>
    private LuaTable _luaGlobal;
    /// <summary>
    /// lua脚本路径
    /// </summary>
    private const string LUA_SCRIPT_PATH = "LuaScripts/";
    /// <summary>
    /// 是否完成加载
    /// </summary>
    public bool Initialized => _luaEnv != null;
    /// <summary>
    /// 用于存放lua脚本路径
    /// </summary>
    /// <typeparam name="string"></typeparam>
    /// <returns></returns>
    private readonly List<string> _loadedFiles = new List<string>();

    /// <summary>
    /// 初始化lua环境
    /// </summary>
    public void InitLuaEnv()
    {
        if (_luaEnv != null) return;
        _luaEnv = new();
        _luaEnv.AddLoader(CustomLoader);
        //注册lua基础组件
        //     _luaEnv.DoString(@"
        //     -- 设置包路径
        //     package.path = package.path .. ';Assets/XLua/Resources/?.lua.txt'

        //     -- 加载 xLua 核心
        //     require 'xlua'
        // ");
        //注册常用c#类
        // _luaEnv.DoString(@"
        // CS = CS or {}
        // CS.UnityEngine = UnityEngine
        // CS.GameManager = GameManager.Instance
        // CS.TimerManager = TimerManager"
        // );

        //加载主入口脚本
        DoLuaFile("Main");
        _luaGlobal = _luaEnv.Global;
    }

    /// <summary>
    /// 加载自定义路径
    /// </summary>
    /// <param name="filepath"></param>
    /// <returns></returns>
    private byte[] CustomLoader(ref string filepath)
    {
        // 处理 xLua 核心文件
        if (filepath == "xlua")
        {
            // 从 xLua 插件资源加载
            var xluaCore = Resources.Load<TextAsset>("XLua/xlua");
            if (xluaCore != null)
            {
                if (!_loadedFiles.Contains(filepath))
                    _loadedFiles.Add(filepath);
                return xluaCore.bytes;
            }
        }
        //转换路径模式：a,b,c->a/b/c.lua
        string path = filepath.Replace('.', '/');
        string fullPath = $"{LUA_SCRIPT_PATH}{path}";

        // 尝试加载 .lua 文件
        var luaFile = Resources.Load<TextAsset>(fullPath);
        if (luaFile != null)
        {
            if (!_loadedFiles.Contains(filepath))
                _loadedFiles.Add(filepath);
            return luaFile.bytes;
        }

        // 尝试加载 .lua.txt 文件（xLua 标准格式）
        var luaTxtFile = Resources.Load<TextAsset>($"{fullPath}.txt");
        if (luaTxtFile != null)
        {
            if (!_loadedFiles.Contains(filepath))
                _loadedFiles.Add(filepath);
            return luaTxtFile.bytes;
        }
        Debug.LogError("未找到文件路径");
        return null;
    }

    /// <summary>
    /// 执行lua脚本
    /// </summary>
    /// <param name="script"></param>
    public void Dostring(string script)
    {
        if (_luaEnv == null) return;
        _luaEnv.DoString(script);
    }

    /// <summary>
    /// 执行lua头文件
    /// </summary>
    /// <param name="LuaScripts"></param>
    private void DoLuaFile(string LuaScripts)
    {
        if (_luaEnv == null) return;
        _luaEnv.DoString($"require '{LuaScripts}'");
    }
    /// <summary>
    /// 调用lua方法
    /// </summary>
    /// <param name="funcName"></param>
    /// <param name="args"></param>
    public void CallLuaFunction(string funcName, params object[] args)
    {
        if (_luaGlobal == null) return;

        var luaFunc = _luaGlobal.Get<Action<object[]>>(funcName);
        if (luaFunc != null)
            luaFunc(args);
        else
            Debug.LogWarning($"Lua方法{funcName}没有找到");
    }
    /// <summary>
    /// 获得lua全局变量
    /// </summary>
    /// <param name="GlobalName"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public T GetGlobal<T>(string GlobalName)
    {
        return _luaGlobal.Get<T>(GlobalName);
    }
    /// <summary>
    /// 注册lua方法
    /// </summary>
    /// <param name="luaFuncName"></param>
    /// <param name="callback"></param>
    public void RegisterFunction(string luaFuncName, Delegate callback)
    {
        if (_luaGlobal == null) return;
        _luaGlobal.Set(luaFuncName, callback);
    }
    /// <summary>
    /// 获得加载后的文件
    /// </summary>
    /// <returns></returns>
    public List<string> GetLoadedFiles()
    {
        return new List<string>(_loadedFiles);
    }
    /// <summary>
    /// 存储全局表变量
    /// 在try里面添加所有全局变量
    /// </summary>
    /// <returns></returns>
    public Dictionary<string, object> GetGlobalVariables()
    {
        Dictionary<string, object> globals = new();
        if (_luaGlobal == null) return globals;
        try
        {

        }
        catch (System.Exception ex)
        {
            Debug.LogError("Failed to get Lua globals: " + ex.Message);
        }
        return globals;
    }
    /// <summary>
    /// lua环境重载
    /// </summary>
    public void Reload()
    {
        if (_luaEnv != null)
        {
            _luaEnv.Dispose();
            _luaEnv = null;
            _loadedFiles.Clear();
        }

        InitLuaEnv();
        Debug.Log("Lua Environment 重载");
    }
    #region 生命周期
    public void OnGameStart()
    {
        //lua环境在GameManager中提前初始化
    }

    public void OnLogin(Action onComplete)
    {
        //加载游戏逻辑lua脚本
        onComplete?.Invoke();
    }

    public void OnLoginOut()
    {
        //清理lua资源
        if (_luaEnv != null)
            _luaEnv.FullGc();
    }
    #endregion
}

