using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 界面动画组件
/// </summary>
public abstract class AniComponment : MonoBehaviour
{
    /// <summary>
    /// 动画播放，当播放执行callWhenFinished
    /// </summary>
    /// <param name="target"></param>
    /// <param name="callWhenFinished"></param>
    public abstract void Animate(Transform target, Action callWhenFinished);
}
