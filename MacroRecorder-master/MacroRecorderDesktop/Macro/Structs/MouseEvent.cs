﻿using System;
using System.Runtime.InteropServices;


namespace MacroRecorder.Macro.Structs
{
    [Flags]
    public enum eMouseCommand
    {
        Absolute = 0x8000,
        HWHeel = 1000,
        Move = 1,
        LDown = 0x2,
        LUp = 0x4,
        RDown = 0x8,
        RUp = 0x10,
        VrtDesk = 0x4000,
        MWheel = 0x800

    }

    public enum eMouseButton
    {
        MOVE = 0x200,
        LDOWN = 0x201,
        LUP = 0x202,
        RDOWN = 0x204,
        RUP = 0x205,
        MWheel = 0x20A,
        MHWheel = 0x20E
    }
    [StructLayout(LayoutKind.Sequential)]
    public struct mPoint
    {
        int _X;
        int _Y;
        public int X 
        { 
            get { return _X; }
            set { _X = value; }
        }
        public int Y 
        { 
            get { return _Y; }
            set { _Y = value; }
        }
        public override string ToString()
        {
            return string.Format("({0},{1})", X, Y);
        }
    }

    public struct MouseEvent
    {
        public mPoint Location;
        public uint mouseData;
        public uint Flags;
        uint Time;
        public IntPtr extraInfo;
    }

}
