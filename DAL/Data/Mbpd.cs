using System;
using System.Collections.Generic;

namespace DAL.Data
{
    public partial class Mbpd
    {
        public int No { get; set; }
        public string Bpcode { get; set; } = null!;
        public string Bpname { get; set; } = null!;
        public string Bptype { get; set; } = null!;
        public string Tel { get; set; } = null!;
        public string? Mobile { get; set; }
        public string? Fax { get; set; }
        public string Createby { get; set; } = null!;
        public DateTime Createdate { get; set; }
        public bool? Isactive { get; set; }
        public string? Loginpwd { get; set; }
        public string? Email { get; set; }
        public bool? IsNotifyDaily { get; set; }
        public string? Bpref1 { get; set; }
        public string? Bpref2 { get; set; }
        public string? Bpref3 { get; set; }
        public string? Bpref4 { get; set; }
    }
}
