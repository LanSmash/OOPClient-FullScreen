using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using BMDSwitcherAPI;
using System.Windows.Forms;

namespace OOPClient
{
    public class ATEMController
    {
        private IBMDSwitcherDiscovery m_switcherDiscovery;
        private IBMDSwitcher m_switcher;
        public IBMDSwitcherMixEffectBlock m_mixEffectBlock1;
        private IBMDSwitcherTransitionParameters m_transition;

        //Switcher keys (usk)
        private IBMDSwitcherKey me1_key1, me1_key2;

        //Monitors
        private SwitcherMonitor m_switcherMonitor;
        private MixEffectBlockMonitor m_mixEffectBlockMonitor;
        private List<InputMonitor> m_inputMonitors;

        //Delegate for events
        public delegate void SwitcherEventHandler(int id);

        //Events
        public event SwitcherEventHandler SwitcherDisconnected2;
        public event SwitcherEventHandler SwitcherConnected2;
        public event SwitcherEventHandler UpdateProgramButtonSelection2;
        public event SwitcherEventHandler UpdatePreviewButtonSelection2;

        public ATEMController()
        {
            m_inputMonitors = new List<InputMonitor>();
            m_switcherMonitor = new SwitcherMonitor();
            m_switcherMonitor.SwitcherDisconnected += SwitcherDisconnectedFix;

            m_mixEffectBlockMonitor = new MixEffectBlockMonitor();

            /* Old events
            m_mixEffectBlockMonitor.ProgramInputChanged += UpdateProgramButtonSelection;
            m_mixEffectBlockMonitor.PreviewInputChanged += UpdatePreviewButtonSelection; */

            //Fix
            m_mixEffectBlockMonitor.ProgramInputChanged += UpdateProgramFix;
            m_mixEffectBlockMonitor.PreviewInputChanged += UpdatePreviewFix;

            m_switcherDiscovery = new CBMDSwitcherDiscovery();

            if (m_switcherDiscovery == null)
            {
                throw new System.SystemException("Could not create Switcher Discovery Instance.\nATEM Switcher Software may not be installed.");
            }

            SwitcherDisconnectedFix(null, null);
        }

        //When switcher is connected
        private void SwitcherConnected()
        {
            // Install SwitcherMonitor callbacks:
            m_switcher.AddCallback(m_switcherMonitor);

            // Input monitors for all inputs
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
                throw new System.SystemException("Unexpected: Could not get first mix effect block.");
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

            SwitcherConnected2(1);
        }

        //When switcher disconnected
        private void SwitcherDisconnectedFix(object sender, object args)
        {
            //Hack! Avoiding NullReferenceException - haven't looked into it yet. If you have a fix, please post on git!
            try
            {
                SwitcherDisconnected2(999);
            }
            catch (Exception)
            {

            }
        }

        //When switcher is disconnected
        public void SwitcherDisconnected(object sender, object args)
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

            SwitcherDisconnected2(1);
        }

        public long[] allowedInputs()
        {
            long[] allowedInputs = { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 1000, 6000 };
            return allowedInputs;
        }

