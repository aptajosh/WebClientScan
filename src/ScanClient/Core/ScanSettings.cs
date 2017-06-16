using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace ScanClient.Core
{
    [DataContract]
    public class ScanSettings
    {
        [DataMember]
        public string DeviceId { get; set; }
        [DataMember]
        public int WIA_Intent { get; set; }// GreyScale, Color etc.
        //public int WIA_Horizontal_DPI { get; set; }
        //public int WIA_Vertical_DPI { get; set; }
        
    }
}
