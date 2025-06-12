/// <summary>
/// 面板层级
/// </summary>
public enum PanelPriority
{
    None = 0,
    Prioritary = 1,
    Tutorial = 2,
    Blocker = 3,
}

/// <summary>
/// 枚举类型，用于定义窗口在打开时、在历史记录和队列中的行为
/// </summary>
public enum WindowPriority
{
    ForceForeground = 0,
    Enqueue = 1,
}