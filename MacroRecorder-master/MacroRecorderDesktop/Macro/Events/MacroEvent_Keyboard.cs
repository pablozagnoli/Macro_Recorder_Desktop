using MacroRecorder.Macro.Structs;
using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;


namespace MacroRecorder.Macro.Events
{
    class MacroEvent_Keyboard : IMacroEvent
    {
        public eKeyboardEvent vEventType;
        public KeyboardEvent vEvent;

        INPUT[] vInputList = new INPUT[1];
        int vInputSize = 0;
        public MacroEvent_Keyboard(eKeyboardEvent pEType, KeyboardEvent pEvent)
        {
            vEventType = pEType;
            vEvent = pEvent;

            uint vKeyState = Convert.ToUInt32(vEventType == eKeyboardEvent.KeyDown ? 0 : 0x2);
            INPUT vKeyInput = new INPUT();
            vKeyInput.type = 1;
            KEYBDINPUT vKbInput = new KEYBDINPUT();
            vKbInput.wVk = Convert.ToUInt16(vEvent.vKeyCode);
            vKbInput.wScan = Convert.ToUInt16(vEvent.scanCode);
            vKbInput.dwFlags = vKeyState;
            vKbInput.time = 0;
            vKbInput.dwExtraInfo = vEvent.dwExtraInfo;

            vKeyInput.iUinion.ki = vKbInput;

            vInputList[0] = vKeyInput;
            vInputSize = Marshal.SizeOf(vInputList[0]);
        }
        public void ExecuteEvent()
        {
            WinApi.SendInput(1, vInputList, vInputSize);
        }

        public string GetEventString()
        {
            return string.Format("{1} {0}", vEventType == eKeyboardEvent.KeyDown ? "Press" : "Release", ((Keys)vEvent.vKeyCode).ToString());
        }

        public string GetEventType()
        {
            return "Keyboard";
        }
    }
}
