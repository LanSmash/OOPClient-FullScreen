using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sanford.Multimedia.Midi;
using Sanford.Threading;

namespace OOPClient
{
    class LaunchPadController
    {
        int OFF = 12;
        int RED_LOW = 13;
        int RED_FULL = 15;
        int AMB_LOW = 29;
        int AMB_FULL = 63;
        int GRN_LOW = 28;
        int GRN_FULL = 60;

        Dictionary<int, int> keyColor;

        OutputDevice outDevice;
        InputDevice inDevice;

        public delegate void ButtonHandler(int row, int col);
        public event ButtonHandler ButtonPressed;

        public LaunchPadController()
        {
            keyColor = new Dictionary<int, int>();
        }

        //Open input and output midi device
        public void Open(int input, int output) {
            outDevice = new OutputDevice(output);

            inDevice = new InputDevice(input);
            inDevice.SysExBufferSize = 16;
            inDevice.ChannelMessageReceived += inDevice_ButtonPressed;
            inDevice.Error += inDevice_Error;
            inDevice.StartRecording();

            Clear();
        }

        void inDevice_Error(object sender, Sanford.Multimedia.ErrorEventArgs e)
        {
            
        }

        public void Clear() {
            ChannelMessageBuilder builder = new ChannelMessageBuilder();
            for (int i = 0; i < 8; i++)
            {
                for (int p = 0; p < 9; p++)
                {
                    int channel = ToChannel(i, p);
                    builder.Command = ChannelCommand.NoteOn;
                    builder.MidiChannel = 0;
                    builder.Data1 = channel;
                    builder.Data2 = 12;
                    builder.Build();
                    outDevice.Send(builder.Result);
                    keyColor[channel] = 0;
                }
            }
        }

        //Handle button presses
        private void inDevice_ButtonPressed(object sender, ChannelMessageEventArgs e)
        {
            if (e.Message.Data2 == 127)
            {
                ButtonPressed(GetRow(e.Message.Data1), GetCol(e.Message.Data1));
            }
        }

        //Channels - see Launchpad Programmers Reference PDF from Novation
        private int ToChannel(int row, int col)
        {
            return (16 * row) + col;
        }

        //Get row
        private int GetRow(int channel)
        {
            return (int)Math.Floor((double)((channel) / 16));
        }

        //Get column
        private int GetCol(int channel)
        {
            return (int)channel % 16;
        }

        /*
         * Colors:
         * 0 = OFF
         * 1 = RED LOW
         * 2 = RED HIGH
         * 3 = AMB LOW
         * 4 = AMB HIGH
         * 5 = GRN LOW
         * 6 = GRN HIGH
         */
        private int ToColor(int color)
        {
            if (color == 0)
            {
                return OFF;
            }
            else if (color == 1)
            {
                return RED_LOW;
            }
            else if (color == 2)
            {
                return RED_FULL;
            }
            else if (color == 3)
            {
                return AMB_LOW;
            }
            else if (color == 4)
            {
                return AMB_FULL;
            }
            else if (color == 5)
            {
                return GRN_LOW;
            }
            else if (color == 6)
            {
                return GRN_FULL;
            }
            else
            {
                return OFF;
            }
        }

        //Set a color
        public void SetColor(int row, int col, int color)
        {
            ChannelMessageBuilder builder = new ChannelMessageBuilder();
            builder.Command = ChannelCommand.NoteOn;
            builder.MidiChannel = 0;
            builder.Data1 = ToChannel(row, col);
            builder.Data2 = ToColor(color);
            builder.Build();
            outDevice.Send(builder.Result);
            keyColor[ToChannel(row, col)] = color;
        }

        //Get a color
        public int GetColor(int row, int col)
        {
            return keyColor[ToChannel(row, col)];
        }

        //Uninitialize
        public void Close()
        {
            inDevice.StopRecording();
            outDevice.Close();
        }
    }
}
