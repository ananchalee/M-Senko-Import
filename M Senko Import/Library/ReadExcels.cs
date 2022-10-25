using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ClosedXML.Excel;

namespace M_Senko_Import.Library
{
    internal class ReadExcels
    {
        public List<T> ReadExcel<T>(string Path)
        {
            List<T> list = new List<T>();
            Type typeOfObject = typeof(T);
            using (IXLWorkbook workbook = new XLWorkbook(Path))
            {
                var worksheetName = workbook.Worksheets.Select(w => w.Name).First();

                var worksheet = workbook.Worksheets.Where(w => w.Name == worksheetName).First();
                var properties = typeOfObject.GetProperties();
                var properties2 = typeOfObject.GetProperties().Select((p, i) => new { p.Name, Index = i + 1, p.PropertyType });

                var columns = worksheet.FirstRow().Cells().Select((v, i) => new { v.Value, Index = i + 1 });
                var RowNo = 2;

                foreach (IXLRow row in worksheet.RowsUsed().Skip(1))
                {
                    T obj = (T)Activator.CreateInstance(typeOfObject);
                    for (int i = 0; i < properties.Length; i++)
                    {
                        if (i == 0)
                        {
                            //Console.WriteLine(properties[i]);
                            var type = properties[i].PropertyType;
                            properties[i].SetValue(obj, Convert.ChangeType(RowNo++, type));
                        }
                        else if (properties[i].Name == "ITEM")
                        {
                            int colIndex = columns.SingleOrDefault(c => c.Index == i).Index;
                            var val = row.Cell(colIndex).Value;
                            var type = properties[i].PropertyType;

                            decimal item;
                            // check E+12
                            if (decimal.TryParse(Convert.ToString(val, CultureInfo.InvariantCulture), System.Globalization.NumberStyles.Any, NumberFormatInfo.InvariantInfo, out item))
                            {
                                item = decimal.Parse((string)val, System.Globalization.NumberStyles.Float);

                                properties[i].SetValue(obj, Convert.ChangeType(item, type));
                            }
                            else
                            {
                                properties[i].SetValue(obj, Convert.ChangeType(val, type));
                            }
                        }
                        else
                        {
                            //Console.WriteLine(properties[i]);
                            int colIndex = columns.SingleOrDefault(c => c.Index == i).Index;
                            var val = row.Cell(colIndex).Value;
                            var type = properties[i].PropertyType;
                            properties[i].SetValue(obj, Convert.ChangeType(val, type));
                        }

                    };
                    list.Add(obj);
                }
            }
            return list;
        }
    }
}
