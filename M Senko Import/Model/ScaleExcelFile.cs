using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace M_Senko_Import.Model
{
    internal class ScaleExcelFile
    {
        public int RowNo { get; set; }
        public string DELIVERY_ORDER { get; set; }
        public string SHIP_TO { get; set; }
        public string ITEM { get; set; }
        public double ALLOCATED_QTY { get; set; }
        public string QUANTITY_UM { get; set; }
        public double ITEM_WEIGHT { get; set; }
        public string WEIGHT_UM { get; set; }
        public double ITEM_WIDTH { get; set; }
        public double ITEM_LENGTH { get; set; }
        public double ITEM_HEIGHT { get; set; }
        public string ITEM_DIMENSION_UM { get; set; }
        public string Site { get; set; }
        public DateTime ORDER_DATE { get; set; }
        public DateTime PLANNED_SHIP_DATE { get; set; }
        public string SHIPMENT { get; set; }
    }
}
