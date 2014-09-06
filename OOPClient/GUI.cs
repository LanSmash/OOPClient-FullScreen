using System;
using System.Collections.Generic;
using System.Management;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using BMDSwitcherAPI;
using Midi;
using System.Text.RegularExpressions;

namespace OOPClient
{
    public partial class GUI : Form
    {
        private ATEMController atem;
        private Color c_green, c_red, c_white, c_grey;
        
        private LaunchPadController launchpad;
        private AudioController audioMixer;

        private int audioIn, audioOut, controlIn, controlOut;

        private bool lpIsOpen, audioIsOpen;

        public GUI()
        {
            InitializeComponent();

            lpIsOpen = audioIsOpen = false;

            atem = new ATEMController();
            atem.SwitcherConnected2 += ATEMConnected;
            atem.SwitcherDisconnected2 += ATEMDisconnected;
            atem.UpdatePreviewButtonSelection2 += ATEMUpdatePreview;
            atem.UpdateProgramButtonSelection2 += ATEMUpdateProgram;

            c_green = Color.FromArgb(192, 255, 192);
            c_red = Color.FromArgb(255, 192, 192);
            c_white = Color.FromArgb(255, 255, 255);
            c_grey = Color.FromArgb(224, 224, 224);

            txtAtemAddress.Text = "192.168.0.201";

            slcAudioIn.SelectedIndex = 0;
            slcAudioOut.SelectedIndex = 0;
            slcControlIn.SelectedIndex = 0;
            slcControlOut.SelectedIndex = 0;

            launchpad = new LaunchPadController();
            launchpad.ButtonPressed += launchpad_ButtonPressed;

            audioMixer = new AudioController();
            audioMixer.TrackBarChange += audioMixer_TrackBarChange;
            audioMixer.CheckBoxChange += audioMixer_CheckBoxChange;
        }

        #region audioMixer
        private void audioMixer_CheckBoxChange(int channel, int val)
        {
            bool isOn = (val == 127) ? true : false;
            ((CheckBox)this.boxAudioControl.Controls["checkBox" + channel]).Checked = isOn;

            if (isOn)
            {
                ((CheckBox)this.boxAudioControl.Controls["checkBox" + channel]).BackColor = c_green;
            }
            else
            {
                ((CheckBox)this.boxAudioControl.Controls["checkBox" + channel]).BackColor = c_red;
            }
        }

        private void audioMixer_TrackBarChange(int channel, int val)
        {
            ((TrackBar)this.boxAudioControl.Controls["trackBar" + channel]).Value = val;
        }

        private void trackBar_Scroll(object sender, EventArgs e)
        {
            int channel = Convert.ToInt32(Regex.Replace(((TrackBar)sender).Name, "[^0-9.]", "")); // Remove letters, and converts to int.
            audioMixer.programChangeSlider(channel, ((TrackBar)sender).Value, false);
        }

        private void checkBox_CheckedChanged(object sender, EventArgs e)
        {
            int channel = Convert.ToInt32(Regex.Replace(((CheckBox)sender).Name, "[^0-9.]", "")); // Remove letters, and converts to int.
            audioMixer.programChangeOnOff(channel, ((CheckBox)sender).Checked, false);

            if (((CheckBox)sender).Checked)
            {
                ((CheckBox)this.boxAudioControl.Controls["checkBox" + channel]).BackColor = c_green;
            }
            else
            {
                ((CheckBox)this.boxAudioControl.Controls["checkBox" + channel]).BackColor = c_red;
            }
        }

        private void btnAudioConnect_Click(object sender, EventArgs e)
        {
            audioMixer.Open(audioIn, audioOut);
        }

        private void chkAudioDebug_CheckedChanged(object sender, EventArgs e)
        {
            if (chkAudioDebug.Checked)
            {
                audioMixer.debugMode = true;
            }
            else
            {
                audioMixer.debugMode = false;
            }
        }

