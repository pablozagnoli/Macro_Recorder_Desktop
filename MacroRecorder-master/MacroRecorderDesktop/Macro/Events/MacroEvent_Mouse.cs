using MacroRecorder.Macro.Structs;
using System.Runtime.InteropServices;


namespace MacroRecorder.Macro.Events
{
    class MacroEvent_Mouse : IMacroEvent
    {
        public MouseEvent vEvent;
        public eMouseButton vButton;
        INPUT[] vInputList = new INPUT[1];
        int vInputSize = 0;
        public MacroEvent_Mouse(MouseEvent pEvent, eMouseButton pButton)
        {
            vEvent = pEvent;
            vButton = pButton;

            INPUT vKeyInput = new INPUT();
            vKeyInput.type = 0;
            MOUSEINPUT vMInput = new MOUSEINPUT();

            eMouseCommand vCommand = eMouseCommand.Move;
            if (vButton == eMouseButton.LDOWN) vCommand = eMouseCommand.LDown;
            if (vButton == eMouseButton.LUP) vCommand = eMouseCommand.LUp;
            if (vButton == eMouseButton.RDOWN) vCommand = eMouseCommand.RDown;
            if (vButton == eMouseButton.RUP) vCommand = eMouseCommand.RUp;
            if (vButton == eMouseButton.MWheel) vCommand = eMouseCommand.MWheel;
            if (vButton == eMouseButton.MHWheel) vCommand = eMouseCommand.HWHeel;

            vMInput.dx = 0;
            vMInput.dy = 0;
            vMInput.mouseData = vEvent.mouseData;
            vMInput.time = 0;
            vMInput.dwExtraInfo = vEvent.extraInfo;
            vMInput.dwFlags = (uint)vCommand;

            vKeyInput.iUinion.mi = vMInput;

            vInputList[0] = vKeyInput;

            vInputSize = Marshal.SizeOf(vInputList[0]);
        }

        public void ExecuteEvent()
        {
            WinApi.SetCursorPos(vEvent.Location.X, vEvent.Location.Y);
            if(vButton != eMouseButton.MOVE)
                WinApi.SendInput(1, vInputList, vInputSize);
        }


        public string GetEventString()
        {
            return string.Format("{0} {1}", vButton, vEvent.Location);
        }
        public string GetEventType()
        {
            return "Mouse";
        }
    }
}
