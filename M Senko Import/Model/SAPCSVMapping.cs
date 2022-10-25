using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CsvHelper.Configuration;

namespace M_Senko_Import.Model
{
    internal class SAPCSVMapping: ClassMap<Shipment>
    {
        public SAPCSVMapping()
        {
            Map(m => m.RowNo).Convert(args => args.Row.Parser.RawRow);
            //Map(m => m.ShipmentID).Name("Shipment");
            Map(m => m.Delivery).Name("Delivery");
            Map(m => m.ShipTo).Name("Ship-to");
            Map(m => m.DPrio).Name("DPrio");
            Map(m => m.Article).Name("Article");
            Map(m => m.Description).Name("Description");
            Map(m => m.DeliveryQuantity).Name("Delivery quantity");
            Map(m => m.Unit).Name("SU");
            Map(m => m.TotalWeight).Name("Total Weight");
            Map(m => m.Volume).Name("Volume");
            Map(m => m.Site).Name("Site");
            Map(m => m.PickDate).Name("Pick Date");
            Map(m => m.DelivDate).Name("Deliv.date");
        }
    }
}
