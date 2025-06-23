using UnityEngine;
using System;
using System.Collections.Generic;

namespace UIFramework
{
    /// <summary>
    /// 这个layer层控制所有的窗口
    /// 有显示记录和队列的，并且一次只显示一个
    /// </summary>
    public class WindowUILayer : UILayer<IWindowController>
    {

        [SerializeField]
        private WindowParaLayer priorityParaLayer = null;

        public IWindowController CurrentWindow { get; private set; }

        private Queue<WindowHistoryEntry> windowQueue;
        private Stack<WindowHistoryEntry> windowHistory;

        public event Action RequestScreenBlock;
        public event Action RequestScreenUnblock;

        private HashSet<IScreenController> screensTransitioning;
        private bool IsScreenTransitionInProgress
        {
            get
            {
                return screensTransitioning.Count != 0;
            }
        }
        public override void Initialize()
        {
            base.Initialize();
            registeredScreens = new();
            windowQueue = new();
            windowHistory = new();
            screensTransitioning = new();
        }

        protected override void ProcessScreenRegister(string screenId, IWindowController controller)
        {
            base.ProcessScreenRegister(screenId, controller);
            controller.InTransitionFinished += OnAnimationFinished;
            controller.OutTransitionFinished += OnOutAnimationFinished;
            controller.CloseRequest += OnCloseRequestedByWindow;
        }

        protected override void ProcessScreenUnRegister(string screenId, IWindowController controller)
        {
            base.ProcessScreenUnRegister(screenId, controller);
            controller.InTransitionFinished -= OnAnimationFinished;
            controller.OutTransitionFinished -= OnOutAnimationFinished;
            controller.CloseRequest -= OnCloseRequestedByWindow;
        }

        /// <summary>
        /// 显示窗口
        /// </summary>
        /// <param name="screen"></param>
        public override void ShowScreen(IWindowController screen)
        {
            ShowScreen<IWindowProperties>(screen, null);
        }
        /// <summary>
        /// 显示窗口，带点参数
        /// </summary>
        /// <param name="screen"></param>
        /// <param name="properties"></param>
        /// <typeparam name="TProps"></typeparam>
        public override void ShowScreen<TProps>(IWindowController screen, TProps properties)
        {
            IWindowProperties windowProp = properties as IWindowProperties;
            if (ShouldEnqueue(screen, windowProp))
                EnqueueWindow(screen, properties);
            else
                DoShow(screen, windowProp);
        }
        public override void HideScreen(IWindowController screen)
        {
            if (screen == CurrentWindow)
            {
                windowHistory.Pop();
                AddTransition(screen);
                screen.Hide();

                CurrentWindow = null;

                if (windowQueue.Count > 0)
                    ShowNextInQueue();
                else
                    ShowPreviousInHistory();
            }
            else
            {
                Debug.LogError(
        string.Format(
            "[WindowUILayer] Hide requested on WindowId {0} but that's not the currently open one ({1})! Ignoring request.",
            screen.ScreenId, CurrentWindow != null ? CurrentWindow.ScreenId : "current is null"));
            }
        }
        public override void HideAll(bool shouldAnimateWhenHiding = true)
        {
            base.HideAll(shouldAnimateWhenHiding);
            CurrentWindow = null;
            priorityParaLayer.RefreshDarken();
            windowHistory.Clear();
        }

        public override void ReparentScreen(IScreenController controller, Transform screenTransform)
        {
            IWindowController window = controller as IWindowController;
            if (window == null)
                Debug.LogError("[WindowUILayer] Screen " + screenTransform.name + " is not a Window!");
            else
            {
                if (window.IsPopup)
                {
                    priorityParaLayer.AddScreen(screenTransform);
                    return;
                }
            }
            base.ReparentScreen(controller, screenTransform);
        }

