using Svt.Caspar;
using Svt.Network;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace OOPClient
{
    class CasparController
    {
        CultureInfo provider = CultureInfo.InvariantCulture;
        CasparDevice caspar_ = new CasparDevice();
        CasparCGDataCollection cgData = new CasparCGDataCollection();

        private string flashCmd;

        private string CasparChannel;
        private string CasparVideoLayer;
        private string FlashLayer;


        public CasparController()
        {
            //CasparCG Events.
            caspar_.Connected += new EventHandler<NetworkEventArgs>(caspar__Connected);
            caspar_.FailedConnect += new EventHandler<NetworkEventArgs>(caspar__FailedConnected);
            caspar_.Disconnected += new EventHandler<NetworkEventArgs>(caspar__Disconnected);

            //Creates the first part of every CG command ("CG CC-FL")
            flashCmd = "CG " + CasparChannel + "-" + FlashLayer;
        }

        public void Connect(string host, int port) {
            if(!caspar_.IsConnected) {
                caspar_.Settings.Hostname = host; // Properties.Settings.Default.Hostname;
                caspar_.Settings.Port = port;
                caspar_.Connect();
            }
            else
            {
                //Error already connected
            }
        }

        public void Disconnect()
        {
            if (caspar_.IsConnected)
            {
                caspar_.Disconnect();
            }
            else
            {
                //Error not connected
            }
        }

        public void SendRaw(string send)
        {
            caspar_.SendString(send);
        }

        public void SendFlash(string send)
        {
            caspar_.SendString("CG " + CasparChannel + "-" + FlashLayer + " " + send);
        }

        //caspar event - connected
        private void caspar__Connected(object sender, NetworkEventArgs e)
        {
            OnCasparConnected(e);
        }

        private void OnCasparConnected(object param)
        {
            caspar_.RefreshMediafiles();
            caspar_.RefreshDatalist();

            NetworkEventArgs e = (NetworkEventArgs)param;
            //buttonConnect.Enabled = true;
            //updateConnectButtonText();
            //statusStrip1.BackColor = Color.LightGreen;
            //toolStripStatusLabel1.Text = "Connected to " + caspar_.Settings.Hostname; // Properties.Settings.Default.Hostname;
        }

        //caspar event - failed connect
        private void caspar__FailedConnected(object sender, NetworkEventArgs e)
        {
            OnCasparFailedConnect(e);
        }

        private void OnCasparFailedConnect(object param)
        {
            NetworkEventArgs e = (NetworkEventArgs)param;
            //statusStrip1.BackColor = Color.LightCoral;
            //toolStripStatusLabel1.Text = "Failed to connect to " + caspar_.Settings.Hostname; // Properties.Settings.Default.Hostname;
            //buttonConnect.Enabled = true;
            //updateConnectButtonText();
        }

        //caspar event - disconnected
        private void caspar__Disconnected(object sender, NetworkEventArgs e)
        {
            OnCasparDisconnected(e);
        }

        //When caspar is disconnected
        private void OnCasparDisconnected(object param)
        {
            NetworkEventArgs e = (NetworkEventArgs)param;
            //GUI Update
            //Send event!
            //statusStrip1.BackColor = Color.LightCoral;
            //toolStripStatusLabel1.Text = "Disconnected from " + caspar_.Settings.Hostname; // Properties.Settings.Default.Hostname;
            //buttonConnect.Enabled = true;
            //updateConnectButtonText();
        }
    }
}
