﻿using MacroRecorder.Macro.Events;
using MacroRecorder.Macro.Structs;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Threading;


namespace MacroRecorder.Macro
{
    /// <summary>
    /// Made By BahNahNah
    /// uid=2388291
    /// </summary>
    public class MRecorder
    {
        #region " Delegates "

        public delegate void OnEventCallback(IMacroEvent mEvent);

        #endregion

        #region " Events "

        public event OnEventCallback OnEvent;

        #endregion

        #region " Properties "

        public bool RecordingStopped { get { return ThreadExited; } }
        public IMacroEvent[] RecordedEvents { get { return EventList.ToArray(); } }
        public int MouseMovementCaptureDelay { get; set; }
        public bool CaptureMouseMovements { get; set; }

        #endregion

        #region " Fields "

        List<IMacroEvent> EventList = new List<IMacroEvent>();
        Thread threadTask = null;
        DateTime delayBegin = DateTime.Now;
        bool ThreadRunning = false;
        bool ThreadExited = true;
        bool AcceptMouseMovement = false;
        bool PopulateInternalEventList = true;

        bool mouseDelayEnabled = false;

        bool RecordKeyboard = false;
        bool RecordMouse = false;


        HookCallback KBCallback;
        HookCallback MCallback;

        IntPtr KeyboardHook = IntPtr.Zero;
        IntPtr MouseHook = IntPtr.Zero;
        
        #endregion

        #region " Functions "

        public MRecorder()
        {
            KBCallback = new HookCallback(KeyboardEventCallback);
            MCallback = new HookCallback(MouseEventCallback);
            CaptureMouseMovements = true;
            MouseMovementCaptureDelay = 10;
        }

        public MRecorder(bool _populateinternaleventlist) : this()
        {
            PopulateInternalEventList = _populateinternaleventlist;
        }

        public void Remove(IMacroEvent mEvent)
        {
            EventList.Remove(mEvent);
        }
        public void Play()
        {
            Play(RecordedEvents);
        }

        public void Play(IMacroEvent[] events)
        {
            foreach(IMacroEvent e in events)
            {
                e.ExecuteEvent();
            }
        }

        public void StopRecording()
        {
            if (ThreadRunning)
            {
                ThreadRunning = false;
            }
            else
            {
                if (RecordKeyboard)
                    WinApi.UnhookWindowsHookEx(KeyboardHook);
                if (RecordMouse)
                    WinApi.UnhookWindowsHookEx(MouseHook);
                ThreadExited = true;
            }
        }
        public void StartRecording(bool _recordKeyboard, bool _recordMouse)
        {
            if(ThreadExited)
            {
                mouseDelayEnabled = false;
                RecordKeyboard = _recordKeyboard;
                RecordMouse = _recordMouse;
                if (RecordKeyboard)
                    KeyboardHook = WinApi.SetWindowsHookEx(13, KBCallback, IntPtr.Zero, 0);
                if (RecordMouse)
                    MouseHook = WinApi.SetWindowsHookEx(14, MCallback, IntPtr.Zero, 0);
                ThreadExited = false;
                AcceptMouseMovement = true;
                delayBegin = DateTime.Now;
                if (CaptureMouseMovements && MouseMovementCaptureDelay > 0)
                {
                    mouseDelayEnabled = true;
                    ThreadRunning = true;
                    threadTask = new Thread(() =>
                    {
                        while (ThreadRunning)
                        {
                            Thread.Sleep(MouseMovementCaptureDelay);
                            AcceptMouseMovement = true;
                        }
                        if (RecordKeyboard)
                            WinApi.UnhookWindowsHookEx(KeyboardHook);
                        if (RecordMouse)
                            WinApi.UnhookWindowsHookEx(MouseHook);
                        ThreadExited = true;
                    });
                    threadTask.Start();
                }
            }
        }
        public void ClearList()
        {
            EventList.Clear();
        }

        #endregion

        #region " Callbacks "

        private IntPtr MouseEventCallback(int nCode, IntPtr wParam, IntPtr lParam)
        {
            if(!(nCode < 0))
            {
                lock(this)
                {
                    eMouseButton button = (eMouseButton)wParam;
                    if (button == eMouseButton.MOVE && !AcceptMouseMovement && mouseDelayEnabled)
                        return WinApi.CallNextHookEx(MouseHook, nCode, wParam, lParam);

                    MouseEvent eventData = (MouseEvent)Marshal.PtrToStructure(lParam, typeof(MouseEvent));
                    DateTime cTime = DateTime.Now;
                    int delay = (cTime - delayBegin).Milliseconds;

                    MacroEvent_Delay dEvent = new MacroEvent_Delay(delay);
                    MacroEvent_Mouse mEvent = new MacroEvent_Mouse(eventData, button);

                    if (PopulateInternalEventList)
                    {
                        EventList.Add(dEvent);
                        EventList.Add(mEvent);
                    }

                    if (OnEvent != null)
                    {
                        OnEvent(dEvent);
                        OnEvent(mEvent);
                    }

                    
                    AcceptMouseMovement = false;
                    delayBegin = cTime;
                }
            }
            return WinApi.CallNextHookEx(MouseHook, nCode, wParam, lParam);
        }

        private IntPtr KeyboardEventCallback(int nCode, IntPtr wParam, IntPtr lParam)
        {
            eKeyboardEvent eKB = (eKeyboardEvent)wParam;
            if(!(nCode < 0) && (eKB == eKeyboardEvent.KeyDown || eKB == eKeyboardEvent.KeyUp))
            {
                lock (this)
                {
                    KeyboardEvent kbEvent = (KeyboardEvent)Marshal.PtrToStructure(lParam, typeof(KeyboardEvent));
                    DateTime cTime = DateTime.Now;
                    int delay = (cTime - delayBegin).Milliseconds;

                    MacroEvent_Delay dEvent = new MacroEvent_Delay(delay);
                    MacroEvent_Keyboard kEvent = new MacroEvent_Keyboard(eKB, kbEvent);

                    if (PopulateInternalEventList)
                    {
                        EventList.Add(dEvent);
                        EventList.Add(kEvent);
                    }

                    if (OnEvent != null)
                    {
                        OnEvent(dEvent);
                        OnEvent(kEvent);
                    }
                    delayBegin = cTime;
                }
            }
            return WinApi.CallNextHookEx(KeyboardHook, nCode, wParam, lParam);
        }

        #endregion
    }
}
