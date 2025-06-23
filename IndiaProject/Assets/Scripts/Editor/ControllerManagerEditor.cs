using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UnityEditor;
using UnityEngine;

#if UNITY_EDITOR
[CustomEditor(typeof(GameManager))]
/// <summary>
/// GameManager可视化编辑器
/// </summary>
public class ControllerManagerEditor : Editor
{
    private GameManager _gameManager;
    /// <summary>
    /// 显示控制器
    /// </summary>
    private bool _showControllers = true;
    /// <summary>
    /// 显示lua环境
    /// </summary>
    private bool _showLuaEnvironment = true;

    private bool _showTimer = true;
    private Vector2 _scrollPosition;
    void OnEnable()
    {
        _gameManager = (GameManager)target;
    }

    public override void OnInspectorGUI()
    {
        _scrollPosition = EditorGUILayout.BeginScrollView(_scrollPosition);
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("GameManager Dashboard", EditorStyles.boldLabel);
        EditorGUILayout.Separator();

        DrawSystemStatus();
        DrawControllerButtons();

        _showControllers = EditorGUILayout.Foldout(_showControllers, "Controllers", true);
        if (_showControllers)
        {
            DrawControllerSection();
        }
        _showLuaEnvironment = EditorGUILayout.Foldout(_showLuaEnvironment, "Lua Environment", true);
        if (_showLuaEnvironment)
        {
            DrawLuaSection();
        }

        if (_showTimer)
        {
            DrawTimerSection();
        }

        EditorGUILayout.EndScrollView();
    }
    /// <summary>
    /// 绘制系统状态
    /// </summary>
    private void DrawSystemStatus()
    {
        EditorGUILayout.BeginVertical("box");
        //初始化状态
        string initStatus = _gameManager.IsInitializing ? "Initializing..." : _gameManager.AreAllControllersInitialized ? "Initialized" :
        "Not Initialized";
        Color statusColor = _gameManager.AreAllControllersInitialized ? Color.green : _gameManager.IsInitializing ?
        Color.yellow : Color.red;

        EditorGUILayout.LabelField("System Status", EditorStyles.boldLabel);
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Initalization");
        EditorGUILayout.LabelField(initStatus, GetStatusStyle(statusColor));
        EditorGUILayout.EndHorizontal();

        //lua状态
        bool luaReady = _gameManager.IsLuaInitialized;
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Lua Environment");
        EditorGUILayout.LabelField(luaReady ? "Ready" : "Not Ready", GetStatusStyle(luaReady ? Color.green : Color.red));
        EditorGUILayout.EndHorizontal();

        //控制器统计
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Controllers:");
        EditorGUILayout.LabelField($"{_gameManager.InitializedControllerCount}/{_gameManager.TotalControllerCount} Initialized");
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.EndVertical();
    }
    /// <summary>
    /// 绘制按钮状态
    /// </summary>
    private void DrawControllerButtons()
    {
        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Initialize Controllers"))
        {
            _gameManager.InitializeControllers(() =>
            {
                Debug.Log("Initialization Complete From Editor");
            });
        }

        if (GUILayout.Button("Reload Lua"))
        {
            _gameManager.ReloadLuaEnvironment();
        }

        if (GUILayout.Button("Logout"))
        {
            _gameManager.OnLogout();
        }
        EditorGUILayout.EndHorizontal();
    }
    /// <summary>
    /// 绘制控制器选择
    /// </summary>
    private void DrawControllerSection()
    {
        EditorGUILayout.BeginVertical("box");
        if (_gameManager.Controllers.Count == 0)
        {
            EditorGUILayout.LabelField("No Controller Register");
            EditorGUILayout.EndVertical();
            return;
        }
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Controller", EditorStyles.boldLabel, GUILayout.Width(200));
        EditorGUILayout.LabelField("Status", EditorStyles.boldLabel, GUILayout.Width(100));
        EditorGUILayout.LabelField("Dependencies", EditorStyles.boldLabel);
        EditorGUILayout.EndHorizontal();
        foreach (var controller in _gameManager.Controllers)
        {
            var type = controller.GetType();
            var state = _gameManager.GetControllerState(type);
            var dependencies = GetControllerDependencies(type);

            EditorGUILayout.BeginHorizontal();

            //控制器名称
            EditorGUILayout.LabelField(type.Name, GUILayout.Width(200));
            //状态
            EditorGUILayout.LabelField(state.ToString(), GetStateStyle(state), GUILayout.Width(100));

            //依赖关系
            if (dependencies.Count > 0)
            {
                EditorGUILayout.LabelField(string.Join(",", dependencies.Select(d => d.Name)));
            }
            else
            {
                EditorGUILayout.LabelField("None");
            }
            EditorGUILayout.EndHorizontal();
        }
        EditorGUILayout.EndVertical();
    }
    /// <summary>
    /// 绘制lua选择
    /// </summary>
    private void DrawLuaSection()
    {
        EditorGUILayout.BeginVertical("box");

        //lua环境状态
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Lua Initialized:", GUILayout.Width(150));
        EditorGUILayout.Toggle(_gameManager.IsLuaInitialized);
        EditorGUILayout.EndHorizontal();

        //加载lua文件
        EditorGUILayout.LabelField("Loaded Lua Files:", EditorStyles.boldLabel);
        var loadFiles = _gameManager.GetLoadedLuaFiles();
        if (loadFiles.Count == 0)
        {
            EditorGUILayout.LabelField("No Lua Files loaded");
        }
        else
        {
            foreach (var file in loadFiles)
            {
                EditorGUILayout.LabelField($"-{file}");
            }
        }

        //Lua全局变量
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Global Variables:", EditorStyles.boldLabel);

        var globals = _gameManager.GetLuaGlobals();
        if (globals.Count == 0)
        {
            EditorGUILayout.LabelField("No Global variables");
        }
        else
        {
            foreach (var global in globals)
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField(global.Key, GUILayout.Width(150));
                EditorGUILayout.LabelField(global.Value?.ToString() ?? "null");
                EditorGUILayout.EndHorizontal();
            }
        }
        EditorGUILayout.EndVertical();
    }
    /// <summary>
    /// 绘制定时器选择
    /// </summary>
    private void DrawTimerSection()
    {
        EditorGUILayout.BeginVertical("box");

        var timers = TimerManager.GetActiveTimers();
        if (timers.Count == 0)
        {
            EditorGUILayout.LabelField("No Active timer");
            EditorGUILayout.EndVertical();
            return;
        }

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("ID", EditorStyles.boldLabel, GUILayout.Width(50));
        EditorGUILayout.LabelField("Time Left", EditorStyles.boldLabel, GUILayout.Width(80));
        EditorGUILayout.LabelField("Callback", EditorStyles.boldLabel);

        foreach (var timer in timers)
        {
            float timeLeft = Mathf.Max(0, timer.EndTime - Time.time);

            EditorGUILayout.BeginHorizontal();

            //ID
            EditorGUILayout.LabelField(timer.Id.ToString(), GUILayout.Width(50));

            //剩余时间
            EditorGUILayout.LabelField($"{timeLeft:F2}s", GUILayout.Width(80));

            //回调信息

            EditorGUILayout.LabelField(timer.CallbackInfo);

            EditorGUILayout.EndHorizontal();
        }
    }

    /// <summary>
    /// 获得控制器依赖
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    private List<Type> GetControllerDependencies(Type controllerType)
    {
        List<Type> dependencies = new();
        var dependsAttributes = controllerType.GetCustomAttributes(typeof(DependsOnAttribute), true).Cast<DependsOnAttribute>();

        foreach (var attr in dependsAttributes)
        {
            dependencies.AddRange(attr.DependencyTypes);
        }

        return dependencies.Distinct().ToList();
    }

    /// <summary>
    /// 状态风格
    /// </summary>
    /// <param name="color"></param>
    /// <returns></returns>
    private GUIStyle GetStatusStyle(Color color)
    {
        var style = new GUIStyle(EditorStyles.label)
        {
            normal = { textColor = color },
            fontStyle = FontStyle.Bold
        };
        return style;
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="state"></param>
    /// <returns></returns>
    private GUIStyle GetStateStyle(ControllerState state)
    {
        var style = new GUIStyle(EditorStyles.label);

        switch (state)
        {
            case ControllerState.Initialized:
                style.normal.textColor = Color.green;
                break;
            case ControllerState.Initializing:
                style.normal.textColor = Color.yellow;
                break;
            case ControllerState.Failed:
                style.normal.textColor = Color.red;
                style.fontStyle = FontStyle.Bold;
                break;
            case ControllerState.UnInitialized:
                style.normal.textColor = Color.gray;
                break;
            case ControllerState.Queued:
                style.normal.textColor = new Color(1f, 0.5f, 0f); // 橙色
                break;
        }
        return style;
    }
}
#endif
