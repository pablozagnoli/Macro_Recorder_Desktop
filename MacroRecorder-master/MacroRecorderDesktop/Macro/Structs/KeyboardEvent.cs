using System;
using System.Runtime.InteropServices;


namespace MacroRecorder.Macro.Structs
{
    [StructLayout(LayoutKind.Sequential)]
    public struct KeyboardEvent
    {
        public uint vKeyCode;
        public uint scanCode;
        public uint Flags;
        public uint Time;
        public IntPtr dwExtraInfo;
    }
}
