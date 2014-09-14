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

        private string CasparChannel;
        private string CasparVideoLayer;
        private string FlashLayer;

        //Delegate for events
        public delegate void CasparEventHandler(string message);

        //Events
        public event CasparEventHandler OnCasparConnected2, OnCasparFailedConnect, OnCasparDisconnected;

        public CasparController()
        {
            //CasparCG Events.
            caspar_.Connected += new EventHandler<NetworkEventArgs>(caspar__Connected);
            caspar_.FailedConnect += new EventHandler<NetworkEventArgs>(caspar__FailedConnected);
            caspar_.Disconnected += new EventHandler<NetworkEventArgs>(caspar__Disconnected);
        }

        public void Connect(string host, int port) {
            try
            {
                if (!caspar_.IsConnected)
                {
                    caspar_.Settings.Hostname = host;
                    caspar_.Settings.Port = port;
                    caspar_.Connect();
                }
                else
                {
                    caspar_.Disconnect();
                }
            }
            catch (Exception e)
            {
                throw new System.SystemException("Unexpected: " + e.Message);
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
            OnCasparConnected2("Connected");
        }

        //caspar event - failed connect
        private void caspar__FailedConnected(object sender, NetworkEventArgs e)
        {
            OnCasparFailedConnect("Couldn't connect to the CasparCG server!");
        }

        //caspar event - disconnected
        private void caspar__Disconnected(object sender, NetworkEventArgs e)
        {
            OnCasparDisconnected("Disconnected");
        }
    }
}
