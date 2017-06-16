using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScanClient.Core
{
    [AttributeUsage(AttributeTargets.All, AllowMultiple = true)]
    public class WiaPropertyIdAttribute : Attribute
    {
        /// <summary>
        /// Gibt die WIA-Eigenschaften-ID zurück.
        /// </summary>
        public int PropertyID { get; private set; }


        /// <summary>
        /// Erstellt ein neues WiaProperty-Attribut
        /// </summary>
        /// <param name="propertyID">die WIA-Eigenschaften-ID</param>
        public WiaPropertyIdAttribute(int propertyID)
        {
            this.PropertyID = propertyID;
        }
    }
}
