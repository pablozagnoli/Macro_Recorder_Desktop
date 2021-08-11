using MacroRecorder.Macro;
using MacroRecorder.Macro.Events;
using MacroRecorder.Macro.Structs;
using MacroRecorderDesktop.Properties;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;


namespace MacroRecorder.Forms
{
    public partial class MainWindow : Form
    {
        MRecorder vRecorder = new MRecorder(false);
        List<IMacroEvent> vCurrentMacro = new List<IMacroEvent>();
        List<ListViewItem> vSelectedItems = new List<ListViewItem>();
        public MainWindow()
        {
            InitializeComponent();
            vRecorder.OnEvent += Recorder_OnEvent;
        }

        void ClearCurrent()
        {
            vCurrentMacro.Clear();
           // lvEvents.Items.Clear(); Clear For List Grid Events
        }

        void Recorder_OnEvent(IMacroEvent pMEvent)
        {
            AddEvent(pMEvent);
        }

        void AddEvent(IMacroEvent pMEvent)
        {
            ListViewItem vListMEvent = new ListViewItem(pMEvent.GetEventType());
            vListMEvent.SubItems.Add(pMEvent.GetEventString());
            vListMEvent.ImageKey = pMEvent.GetEventType();
            vListMEvent.Tag = pMEvent;
            //lvEvents.Items.Add(vListMEvent); Add events on List 
            vCurrentMacro.Add(pMEvent);
        }
        private void MainWindow_Load(object sender, EventArgs e)
        {
            ImageList vImgList = new ImageList();
            vImgList.Images.Add("Mouse", Resources.mouse);
            vImgList.Images.Add("Keyboard", Resources.keyboard);
            vImgList.Images.Add("Delay", Resources.time);
           // lvEvents.SmallImageList = vImgList;
        }
        private void recordANewMacroToolStripMenuItem_Click(object sender, EventArgs e) //< AQUI GRAVA
        {
            using(formNewMacro vNewMacro = new formNewMacro())
            {
                if (vNewMacro.ShowDialog() != System.Windows.Forms.DialogResult.OK)
                    return;

                vRecorder.MouseMovementCaptureDelay = vNewMacro.vMouseMovementCaptureDelay;
                vRecorder.CaptureMouseMovements = vNewMacro.vCaptureMouseMovements;

                using(formMacroNotify vFormNoftfy = new formMacroNotify(vRecorder, vNewMacro.vRecordKeyboard, vNewMacro.vRecordMouse, vNewMacro.vDelayBeforeRecord))
                {
                    ClearCurrent();
                    this.Hide();
                    vFormNoftfy.ShowDialog();
                    this.Show();
                }
            }
        }

        private void playMacroToolStripMenuItem_Click(object sender, EventArgs e)  //< AQUI EXECUTA
        {
            if (vCurrentMacro.Count < 1)
                return;
            using(formMacroPlaying vFormPlaying = new formMacroPlaying(vCurrentMacro.ToArray()))
            {
                this.Hide();
                vFormPlaying.ShowDialog();
                this.Show();
            }
        }

        private void removeSelectedEventToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //if(lvEvents.SelectedItems.Count > 0)
            //{
            //    foreach (ListViewItem i in lvEvents.SelectedItems)
            //    {
            //       // ListViewItem i = lvEvents.SelectedItems[0];
            //        IMacroEvent mEvent = (IMacroEvent)i.Tag;
            //        vCurrentMacro.Remove(mEvent);
            //        vRecorder.Remove(mEvent);
            //        lvEvents.Items.Remove(i);
            //    }
            //}
        }

        private void saveMacroToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string path = string.Empty;
            using(SaveFileDialog sfd = new SaveFileDialog())
            {
                sfd.Filter = "Macro File|*.bmr";
                if (sfd.ShowDialog() != System.Windows.Forms.DialogResult.OK)
                    return;
                path = sfd.FileName;
            }

