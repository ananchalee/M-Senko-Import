using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace M_Senko_Import.Model
{
    internal class Shipment
    {
        public int RowNo { get; set; }
        public string ShipmentID { get; set; }
        public string Delivery { get; set; }
        public int Itemno { get; set; }
        public string ShipTo { get; set; }
        public string DPrio { get; set; }
        public string Article { get; set; }
        public string Description { get; set; }
        public double DeliveryQuantity { get; set; }
        public string Unit { get; set; }
        public double Weight { get; set; }
        public double TotalWeight { get; set; }
        public double Lenght { get; set; }
        public double Height { get; set; }
        public double Width { get; set; }
        public double Volume { get; set; }
        public string Site { get; set; }
        public DateTime PickDate { get; set; }
        public DateTime DelivDate { get; set; }
        public string Jobno { get; set; }
        public int Jobtype { get; set; }
        public string Jobstatus { get; set; }
        public string Hhid { get; set; }
        public string Cby { get; set; }
        public string Groupid { get; set; }
        public string Remark { get; set; }
    }
}
