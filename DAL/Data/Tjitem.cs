using System;
using System.Collections.Generic;

namespace DAL.Data
{
    /// <summary>
    /// Job Item Transaction
    /// </summary>
    public partial class Tjitem
    {
        /// <summary>
        /// Job No.
        /// </summary>
        public string Jobno { get; set; } = null!;
        /// <summary>
        /// Item No.
        /// </summary>
        public int Itemno { get; set; }
        /// <summary>
        /// Item Name
        /// </summary>
        public string Itemname { get; set; } = null!;
        /// <summary>
        /// Width
        /// </summary>
        public double? Width { get; set; }
        /// <summary>
        /// Item Lenght
        /// </summary>
        public double? Lenght { get; set; }
        /// <summary>
        /// Item height
        /// </summary>
        public double? Height { get; set; }
        /// <summary>
        /// Item weight
        /// </summary>
        public double? Weight { get; set; }
        /// <summary>
        /// Item Quntity
        /// </summary>
        public double Qty { get; set; }
        /// <summary>
        /// Create Date
        /// </summary>
        public DateTime Cdate { get; set; }
        /// <summary>
        /// Container No.
        /// </summary>
        public string? Containerno { get; set; }
        /// <summary>
        /// Is QA (QA-True, No QA-False)
        /// </summary>
        public bool? Isqa { get; set; }
        /// <summary>
        /// Reference Number (Coil Number)
        /// </summary>
        public string? Ref { get; set; }
        /// <summary>
        /// Is Rusty (True-Rusty, False-No Rusty)
        /// </summary>
        public bool? Rusty { get; set; }
        /// <summary>
        /// Is Wet (True-Wet, False-No Wet)
        /// </summary>
        public bool? Wet { get; set; }
        /// <summary>
        /// Is Crack (True-Crack, False-No Crack)
        /// </summary>
        public bool? Crack { get; set; }
        /// <summary>
        /// Is Dent (True-Dent, False-No Dent)
        /// </summary>
        public bool? Dent { get; set; }
        /// <summary>
        /// Is Cover (True-Cover, False-No Cover)
        /// </summary>
        public bool? Cover { get; set; }
        /// <summary>
        /// Is Hoop (True-Hoop, False-No Hoop)
        /// </summary>
        public bool? Hoop { get; set; }
        /// <summary>
        /// Is RM Out (True-RM Out, False-No RM Out)
        /// </summary>
        public bool? Rmout { get; set; }
        /// <summary>
        /// Is RM In (True-RM In, False-No RM In)
        /// </summary>
        public bool? Rmin { get; set; }
        public string? Sealno { get; set; }
        public double? Rqty { get; set; }
        public double? Dqty { get; set; }
        public string? Rimg { get; set; }
        public string? Dimg { get; set; }
        public string? Rreason { get; set; }
        public string? Dreason { get; set; }
        public string? Rstatus { get; set; }
        public string? Dstatus { get; set; }
        public string? Unit { get; set; }
        public string? ItemType { get; set; }
        public string? ItemSkill { get; set; }
    }
}
