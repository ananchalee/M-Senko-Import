using System;
using System.Collections.Generic;

namespace DAL.Data
{
    /// <summary>
    /// POI transaction
    /// </summary>
    public partial class Tpoi
    {
        /// <summary>
        /// Running Number
        /// </summary>
        public string Poiid { get; set; } = null!;
        /// <summary>
        /// POI Name
        /// </summary>
        public string Name { get; set; } = null!;
        /// <summary>
        /// G-General, B-BP, R-Branch
        /// </summary>
        public string Type { get; set; } = null!;
        /// <summary>
        /// Create By
        /// </summary>
        public string Createby { get; set; } = null!;
        /// <summary>
        /// Sub Type
        /// </summary>
        public string? Subtype { get; set; }
        /// <summary>
        /// Latitude
        /// </summary>
        public decimal? Lat { get; set; }
        /// <summary>
        /// Longitude
        /// </summary>
        public decimal? Lng { get; set; }
        /// <summary>
        /// Create Date
        /// </summary>
        public DateTime Createdate { get; set; }
        /// <summary>
        /// 1-Approve, 0-No Approve
        /// </summary>
        public bool Isapprove { get; set; }
        /// <summary>
        /// BP Code
        /// </summary>
        public string? Bpcode { get; set; }
        /// <summary>
        /// Branch Code
        /// </summary>
        public string? Branchcode { get; set; }
        /// <summary>
        /// Address Number
        /// </summary>
        public string? Addrno { get; set; }
        /// <summary>
        /// Street
        /// </summary>
        public string? Street { get; set; }
        /// <summary>
        /// Subdistrict
        /// </summary>
        public string? Subdistrict { get; set; }
        /// <summary>
        /// district
        /// </summary>
        public string? District { get; set; }
        /// <summary>
        /// province
        /// </summary>
        public string? Province { get; set; }
        /// <summary>
        /// Zipcode
        /// </summary>
        public string? Zipcode { get; set; }
        /// <summary>
        /// Country
        /// </summary>
        public string? Country { get; set; }
        /// <summary>
        /// Error Radius
        /// </summary>
        public int? Radius { get; set; }
        /// <summary>
        /// Default (0-No Default, 1-Default)
        /// </summary>
        public bool? Isdefault { get; set; }
        public bool? Isactive { get; set; }
        public bool? Activestate { get; set; }
        public bool? IsVerify { get; set; }
        public string? Servicetime { get; set; }
        public bool? Isfind { get; set; }
        public string? Skillpoi { get; set; }
        public int? Truckavailable { get; set; }
        public bool? IsException { get; set; }
        public int? Loadtime { get; set; }
        public int? Unloadtime { get; set; }
        public string? WindowTimeStart { get; set; }
        public string? WindowTimeEnd { get; set; }
    }
}
