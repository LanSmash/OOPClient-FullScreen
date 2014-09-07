using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml;

namespace OOPClient
{
    class PresetHandler
    {
        private string currentPreset = "";

        public delegate void TemplateHandler(string type, object args);
        public event TemplateHandler SendAudio, SendRundown;
        

        public PresetHandler(string preset)
        {
            if (preset != "")
            {
                changePreset(preset);
            }
        }

        public string getCurrent()
        {
            return currentPreset;
        }

        public void changeToNone() {
            currentPreset = "";
        }

        public void revertToPreset()
        {
            changePreset(currentPreset, false);
        }

        public void pushToPreset(Dictionary<int, bool> mute, Dictionary<int, int> faders)
        {
            string path = "Presets/" + currentPreset + ".xml";
            if (File.Exists(path))
            {
                XmlDocument pushXml = new XmlDocument();
                pushXml.Load(path);

                XmlNodeList pushNodes = pushXml.SelectNodes("/control/audio/chan");
                foreach (XmlNode node in pushNodes)
                {
                    XmlNode xmlNum = node.SelectSingleNode("num");
                    XmlNode xmlMute = node.SelectSingleNode("on");
                    XmlNode xmlFader = node.SelectSingleNode("fader");

                    int chan = Int32.Parse(xmlNum.InnerText);

                    xmlMute.InnerText = mute[chan].ToString();
                    xmlFader.InnerText = faders[chan].ToString();
                }

                pushXml.Save(path);
            }
        }

        public void changePreset(string preset, bool sendAll = true)
        {
            Dictionary<int, int> fader = new Dictionary<int, int>();
            Dictionary<int, bool> mute = new Dictionary<int, bool>();
            List<string> atem = new List<string>();
            List<string> caspar = new List<string>();
            List<string> main = new List<string>();

            string path = "Presets/" + preset + ".xml";
            if (File.Exists(path))
            {
                using (XmlReader reader = XmlReader.Create(path))
                {
                    string section = "";
                    int chan = 0;

                    while (reader.Read())
                    {
                        // Only detect start elements.
                        if (reader.IsStartElement())
                        {
                            // Get element name and switch on it.
                            switch (reader.Name)
                            {
                                //Audio section
                                case "audio":
                                    // Detect this element.
                                    if (section != "audio")
                                    {
                                        section = "audio";
                                        //Console.WriteLine("START audio");
                                    }
                                    else
                                    {
                                        section = "";
                                    }
                                    break;
                                //Pick channel
                                case "num":
                                    if(section == "audio") {
                                        if (reader.Read())
                                        {
                                            chan = Int32.Parse(reader.Value.Trim());
                                        }
                                    }
                                    break;
                                //Set mute
                                case "on":
                                    if (section == "audio" && chan > 0)
                                    {
                                        //Turn on CHAN
                                        if (reader.Read())
                                        {
                                            mute[chan] = Convert.ToBoolean(reader.Value.Trim());
                                        }
                                    }
                                    break;
                                //Set fader level
                                case "fader":
                                    if (section == "audio" && chan > 0)
                                    {
                                        if (reader.Read())
                                        {
                                            fader[chan] = Int32.Parse(reader.Value.Trim());
                                        }
                                    }
                                    break;

                                //Rundown section
                                case "rundown":
                                    if (section != "rundown")
                                    {
                                        section = "rundown";
                                    }
                                    else
                                    {
                                        section = "";
                                    }
                                    break;

                                case "atem":
                                    if (section == "rundown")
                                    {
                                        if (reader.Read())
                                        {
                                            //Console.WriteLine("ATEM: " + reader.Value.Trim());
                                            atem.Add(reader.Value.Trim());
                                        }
                                    }
                                    break;

                                case "caspar":
                                    if (section == "rundown")
                                    {
                                        if (reader.Read())
                                        {
                                            //Console.WriteLine("CASPAR: " + reader.Value.Trim());
                                            caspar.Add(reader.Value.Trim());
                                        }
                                    }
                                    break;

                                case "main":
                                    if (section == "rundown")
                                    {
                                        if (reader.Read())
                                        {
                                            //Console.WriteLine("MAIN: " + reader.Value.Trim());
                                            main.Add(reader.Value.Trim());
                                        }
                                    }
                                    break;
                            }
                        }
                    }
                }

                currentPreset = preset;
                if (SendAudio != null && SendRundown != null)
                {
                    SendAudio("mute", mute);
                    SendAudio("fader", fader);

                    if (sendAll == true)
                    {
                        SendRundown("atem", atem);
                        SendRundown("caspar", caspar);
                        SendRundown("main", main);
                    }
                }
            }
            else
            {
                throw new System.SystemException("Preset not found!");
            }
        }
    }
}
