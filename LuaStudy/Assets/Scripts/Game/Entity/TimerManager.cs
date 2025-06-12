using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

/// <summary>
/// 简单的计时器
/// </summary>
public static class TimerManager
{
    /// <summary>
    /// 计时器信息
    /// </summary>
    public class TimerInfo
    {
        public int Id;
        public float EndTime;
        public string CallbackInfo;
    }
    private class Timer
    {
        public int Id;
        public float EndTime;
        public Action CallBack;
        public bool isRepeating;
        public float Interval;
    }
    private static int _nextId = 1;
    private static readonly List<Timer> _timers = new();

    /// <summary>
    /// 添加定时器
    /// </summary>
    /// <param name="delay">延迟</param>
    /// <param name="repeat">是否重复执行</param>
    /// <param name="callback">回调</param>
    /// <returns></returns>
    public static int AddTimer(float delay, bool repeat = false, Action callback = null)
    {
        var timer = new Timer
        {
            Id = _nextId++,
            EndTime = Time.time + delay,
            CallBack = callback,
            isRepeating = repeat,
            Interval = delay
        };
        _timers.Add(timer);
        return timer.Id;
    }
    /// <summary>
    /// 添加延迟回调
    /// </summary>
    /// <param name="delay"></param>
    /// <param name="callback"></param>
    /// <returns></returns>
    public static int DelayTimer(float delay, Action callback)
    {
        return AddTimer(delay, false, callback);
    }
    /// <summary>
    /// 添加帧重复回调
    /// </summary>
    /// <param name="frameInterval"></param>
    /// <param name="repeatCount"></param>
    /// <param name="callback"></param>
    /// <returns></returns>
    public static int AddFrameRepeater(int frameInterval, int repeatCount, Action<int> callback)
    {
        // 简化实现
        return AddTimer(frameInterval * Time.deltaTime, repeatCount != 1, () => callback(0));
    }

    /// <summary>
    /// 移除回调
    /// </summary>
    /// <param name="timerId"></param>
    public static void RemoveTimer(int timerId)
    {
        _timers.RemoveAll(t => t.Id == timerId);
    }

    /// <summary>
    /// 计时器update
    /// </summary>
    public static void Update()
    {
        float currentTime = Time.time;

        for (int i = _timers.Count - 1; i >= 0; i--)
        {
            var timer = _timers[i];
            var temp = i;
            if (currentTime >= timer.EndTime)
            {
                try
                {
                    timer.CallBack?.Invoke();
                }
                catch (Exception ex)
                {
                    Debug.LogError($"Timer callback error: {ex}");
                }

                if (timer.isRepeating)
                {
                    timer.EndTime = currentTime + timer.Interval;
                }
                else
                {
                    if (_timers.Count > 0 && _timers.Count > temp)
                    {
                        _timers.RemoveAt(temp);
                    }
                }
            }
        }
    }
    /// <summary>
    /// 获取活动计时器
    /// </summary>
    public static List<TimerInfo> GetActiveTimers()
    {
        var timerInfos = new List<TimerInfo>();

        foreach (var timer in _timers)
        {
            string callbackInfo = "No callback";
            if (timer.CallBack != null)
            {
                var method = timer.CallBack.Method;
                callbackInfo = $"{method.DeclaringType?.Name}.{method.Name}";
            }

            timerInfos.Add(new TimerInfo
            {
                Id = timer.Id,
                EndTime = timer.EndTime,
                CallbackInfo = callbackInfo
            });
        }

        return timerInfos;
    }
}
