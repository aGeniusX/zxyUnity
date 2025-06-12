using System;
using UnityEngine;

/// <summary>
/// UI界面的基类，窗口，面板这些都继承它
/// </summary>
/// <typeparam name="TProps"></typeparam>
public abstract class UIScreenController<TProps> : MonoBehaviour, IScreenController where TProps : IScreenProperties
{
    public string ScreenId { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

    public bool IsVisible => throw new NotImplementedException();

    public Action<IScreenController> InTransitionFinished { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    public Action<IScreenController> OutTransitionFinished { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    public Action<IScreenController> CloseRequest { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    public Action<IScreenController> ScreenDestroyed { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

    public void Hide(bool animate = true)
    {
        throw new NotImplementedException();
    }

    public void Show(IScreenProperties props = null)
    {
        throw new NotImplementedException();
    }
}
