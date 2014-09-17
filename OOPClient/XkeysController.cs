using PIEHidDotNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OOPClient
{
    class XkeysController : PIEDataHandler, PIEErrorHandler
    {
        PIEDevice[] devices;

        int[] cbotodevice; //for each item in the CboDevice list maps this index to the device index.  Max devices =100 
        byte[] wData; //write data buffer
        int selecteddevice; //set to the index of CboDevice

        int[] prevKeys;
        Dictionary<int, string> CboDevices;

        //Delegate for events
        public delegate void ControlEventHandler(int row, int col);

        //Events
        public event ControlEventHandler XKeyPressed;

        public XkeysController()
        {
            try
            {
                CboDevices = new Dictionary<int, string>();

                cbotodevice = null; //for each item in the CboDevice list maps this index to the device index.  Max devices =100 
                wData = null; //write data buffer
                selecteddevice = -1; //set to the index of CboDevice

                prevKeys = new int[10] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
            }
            catch (Exception ex)
            {
                throw new System.SystemException("ERROR: " + ex.Message.ToString());
            }
        }

        public void selectDevice(int _id)
        {
            try
            {
                selecteddevice = cbotodevice[_id];
                wData = new byte[devices[selecteddevice].WriteLength];//size write array
            }
            catch (Exception ex)
            {
                throw new System.SystemException("ERROR: " + ex.Message.ToString());
            }
        }

        public Dictionary<int,string> enumerateDevices()
        {
            try
            {
                CboDevices.Clear();
                cbotodevice = new int[128]; //128=max # of devices
                //enumerate and setupinterfaces for all devices
                devices = PIEHidDotNet.PIEDevice.EnumeratePIE();

                if (devices.Length > 0)
                {
                    int cbocount = 0; //keeps track of how many valid devices were added to the CboDevice box
                    for (int i = 0; i < devices.Length; i++)
                    {
                        //information about device
                        //PID = devices[i].Pid);
                        //HID Usage = devices[i].HidUsage);
                        //HID Usage Page = devices[i].HidUsagePage);
                        //HID Version = devices[i].Version);
                        if (devices[i].HidUsagePage == 0xc)
                        {
                            switch (devices[i].Pid)
                            {
                                case 1121:
                                    CboDevices.Add(i, "X-keys XK60 YAY!");
                                    cbotodevice[cbocount] = i;
                                    cbocount++;
                                    break;
                                default:
                                    CboDevices.Add(i, "Unknown Device (" + devices[i].Pid + ")");
                                    cbotodevice[cbocount] = i;
                                    cbocount++;
                                    break;
                            }
                            devices[i].SetupInterface();
                        }
                    }
                }

                if (CboDevices.Count > 0)
                {
                    selecteddevice = cbotodevice[0];
                    wData = new byte[devices[selecteddevice].WriteLength];//go ahead and setup for write
                }

                return CboDevices;
            }
            catch (Exception ex)
            {
                throw new System.SystemException("ERROR: " + ex.Message.ToString());
            }
        }


        public void HandlePIEHidData(Byte[] data, PIEDevice sourceDevice)
        {
            try
            {
                if (sourceDevice == devices[selecteddevice])
                {
                    byte[] rdata = null;
                    while (0 == sourceDevice.ReadData(ref rdata)) //do this so don't ever miss any data
                    {
                        string thispid = sourceDevice.Pid.ToString();

                        //write raw data to listbox1
                        for (int i = 3; i < 13; i++)
                        {
                            int keys = int.Parse(rdata[i].ToString());

                            string test = getKeys(keys);

                            int p = i - 3;

                            if (test.Length > 1 && (keys > prevKeys[p]))
                            {
                                String rows = getKeys(keys - prevKeys[p]);
                                prevKeys[p] = keys;
                                XKeyPressed(int.Parse(rows.Remove(0, 1)), p);
                            }
                            else if (keys == 0)
                            {
                                prevKeys[p] = 0;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new System.SystemException("ERROR: " + ex.Message.ToString());
            }
        }

        private string getKeys(int number)
        {
            try
            {
                if (number <= 0)
                {
                    return "";
                }
                else if (number == 1)
                {
                    return "R1 ";
                }
                else if (number >= 128)
                {
                    return "R8 " + getKeys(number - 128);
                }
                else if (number >= 64)
                {
                    return "R7 " + getKeys(number - 64);
                }
                else if (number >= 32)
                {
                    return "R6 " + getKeys(number - 32);
                }
                else if (number >= 16)
                {
                    return "R5 " + getKeys(number - 16);
                }
                else if (number >= 8)
                {
                    return "R4 " + getKeys(number - 8);
                }
                else if (number >= 4)
                {
                    return "R3 " + getKeys(number - 4);
                }
                else if (number >= 2)
                {
                    return "R2 " + getKeys(number - 2);
                }
                else
                {
                    return "";
                }
            }
            catch (Exception ex)
            {
                throw new System.SystemException("ERROR: " + ex.Message.ToString());
            }
        }
        //error callback
        public void HandlePIEHidError(Int32 error, PIEDevice sourceDevice)
        {
            //throw new System.SystemException("Error: " + error.ToString());
            Console.WriteLine("ERROR " + error.ToString() + " ON " + sourceDevice.Pid);
        }

        public void SetCallback()
        {
            try
            {
                if (selecteddevice != -1)
                {
                    for (int i = 0; i < CboDevices.Count; i++)
                    {
                        //use the cbotodevice array which contains the mapping of the devices in the CboDevices to the actual device IDs
                        devices[cbotodevice[i]].SetErrorCallback(this);
                        devices[cbotodevice[i]].SetDataCallback(this, DataCallbackFilterType.callOnChangedData);
                    }
                }
            }
            catch (Exception ex)
            {
                throw new System.SystemException("ERROR: " + ex.Message.ToString());
            }
        }

        public void Close()
        {
            try
            {
                for (int i = 0; i < CboDevices.Count; i++)
                {
                    devices[i].CloseInterface();
                }
            }
            catch (Exception ex)
            {
                throw new System.SystemException("ERROR: " + ex.Message.ToString());
            }
        }
    }
}
