using System;
using System.ComponentModel;

namespace HwndExplorer.Utilities
{
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class ProcessModel
    {
        private readonly Win32Window _window;

        public ProcessModel(Win32Window window)
        {
            ArgumentNullException.ThrowIfNull(window);
            _window = window;
        }

        [DisplayName("Handle Count")]
        public int? HandleCount => _window.Process?.HandleCount;

        [DisplayName("Working Set")]
        public long? WorkingSet => _window.Process?.WorkingSet64;

        [DisplayName("Virtual Memory Size")]
        public long? VirtualMemorySize => _window.Process?.VirtualMemorySize64;

        [DisplayName("Start Time")]
        public DateTime? StartTime => _window.Process?.StartTime;

        public bool? Responding => _window.Process?.Responding;

        [DisplayName("File Name")]
        public string? FileName => _window.Process?.MainModule?.FileName;

        [DisplayName("File Version")]
        public string? FileVersion => _window.Process?.MainModule?.FileVersionInfo?.FileVersion;

        [DisplayName("Product Version")]
        public string? ProductVersion => _window.Process?.MainModule?.FileVersionInfo?.ProductVersion;

        public override string ToString() => FileName ?? _window.Process?.ProcessName ?? string.Empty;
    }
}
