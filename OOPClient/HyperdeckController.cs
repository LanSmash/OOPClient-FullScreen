using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace OOPClient
{
    class HyperdeckController
    {
        private IPAddress ip;
        private int port;
        TcpClient tcp;

        public HyperdeckController()
        {
            port = 9993;
            tcp = new TcpClient();
        }

        public void Connect(string _ip)
        {
            try
            {
                ip = IPAddress.Parse(_ip);
                tcp.Connect(ip, port);
                SendMessage("remote: enable: true");
            }
            catch (Exception e)
            {
                throw new System.SystemException("Unexpected: " + e.Message);
            }
        }

        public void SendMessage(string message)
        {
            try
            {
                if (tcp.Connected)
                {
                    Stream stm = tcp.GetStream();
                    ASCIIEncoding asen = new ASCIIEncoding();
                    byte[] ba = asen.GetBytes(message);
                    stm.Write(ba, 0, ba.Length);
                }
            }
            catch (Exception e)
            {
                throw new System.SystemException("Unexpected: " + e.Message);
            }
        }

        public void Play(int speed = 100)
        {
            //Between -1600 and 1600 - percantage of speed (100% = normal)
            SendMessage("play: speed: " + speed);
        }

        public void Record()
        {
            SendMessage("record");
        }

        public void Stop()
        {
            SendMessage("stop");
        }

        public void GotoEndClip()
        {
            SendMessage("goto: clip: \"start\"");
        }

        public void GotoEndTimeline()
        {
            SendMessage("goto: timeline: \"end\"");
        }

        public void Disconnect()
        {
            try
            {
                if (tcp.Connected)
                {
                    SendMessage("quit");
                    tcp.Close();
                }
            }
            catch (Exception e)
            {
                throw new System.SystemException("Unexpected: " + e.Message);
            }
        }
    }
}
