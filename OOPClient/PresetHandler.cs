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
        public event TemplateHandler SendAudio, SendRundown, SendSuperSource;
        

        public PresetHandler()
        {
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

        //Get all presets
        public string[] GetPresets()
        {
            string[] list = Directory.GetFiles(@"Presets\", "*.xml");
            return list.OrderBy(f => f).ToArray();
        }

        public void changePreset(string preset, bool sendAll = true)
        {
            Dictionary<int, int> fader = new Dictionary<int, int>();
            Dictionary<int, bool> mute = new Dictionary<int, bool>();
            List<string> rundown = new List<string>();
            Dictionary<int, Dictionary<string, double>> superSource = new Dictionary<int,Dictionary<string,double>>();
            superSource.Add(0, new Dictionary<string,double>());
            superSource.Add(1, new Dictionary<string,double>());
            superSource.Add(2, new Dictionary<string,double>());
            superSource.Add(3, new Dictionary<string,double>());
            superSource.Add(4, new Dictionary<string,double>());

            string path = "Presets/" + preset + ".xml";
            if (File.Exists(path))
            {
                using (XmlReader reader = XmlReader.Create(path))
                {
                    string section = "";
                    int chan = 0;
                    int box = 0;

                    while (reader.Read())
                    {
                        // Only detect start elements.
                        if (reader.IsStartElement())
                        {
                            // Get element name and switch on it.
                            switch (reader.Name)
                            {
                                /*
                                 * AUDIO SECTION
                                 */
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

                                /*
                                 * RUNDOWN SECTION
                                 */
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
                                            rundown.Add("A|" + reader.Value.Trim());
                                        }
                                    }
                                    break;

                                case "caspar":
                                    if (section == "rundown")
                                    {
                                        if (reader.Read())
                                        {
                                            //Console.WriteLine("CASPAR: " + reader.Value.Trim());
                                            rundown.Add("C|" + reader.Value.Trim());
                                        }
                                    }
                                    break;

                                case "main":
                                    if (section == "rundown")
                                    {
                                        if (reader.Read())
                                        {
                                            //Console.WriteLine("MAIN: " + reader.Value.Trim());
                                            rundown.Add("M|" + reader.Value.Trim());
                                        }
                                    }
                                    break;


                                /*
                                 * SUPERSOURCE SECTION
                                 */
                                case "supersource":
                                    if (section != "supersource")
                                    {
                                        section = "supersource";
                                    }
                                    else
                                    {
                                        section = "";
                                    }
                                    break;

                                case "fill":
                                    if (section == "supersource")
                                    {
                                        if(reader.Read())
                                        {
                                            superSource[box].Add("fill", Convert.ToDouble(reader.Value.Trim()));
                                        }
                                    }
                                    break;

                                case "box1":
                                    if (box != 1 && section == "supersource")
                                    {
                                        box = 1;
                                    }
                                    break;

                                case "box2":
                                    if (box != 2 && section == "supersource")
                                    {
                                        box = 2;
                                    }
                                    break;

                                case "box3":
                                    if (box != 3 && section == "supersource")
                                    {
                                        box = 3;
                                    }
                                    break;

                                case "box4":
                                    if (box != 4 && section == "supersource")
                                    {
                                        box = 4;
                                    }
                                    break;

                                case "art":
                                    if (box != 0 && section == "supersource")
                                    {
                                        box = 0;
                                    }
                                    break;

                                case "enable":
                                    if (box > 0 && box < 5 && section == "supersource")
                                    {
                                        if(reader.Read())
                                        {
                                            double enable = (Convert.ToBoolean(reader.Value.Trim())) ? 1 : 0;
                                            superSource[box].Add("enable", enable);
                                        }
                                    }
                                    break;

                                case "source":
                                    if (box > 0 && box < 5 && section == "supersource")
                                    {
                                        if(reader.Read())
                                        {
                                            superSource[box].Add("source", Double.Parse(reader.Value.Trim()));
                                        }
                                    }
                                    break;

                                case "x":
                                    if (box > 0 && box < 5 && section == "supersource")
                                    {
                                        if (reader.Read())
                                        {
                                            superSource[box].Add("x", Double.Parse(reader.Value.Trim()));
                                        }
                                    }
                                    break;

                                case "y":
                                    if (box > 0 && box < 5 && section == "supersource")
                                    {
                                        if (reader.Read())
                                        {
                                            superSource[box].Add("y", Double.Parse(reader.Value.Trim()));
                                        }
                                    }
                                    break;

                                case "size":
                                    if (box > 0 && box < 5 && section == "supersource")
                                    {
                                        if (reader.Read())
                                        {
                                            superSource[box].Add("size", Double.Parse(reader.Value.Trim()));
                                        }
                                    }
                                    break;

                                case "hue":
                                    if (box == 0 && section == "supersource")
                                    {
                                        if (reader.Read())
                                        {
                                            superSource[box].Add("hue", Double.Parse(reader.Value.Trim()));
                                        }
                                    }
                                    break;

                                case "sat":
                                    if (box == 0 && section == "supersource")
                                    {
                                        if (reader.Read())
                                        {
                                            superSource[box].Add("sat", Double.Parse(reader.Value.Trim()));
                                        }
                                    }
                                    break;

                                case "lum":
                                    if (box == 0 && section == "supersource")
                                    {
                                        if (reader.Read())
                                        {
                                            superSource[box].Add("lum", Double.Parse(reader.Value.Trim()));
                                        }
                                    }
                                    break;

                                case "outerw":
                                    if (box == 0 && section == "supersource")
                                    {
                                        if (reader.Read())
                                        {
                                            superSource[box].Add("outerw", Double.Parse(reader.Value.Trim()));
                                        }
                                    }
                                    break;

                                case "innerw":
                                    if (box == 0 && section == "supersource")
                                    {
                                        if (reader.Read())
                                        {
                                            superSource[box].Add("innerw", Double.Parse(reader.Value.Trim()));
                                        }
                                    }
                                    break;

                                case "outers":
                                    if (box == 0 && section == "supersource")
                                    {
                                        if (reader.Read())
                                        {
                                            superSource[box].Add("outers", Double.Parse(reader.Value.Trim()));
                                        }
                                    }
                                    break;

                                case "inners":
                                    if (box == 0 && section == "supersource")
                                    {
                                        if (reader.Read())
                                        {
                                            superSource[box].Add("inners", Double.Parse(reader.Value.Trim()));
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
                        SendSuperSource("super", superSource);
                        SendRundown("rundown", rundown);
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