        /// <summary>
        /// 窗口入队列
        /// </summary>
        /// <param name="screen"></param>
        /// <param name="properties"></param>
        /// <typeparam name="TProp"></typeparam>
        private void EnqueueWindow<TProp>(IWindowController screen, TProp properties) where TProp : IScreenProperties
        {
            windowQueue.Enqueue(new WindowHistoryEntry(screen, (IWindowProperties)properties));
        }
        /// <summary>
        /// 是否进入窗口队列
        /// </summary>
        /// <param name="controller"></param>
        /// <param name="windowProp"></param>
        /// <returns></returns>
        private bool ShouldEnqueue(IWindowController controller, IWindowProperties windowProp)
        {
            if (CurrentWindow == null && windowQueue.Count == 0)
                return false;
            if (windowProp != null && windowProp.SuppressPrefabProperties)
                return windowProp.WindowQueuePriority != WindowPriority.ForceForeground;
            if (controller.WindowPriority != WindowPriority.ForceForeground)
                return true;
            return false;
        }
        /// <summary>
        /// 显示前一个窗口
        /// </summary>
        private void ShowPreviousInHistory()
        {
            if (windowHistory.Count > 0)
            {
                WindowHistoryEntry window = windowHistory.Pop();
                DoShow(window);
            }
        }
        /// <summary>
        /// 显示下一个窗口队列
        /// </summary>
        private void ShowNextInQueue()
        {
            if (windowQueue.Count > 0)
            {
                WindowHistoryEntry window = windowQueue.Dequeue();
                DoShow(window);
            }
        }

        private void DoShow(IWindowController screen, IWindowProperties properties)
        {
            DoShow(new WindowHistoryEntry(screen, properties));
        }
        private void DoShow(WindowHistoryEntry windowEntry)
        {
            if (CurrentWindow == windowEntry.Screen)
            {
                string.Format(
        "[WindowUILayer] The requested WindowId ({0}) is already open! This will add a duplicate to the " +
        "history and might cause inconsistent behaviour. It is recommended that if you need to open the same" +
        "screen multiple times (eg: when implementing a warning message pop-up), it closes itself upon the player input" +
        "that triggers the continuation of the flow."
        , CurrentWindow.ScreenId);
            }
            else if (CurrentWindow != null
                    && CurrentWindow.HideOnForegroundLost
                    && !windowEntry.Screen.IsPopup)
            {
                CurrentWindow.Hide();
            }

            windowHistory.Push(windowEntry);
            AddTransition(windowEntry.Screen);

            if (windowEntry.Screen.IsPopup)
            {
                priorityParaLayer.DarkenBG();
            }
            windowEntry.Show();
            CurrentWindow = windowEntry.Screen;
        }
        /// <summary>
        /// 淡入动画完成时
        /// </summary>
        /// <param name="screen"></param>
        private void OnAnimationFinished(IScreenController screen)
        {
            RemoveTransitions(screen);
        }
        /// <summary>
        /// 淡出过渡动画完成时
        /// </summary>
        /// <param name="screen"></param>
        private void OnOutAnimationFinished(IScreenController screen)
        {
            RemoveTransitions(screen);
            var window = screen as IWindowController;
            if (window.IsPopup)
                priorityParaLayer.RefreshDarken();
        }
        /// <summary>
        /// 关闭窗口
        /// </summary>
        /// <param name="screen"></param>
        private void OnCloseRequestedByWindow(IScreenController screen)
        {
            HideScreen(screen as IWindowController);
        }
        /// <summary>
        /// 添加过渡效果
        /// </summary>
        /// <param name="screen"></param>
        private void AddTransition(IWindowController screen)
        {
            screensTransitioning.Add(screen);
            if (RequestScreenBlock != null)
                RequestScreenBlock();
        }
        /// <summary>
        /// 去除过渡效果
        /// </summary>
        /// <param name="screen"></param>
        private void RemoveTransitions(IScreenController screen)
        {
            screensTransitioning.Remove(screen);
            if (!IsScreenTransitionInProgress)
            {
                if (RequestScreenUnblock != null)
                    RequestScreenUnblock();
            }
        }
    }
}