            using(FileStream fs = new FileStream(path, FileMode.Create))
            using(BinaryWriter br = new BinaryWriter(fs))
            {
                br.Write(vCurrentMacro.Count);
                foreach(IMacroEvent me in vCurrentMacro)
                {
                    Type eventType = me.GetType();
                    if(eventType == typeof(MacroEvent_Delay))
                    {
                        MacroEvent_Delay dEvent = (MacroEvent_Delay)me;
                        br.Write((byte)MacroEventType.Delay);
                        br.Write(dEvent.vDelayMS);
                    }

                    if (eventType == typeof(MacroEvent_Keyboard))
                    {
                        MacroEvent_Keyboard kEvent = (MacroEvent_Keyboard)me;
                        br.Write((byte)MacroEventType.Keyboard);
                        br.Write((int)kEvent.vEventType);
                        br.Write(kEvent.vEvent.vKeyCode);
                        br.Write(kEvent.vEvent.scanCode);
                        br.Write(kEvent.vEvent.Flags);
                    }
                    if (eventType == typeof(MacroEvent_Mouse))
                    {
                        MacroEvent_Mouse mEvent = (MacroEvent_Mouse)me;
                        br.Write((byte)MacroEventType.Mouse);
                        br.Write((int)mEvent.vButton);
                        br.Write(mEvent.vEvent.mouseData);
                        br.Write(mEvent.vEvent.Location.X);
                        br.Write(mEvent.vEvent.Location.Y);
                        br.Write(mEvent.vEvent.Flags);
                    }
                }
            }
            MessageBox.Show("Macro saved");
        }

        private void loadAMacroToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string filePath = string.Empty;
            using(OpenFileDialog ofd = new OpenFileDialog())
            {
                ofd.Filter = "Macro File|*.bmr";
                if (ofd.ShowDialog() != System.Windows.Forms.DialogResult.OK)
                    return;
                filePath = ofd.FileName;
            }
            try
            {
                IMacroEvent[] loadedEvents = null;
                using (FileStream fs = new FileStream(filePath, FileMode.Open))
                using (BinaryReader br = new BinaryReader(fs))
                {
                    loadedEvents = new IMacroEvent[br.ReadInt32()];
                    int eventIndex = 0;
                    while(fs.Position != fs.Length)
                    {
                        MacroEventType mType = (MacroEventType)br.ReadByte();
                        if(mType == MacroEventType.Delay)
                        {
                            loadedEvents[eventIndex] = new MacroEvent_Delay(br.ReadInt32());
                        }

                        if(mType == MacroEventType.Keyboard)
                        {
                            eKeyboardEvent kbEvent = (eKeyboardEvent)br.ReadInt32();
                            KeyboardEvent eventData = new KeyboardEvent();
                            eventData.vKeyCode = br.ReadUInt32();
                            eventData.scanCode = br.ReadUInt32();
                            eventData.Flags = br.ReadUInt32();
                            eventData.Time = 0;
                            eventData.dwExtraInfo = WinApi.GetMessageExtraInfo();
                            loadedEvents[eventIndex] = new MacroEvent_Keyboard(kbEvent, eventData);
                        }

                        if(mType == MacroEventType.Mouse)
                        {
                            eMouseButton mButton = (eMouseButton)br.ReadInt32();
                            MouseEvent mEvent = new MouseEvent();
                            mEvent.mouseData = br.ReadUInt32();
                            mEvent.Location = new mPoint();
                            mEvent.Location.X = br.ReadInt32();
                            mEvent.Location.Y = br.ReadInt32();
                            mEvent.Flags = br.ReadUInt32();
                            loadedEvents[eventIndex] = new MacroEvent_Mouse(mEvent, mButton);
                        }
                        eventIndex++;
                    }
                }
                ClearCurrent();
                foreach (IMacroEvent lEvent in loadedEvents)
                    AddEvent(lEvent);
                MessageBox.Show("Loaded successfully");
            }
            catch
            {
                MessageBox.Show("Failed to load macro.");
            }
        }

        private void deselectToolStripMenuItem_Click(object sender, EventArgs e)
        {
           
        }

        private void allToolStripMenuItem_Click(object sender, EventArgs e)
        {
            foreach (ListViewItem i in vSelectedItems)
            {
                i.ForeColor = Color.Black;
            }
            vSelectedItems.Clear();
           // lvEvents.ContextMenuStrip = cmMacro;
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }
    }

    public enum MacroEventType : byte
    {
        Mouse,
        Keyboard,
        Delay
    }
}
