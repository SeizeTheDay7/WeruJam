using System;

[AttributeUsage(AttributeTargets.Method, Inherited = true, AllowMultiple = false)]
public sealed class ButtonAttribute : Attribute
{
    public readonly string Label;

    public ButtonAttribute(string label = null)
    {
        Label = label;
    }
}
