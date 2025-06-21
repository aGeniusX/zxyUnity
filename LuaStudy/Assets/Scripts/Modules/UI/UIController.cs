using System;
using System.Collections;
using System.Collections.Generic;
using UIFramework;
using UnityEngine;
using ScreenId;

public class UIController : MonoBehaviour, IController
{
    private UIFrame uIFrame;
    private UISetting uISetting = null;
    public void OnGameStart()
    {
        if (uISetting == null)
        {
            // 尝试加载默认路径的 UISettings
            uISetting = Resources.Load<UISetting>("UISettings");
            // 如果还找不到，尝试在更通用的路径查找
            if (uISetting == null)
            {
                var settings = Resources.FindObjectsOfTypeAll<UISetting>();
                if (settings.Length > 0)
                {
                    uISetting = settings[0];
                }
            }
            // 如果仍然找不到，记录错误
            if (uISetting == null)
            {
                Debug.LogError("Failed to find UISettings asset! Please create one or assign manually.");
                return;
            }
            else
            {
                Debug.Log("Auto-loaded UISettings: " + uISetting.name);
            }
        }

        uIFrame = uISetting.CreateUIInstance();

        if (uIFrame == null)
        {
            Debug.LogError("Failed to create UIFrame instance!");
        }
        DontDestroyOnLoad(uIFrame);
        uIFrame.ShowPanel(ScreenIds.testNameFile);
    }

    public void OnLogin(Action onComplete)
    {
        onComplete?.Invoke();
    }

    public void OnLoginOut()
    {

    }
}
