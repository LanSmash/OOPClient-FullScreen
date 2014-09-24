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
        private CasparController caspar;
        private HyperdeckController hyperdeck;

        private int audioIn, audioOut, controlIn, controlOut;

        private bool atemIsOpen, controlIsOpen, audioIsOpen, controlLoaded;

        private PresetHandler presetHandler;

        XkeysController xk;
        Dictionary<int, string> xkDic;

        public GUI()
        {
            InitializeComponent();

            controlIsOpen = audioIsOpen = atemIsOpen = controlLoaded = false;

            caspar = new CasparController();
            caspar.OnCasparConnected2 += caspar_OnCasparConnected2;
            caspar.OnCasparDisconnected += caspar_OnCasparDisconnected;
            caspar.OnCasparFailedConnect += caspar_OnCasparFailedConnect;

            presetHandler = new PresetHandler();
            presetHandler.SendAudio += audioMixer_HandleTemplate;
            presetHandler.SendRundown += rundownHandler;
            presetHandler.SendSuperSource += superSourceHandler;

            atem = new ATEMController();
            atem.SwitcherConnected2 += ATEMConnected;
            atem.SwitcherDisconnected2 += ATEMDisconnected;
            atem.UpdatePreviewButtonSelection2 += ATEMUpdatePreview;
            atem.UpdateProgramButtonSelection2 += ATEMUpdateProgram;
            atem.UpdateTransitionSelection += ATEMUpdateTransition;

            c_green = Color.FromArgb(192, 255, 192);
            c_red = Color.FromArgb(255, 192, 192);
            c_white = Color.FromArgb(255, 255, 255);
            c_grey = Color.FromArgb(224, 224, 224);

            txtAtemAddress.Text = "192.168.0.201";
            txtCasparAddress.Text = "127.0.0.1";
            txtHyperdeckAddress.Text = "192.168.0.24";

            slcAudioIn.SelectedIndex = 0;
            slcAudioOut.SelectedIndex = 0;
            slcControlOut.SelectedIndex = 0;

            audioMixer = new AudioController();
            audioMixer.TrackBarChange += audioMixer_TrackBarChange;
            audioMixer.CheckBoxChange += audioMixer_CheckBoxChange;

            populatePresets();

            xk = new XkeysController();
            xk.XKeyPressed += xk_XKeyPressed;

            btnControlConnect.Enabled = false;

            hyperdeck = new HyperdeckController();
        }

        #region presethandling
        private void populatePresets()
        {
            this.panelPresets.Enabled = false;
            this.panelPresetsInner.Controls.Clear();

            int left = 0;
            int top = 0;
            int buttonCount = 0;

            foreach (string preset in presetHandler.GetPresets())
            {
                if (buttonCount < 12)
                {
                    Console.WriteLine(preset);
                    Button presetButton = new Button();
                    presetButton.Text = preset.Remove(0, 8).Replace(".xml", "").ToUpper();
                    presetButton.Width = 100;
                    presetButton.Height = 70;
                    presetButton.BackColor = c_red;
                    presetButton.FlatStyle = FlatStyle.Flat;
                    presetButton.Left = left;
                    presetButton.Top = top;
                    presetButton.Tag = "PresetButton";
                    presetButton.Click += presetButton_Click;
                    this.panelPresetsInner.Controls.Add(presetButton);

                    if (buttonCount == 5)
                    {
                        left = 0;
                        top = 90;
                    }
                    else
                    {
                        left = left + 117;
                    }

                    buttonCount++;
                }
            }

            this.panelPresets.Enabled = true;
        }

        private void presetButton_Click(object sender, EventArgs e)
        {
            string preset = ((Button)sender).Text;

            foreach (object button in panelPresetsInner.Controls)
            {
                if ((String)((Button)button).Tag == "PresetButton")
                {
                    ((Button)button).BackColor = c_red;
                }
            }

            ((Button)sender).BackColor = c_green;

            presetHandler.changePreset(preset);
        }

        private void superSourceHandler(string type, object args)
        {
            if (atemIsOpen)
            {
                atem.setSuperSource((Dictionary<int, Dictionary<string, double>>)args);
            }
        }


        private void rundownHandler(string type, object args)
        {
            foreach (string entry in (List<string>)args)
            {
                string[] parts = entry.Split('|');
                string[] cmd = parts[1].Split('-');

                switch (parts[0])
                {
                    //ATEM Case
                    case "A":
                        if (atemIsOpen)
                        {
                            switch (cmd[0])
                            {
                                case "PROG":
                                    atem.changeProg(long.Parse(cmd[1]));
                                    break;

                                case "PREV":
                                    atem.changePrev(long.Parse(cmd[1]));
                                    break;

                                //Transitions and perform them
                                case "TRANS":
                                    switch (cmd[1])
                                    {
                                        case "AUTO":
                                            atem.performAuto();
                                            break;

                                        case "CUT":
                                            atem.performCut();
                                            break;

                                        case "MIX":
                                            atem.changeTransition(1);
                                            break;

                                        case "DIP":
                                            atem.changeTransition(2);
                                            break;

                                        case "WIPE":
                                            atem.changeTransition(3);
                                            break;

                                        case "STING":
                                            atem.changeTransition(4);
                                            break;

                                        case "DVE":
                                            atem.changeTransition(5);
                                            break;
                                    }
                                    break;
                            }
                        }
                        break;

                    //MAIN STUFF
                    case "M":
                        switch (cmd[0])
                        {
                            case "SLEEP":
                                Thread.Sleep(Int32.Parse(cmd[1]));
                                break;
                        }
                        break;
                }
            }
        }

        #region buttons
        private void btnPresetReload_Click(object sender, EventArgs e)
        {
            populatePresets();
        }
        #endregion
        #endregion

        #region audioMixer
        private void audioMixer_HandleTemplate(object sender, object args)
        {
            if (audioIsOpen)
            {
                string type = Convert.ToString(sender);
                if (type == "mute")
                {
                    Dictionary<int, bool> mute = (Dictionary<int, bool>)args;
                    foreach (KeyValuePair<int, bool> entry in mute)
                    {
                        audioMixer.programChangeOnOff(entry.Key, entry.Value);
                    }
                }
                else if (type == "fader")
                {
                    Dictionary<int, int> fader = (Dictionary<int, int>)args;
                    foreach (KeyValuePair<int, int> entry in fader)
                    {
                        audioMixer.programChangeSlider(entry.Key, entry.Value);
                    }
                }
            }
        }

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
            audioIsOpen = true;
            boxAudioControl.Enabled = true;
            statusAudio.BackColor = c_green;

            for (int i = 1; i < 16; i++)
            {
                audioMixer.programChangeSlider(i, 0);
                audioMixer.programChangeOnOff(i, false);
            }
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

        private void pushToPreset()
        {
            Dictionary<int, int> faders = new Dictionary<int, int>();
            Dictionary<int, bool> mute = new Dictionary<int, bool>();

            for (int i = 1; i < 15; i++)
            {
                int muteChan = i + 27;
                if (i > 4)
                {
                    muteChan = muteChan + 1;
                }

                faders.Add(i, ((TrackBar)this.boxAudioControl.Controls["trackBar" + i]).Value);
                mute.Add(i, ((CheckBox)this.boxAudioControl.Controls["checkBox" + muteChan]).Checked);
            }

            presetHandler.pushToPreset(mute, faders);
        }

        #region buttons
        private void btnAudioRevert_Click(object sender, EventArgs e)
        {
            presetHandler.revertToPreset();
        }

        private void btnAudioPush_Click(object sender, EventArgs e)
        {
            pushToPreset();
        }
        #endregion
        #endregion

        #region ATEM
        //When atem connects - update the buttons
        private void ATEMConnected(int id)
        {
            btnAtemConnect.Enabled = false;
            btnAtemConnect.BackColor = c_red;
            btnAtemConnect.Text = "Connected";

            statusAtem.BackColor = c_green;

            panelATEM.Enabled = true;

            atemIsOpen = true;

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

            setTransition(atem.getCurrentTransition());
        }

        private void ATEMUpdateTransition(int id)
        {
            setTransition(id);
        }

        private void setTransition(int id)
        {
            resetTransButtons();

            switch (id)
            {
                case 1:
                    btnTransMix.BackColor = c_green;
                    break;

                case 2:
                    btnTransDip.BackColor = c_green;
                    break;

                case 3:
                    btnTransWipe.BackColor = c_green;
                    break;

                case 4:
                    btnTransSting.BackColor = c_green;
                    break;

                case 5:
                    btnTransDve.BackColor = c_green;
                    break;
            }
        }

        private void resetTransButtons()
        {
            btnTransMix.BackColor = c_red;
            btnTransDip.BackColor = c_red;
            btnTransWipe.BackColor = c_red;
            btnTransSting.BackColor = c_red;
            btnTransDve.BackColor = c_red;
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

                statusAtem.BackColor = c_red;

                panelATEM.Enabled = false;

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

                if (atem.allowedInputs().Contains(programId))
                {
                    panelProg.Controls["progBtn" + programId].BackColor = c_red;

                    if(controlIsOpen) {
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

                if (atem.allowedInputs().Contains(previewId))
                {
                    panelPrev.Controls["prevBtn" + previewId].BackColor = c_green;

                    if (controlIsOpen)
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
            foreach (long i in atem.allowedInputs())
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
            foreach (long i in atem.allowedInputs())
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

        #region buttons
        private void btnTransChange(object sender, EventArgs e)
        {
            resetTransButtons();
            ((Button)sender).BackColor = c_green;
            atem.changeTransition((int)((Button)sender).Tag);
        }

        private void btnTransCut_Click(object sender, EventArgs e)
        {
            atem.performCut();
        }

        private void btnTransAuto_Click(object sender, EventArgs e)
        {
            atem.performAuto();
        }
        #endregion
        #endregion

        #region control
        private void xk_XKeyPressed(int row, int col)
        {
            Console.WriteLine("ROW" + row + " - COL" + col);
        }

        private void btnGetMidi_Click(object sender, EventArgs e)
        {
            for(int i = 0; i < OutputDevice.InstalledDevices.Count; i++)
            {
                OutputDevice output = OutputDevice.InstalledDevices[i];
                if (output.Name.Contains("USB2.0-MIDI") && !output.Name.Contains("MIDIOUT"))
                {
                    audioOut = i;
                    slcAudioOut.Items[0] = output.Name;
                }
            }

            for (int i = 0; i < InputDevice.InstalledDevices.Count; i++)
            {
                InputDevice input = InputDevice.InstalledDevices[i];
                if (input.Name.Contains("USB2.0-MIDI") && !input.Name.Contains("MIDIOUT"))
                {
                    audioIn = i;
                    slcAudioIn.Items[0] = input.Name;
                }
            }

            //Dno hax
            xkDic = xk.enumerateDevices();
            xkDic = xk.enumerateDevices();
            xkDic = xk.enumerateDevices();
            xk.SetCallback();
            xkDic = xk.enumerateDevices();
            xkDic = xk.enumerateDevices();
            xkDic = xk.enumerateDevices();
            slcControlOut.Items.Clear();
            slcControlOut.Enabled = true;

            foreach (KeyValuePair<int, string> entry in xkDic)
            {
                slcControlOut.Items.Add(entry);
            }

            if (slcControlOut.Items.Count > 0)
            {
                slcControlOut.SelectedIndex = 0;
                xk.SetCallback();
                controlLoaded = true;
                btnControlConnect.Enabled = true;
            }
            else
            {
                slcControlOut.Enabled = false;
                controlLoaded = false;
            }
        }

        private void btnControlConnect_Click(object sender, EventArgs e)
        {
            #region deprecated
            /*if (controlIsOpen)
            {
                launchpad.Close();
                btnControlConnect.BackColor = c_green;
                btnControlConnect.Text = "Connect";
                controlIsOpen = false;
                statusControl.BackColor = c_red;
            }
            else
            {
                launchpad.Open(controlIn, controlOut);
                btnControlConnect.BackColor = c_red;
                btnControlConnect.Text = "Disconnect";

                statusControl.BackColor = c_green;

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

                controlIsOpen = true;
            }*/
#endregion
            try
            {
                if (!controlIsOpen)
                {
                    xk.SetCallback();
                    statusControl.BackColor = c_green;
                    xk.test();
                    controlIsOpen = true;
                    btnControlConnect.Enabled = false;
                }
                else
                {
                    xk.selectDevice(-1);
                    controlIsOpen = false;
                    statusControl.BackColor = c_red;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString());
            }
        }

        private void slcControlOut_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (slcControlOut.Items.Count > 0 && slcControlOut.SelectedIndex != -1 && controlLoaded)
            {
                xk.selectDevice(slcControlOut.SelectedIndex);
            }
        }
        #endregion

        #region casparcg
        private void btnCasparConnect_Click(object sender, EventArgs e)
        {
            string cip = "127.0.0.1";
            int cport = 5250;

            string ipport = txtCasparAddress.Text;

            if (ipport.Length > 1)
            {
                if (ipport.Contains(':'))
                {
                    string[] ipparts = ipport.Split(':');
                    cip = ipparts[0];

                    if (ipparts[1].Length > 1)
                    {
                        cport = Int32.Parse(ipparts[1]);
                    }
                }
                else
                {
                    cip = ipport;
                }
            }

            try
            {
                Console.WriteLine(cip + ":" + cport);
                caspar.Connect(cip, cport);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString());
            }
        }

        private void caspar_OnCasparFailedConnect(string message)
        {
            this.Invoke(new MethodInvoker(delegate
            {
                btnCasparConnect.BackColor = c_green;
                btnCasparConnect.Text = "Connect";
                tabOverlays.Enabled = false;
                statusCaspar.BackColor = c_red;
                MessageBox.Show(message);
            }));
        }

        private void caspar_OnCasparDisconnected(string message)
        {
            this.Invoke(new MethodInvoker(delegate
            {
                btnCasparConnect.BackColor = c_green;
                btnCasparConnect.Text = "Connect";
                tabOverlays.Enabled = false;
                statusCaspar.BackColor = c_red;
            }));
        }

        private void caspar_OnCasparConnected2(string message)
        {
            this.Invoke(new MethodInvoker(delegate
            {
                btnCasparConnect.BackColor = c_red;
                btnCasparConnect.Text = "Disconnect";
                tabOverlays.Enabled = true;
                statusCaspar.BackColor = c_green;
            }));
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
                if (controlIsOpen)
                {
                    xk.Close();
                }

                if (audioIsOpen)
                {
                    audioMixer.Close();
                }
            }
        }

        private void btnHyperdeckConnect_Click(object sender, EventArgs e)
        {
            try
            {
                hyperdeck.Connect(txtHyperdeckAddress.Text);
                panelHyperdeck.Enabled = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString());
            }
        }

        private void btnHyperdeckPlay_Click(object sender, EventArgs e)
        {
            hyperdeck.Play(int.Parse(txtHyperdeckSpeed.Text));
        }

        private void btnHyperdeckStop_Click(object sender, EventArgs e)
        {
            hyperdeck.Stop();
        }

        private void btnHyperdeckRecord_Click(object sender, EventArgs e)
        {
            hyperdeck.Record();
        }

        private void btnHyperdeckRewind_Click(object sender, EventArgs e)
        {
            hyperdeck.Play(-300);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            hyperdeck.Play(300);
        }

        private void btnHyperdeckEnd_Click(object sender, EventArgs e)
        {
            hyperdeck.GotoEndTimeline();
        }
    }
}