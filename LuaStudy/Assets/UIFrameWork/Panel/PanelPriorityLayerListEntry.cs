using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PanelPriorityLayerListEntry
{
    [SerializeField]
    [Tooltip("指定下面板层的优先级")]
    private PanelPriority priority;
    [SerializeField]
    [Tooltip("此优先级下所有的父节点")]
    private Transform targetParent;

    public Transform TargetParent
    {
        get { return targetParent; }
        set => targetParent = value;
    }
    public PanelPriority Priority
    {
        get => priority;
        set => priority = value;
    }

    public PanelPriorityLayerListEntry(PanelPriority prio, Transform parent)
    {
        priority = prio;
        targetParent = parent;
    }
}

