using System;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using UIFramework;
using UnityEditor;
using UnityEngine;

namespace UIFrameWork.Editor
{
    public class ScreenIdProcessor : AssetPostprocessor
    {
        private const string DefaultUIPrefabFolder = "Assets/Resources/UI";
        private const string DefaultUIIdScriptFolder = "Assets/Scripts/UI";
        private const string ScreenIdScriptName = "ScreenIds";
        private const string ScreenIdScriptNamespace = "ScreenId";

        [MenuItem("Assets/Create/UI/Re-generate UI ScreenIds")]
        public static void RegenerateScreenIdsAndRefresh()
        {
            RegenerateScreenIds(true);
        }

        private static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
        {
            bool shouldRegenerate = false;
            //检查UI预制体文件夹变化
            string uiPrefabFolder = GetUIPrefabFolder();
            foreach (string str in importedAssets)
            {
                if (str.Contains(uiPrefabFolder))
                {
                    shouldRegenerate = true;
                    break;
                }
            }
            if (!shouldRegenerate)
            {
                foreach (string str in deletedAssets)
                {
                    if (str.Contains(DefaultUIPrefabFolder))
                    {
                        shouldRegenerate = true;
                        break;
                    }
                }
            }
            if (!shouldRegenerate)
            {
                for (int i = 0; i < movedAssets.Length; i++)
                {
                    if (movedAssets[i].Contains(DefaultUIPrefabFolder) ||
                        movedFromAssetPaths[i].Contains(DefaultUIPrefabFolder))
                    {
                        shouldRegenerate = true;
                        break;
                    }
                }
            }
            //检查配置文件变化
            if (!shouldRegenerate)
            {
                string settingPath = GetSettingPath();
                foreach (var str in importedAssets)
                {
                    if (str.EndsWith("UIFrameworkSettings.asset")) ;
                    shouldRegenerate = true;
                    break;
                }
            }
            if (shouldRegenerate)
                RegenerateScreenIds(true);
        }



        private static void RegenerateScreenIds(bool refreshAssetDatabase)
        {
            string uiPrefabFolder = GetUIPrefabFolder();
            string uiIdScriptsFolder = GetUIIdScriptsFolder();

            Debug.Log($"Generating ScreenIds Using prefab folder:{uiPrefabFolder}");
            Debug.Log($"Generating ScreenIds script at:{uiIdScriptsFolder}");

            Dictionary<string, string> paths = new();

            var assets = AssetDatabase.FindAssets("t:prefab", new[] { uiPrefabFolder });
            foreach (var asset in assets)
            {
                string path = AssetDatabase.GUIDToAssetPath(asset);
                var go = AssetDatabase.LoadAssetAtPath<GameObject>(path);
                var screenController = go.GetComponent<IScreenController>();
                var name = go.name.Replace(" ", string.Empty);
                if (screenController != null)
                {
                    if (paths.ContainsKey(name))
                    {
                        Debug.LogError(string.Format("多个ui预制体用了{0}这个名字，路径在{1},当前路径{2}", name, paths[name], path));
                    }
                    else
                    {
                        paths.Add(name, path);
                        Debug.Log(string.Format("在{0}路径下,注册了{1}", path, name));
                    }
                }
            }

            var scripts = AssetDatabase.FindAssets(string.Format("t:script {0}", ScreenIdScriptName), new[] { uiIdScriptsFolder });
            if (scripts.Length > 0)
            {
                string filePath = AssetDatabase.GUIDToAssetPath(scripts[0]);
                WriteIdClass(paths, filePath);
                if (refreshAssetDatabase)
                    AssetDatabase.Refresh();
            }
            else
            {
                Debug.LogError($"Could not find {ScreenIdScriptName} script in {uiIdScriptsFolder}! Create the file and try again.");
            }
        }

