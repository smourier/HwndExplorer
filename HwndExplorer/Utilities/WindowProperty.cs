using System;

namespace HwndExplorer.Utilities
{
    public sealed class WindowProperty
    {
        public WindowProperty(string name, IntPtr handle)
        {
            ArgumentNullException.ThrowIfNull(name);
            Name = name;
            Handle = handle;
        }

        public string Name { get; }
        public IntPtr Handle { get; }
        public override string ToString() => Name + ": " + Handle;
    }
}
