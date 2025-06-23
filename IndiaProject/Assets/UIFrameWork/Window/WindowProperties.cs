using UnityEngine;

namespace UIFramework
{
    [System.Serializable]
    public class WindowProperties : IWindowProperties
    {
        [SerializeField]
        protected bool hideOnForegroundLost = true;
        [SerializeField]
        protected WindowPriority windowQueuePriority = WindowPriority.ForceForeground;
        [SerializeField]
        protected bool isPopup = false;

        public WindowProperties()
        {
            hideOnForegroundLost = true;
            windowQueuePriority = WindowPriority.ForceForeground;
            isPopup = false;
        }
        /// <summary>
        /// 如果另一个窗口已经打开，此窗口应该如何表现？ 
        /// </summary>
        /// <value>Force Foreground 会立即打开它，Enqueue 会将它排队，以便在当前窗口关闭后立即打开。</value>
        public WindowPriority WindowQueuePriority { get => windowQueuePriority; set => windowQueuePriority = value; }
        /// <summary>
        /// 当其他窗口被之前的时候，自己是否隐藏
        /// </summary>
        /// <value></value>
        public bool HideOnForegroundLost { get => hideOnForegroundLost; set => hideOnForegroundLost = value; }
        /// <summary>
        /// 当在Open()调用传递
        /// </summary>
        /// <value></value>
        public bool SuppressPrefabProperties { get; set; }
        /// <summary>
        /// 弹出窗口在它们后面显示一个黑色背景，并在所有其他窗口的前面显示
        /// </summary>
        /// <value></value>
        public bool IsPopup { get => isPopup; set => isPopup = value; }

        public WindowProperties(bool suppressPrefabProperties = false)
        {
            WindowQueuePriority = WindowPriority.ForceForeground;
            HideOnForegroundLost = false;
            SuppressPrefabProperties = suppressPrefabProperties;
        }

        public WindowProperties(WindowPriority priority, bool hideOnForegroundLost = false, bool suppressPrefabProperties = false)
        {
            WindowQueuePriority = priority;
            HideOnForegroundLost = hideOnForegroundLost;
            SuppressPrefabProperties = suppressPrefabProperties;
        }
    }
}