        private static void WriteIdClass(Dictionary<string, string> idPaths, string filePath)
        {
            var targetUnit = new CodeCompileUnit();
            var codeNamespace = new CodeNamespace(ScreenIdScriptNamespace);
            codeNamespace.Imports.Add(new CodeNamespaceImport("System.Collections.Generic"));

            var targetClass = new CodeTypeDeclaration(ScreenIdScriptName)
            {
                IsClass = true,
                TypeAttributes = TypeAttributes.Public | TypeAttributes.Sealed
            };
            //添加头文件注释
            targetClass.Comments.Add(new CodeCommentStatement("// ------------------------------------------------------------------------------"));
            targetClass.Comments.Add(new CodeCommentStatement("// <auto-generated>"));
            targetClass.Comments.Add(new CodeCommentStatement("//     This code was generated by the UIFramework ScreenIdProcessor."));
            targetClass.Comments.Add(new CodeCommentStatement("//     Changes to this file may cause incorrect behavior and will be lost if"));
            targetClass.Comments.Add(new CodeCommentStatement("//     the code is regenerated."));
            targetClass.Comments.Add(new CodeCommentStatement("// </auto-generated>"));
            targetClass.Comments.Add(new CodeCommentStatement("// ------------------------------------------------------------------------------"));

            codeNamespace.Types.Add(targetClass);
            targetUnit.Namespaces.Add(codeNamespace);

            //1.生成ScreenId常量
            foreach (var idPathPair in idPaths)
            {
                var idField = new CodeMemberField(typeof(string), idPathPair.Key)
                {
                    Attributes = MemberAttributes.Public | MemberAttributes.Const,
                    InitExpression = new CodePrimitiveExpression(idPathPair.Key)
                };
                targetClass.Members.Add(idField);
            }
            // 2. 生成路径字典
            // 创建字典变量
            var pathMapField = new CodeMemberField()
            {
                Name = "PathMap",
                Type = new CodeTypeReference("Dictionary<string,string>"),
                Attributes = MemberAttributes.Public | MemberAttributes.Static,
                InitExpression = new CodeObjectCreateExpression(new CodeTypeReference("Dictionary<string,string>"))
            };
            targetClass.Members.Add(pathMapField);

            //添加静态构造函数初始化字典
            var staticCtor = new CodeConstructor
            {
                Attributes = MemberAttributes.Static | MemberAttributes.Public
            };

            //添加字典初始化代码
            foreach (var kvp in idPaths)
            {
                staticCtor.Statements.Add(new CodeMethodInvokeExpression(
                    new CodeFieldReferenceExpression(
                        new CodeTypeReferenceExpression(ScreenIdScriptName), "PathMap"
                    ), "Add", new CodePrimitiveExpression(kvp.Key),
                    new CodePrimitiveExpression(kvp.Value)
                ));
            }

            targetClass.Members.Add(staticCtor);

            GenerateCSharpCode(targetUnit, filePath);
        }

        private static void GenerateCSharpCode(CodeCompileUnit targetUnit, string fileName)
        {
            var provider = CodeDomProvider.CreateProvider("CSharp");
            var options = new CodeGeneratorOptions()
            {
                BracingStyle = "C",
                BlankLinesBetweenMembers = true,
                VerbatimOrder = true
            };
            //确保目录存在
            string directory = Path.GetDirectoryName(fileName);
            if (!Directory.Exists(directory))
                Directory.CreateDirectory(directory);
            using (var sourceWriter = new StreamWriter(fileName))
            {
                provider.GenerateCodeFromCompileUnit(targetUnit, sourceWriter, options);
            }
        }

        #region 配置文件处理

        private const string SettingsPath = "Assets/UIFramework/Editor/Resources/UIFrameworkSettings.asset";
        private const string UISettingPath = "Assets/Resources/UI/UISettings.asset";

        [MenuItem("UIFramework/Configure Paths", priority = 1)]
        public static void OpenSettings()
        {
            var settings = GetOrCreateSettings();
            Selection.activeObject = settings;
        }

        [MenuItem("UIFramework/Open UI Setting", priority = 1)]
        private static void OpenUISetting()
        {
            var settings = AssetDatabase.LoadAssetAtPath<UISetting>(UISettingPath);
            if (settings == null)
            {
                UIFrameWorkTools.CreateUIFramePrefab();
                return;
            }
            else
            {
                Selection.activeObject = settings;
            }
        }
        private static string GetSettingPath()
        {
            return SettingsPath;
        }

        private static UIFrameworkSettings GetOrCreateSettings()
        {
            var settings = AssetDatabase.LoadAssetAtPath<UIFrameworkSettings>(SettingsPath);
            if (settings == null)
            {
                //确保目录存在
                string directory = Path.GetDirectoryName(SettingsPath);
                if (!Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }
                settings = ScriptableObject.CreateInstance<UIFrameworkSettings>();
                settings.uiPrefabFolder = DefaultUIPrefabFolder;
                settings.uiIdScriptFolder = DefaultUIIdScriptFolder;

                AssetDatabase.CreateAsset(settings, SettingsPath);
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();

                Debug.Log("Created new UIFrameworkSettings at: " + SettingsPath);
            }
            return settings;
        }

        private static string GetUIPrefabFolder()
        {
            var settings = GetOrCreateSettings();
            return !string.IsNullOrEmpty(settings.uiPrefabFolder) ?
                settings.uiPrefabFolder : DefaultUIPrefabFolder;
        }

        private static string GetUIIdScriptsFolder()
        {
            var settings = GetOrCreateSettings();
            return !string.IsNullOrEmpty(settings.uiIdScriptFolder) ?
                settings.uiIdScriptFolder : DefaultUIIdScriptFolder;
        }

        #endregion
    }
    public class UIFrameworkSettings : ScriptableObject
    {
        [Tooltip("包含UI预制体的文件夹路径")]
        public string uiPrefabFolder;
        [Tooltip("ScreenIds脚本的保存路径")]
        public string uiIdScriptFolder;
    }
}