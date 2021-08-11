using MacroRecorder.Macro;
using System;
using System.Drawing;
using System.Windows.Forms;


namespace MacroRecorder.Forms
{
    public partial class formMacroNotify : Form
    {
        MRecorder vRecorder;
        int vElapsed = 0;
        int vWait = 0;

        bool vKeyboard = false;
        bool vMouse = false;
        Timer vDelayTimer = new Timer();

        public formMacroNotify(MRecorder pRecorder, bool pKeyboard, bool pMouse, int pMsWait)
        {
            InitializeComponent();
            vRecorder = pRecorder;
            vWait = pMsWait;

            vKeyboard = pKeyboard;
            vMouse = pMouse;

            this.Location = new Point(Screen.PrimaryScreen.WorkingArea.Right - this.Width, Screen.PrimaryScreen.WorkingArea.Bottom - this.Height);

            if(pMsWait == 0)
            {
                statusLabel.Text = "GRAVANDO";
                statusLabel.ForeColor = Color.Red;
                vRecorder.StartRecording(vKeyboard, vMouse);
            }
            else
            {
                statusLabel.Text = string.Format("Waiting {0}ms", vWait/100);
                statusLabel.ForeColor = Color.Blue;
                
                vDelayTimer.Tick += TimerLoop;
                vDelayTimer.Interval = 1;
                vDelayTimer.Start();
            }
        }

        void TimerLoop(object sender, EventArgs e)
        {
            vElapsed++;
            if(vElapsed >= vWait)
            {
                ((Timer)sender).Stop();
                statusLabel.Text = "GRAVANDO";
                statusLabel.ForeColor = Color.Red;
                vRecorder.StartRecording(vKeyboard, vMouse);
            }
            else
            {
                statusLabel.Text = string.Format("Waiting {0}s",(float)(vWait-vElapsed) / 100);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            vDelayTimer.Stop();
            vRecorder.StopRecording();
            this.DialogResult = System.Windows.Forms.DialogResult.OK;
        }

        private void formMacroNotify_Load(object sender, EventArgs e)
        {

        }

        private void statusLabel_Click(object sender, EventArgs e)
        {

        }
    }
}
