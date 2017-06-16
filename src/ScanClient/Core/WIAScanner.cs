using System;
using System.Collections.Generic;
using System.IO;
using System.Drawing;
using WIA;
using System.Diagnostics;
using System.Drawing.Imaging;
using System.Configuration;

namespace ScanClient.Core
{
    class WIAScanner
    {
        const string wiaFormatBMP = "{B96B3CAB-0728-11D3-9D7B-0000F81EF32E}";
        class WIA_DPS_DOCUMENT_HANDLING_SELECT
        {
            public const uint FEEDER = 0x00000001;
            public const uint FLATBED = 0x00000002;
        }
        class WIA_DPS_DOCUMENT_HANDLING_STATUS
        {
            public const uint FEED_READY = 0x00000001;
        }
        class WIA_PROPERTIES
        {
            public const uint WIA_RESERVED_FOR_NEW_PROPS = 1024;
            public const uint WIA_DIP_FIRST = 2;
            public const uint WIA_DPA_FIRST = WIA_DIP_FIRST + WIA_RESERVED_FOR_NEW_PROPS;
            public const uint WIA_DPC_FIRST = WIA_DPA_FIRST + WIA_RESERVED_FOR_NEW_PROPS;
            //
            // Scanner only device properties (DPS)
            //
            public const uint WIA_DPS_FIRST = WIA_DPC_FIRST + WIA_RESERVED_FOR_NEW_PROPS;
            public const uint WIA_DPS_DOCUMENT_HANDLING_STATUS = WIA_DPS_FIRST + 13;
            public const uint WIA_DPS_DOCUMENT_HANDLING_SELECT = WIA_DPS_FIRST + 14;
        }

        private static void AdjustScannerSettings(IItem scannnerItem, int currentIntent=1)
        {

            const string WIA_IPS_CUR_INTENT = "6146";
            
            SetWIAProperty(scannnerItem.Properties, WIA_IPS_CUR_INTENT, currentIntent);
        }

        private static void SetWIAProperty(IProperties properties, object propName, object propValue)
        {
            Property prop = properties.get_Item(ref propName);
            prop.set_Value(ref propValue);
        }

        /// <summary>
        /// Use scanner to scan an image (with user selecting the scanner from a dialog).
        /// </summary>
        /// <returns>Scanned images. (Base64 strings)</returns>
        public static List<string> Scan()
        {
            WIA.ICommonDialog dialog = new WIA.CommonDialog();
            WIA.Device device = dialog.ShowSelectDevice(WIA.WiaDeviceType.UnspecifiedDeviceType, true, false);
            ScanSettings settings = new ScanSettings();
            settings.DeviceId = device.DeviceID;
            if (device != null)
            {
                return Scan(settings);
            }
            else
            {
                throw new Exception("You must select a device for scanning.");
            }
        }

        /// <summary>
        /// Use scanner to scan an image (scanner is selected by its unique id).
        /// </summary>
        /// <param name="scannerName"></param>
        /// <returns>Scanned images.</returns>
        public static List<string> Scan(ScanSettings settings)
        {
           List<string> images = new List<string>();
            bool hasMorePages = true;
            while (hasMorePages)
            {
                // select the correct scanner using the provided scannerId parameter
                WIA.DeviceManager manager = new WIA.DeviceManager();
                WIA.Device device = null;
                foreach (WIA.DeviceInfo info in manager.DeviceInfos)
                {
                    if (info.DeviceID == settings.DeviceId)
                    {
                        // connect to scanner
                        device = info.Connect();
                        break;
                    }
                }
                // device was not found
                if (device == null)
                {
                    // enumerate available devices
                    string availableDevices = "";
                    foreach (WIA.DeviceInfo info in manager.DeviceInfos)
                    {
                        availableDevices += info.DeviceID + "\n";
                    }

                    // show error with available devices
                    throw new Exception("The device with provided ID could not be found. Available Devices:\n" + availableDevices);
                }
                WIA.Item item = device.Items[1] as WIA.Item;
                try
                {
                    // scan image
                    WIA.ICommonDialog wiaCommonDialog = new WIA.CommonDialog();
                    WIAScanner.AdjustScannerSettings(item, settings.WIA_Intent);
                    WIA.ImageFile image = (WIA.ImageFile)wiaCommonDialog.ShowTransfer(item, wiaFormatBMP, false);

                    var imageBytes = (byte[])image.FileData.get_BinaryData();
                    images.Add(Convert.ToBase64String(imageBytes));
                }
                catch (Exception exc)
                {
                    throw exc;
                }
                finally
                {
                    item = null;
                    //determine if there are any more pages waiting
                    WIA.Property documentHandlingSelect = null;
                    WIA.Property documentHandlingStatus = null;
                    foreach (WIA.Property prop in device.Properties)
                    {
                        if (prop.PropertyID == WIA_PROPERTIES.WIA_DPS_DOCUMENT_HANDLING_SELECT)
                            documentHandlingSelect = prop;
                        if (prop.PropertyID == WIA_PROPERTIES.WIA_DPS_DOCUMENT_HANDLING_STATUS)
                            documentHandlingStatus = prop;
                    }
                    // assume there are no more pages
                    hasMorePages = false;
                    // may not exist on flatbed scanner but required for feeder
                    if (documentHandlingSelect != null)
                    {
                        // check for document feeder
                        if ((Convert.ToUInt32(documentHandlingSelect.get_Value()) & WIA_DPS_DOCUMENT_HANDLING_SELECT.FEEDER) != 0)
                        {
                            hasMorePages = ((Convert.ToUInt32(documentHandlingStatus.get_Value()) & WIA_DPS_DOCUMENT_HANDLING_STATUS.FEED_READY) != 0);
                        }
                    }
                }
            }
            return images;
        }

        /// <summary>
        /// Gets the list of available WIA devices.
        /// </summary>
        /// <returns></returns>
        public static List<ScannerDevice> GetDevices()
        {
            List<ScannerDevice> devices = new List<ScannerDevice>();
            WIA.DeviceManager manager = new WIA.DeviceManager();

            foreach (WIA.DeviceInfo info in manager.DeviceInfos)
            {
                var device = new ScannerDevice();
                device.DeviceId = info.DeviceID;
                device.DeviceName=info.Properties["Name"].get_Value();
                devices.Add(device);
            }
            return devices;
        }
    }
}
