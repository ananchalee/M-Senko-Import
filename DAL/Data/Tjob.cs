using System;
using System.Collections.Generic;

namespace DAL.Data
{
    /// <summary>
    /// Job Transaction
    /// </summary>
    public partial class Tjob
    {
        /// <summary>
        /// Job No.
        /// </summary>
        public string Jobno { get; set; } = null!;
        /// <summary>
        /// Job Type (D-Delivery, R-Return Container)
        /// </summary>
        public int Jobtype { get; set; }
        /// <summary>
        /// Poi Receive (poiid FK tpoi Table)
        /// </summary>
        public string Poireceive { get; set; } = null!;
        /// <summary>
        /// Poi Delivery (poiid FK tpoi Table)
        /// </summary>
        public string Poidelivery { get; set; } = null!;
        /// <summary>
        /// Ref1 (Container Number)
        /// </summary>
        public string? Ref1 { get; set; }
        /// <summary>
        /// Ref2 (Trailer Number)
        /// </summary>
        public string? Ref2 { get; set; }
        /// <summary>
        /// Ref3 (Return Place)
        /// </summary>
        public string? Ref3 { get; set; }
        /// <summary>
        /// Ref4 (Chassis Number)
        /// </summary>
        public string? Ref4 { get; set; }
        /// <summary>
        /// Remark
        /// </summary>
        public string? Remark { get; set; }
        /// <summary>
        /// Group ID
        /// </summary>
        public string? Groupid { get; set; }
        /// <summary>
        /// Handheld
        /// </summary>
        public string? Hhid { get; set; }
        /// <summary>
        /// job status (B-Blank, R-Receive, S-Send, C-Complete)
        /// </summary>
        public string Jobstatus { get; set; } = null!;
        /// <summary>
        /// Create date
        /// </summary>
        public DateTime Cdate { get; set; }
        /// <summary>
        /// Create by
        /// </summary>
        public string Cby { get; set; } = null!;
        /// <summary>
        /// Delivery Date
        /// </summary>
        public DateTime? Ddate { get; set; }
        /// <summary>
        /// Receive Date
        /// </summary>
        public DateTime? Rdate { get; set; }
        /// <summary>
        /// Priority of job (1-High, 0-Low, NULL- no)
        /// </summary>
        public bool? Ispriority { get; set; }
        /// <summary>
        /// Contact Person - Receive (FK mcontact.id)
        /// </summary>
        public int? Contactr { get; set; }
        /// <summary>
        /// Contact Person - Delivery (FK mcontact.id)
        /// </summary>
        public int? Contactd { get; set; }
        /// <summary>
        /// Customer Code
        /// </summary>
        public string Bpcode { get; set; } = null!;
        /// <summary>
        /// Attach File
        /// </summary>
        public string? Attachfile { get; set; }
        /// <summary>
        /// Attach Name
        /// </summary>
        public string? Attachname { get; set; }
        /// <summary>
        /// Receive Signature Image
        /// </summary>
        public string? Rsignimg { get; set; }
        /// <summary>
        /// Delivery Signature Image
        /// </summary>
        public string? Dsignimg { get; set; }
        /// <summary>
        /// Man Receive Image
        /// </summary>
        public string? Rmanimg { get; set; }
        /// <summary>
        /// Man Delivery Image
        /// </summary>
        public string? Dmanimg { get; set; }
        public DateTime? Rduedate { get; set; }
        public DateTime? Dduedate { get; set; }
        public decimal? Rlat { get; set; }
        public decimal? Rlng { get; set; }
        public decimal? Dlat { get; set; }
        public decimal? Dlng { get; set; }
        public DateTime? Printdate { get; set; }
        public string? Rqr { get; set; }
        public string? Dqr { get; set; }
        public double? Rdistance { get; set; }
        public double? Ddistance { get; set; }
        public string? UpdateBy { get; set; }
        public bool? IsApprove { get; set; }
        public string? Approveby { get; set; }
        public double? Payamount { get; set; }
        public DateTime? Paydate { get; set; }
        public string? Payref { get; set; }
        public DateTime? Approvedate { get; set; }
        public int? Seq { get; set; }
        public DateTime? Epdate { get; set; }
        public DateTime? Ackdate { get; set; }
        public DateTime? Ackduedate { get; set; }
        public string? Ackstatus { get; set; }
        public DateTime? Rchkindate { get; set; }
        public string? Rchkinlatlng { get; set; }
        public DateTime? Rchkoutdate { get; set; }
        public string? Rchkoutlatlng { get; set; }
        public DateTime? Dchkindate { get; set; }
        public string? Dchkinlatlng { get; set; }
        public DateTime? Dchkoutdate { get; set; }
        public string? Dchkoutlatlng { get; set; }
        public bool? Recognize { get; set; }
        public DateTime? RecognizeDate { get; set; }
        public string? LoadId { get; set; }
        public int? LoadOrder { get; set; }
        public int? VerifyByContact { get; set; }
        public decimal? Amount { get; set; }
        public string? Ref5 { get; set; }
        public string? Ref6 { get; set; }
        public string? Ref7 { get; set; }
        public string? Ref8 { get; set; }
        public string? Ref9 { get; set; }
        public string? Ref10 { get; set; }
        public string? Ref11 { get; set; }
        public string? Ref12 { get; set; }
        public DateTime? UpdateDate { get; set; }
        public DateTime? ReceiveUpdateDate { get; set; }
        public DateTime? DeliveryUpdateDate { get; set; }
        public DateTime? Eta { get; set; }
        public double? Tdistance { get; set; }
        public double? Ttime { get; set; }
        public string? Zone { get; set; }
        public string? JobSkill { get; set; }
    }
}
