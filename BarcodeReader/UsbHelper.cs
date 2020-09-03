using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Management;

namespace BarcodeReader
{
    public class UsbHelper
    {
        public UsbHelper()
        {
            //ManagementObjectCollection collection;
            //using (var searcher = new ManagementObjectSearcher(@"Select * From Win32_USBHub"))
            //    collection = searcher.Get();
            // Get a list of serial port names.
            string[] ports = SerialPort.GetPortNames();

            Console.WriteLine("The following serial ports were found:");

            // Display each port name to the console.
            foreach (string port in ports)
            {
                Console.WriteLine(port);
            }

            Console.ReadLine();

        }

        public List<USBDeviceInfo> GetUSBDevices()
        {
            List<USBDeviceInfo> devices = new List<USBDeviceInfo>();
            ManagementObjectCollection collection;

            using (var searcher = new ManagementObjectSearcher(@"Select * From Win32_USBHub"))
                collection = searcher.Get();

            foreach (var device in collection)
            {
                devices.Add(new USBDeviceInfo(
                    (string)device.GetPropertyValue("DeviceID"),
                    (string)device.GetPropertyValue("PNPDeviceID"),
                    (string)device.GetPropertyValue("Description")
                    ));
            }
            collection.Dispose();
            return devices;
        }
    }
}
