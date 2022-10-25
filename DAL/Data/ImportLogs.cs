using System;
using System.Collections.Generic;

namespace DAL.Data
{
    public partial class ImportLogs
    {
        public int No { get; set; }
        public string FileName { get; set; } = null!;
        public string? JobNo { get; set; }
        public bool ImportStatus { get; set; }
        public int? Row { get; set; }
        public int? Col { get; set; }
        public string? Description { get; set; }
        public DateTime CreateDate { get; set; }
        public string? CreateBy { get; set; }
        public int? CurrentRun { get; set; }
        public string? Hhid { get; set; }
    }
}
