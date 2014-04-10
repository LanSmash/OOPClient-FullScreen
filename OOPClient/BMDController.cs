using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BMDSwitcherAPI;
using System.Runtime.InteropServices;

namespace OOPClient
{
    class BMDController
    {
        private IBMDSwitcherDiscovery m_switcherDiscovery;
        private IBMDSwitcher m_switcher;
        private IBMDSwitcherMixEffectBlock m_mixEffectBlock1;
        private IBMDSwitcherTransitionParameters m_transition;

        private IBMDSwitcherKey me1_key1, me1_key2;

        private int onAir_1, onAir_2;

        private SwitcherMonitor m_switcherMonitor;
        private MixEffectBlockMonitor m_mixEffectBlockMonitor;

        private List<InputMonitor> m_inputMonitors;

        public BMDController()
        {
            m_inputMonitors = new List<InputMonitor>();
            m_switcherMonitor = new SwitcherMonitor();
            m_switcherMonitor.SwitcherDisconnected += new SwitcherEventHandler((s, a) => this.Invoke((Action)(() => SwitcherDisconnected())));

            m_mixEffectBlockMonitor = new MixEffectBlockMonitor();
            m_mixEffectBlockMonitor.ProgramInputChanged += new SwitcherEventHandler((s, a) => this.Invoke((Action)(() => UpdateProgramButtonSelection())));
            m_mixEffectBlockMonitor.PreviewInputChanged += new SwitcherEventHandler((s, a) => this.Invoke((Action)(() => UpdatePreviewButtonSelection())));
            m_mixEffectBlockMonitor.InTransitionChanged += new SwitcherEventHandler((s, a) => this.Invoke((Action)(() => OnInTransitionChanged())));

            m_switcherDiscovery = new CBMDSwitcherDiscovery();
            if (m_switcherDiscovery == null)
            {
                //Could not create Switcher Discovery Instance.\nATEM Switcher Software may not be installed. - Exception
                Environment.Exit(1);
            }

            SwitcherDisconnected();
        }

        //When switcher is connected
        private void SwitcherConnected()
        {
            // Install SwitcherMonitor callbacks:
            m_switcher.AddCallback(m_switcherMonitor);

            // We create input monitors for each input. To do this we iterator over all inputs:
            // This will allow us to update the combo boxes when input names change:
            IBMDSwitcherInputIterator inputIterator;
            if (SwitcherAPIHelper.CreateIterator(m_switcher, out inputIterator))
            {
                IBMDSwitcherInput input;
                inputIterator.Next(out input);
                while (input != null)
                {
                    InputMonitor newInputMonitor = new InputMonitor(input);
                    input.AddCallback(newInputMonitor);

                    m_inputMonitors.Add(newInputMonitor);

                    inputIterator.Next(out input);
                }
            }

            // We want to get the first Mix Effect block (ME 1). We create a ME iterator,
            // and then get the first one:
            m_mixEffectBlock1 = null;
            IBMDSwitcherMixEffectBlockIterator meIterator;
            SwitcherAPIHelper.CreateIterator(m_switcher, out meIterator);

            if (meIterator != null)
            {
                meIterator.Next(out m_mixEffectBlock1);
            }

            if (m_mixEffectBlock1 == null)
            {
                //Unexpected: Could not get first mix effect block - Exception
                return;
            }

            // Install MixEffectBlockMonitor callbacks:
            m_mixEffectBlock1.AddCallback(m_mixEffectBlockMonitor);

            me1_key1 = null;
            IBMDSwitcherKeyIterator keyIterator;
            SwitcherAPIHelper.CreateIterator(m_mixEffectBlock1, out keyIterator);
            if (keyIterator != null)
            {
                keyIterator.Next(out me1_key1);
                keyIterator.Next(out me1_key2);
            }
        }

        //When switcher is disconnected
        private void SwitcherDisconnected()
        {
            // Remove all input monitors, remove callbacks
            foreach (InputMonitor inputMon in m_inputMonitors)
            {
                inputMon.Input.RemoveCallback(inputMon);
            }
            m_inputMonitors.Clear();

            if (m_mixEffectBlock1 != null)
            {
                // Remove callback
                m_mixEffectBlock1.RemoveCallback(m_mixEffectBlockMonitor);

                // Release reference
                m_mixEffectBlock1 = null;
            }

            if (m_switcher != null)
            {
                // Remove callback:
                m_switcher.RemoveCallback(m_switcherMonitor);

                // release reference:
                m_switcher = null;
            }
        }

        //Update the buttons with the text
        private void GetSources()
        {
            Dictionary<int,string> sources = new Dictionary<int,string>();

            // Get an input iterator. We use the SwitcherAPIHelper to create the iterator for us:
            IBMDSwitcherInputIterator inputIterator;
            if (!SwitcherAPIHelper.CreateIterator(m_switcher, out inputIterator))
                return;

            string[] ignore = { "Color Bars", "Color 1", "Color 2", "Media Player 1", "Media Player 1 Key", "Media Player 2", "Media Player 2 Key", "Program", "Preview", "Clean Feed 1", "Clean Feed 2" };

            IBMDSwitcherInput input;
            inputIterator.Next(out input);
            while (input != null)
            {
                string inputName;
                long inputId;

                input.GetInputId(out inputId);
                input.GetString(_BMDSwitcherInputPropertyId.bmdSwitcherInputPropertyIdLongName, out inputName);

                // Add items to list
                if (!ignore.Contains(inputName) && inputId < 9)
                {
                    sources.Add((int) inputId, (string) inputName);
                }

                inputIterator.Next(out input);
            }

            UpdateProgramButtonSelection();
            UpdatePreviewButtonSelection();
        }

