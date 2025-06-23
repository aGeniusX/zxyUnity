using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

namespace UIFramework
{
    public class FadeAni : AniComponment
    {
        [SerializeField]
        /// <summary>
        /// 隐去延迟
        /// </summary>
        private float fadeDuration = 0.5f;
        [SerializeField]
        /// <summary>
        /// 是否进行隐去,关闭窗口之类的
        /// </summary>
        private bool fadeOut = false;

        private CanvasGroup canvasGroups;
        private float timer;
        /// <summary>
        /// 当前行为
        /// </summary>
        private Action currentAction;
        private Transform currentTarget;
        /// <summary>
        /// 透明度开始值
        /// </summary>
        private float startValue;
        /// <summary>
        /// 透明度结束值
        /// </summary>
        private float endValue;

        private bool shouldAnimate;
        public override void Animate(Transform target, Action callWhenFinished)
        {
            if (currentAction != null)
            {
                canvasGroups.alpha = endValue;
                currentAction();
            }

            canvasGroups = target.GetComponent<CanvasGroup>();
            if (canvasGroups == null)
            {
                canvasGroups = target.gameObject.AddComponent<CanvasGroup>();
            }

            if (fadeOut)
            {
                startValue = 1f;
                endValue = 0;
            }
            else
            {
                startValue = 0f;
                endValue = 1f;
            }

            currentAction = callWhenFinished;
            timer = fadeDuration;

            canvasGroups.alpha = startValue;
            shouldAnimate = true;
        }

        void Update()
        {
            if (!shouldAnimate)
                return;
            if (timer > 0f)
            {
                timer -= Time.deltaTime;
                canvasGroups.alpha = Mathf.Lerp(endValue, startValue, timer / fadeDuration);
            }
            else
            {
                canvasGroups.alpha = 1f;
                if (currentAction != null)
                {
                    currentAction();
                }
                currentAction = null;
                shouldAnimate = true;
            }
        }
    }
}
