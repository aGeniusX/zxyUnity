using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

/// <summary>
/// 具体动画实现
/// </summary>
public class AnimationView : AniComponment
{
    [SerializeField]
    /// <summary>
    /// 对面片段
    /// </summary>
    private AnimationClip clip = null;

    [SerializeField]
    /// <summary>
    /// 重复播放
    /// </summary>
    private bool playReverse = false;

    /// <summary>
    /// 播放动画
    /// </summary>
    /// <param name="target"></param>
    /// <param name="callWhenFinished"></param>
    public override void Animate(Transform target, Action callWhenFinished)
    {

    }

    
}
