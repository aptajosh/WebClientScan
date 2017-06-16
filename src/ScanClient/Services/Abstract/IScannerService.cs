using CorsEnabledService;
using ScanClient.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using System.Threading.Tasks;

namespace ScanClient.Service
{
    [ServiceContract]
    interface IScannerService
    {
        [OperationContract]
        [WebInvoke(Method = "GET",
           RequestFormat = WebMessageFormat.Json,
           ResponseFormat = WebMessageFormat.Json,
           UriTemplate = "GetDevices"), CorsEnabled]
        List<ScannerDevice> GetDevices();

        [OperationContract]
        [WebGet]
        string Scan(string deviceId);

        /*
         * Scan with first device
         */
        [OperationContract]
        [WebInvoke(Method = "GET",
           RequestFormat = WebMessageFormat.Json,
           ResponseFormat = WebMessageFormat.Json,
           UriTemplate = "ScanDirect"),CorsEnabled]
        string ScanDefault();
        //scan with selected device and with given Intent type(i.e. grayscale, color, b&W default etc)
        [OperationContract]
        [WebInvoke(Method = "GET",
           RequestFormat = WebMessageFormat.Json,
           ResponseFormat = WebMessageFormat.Json,
           UriTemplate = "ScanWithSettings/?intent={intent}&deviceId={deviceId}"),CorsEnabled]
        string ScanWithSettings(int intent, string deviceId);

    }

   
}
