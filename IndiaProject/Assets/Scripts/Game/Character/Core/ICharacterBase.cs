using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 角色基类
/// </summary>
public interface ICharacterBase
{
    /// <summary>
    /// 角色移动
    /// </summary>
    void Move();
    /// <summary>
    /// 角色交互
    /// </summary>
    /// <param name="pos">交互类所在的坐标</param>
    void Interact(Vector3 pos);
}
