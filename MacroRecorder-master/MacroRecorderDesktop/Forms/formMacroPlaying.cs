using MacroRecorder.Macro;
using System;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;


namespace MacroRecorder.Forms
{
    public partial class formMacroPlaying : Form
    {
        public formMacroPlaying(IMacroEvent[] vEvents)
        {
            InitializeComponent();


            this.Location = new Point(Screen.PrimaryScreen.WorkingArea.Right - this.Width, Screen.PrimaryScreen.WorkingArea.Bottom - this.Height);

            new Thread(() =>
            {
                foreach(IMacroEvent e in vEvents)
                {
                    e.ExecuteEvent();
                }
                Invoke((MethodInvoker)delegate()
                {
                    this.DialogResult = System.Windows.Forms.DialogResult.OK;
                });
            }).Start();
        }

        private void formMacroPlaying_Load(object sender, EventArgs e)
        {

        }
    }
}
