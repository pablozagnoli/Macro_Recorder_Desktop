using System.Threading;


namespace MacroRecorder.Macro.Events
{
    public class MacroEvent_Delay : IMacroEvent
    {
        public int vDelayMS = 0;
        public MacroEvent_Delay(int pMs)
        {
            vDelayMS = pMs;
        }
        public void ExecuteEvent()
        {
            Thread.Sleep(vDelayMS);
        }
        public string GetEventString()
        {
            return string.Format("{0}ms", vDelayMS);
        }
        public string GetEventType()
        {
            return "Delay";
        }
    }
}
