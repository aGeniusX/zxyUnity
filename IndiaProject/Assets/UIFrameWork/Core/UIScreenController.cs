using System;
using UnityEngine;

namespace UIFramework
{


    /// <summary>
    /// UI界面的基类，窗口，面板这些都继承它
    /// </summary>
    /// <typeparam name="TProps"></typeparam>
    public abstract class UIScreenController<TProps> : MonoBehaviour, IScreenController where TProps : IScreenProperties
    {
        [Header("Screen Animations")]
        [Tooltip("界面显示的动画")]
        [SerializeField]
        /// <summary>
        /// 界面显示的动画
        /// </summary>
        private AniComponment animIn;
        [Tooltip("界面隐藏的动画")]
        [SerializeField]
        /// <summary>
        /// 界面隐藏的动画
        /// </summary>
        private AniComponment animOut;

        [Header("Screen Properties")]
        [Tooltip("界面的属性参数")]
        [SerializeField]
        /// <summary>
        /// 界面的属性参数
        /// </summary>
        private TProps properties;

        /// <summary>
        /// 界面ID
        /// </summary>
        /// <value></value>
        public string ScreenId { get; set; }

        /// <summary>
        /// 动画组件，为了界面有统一的弹出效果
        /// </summary>
        /// <value></value>
        public AniComponment AnimIn
        {
            get { return animIn; }
            set { animIn = value; }
        }
        /// <summary>
        /// 动画组件，为了界面有统一的隐藏效果
        /// </summary>
        /// <value></value>
        public AniComponment AnimOut
        {
            get { return animOut; }
            set { animOut = value; }
        }

        /// <summary>
        /// 界面是否显示中
        /// </summary>
        /// <value></value>
        public bool IsVisible { get; set; }

        /// <summary>
        /// 弹出（渐入）动画完成之后回调
        /// </summary>
        /// <value></value>
        public Action<IScreenController> InTransitionFinished { get; set; }
        /// <summary>
        /// 关闭（渐隐）动画完成之后回调
        /// </summary>
        /// <value></value>
        public Action<IScreenController> OutTransitionFinished { get; set; }
        /// <summary>
        /// 关闭界面的回调
        /// </summary>
        /// <value></value>
        public Action<IScreenController> CloseRequest { get; set; }
        /// <summary>
        /// 界面销毁的回调
        /// </summary>
        /// <value></value>
        public Action<IScreenController> ScreenDestroyed { get; set; }

        public TProps Properties
        {
            get { return properties; }
            set { properties = value; }
        }

        protected virtual void Awake()
        {
            AddListeners();
        }


        protected virtual void OnDestroy()
        {
            if (ScreenDestroyed != null)
            {
                ScreenDestroyed(this);
            }
            InTransitionFinished = null;
            OutTransitionFinished = null;
            CloseRequest = null;
            ScreenDestroyed = null;
            RemoveListeners();
        }

        /// <summary>
        /// 添加监听事件，awake是注册
        /// </summary>
        protected virtual void AddListeners() { }
        /// <summary>
        /// 移除监听事件，Destroy会自动调
        /// </summary>
        protected virtual void RemoveListeners() { }

        /// <summary>
        /// 属性参数设置到界面的时候触发，在SetProperties之后触发，比较安全的能取到值
        /// </summary>
        protected virtual void OnPropertiesSet() { }
        /// <summary>
        /// 界面隐藏的时候触发，便于处理一些操作
        /// </summary>
        protected virtual void WhileHiding() { }

        /// <summary>
        /// 设置属性参考
        /// </summary>
        /// <param name="props"></param>
        protected virtual void SetProperties(TProps props) { }

        /// <summary>
        /// 在显示的时候处理一些层级，或者属性处理等，具体看继承者重写了
        /// </summary>
        protected virtual void HierarchyFixOnShow() { }

        /// <summary>
        /// 隐藏界面
        /// </summary>
        /// <param name="animate"></param>
        public void Hide(bool animate = true)
        {
            DoAnimation(animate ? animOut : null, OnTransitionOutFinished, false);
        }


        /// <summary>
        /// 显示界面
        /// </summary>
        /// <param name="props"></param>
        public void Show(IScreenProperties props = null)
        {
            if (props != null)
            {
                if (props is TProps)
                    SetProperties((TProps)props);
                else
                {
                    Debug.LogError("Properties passed have wrong type! (" + props.GetType() + " instead of " +
                                       typeof(TProps) + ")");
                    return;
                }
            }
            HierarchyFixOnShow();
            OnPropertiesSet();
            if (!gameObject.activeSelf)
            {
                DoAnimation(animIn, OnTransitionInFinished, true);
            }
        }

        private void OnTransitionInFinished()
        {
            IsVisible = true;

            if (InTransitionFinished != null)
            {
                InTransitionFinished(this);
            }
        }

        /// <summary>
        /// 完成过渡后调用
        /// </summary>
        private void OnTransitionOutFinished()
        {
            IsVisible = false;
            if (OutTransitionFinished != null)
            {
                OutTransitionFinished(this);
            }
        }
        /// <summary>
        /// 播放ui动画
        /// </summary>
        /// <param name="caller"></param>
        /// <param name="callWhenFinished"></param>
        /// <param name="isVisible"></param>
        private void DoAnimation(AniComponment caller, Action callWhenFinished, bool isVisible)
        {
            if (caller == null)
            {
                gameObject.SetActive(isVisible);
                if (callWhenFinished != null)
                    callWhenFinished();
            }
            else
            {
                if (isVisible && gameObject.activeSelf)
                    gameObject.SetActive(true);
                caller.Animate(transform, callWhenFinished);
            }
        }
    }
}

