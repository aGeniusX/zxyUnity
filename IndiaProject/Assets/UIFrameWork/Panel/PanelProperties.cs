using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UIFramework
{
    [System.Serializable]
    public class PanelProperties : IPanelProperties
    {
        [SerializeField]
        [Tooltip("面板根据其优先级进入不同的副层级。可以在“面板层级”中设置副层级。")]
        /// <summary>
        /// 面板根据其优先级进入不同的副层级。可以在“面板层级”中设置副层级
        /// </summary>
        private PanelPriority priority;
        public PanelPriority Priority { get => priority; set => priority = value; }
    }
}
