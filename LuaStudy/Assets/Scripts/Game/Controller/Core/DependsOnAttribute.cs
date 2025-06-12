using UnityEngine;
using System;
/// <summary>
/// 依赖标记特性
/// </summary>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
public class DependsOnAttribute : Attribute
{
    public Type[] DependencyTypes { get; }

    public DependsOnAttribute(params Type[] types)
    {
        DependencyTypes = types;
    }
}