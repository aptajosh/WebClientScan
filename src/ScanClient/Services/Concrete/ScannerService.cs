using ScanClient.Core;

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.IO;
using System.Configuration;
using System.Drawing;

namespace ScanClient.Service
{
    class ScannerService : IScannerService
    {
        public List<ScannerDevice> GetDevices()
        {
            return WIAScanner.GetDevices();
        }

        public string Scan(string deviceId)
        {   
            try
            {
                ScanSettings settings = new ScanSettings();
                settings.DeviceId = deviceId;
                var image= WIAScanner.Scan(settings);
                return image.First(); 
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                Debug.WriteLine(String.Format("Device Id is {0}",deviceId));
                return ex.Message;
            }
        }


        public string ScanDefault()
        {
            var devices = this.GetDevices();
            try
            {
                ScanSettings settings = new ScanSettings();
                settings.DeviceId = devices.FirstOrDefault().DeviceId;
                var images=WIAScanner.Scan(settings);
                return images.First();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return  "Error: "+ex.Message;
            }

        }

        public string ScanWithSettings(int intent, string deviceId)
        {
            try
            {
                StatusVariables.Path = "";
                ScanSettings settings = new ScanSettings();
                settings.DeviceId = deviceId;
                settings.WIA_Intent = intent;
                var image=WIAScanner.Scan(settings);
                return image.First();  
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return "Error: "+String.Empty;
            }

            
        }


    }
}
