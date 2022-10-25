using System;
using System.Collections.Generic;

namespace DAL.Data
{
    /// <summary>
    /// Docment Running Table
    /// </summary>
    public partial class Mdocrun
    {
        public int Docno { get; set; }
        /// <summary>
        /// Document ID
        /// </summary>
        public int Docid { get; set; }
        /// <summary>
        /// Type of Running Number
        /// </summary>
        public string Type { get; set; } = null!;
        /// <summary>
        /// Document Name (Ex Job)
        /// </summary>
        public string Docname { get; set; } = null!;
        /// <summary>
        /// Prefix of Running Number
        /// </summary>
        public string? Prefix { get; set; }
        /// <summary>
        /// Start Running Number
        /// </summary>
        public string? Startrun { get; set; }
        /// <summary>
        /// Current Running Number
        /// </summary>
        public string? Currentrun { get; set; }
        public string? Customtext { get; set; }
        public bool? Isdefault { get; set; }
        public bool? Ishidden { get; set; }
        public bool? Isactive { get; set; }
        public DateTime? Lastcreate { get; set; }
        public string? Docformat { get; set; }
        public int? Unloadtime { get; set; }
        public int? Loadtime { get; set; }
        public int? Subtype { get; set; }
        public int? Maxloadtime { get; set; }
        public int? Maxunloadtime { get; set; }
    }
}
