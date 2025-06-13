using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;


public class FadeAni : AniComponment
{
    [SerializeField]
    /// <summary>
    /// 隐去延迟
    /// </summary>
    private float fadeDuration = 0.5f;
    [SerializeField]
    /// <summary>
    /// 是否进行隐去
    /// </summary>
    private bool fadeOut = false;

    private CanvasGroup canvasGroups;
    private float timer;
    private Action currentActrion;
    private Transform currentTarget;
    private float startValue;
    private float endValue;

    private bool shouldAnimate;
    public override void Animate(Transform target, Action callWhenFinished)
    {
        
    }
}
