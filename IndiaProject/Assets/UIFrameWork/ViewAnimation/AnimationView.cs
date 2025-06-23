using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

namespace UIFramework
{
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
            FininshPrevious();
            var targetAnimation = target.GetComponent<Animation>();
            if (targetAnimation == null)
            {

            }
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
            FininshPrevious();
        }

        /// <summary>
        /// 结束先前动画
        /// </summary>
        private void FininshPrevious()
        {
            if (previousCallBackWhenFinished != null)
            {
                previousCallBackWhenFinished();
                previousCallBackWhenFinished = null;
            }
            StopAllCoroutines();
        }
    }
}
