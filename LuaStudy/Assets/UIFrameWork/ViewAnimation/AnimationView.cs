using System;
using System.Collections;
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
    /// 之前ui动画片段
    /// </summary>
    private Action previousCallBackWhenFinished;

    /// <summary>
    /// 播放动画
    /// </summary>
    /// <param name="target"></param>
    /// <param name="callWhenFinished"></param>
    public override void Animate(Transform target, Action callWhenFinished)
    {

    }

    private IEnumerator PlayerAnimationRoutine(Animation targetAnimation, Action callWhenFinished)
    {
        previousCallBackWhenFinished = callWhenFinished;
        foreach (AnimationState state in targetAnimation)
        {
            state.time = playReverse ? state.clip.length : 0f;
            state.speed = playReverse ? -1f : 1f;
        }
        targetAnimation.Play(PlayMode.StopAll);
        yield return new WaitForSeconds(targetAnimation.clip.length);
        // FininshPrevious();
    }

}
