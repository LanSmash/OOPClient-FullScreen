using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

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

        public bool Connected()
        {
            return tcp.Connected;
        }

        public void SendMessage(string message)
        {
            try
            {
                if (tcp.Connected)
                {
                    NetworkStream stream;
                    stream = tcp.GetStream();
                    byte[] messageBytes = Encoding.ASCII.GetBytes(message + System.Environment.NewLine);
                    stream.Write(messageBytes, 0, messageBytes.Length);
                    Console.WriteLine("WRITE:: " + Encoding.ASCII.GetString(messageBytes));                  
                }
            }
            catch (Exception e)
            {
                throw new System.SystemException("Unexpected: " + e.Message);
            }
        }

        static byte[] StringToBytes(string str)
        {
            byte[] bytes = new byte[str.Length * sizeof(char)];
            System.Buffer.BlockCopy(str.ToCharArray(), 0, bytes, 0, bytes.Length);
            return bytes;
        }

        static string BytesToString(byte[] bytes)
        {
            char[] chars = new char[bytes.Length / sizeof(char)];
            System.Buffer.BlockCopy(bytes, 0, chars, 0, bytes.Length);
            return new string(chars);
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