        //Update program buttons
        private int UpdateProgramButtonSelection()
        {
            long programId;

            m_mixEffectBlock1.GetInt(_BMDSwitcherMixEffectBlockPropertyId.bmdSwitcherMixEffectBlockPropertyIdProgramInput, out programId);

            if (programId >= 0 && programId < 9)
            {
                return (int) programId;
            }
            else
            {
                return 9999;
            }
        }

        //Update preview buttons
        private int UpdatePreviewButtonSelection()
        {
            long previewId;

            m_mixEffectBlock1.GetInt(_BMDSwitcherMixEffectBlockPropertyId.bmdSwitcherMixEffectBlockPropertyIdPreviewInput, out previewId);

            if (previewId >= 0 && previewId < 9)
            {
                return (int)previewId;
            }
            else
            {
                return 9999;
            }
        }

        private void changePrev(long inputId)
        {
            if (m_mixEffectBlock1 != null)
            {
                m_mixEffectBlock1.SetInt(_BMDSwitcherMixEffectBlockPropertyId.bmdSwitcherMixEffectBlockPropertyIdPreviewInput,
                    inputId);
            }
        }

        private void changeProg(long inputId)
        {
            if (m_mixEffectBlock1 != null)
            {
                m_mixEffectBlock1.SetInt(_BMDSwitcherMixEffectBlockPropertyId.bmdSwitcherMixEffectBlockPropertyIdProgramInput,
                    inputId);
            }
        }

        //When connect is pressed on the ATEM connect
        private void Connect(String address)
        {
            _BMDSwitcherConnectToFailure failReason = 0;

            try
            {
                // Note that ConnectTo() can take several seconds to return, both for success or failure,
                // depending upon hostname resolution and network response times, so it may be best to
                // do this in a separate thread to prevent the main GUI thread blocking.
                m_switcherDiscovery.ConnectTo(address, out m_switcher, out failReason);
            }
            catch (COMException)
            {
                // An exception will be thrown if ConnectTo fails. For more information, see failReason.
                /*switch (failReason)
                {
                    case _BMDSwitcherConnectToFailure.bmdSwitcherConnectToFailureNoResponse:
                        MessageBox.Show("No response from Switcher", "Error");
                        break;
                    case _BMDSwitcherConnectToFailure.bmdSwitcherConnectToFailureIncompatibleFirmware:
                        MessageBox.Show("Switcher has incompatible firmware", "Error");
                        break;
                    default:
                        MessageBox.Show("Connection failed for unknown reason", "Error");
                        break;
                }
                return;*/
            }

            SwitcherConnected();
        }

        //Perform auto
        private void performAuto()
        {
            try
            {
                if (m_mixEffectBlock1 != null)
                {
                    m_mixEffectBlock1.PerformAutoTransition();
                }
            }
            catch (Exception e)
            {
                //Warning: Exception - " + e.Message - Exception
            }
        }

        //Perform cut
        private void performCut()
        {
            if (m_mixEffectBlock1 != null)
            {
                m_mixEffectBlock1.PerformCut();
            }
        }

        //Scrolling the track bar (fade)
        private void trackBarTransitionPos_Scroll(object sender, EventArgs e)
        {
            if (m_mixEffectBlock1 != null)
            {

            }
        }

        public int getCurrentTransition()
        {
            int curTrans = 0;

            m_transition = (BMDSwitcherAPI.IBMDSwitcherTransitionParameters)m_mixEffectBlock1;
            _BMDSwitcherTransitionStyle m_style;

            m_transition.GetNextTransitionStyle(out m_style);

            switch (m_style)
            {
                case (_BMDSwitcherTransitionStyle.bmdSwitcherTransitionStyleMix):
                    curTrans = 1;
                    break;

                case (_BMDSwitcherTransitionStyle.bmdSwitcherTransitionStyleDip):
                    curTrans = 2;
                    break;

                case (_BMDSwitcherTransitionStyle.bmdSwitcherTransitionStyleWipe):
                    curTrans = 3;
                    break;

                case (_BMDSwitcherTransitionStyle.bmdSwitcherTransitionStyleStinger):
                    curTrans = 4;
                    break;

                case (_BMDSwitcherTransitionStyle.bmdSwitcherTransitionStyleDVE):
                    curTrans = 5;
                    break;
            }

            return curTrans;
        }

        /// <summary>
        /// Used for putting other object types into combo boxes.
        /// </summary>
        struct StringObjectPair<T>
        {
            public string name;
            public T value;

            public StringObjectPair(string name, T value)
            {
                this.name = name;
                this.value = value;
            }

            public override string ToString()
            {
                return name;
            }
        }
    }
}