        private void btnAudioTest_Click(object sender, EventArgs e)
        {
            for (int i = 1; i < 16; i++)
            {
                audioMixer.programChangeSlider(i, 127);
                audioMixer.programChangeOnOff(i, true);
            }
        }
        #endregion

        #region ATEM
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

                    if(lpIsOpen) {
                        int set = (programId == 0) ? 8 : programId + 1;
                        launchpad.SetColor(0, set, 2);
                    }
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

                    if (lpIsOpen)
                    {
                        int set = (previewId == 0) ? 8 : previewId + 1;
                        launchpad.SetColor(1, set, 6);
                    }
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
        #endregion

        #region launchpad
        void launchpad_ButtonPressed(int row, int col)
        {
            //Console.WriteLine(row + " - " + col);
            launchpad.SetColor(row, col, 6);
        }

        private void btnGetMidi_Click(object sender, EventArgs e)
        {
            for(int i = 0; i < OutputDevice.InstalledDevices.Count; i++)
            {
                OutputDevice output = OutputDevice.InstalledDevices[i];
                if (output.Name.Contains("Launchpad S"))
                {
                    controlOut = i;
                    slcControlOut.Items[0] = output.Name;
                }
                else if (output.Name.Contains("USB2.0-MIDI") && !output.Name.Contains("MIDIOUT"))
                {
                    audioOut = i;
                    slcAudioOut.Items[0] = output.Name;
                }

                Console.WriteLine("OUT: " + i + " - " + output.Name);
            }

            for (int i = 0; i < InputDevice.InstalledDevices.Count; i++)
            {
                InputDevice input = InputDevice.InstalledDevices[i];
                if (input.Name.Contains("Launchpad S"))
                {
                    controlIn = i;
                    slcControlIn.Items[0] = input.Name;
                }
                else if (input.Name.Contains("USB2.0-MIDI") && !input.Name.Contains("MIDIOUT"))
                {
                    audioIn = i;
                    slcAudioIn.Items[0] = input.Name;
                }

                Console.WriteLine("IN: " + i + " - " + input.Name);
            }
        }

        private void btnControlConnect_Click(object sender, EventArgs e)
        {
            if (lpIsOpen)
            {
                launchpad.Close();
                btnControlConnect.BackColor = c_green;
                btnControlConnect.Text = "Connect";
                lpIsOpen = false;
            }
            else
            {
                launchpad.Open(controlIn, controlOut);
                btnControlConnect.BackColor = c_red;
                btnControlConnect.Text = "Disconnect";

                //ATEM Program Layout
                launchpad.SetColor(0, 0, 1);
                launchpad.SetColor(0, 1, 1);
                launchpad.SetColor(0, 2, 1);
                launchpad.SetColor(0, 3, 1);
                launchpad.SetColor(0, 4, 1);
                launchpad.SetColor(0, 5, 1);
                launchpad.SetColor(0, 6, 1);
                launchpad.SetColor(0, 7, 1);
                launchpad.SetColor(0, 8, 1);

                //ATEM Preview Layout
                launchpad.SetColor(1, 0, 5);
                launchpad.SetColor(1, 1, 5);
                launchpad.SetColor(1, 2, 5);
                launchpad.SetColor(1, 3, 5);
                launchpad.SetColor(1, 4, 5);
                launchpad.SetColor(1, 5, 5);
                launchpad.SetColor(1, 6, 5);
                launchpad.SetColor(1, 7, 5);
                launchpad.SetColor(1, 8, 5);

                lpIsOpen = true;
            }
        }
        #endregion

        private void GUI_FormClosing(object sender, System.ComponentModel.CancelEventArgs e)
        {

            // Display a MsgBox asking if you want to close application
            if (MessageBox.Show("Do you want to close the application", "DON'T LEAVE ME!!!!", MessageBoxButtons.YesNo) == DialogResult.No)
            {
                e.Cancel = true;
            }
            else
            {
                launchpad.Close();
                audioMixer.Close();
            }
        }
    }
}