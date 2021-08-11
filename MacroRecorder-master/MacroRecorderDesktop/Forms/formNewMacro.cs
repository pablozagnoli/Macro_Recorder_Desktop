using System;
using System.Windows.Forms;


namespace MacroRecorder.Forms
{
    public partial class formNewMacro : Form
    {
        public bool vRecordMouse { get; private set; }
        public bool vRecordKeyboard { get; private set; }
        public int vDelayBeforeRecord { get; private set; }
        public int vMouseMovementCaptureDelay { get; private set; }
        public bool vCaptureMouseMovements { get; private set; }

        public formNewMacro()
        {
            InitializeComponent();
        }

        private void formNewMacro_Load(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            vCaptureMouseMovements = cbCaptureMouseMovement.Checked;
            if(rbRecordKeyboard.Checked)
            {
                vCaptureMouseMovements = false;
                vRecordKeyboard = true;
                vRecordMouse = false;
            }
            if(rbRecordMouse.Checked)
            {
                vRecordMouse = true;
                vRecordKeyboard = false;
            }
            if(rbRecordBoth.Checked)
            {
                vRecordMouse = true;
                vRecordKeyboard = true;
            }
            vDelayBeforeRecord = (int)nudRecordDelay.Value * 100;
            vMouseMovementCaptureDelay = tbMouseSmothness.Maximum - tbMouseSmothness.Value;
            this.DialogResult = System.Windows.Forms.DialogResult.OK;
        }

        private void trackBar1_Scroll(object sender, EventArgs e)
        {
            lblSmoothness.Text = tbMouseSmothness.Value.ToString();
        }

        private void rbRecordKeyboard_CheckedChanged(object sender, EventArgs e)
        {
            gbMouseSettings.Enabled = !rbRecordKeyboard.Checked;
        }
    }
}
