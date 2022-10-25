using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace M_Senko_Import.Model
{
    internal class SAPXLSX
    {
        public int RowNo { get; set; }
        public string Delivery { get; set; }
        public string Item { get; set; }
        public string ShipTo { get; set; }
        public string DPrio { get; set; }
        public string Article { get; set; }
        public string Description { get; set; }
        public double DeliveryQuantity { get; set; }
        public string Unit { get; set; }
        public double TotalWeight { get; set; }
        public string WUn { get; set; }
        public double Volume { get; set; }
        public string VUn { get; set; }
        public string Site { get; set; }
        public DateTime PickDate { get; set; }
        public DateTime Art_Av_Dt { get; set; }
        public DateTime DelivDate { get; set; }
    }
}