        //Update the buttons with the text
        /* Here is a list of inputIds for the sources:
         *  0 - Black
         *  1 - GAME 1
         *  2 - GAME 2
         *  3 - CASTERS
         *  4 - STAGE
         *  5 - INTERVIEW
         *  6 - OVERVIEW
         *  7 - Camera 7
         *  8 - PLAYERCAM LEFT
         *  9 - PLAYERCAM RIGHT
         *  10 - Camera 10
         *  11 - Camera 11
         *  12 - Camera 12
         *  13 - Camera 13
         *  14 - Camera 14
         *  15 - Camera 15
         *  16 - Camera 16
         *  17 - Camera 17
         *  18 - Camera 18
         *  19 - KEY
         *  20 - FILL
         *  1000 - Color Bars
         *  2001 - Color 1
         *  2002 - Color 2
         *  3010 - Media Player 1
         *  3011 - Media Player 1 Key
         *  3020 - Media Player 2
         *  3021 - Media Player 2 Key
         *  4010 - ME 1 Key 1 Mask
         *  4020 - ME 1 Key 2 Mask
         *  4030 - ME 2 Key 1 Mask
         *  4040 - ME 2 Key 2 Mask
         *  5010 - DSK 1 Mask
         *  5020 - DSK 2 Mask
         *  6000 - SuperSource
         *  7001 - Clean Feed 1
         *  7002 - Clean Feed 2
         *  8001 - Auxiliary 1
         *  8002 - Auxiliary 2
         *  8003 - Auxiliary 3
         *  8004 - Auxiliary 4
         *  8005 - Auxiliary 5
         *  8006 - Auxiliary 6
         *  10010 - ME 1 Prog
         *  10011 - ME 1 Prev
         *  10020 - ME 2 Prog
         *  10021 - ME 2 Prev
        */
        public Dictionary<int,string> GetSources()
        {
            Dictionary<int,string> sources = new Dictionary<int,string>();

            // Get an input iterator. We use the SwitcherAPIHelper to create the iterator for us:
            IBMDSwitcherInputIterator inputIterator;
            if (!SwitcherAPIHelper.CreateIterator(m_switcher, out inputIterator))
                return sources;

            //string[] ignore = { "Color Bars", "Color 1", "Color 2", "Media Player 1", "Media Player 1 Key", "Media Player 2", "Media Player 2 Key", "Program", "Preview", "Clean Feed 1", "Clean Feed 2" };

            IBMDSwitcherInput input;
            inputIterator.Next(out input);
            while (input != null)
            {
                string inputName;
                long inputId;

                input.GetInputId(out inputId);
                input.GetString(_BMDSwitcherInputPropertyId.bmdSwitcherInputPropertyIdLongName, out inputName);

                // Add items to list
                if (allowedInputs().Contains(inputId))
                {
                    sources.Add((int) inputId, (string) inputName);
                }

                inputIterator.Next(out input);
                //Console.WriteLine(inputId + " - " + inputName);
            }

            UpdateProgramButtonSelection(null,null);
            UpdatePreviewButtonSelection(null,null);

            return sources;
        }

        /*
         * I'm having an issue with some COM/threading thing, so if I don't apply these fixes so that the UpdateProgram/PreviewButtonSelection aren't called from
         * main GUI thread (in GUI.cs) then I get a cast error. If you want to try and help me, please post on git!
         */
        private void UpdateProgramFix(object sender, object args)
        {
            UpdateProgramButtonSelection2(999);
        }

        private void UpdatePreviewFix(object sender, object args)
        {
            UpdatePreviewButtonSelection2(999);
        }

        //Update program buttons
        public void UpdateProgramButtonSelection(object sender, object args)
        {
            long programId;

            if (m_mixEffectBlock1 != null)
            {
                m_mixEffectBlock1.GetInt(_BMDSwitcherMixEffectBlockPropertyId.bmdSwitcherMixEffectBlockPropertyIdProgramInput, out programId);

                UpdateProgramButtonSelection2((int)programId);
            }
        }

        //Update preview buttons
        public void UpdatePreviewButtonSelection(object sender, object args)
        {
            long previewId;

            if (m_mixEffectBlock1 != null)
            {
                m_mixEffectBlock1.GetInt(_BMDSwitcherMixEffectBlockPropertyId.bmdSwitcherMixEffectBlockPropertyIdPreviewInput, out previewId);
                
                UpdatePreviewButtonSelection2((int)previewId);
            }
        }

        //Change program
        public void changeProg(long inputId)
        {
            if (m_mixEffectBlock1 != null)
            {
                m_mixEffectBlock1.SetInt(_BMDSwitcherMixEffectBlockPropertyId.bmdSwitcherMixEffectBlockPropertyIdProgramInput, inputId);
            }
        }

        //Change preview
        public void changePrev(long inputId)
        {
            if (m_mixEffectBlock1 != null)
            {
                m_mixEffectBlock1.SetInt(_BMDSwitcherMixEffectBlockPropertyId.bmdSwitcherMixEffectBlockPropertyIdPreviewInput, inputId);
            }
        }

        //When connect is pressed on the ATEM connect
        public void Connect(String address)
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
                if (failReason == _BMDSwitcherConnectToFailure.bmdSwitcherConnectToFailureNoResponse)
                {
                    throw new SystemException("No response from Switcher");
                }
                else if (failReason == _BMDSwitcherConnectToFailure.bmdSwitcherConnectToFailureIncompatibleFirmware)
                {
                    throw new SystemException("Switcher has incompatible firmware");
                }
                else
                {
                    throw new SystemException("Connection failed for unknown reason");
                }
            }
            
            SwitcherConnected();
        }

        //Perform auto
        public void performAuto()
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
                throw new System.SystemException("Warning: Exception - " + e.Message);
            }
        }

        //Perform cut
        public void performCut()
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
