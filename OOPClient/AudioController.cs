using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Sanford.Multimedia.Midi;
using Sanford.Threading;
using System.Threading;
using System.Text.RegularExpressions;

namespace OOPClient
{
    public class AudioController
    {
        OutputDevice outDevice;
        ChannelMessageBuilder builder;
        InputDevice inDevice;

        public bool debugMode = false;

        public delegate void InputHandler(int channel, int val);
        public event InputHandler TrackBarChange;
        public event InputHandler CheckBoxChange;

        public AudioController()
        {
            builder = new ChannelMessageBuilder();
            builder.Command = ChannelCommand.Controller;
            builder.MidiChannel = 0;
        }

        public void Open(int input, int output)
        {
            outDevice = new OutputDevice(output);

            inDevice = new InputDevice(input);
            inDevice.SysExBufferSize = 128;
            inDevice.StartRecording();
            inDevice.ChannelMessageReceived += inDevice_ChannelMessageReceived;
        }

        void inDevice_ChannelMessageReceived(object sender, ChannelMessageEventArgs e1)
        {
            if (debugMode)
            {
                Console.WriteLine("Channel: " + e1.Message.Data1 + " - Value: " + e1.Message.Data2 + " - Command type: " + e1.Message.Command + " - Message type: " + e1.Message.MessageType);
            }

            if (e1.Message.Data1 < 15 || e1.Message.Data1 == 27)
            {
                TrackBarChange(e1.Message.Data1, e1.Message.Data2);
            }
            else if (e1.Message.Data1 > 27 && e1.Message.Data1 < 32 || e1.Message.Data1 > 32 && e1.Message.Data1 < 43 || e1.Message.Data1 == 55)
            {
                CheckBoxChange(e1.Message.Data1, e1.Message.Data2);
            }
        }

        public void programChangeSlider(int channel, int value, Boolean updateUI = true)
        {
            if (channel < 16 && channel > 0 || channel == 27)
            {
                channel = (channel == 15) ? 27 : channel;

                changeSliderValue(channel, value);

                if (updateUI)
                {
                    TrackBarChange(channel, value);
                }
            }
        }

        public void programChangeOnOff(int channel, Boolean isOn, Boolean updateUI = true)
        {
            int val = (isOn) ? 127 : 0;

            if (updateUI)
            {
                if (channel < 16 && channel > 0)
                {
                    if (channel == 15)
                    {
                        channel = 55;
                    }
                    else if (channel > 4)
                    {
                        channel += 28;
                    }
                    else
                    {
                        channel += 27;
                    }

                    CheckBoxChange(channel, val);
                }
            }

            switchOnOff(channel, isOn);
        }

        public void Close()
        {
            inDevice.StopRecording();
            outDevice.Close();
        }

        private void switchOnOff(int channel, Boolean isOn)
        {
            builder.Data1 = channel;
            if (isOn)
            {
                builder.Data2 = 127;
            }
            else
            {
                builder.Data2 = 0;
            }
            builder.Build();
            outDevice.Send(builder.Result);
        }

        private void changeSliderValue(int channel, int value)
        {
            ChannelMessageBuilder builderSupreme = new ChannelMessageBuilder();
            builderSupreme.MidiChannel = 0;
            builderSupreme.Data1 = channel;
            builderSupreme.Data2 = value;
            builderSupreme.Build();
            outDevice.Send(builderSupreme.Result);
        }
    }
}