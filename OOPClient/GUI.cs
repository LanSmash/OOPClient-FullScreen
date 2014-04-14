using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using BMDSwitcherAPI;

namespace OOPClient
{
    public partial class GUI : Form
    {
        private ATEMController atem;
        private Color c_green, c_red, c_white, c_grey;

        public GUI()
        {
            InitializeComponent();

            atem = new ATEMController();
            atem.SwitcherConnected2 += ATEMConnected;
            atem.SwitcherDisconnected2 += ATEMDisconnected;
            atem.UpdatePreviewButtonSelection2 += ATEMUpdatePreview;
            atem.UpdateProgramButtonSelection2 += ATEMUpdateProgram;

            c_green = Color.FromArgb(192, 255, 192);
            c_red = Color.FromArgb(255, 192, 192);
            c_white = Color.FromArgb(255, 255, 255);
            c_grey = Color.FromArgb(224, 224, 224);

            txtAtemAddress.Text = "192.168.0.156";
        }

        //When atem connects - update the buttons
        private void ATEMConnected(int id)
        {
            btnAtemConnect.Enabled = false;
            btnAtemConnect.BackColor = c_red;
            btnAtemConnect.Text = "Connected";

            Dictionary<int, string> btns = atem.GetSources();

            foreach (KeyValuePair<int, string> pair in btns)
            {
                int inputId = pair.Key;
                string inputName = pair.Value.ToString();

                if (inputName.Length > 0 && panelPrev.Controls["prevBtn" + inputId] != null && panelProg.Controls["progBtn" + inputId] != null)
                {
                    panelPrev.Controls["prevBtn" + inputId].Text = inputName;
                    panelProg.Controls["progBtn" + inputId].Text = inputName;

                    panelPrev.Controls["prevBtn" + inputId].Tag = inputId;
                    panelProg.Controls["progBtn" + inputId].Tag = inputId;

                    panelPrev.Controls["prevBtn" + inputId].Enabled = true;
                    panelProg.Controls["progBtn" + inputId].Enabled = true;
                }
                else if (inputName.Length < 1 && panelPrev.Controls["prevBtn" + inputId] != null && panelProg.Controls["progBtn" + inputId] != null)
                {
                    panelProg.Controls["progBtn" + inputId].BackColor = c_grey;
                    panelPrev.Controls["prevBtn" + inputId].BackColor = c_grey;
                }
            }
        }

        //When atem disconnects - update the buttons
        private void ATEMDisconnected(int id)
        {
            if (id == 999)
            {
                this.Invoke(new MethodInvoker(delegate { atem.SwitcherDisconnected(null, null); }));
            }
            else
            {
                btnAtemConnect.Enabled = true;
                btnAtemConnect.BackColor = c_green;
                btnAtemConnect.Text = "Connect";

                for (int i = 0; i < 9; i++)
                {
                    panelProg.Controls["progBtn" + i].Enabled = false;
                    panelProg.Controls["progBtn" + i].BackColor = c_grey;
                    panelProg.Controls["progBtn" + i].Text = "";

                    panelPrev.Controls["prevBtn" + i].Enabled = false;
                    panelPrev.Controls["prevBtn" + i].BackColor = c_grey;
                    panelPrev.Controls["prevBtn" + i].Text = "";
                }
            }
        }

        //Set color of the active Program source
        private void ATEMUpdateProgram(int programId)
        {
            //Fix - see BMDController.cs for more info
            if (programId == 999)
            {
                this.Invoke(new MethodInvoker(delegate { atem.UpdateProgramButtonSelection(null, null); }));
            }
            else
            {
                ATEMResetProg();

                if (programId >= 0 && programId < 9)
                {
                    panelProg.Controls["progBtn" + programId].BackColor = c_red;
                }
            }
            
        }

        //Set color of the active Preview source
        private void ATEMUpdatePreview(int previewId)
        {
            //Fix - see BMDController.cs for more info
            if (previewId == 999)
            {
                this.Invoke(new MethodInvoker(delegate { atem.UpdatePreviewButtonSelection(null, null); }));
            }
            else
            {
                ATEMResetPrev();

                if (previewId >= 0 && previewId < 9)
                {
                    panelPrev.Controls["prevBtn" + previewId].BackColor = c_green;
                }
            }
            
        }

        //Reset all program buttons to non-active colors
        private void ATEMResetProg()
        {
            for (int i = 0; i < 9; i++)
            {
                if (panelProg.Controls["progBtn" + i] != null && panelProg.Controls["progBtn" + i].Enabled == true)
                {
                    panelProg.Controls["progBtn" + i].BackColor = c_white;
                }
            }
        }

        //Reset all preview buttons to non-active colors
        private void ATEMResetPrev()
        {
            for (int i = 0; i < 9; i++)
            {
                if (panelPrev.Controls["prevBtn" + i] != null && panelPrev.Controls["prevBtn" + i].Enabled == true)
                {
                    panelPrev.Controls["prevBtn" + i].BackColor = c_white;
                }
            }
        }

        //Change a preview button
        private void changePrev(object sender, EventArgs e)
        {
            long inputId = (long)Convert.ToDouble(((Button)sender).Tag);

            atem.changePrev(inputId);
        }

        //Change a program button
        private void changeProg(object sender, EventArgs e)
        {
            long inputId = (long)Convert.ToDouble(((Button)sender).Tag);

            atem.changeProg(inputId);
        }

        //When the connect button is clicked, connect!
        private void btnAtemConnect_Click(object sender, EventArgs e)
        {
            try
            {
                if (txtAtemAddress.Text.Length > 0)
                {
                    atem.Connect(txtAtemAddress.Text);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Exception: " + ex.Message, "Exception Error");
            }
        }
    }
}
