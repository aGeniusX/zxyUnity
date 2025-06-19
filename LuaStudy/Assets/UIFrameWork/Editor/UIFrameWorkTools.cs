using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

namespace UIFramework
{
    public static class UIFrameWorkTools
    {
        [MenuItem("Assets/Create/UI/UI Frame in Scene", priority = 2)]
        public static void CreateUIFrameInScene()
        {

        }
        [MenuItem("Assets/Create/UI/UI Frame Prefab", priority = 1)]
        public static void CreateUIFramePrefab()
        {

        }

        private static GameObject CreateUIFrame()
        {
            var uiLayer = LayerMask.NameToLayer("UI");
            var root = new GameObject("UIFrame");
            var camera = new GameObject("UICamera");

            var cam = camera.AddComponent<Camera>();
            cam.clearFlags = CameraClearFlags.Depth;
            cam.cullingMask = LayerMask.GetMask("UI");
            cam.orthographic = true;
            cam.farClipPlane = 25;

            root.AddComponent<UIFrame>();
            var canvas = root.AddComponent<Canvas>();
            root.layer = uiLayer;

            return null;
        }
    }
}
