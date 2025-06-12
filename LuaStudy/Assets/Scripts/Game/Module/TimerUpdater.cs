using System;
using System.Collections.Generic;
using UnityEngine;

[DefaultExecutionOrder(-100)]
public class TimerUpdater : MonoBehaviour
{
    void Update()
    {
        TimerManager.Update();
    }
}
