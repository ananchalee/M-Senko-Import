using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace M_Senko_Import.Library
{
    internal class ErrorShipment
    {
        public int No { get; set; }
        public string FileName { get; set; }
        public string Delivery { get; set; }
        public string ShipmentID { get; set; }
        public bool ImportStatus { get; set; }
        public int? Row { get; set; }
        public int? Col { get; set; }
        public string Description { get; set; }
        public DateTime CreateDate { get; set; }
        public string CreateBy { get; set; }
        public string Hhid { get; set; }
    }
}
