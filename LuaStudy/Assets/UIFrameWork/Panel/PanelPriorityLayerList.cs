
using System;
using System.Collections.Generic;
using UnityEngine;

namespace UIFramework
{
    [System.Serializable]
    public class PanelPriorityLayerList
    {
        [SerializeField]
        [Tooltip("根据面板的优先级查找并存储对应的GameObject。渲染优先级由这些GameObject在层级结构中的顺序决定")]
        private List<PanelPriorityLayerListEntry> paraLayer = null;
        private Dictionary<PanelPriority, Transform> lookup;

        public Dictionary<PanelPriority, Transform> ParaLayerLookUp
        {
            get
            {
                if (lookup == null || lookup.Count == 0)
                {
                    CacheLookup();
                }
                return lookup;
            }
        }
        /// <summary>
        /// 初始化面板
        /// </summary>
        private void CacheLookup()
        {
            lookup = new();
            foreach (var item in paraLayer)
            {
                lookup.Add(item.Priority, item.TargetParent);
            }
        }

        public PanelPriorityLayerList(List<PanelPriorityLayerListEntry> entries)
        {
            paraLayer = entries;
        }
    }
}
