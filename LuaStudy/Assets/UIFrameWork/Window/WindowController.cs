using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class WindowController
{

}

public abstract class WindowController<TProps> : UIScreenController<TProps>, IWindowController
    where TProps : IWindowProperties
{
    public bool HideOnForegroundLost => throw new System.NotImplementedException();

    public bool IsPopup => throw new System.NotImplementedException();

    public WindowPriority WindowPriority => throw new System.NotImplementedException();
}
