using System;
using System.Collections.Generic;
using XLua;


public static class GenConfig
{
    [CSharpCallLua]
    public static List<Type> CSharpCallLuaList = new()
    {
        typeof(Func<int,int,int>),
        typeof(Func<int,int,int,int>),
        typeof(Action<string>),
        typeof(Action<object[]>),
    };
}
