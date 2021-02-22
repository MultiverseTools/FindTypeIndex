// Copyright (c) 2015 Multiverse

// Namespace:   Multiverse.Tools
// Class:       FindTypeIndexWindow
// Author:      Sora
// CreateTime:  2021-02-22-19:06


using System;
using System.Linq;
using Unity.Entities;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Multiverse.Tools
{
    /// <summary>
    /// 查找TypeIndex
    /// </summary>
    public class FindTypeIndexWindow : EditorWindow
    {
        [MenuItem("Multiverse/FindTypeIndex")]
        private static void ShowWindow()
        {
            var window = GetWindow<FindTypeIndexWindow>();
            window.titleContent = new GUIContent("Find Type Index");
            window.Show();
        }

        private void OnEnable()
        {
            SetWindowSize(new Vector2(420, 25));
            GetVisualTree("FindTypeIndexWindow").CloneTree(rootVisualElement);
            var textField                 = rootVisualElement.Q<TextField>("TypeIndex");
            var queryButton               = rootVisualElement.Q<Button>("Query");
            var displayElement            = rootVisualElement.Q("Display");
            var displayNamespaceTextField = rootVisualElement.Q<TextField>("DisplayNamespace");
            var displayClassTextField     = rootVisualElement.Q<TextField>("DisplayClass");

            queryButton.clicked += () =>
            {
                var parseResult = ulong.TryParse(textField.value, out var queryTypeIndex);

                if (!parseResult)
                {
                    Debug.LogError("输入错误");
                    return;
                }

                var findResult = false;

                foreach (var type in AppDomain.CurrentDomain.GetAssemblies().SelectMany(_assembly => _assembly.GetTypes()))
                {
                    var typeHash = TypeHash.CalculateStableTypeHash(type);

                    if (typeHash == queryTypeIndex)
                    {
                        findResult = true;
                        SetWindowSize(new Vector2(420, 45));
                        displayNamespaceTextField.value = type.Namespace;
                        displayClassTextField.value     = type.Name;
                        displayElement.style.display    = DisplayStyle.Flex;

                        break;
                    }
                }

                if (!findResult)
                {
                    var errorMessage = $"找不到\"{textField.value}\"对应的类型";
                    Debug.LogError(errorMessage);
                }
            };

            textField.RegisterValueChangedCallback(_TypeIndexChangeCallback);
            void _TypeIndexChangeCallback(ChangeEvent<string> _evt) { queryButton.SetEnabled(!string.IsNullOrEmpty(_evt.newValue)); }
        }


        private static void SetWindowSize(Vector2 _maxSize)
        {
            var window = GetWindow<FindTypeIndexWindow>();
            window.maxSize = _maxSize;
            window.minSize = _maxSize;
        }

        private VisualTreeAsset GetVisualTree(string _fileName) => Resources.Load<VisualTreeAsset>(_fileName);
    }
}