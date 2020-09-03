using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BarcodeReader
{
    public class USBDeviceInfo
    {
        public USBDeviceInfo(string deviceID, string pnpDeivceID, string description)
        {
            this.DeviceID = deviceID;
            this.PnpDeviceID = pnpDeivceID;
            this.Description = description;
        }
        public string DeviceID
        {
            get;
            private set;
        }
        public string PnpDeviceID
        {
            get;
            private set;
        }
        public string Description
        {
            get;
            private set;
        }
    }
}
