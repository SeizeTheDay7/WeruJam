using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(MonoBehaviour), true)]
[CanEditMultipleObjects]
public sealed class ButtonsEditor : Editor
{
    private sealed class ButtonMethod
    {
        public MethodInfo Method;
        public GUIContent Label;
    }

    private static readonly Dictionary<Type, ButtonMethod[]> Cache = new();

    private static ButtonMethod[] GetButtons(Type type)
    {
        if (Cache.TryGetValue(type, out var cached))
            return cached;

        const BindingFlags flags =
            BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;

        var methods = type.GetMethods(flags)
            .Where(m =>
                m.ReturnType == typeof(void) &&
                m.GetParameters().Length == 0 &&
                m.GetCustomAttribute<ButtonAttribute>(true) != null)
            .Select(m =>
            {
                var attr = m.GetCustomAttribute<ButtonAttribute>(true);
                var label = string.IsNullOrWhiteSpace(attr.Label)
                    ? ObjectNames.NicifyVariableName(m.Name)
                    : attr.Label;

                return new ButtonMethod
                {
                    Method = m,
                    Label = new GUIContent(label)
                };
            })
            .ToArray();

        Cache[type] = methods;
        return methods;
    }

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        var buttons = GetButtons(target.GetType());
        if (buttons.Length == 0)
            return;

        EditorGUILayout.Space(8);

        foreach (var button in buttons)
        {
            if (GUILayout.Button(button.Label))
            {
                foreach (var obj in targets)
                {
                    if (obj == null) continue;

                    Undo.RecordObject(obj, button.Label.text);
                    button.Method.Invoke(obj, null);
                    EditorUtility.SetDirty(obj);
                }
            }
        }
    }
}
