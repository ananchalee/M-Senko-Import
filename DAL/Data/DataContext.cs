using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace DAL.Data
{
    public partial class DataContext : DbContext
    {
        public DataContext()
        {
        }

        public DataContext(DbContextOptions<DataContext> options)
            : base(options)
        {
        }

        public virtual DbSet<ImportLogs> ImportLogs { get; set; } = null!;
        public virtual DbSet<Mbpd> Mbpd { get; set; } = null!;
        public virtual DbSet<Mdocrun> Mdocrun { get; set; } = null!;
        public virtual DbSet<Tjitem> Tjitem { get; set; } = null!;
        public virtual DbSet<Tjob> Tjob { get; set; } = null!;
        public virtual DbSet<Tpoi> Tpoi { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.UseCollation("SQL_Latin1_General_CP1_CI_AS");

            modelBuilder.Entity<ImportLogs>(entity =>
            {
                entity.HasKey(e => e.No)
                    .HasName("PK__ImportLo__3214D4A863DF1724");

                entity.Property(e => e.CreateBy).HasMaxLength(20);

                entity.Property(e => e.CreateDate).HasColumnType("datetime");

                entity.Property(e => e.FileName).HasMaxLength(100);

                entity.Property(e => e.Hhid)
                    .HasMaxLength(20)
                    .HasColumnName("HHID");

                entity.Property(e => e.JobNo).HasMaxLength(50);
            });

            modelBuilder.Entity<Mbpd>(entity =>
            {
                entity.HasKey(e => e.Bpcode);

                entity.ToTable("mbpd");

                entity.Property(e => e.Bpcode)
                    .HasMaxLength(20)
                    .HasColumnName("bpcode");

                entity.Property(e => e.Bpname)
                    .HasMaxLength(150)
                    .HasColumnName("bpname");

                entity.Property(e => e.Bpref1)
                    .HasMaxLength(100)
                    .HasColumnName("bpref1");

                entity.Property(e => e.Bpref2)
                    .HasMaxLength(100)
                    .HasColumnName("bpref2");

                entity.Property(e => e.Bpref3)
                    .HasMaxLength(100)
                    .HasColumnName("bpref3");

                entity.Property(e => e.Bpref4)
                    .HasMaxLength(100)
                    .HasColumnName("bpref4");

                entity.Property(e => e.Bptype)
                    .HasMaxLength(1)
                    .IsUnicode(false)
                    .HasColumnName("bptype")
                    .IsFixedLength();

                entity.Property(e => e.Createby)
                    .HasMaxLength(50)
                    .HasColumnName("createby");

                entity.Property(e => e.Createdate)
                    .HasColumnType("datetime")
                    .HasColumnName("createdate");

                entity.Property(e => e.Email).HasMaxLength(100);

                entity.Property(e => e.Fax)
                    .HasMaxLength(20)
                    .HasColumnName("fax");

                entity.Property(e => e.Isactive).HasColumnName("isactive");

                entity.Property(e => e.Loginpwd)
                    .HasMaxLength(50)
                    .HasColumnName("loginpwd");

                entity.Property(e => e.Mobile)
                    .HasMaxLength(20)
                    .HasColumnName("mobile");

                entity.Property(e => e.No)
                    .ValueGeneratedOnAdd()
                    .HasColumnName("no");

                entity.Property(e => e.Tel)
                    .HasMaxLength(50)
                    .HasColumnName("tel");
            });

            modelBuilder.Entity<Mdocrun>(entity =>
            {
                entity.HasKey(e => new { e.Docid, e.Type, e.Docname });

                entity.ToTable("mdocrun");

                entity.HasComment("Docment Running Table");

                entity.Property(e => e.Docid)
                    .HasColumnName("docid")
                    .HasComment("Document ID");

                entity.Property(e => e.Type)
                    .HasMaxLength(20)
                    .HasColumnName("type")
                    .HasComment("Type of Running Number");

                entity.Property(e => e.Docname)
                    .HasMaxLength(50)
                    .HasColumnName("docname")
                    .HasComment("Document Name (Ex Job)");

                entity.Property(e => e.Currentrun)
                    .HasMaxLength(50)
                    .HasColumnName("currentrun")
                    .HasComment("Current Running Number");

                entity.Property(e => e.Customtext)
                    .HasMaxLength(100)
                    .HasColumnName("customtext");

                entity.Property(e => e.Docformat)
                    .HasMaxLength(50)
                    .HasColumnName("docformat");

                entity.Property(e => e.Docno)
                    .ValueGeneratedOnAdd()
                    .HasColumnName("docno");

                entity.Property(e => e.Isactive).HasColumnName("isactive");

                entity.Property(e => e.Isdefault).HasColumnName("isdefault");

                entity.Property(e => e.Ishidden).HasColumnName("ishidden");

                entity.Property(e => e.Lastcreate)
                    .HasColumnType("datetime")
                    .HasColumnName("lastcreate");

                entity.Property(e => e.Loadtime).HasColumnName("loadtime");

                entity.Property(e => e.Maxloadtime).HasColumnName("maxloadtime");

                entity.Property(e => e.Maxunloadtime).HasColumnName("maxunloadtime");

                entity.Property(e => e.Prefix)
                    .HasMaxLength(10)
                    .HasColumnName("prefix")
                    .HasComment("Prefix of Running Number");

                entity.Property(e => e.Startrun)
                    .HasMaxLength(50)
                    .HasColumnName("startrun")
                    .HasComment("Start Running Number");

                entity.Property(e => e.Subtype).HasColumnName("subtype");

                entity.Property(e => e.Unloadtime).HasColumnName("unloadtime");
            });

            modelBuilder.Entity<Tjitem>(entity =>
            {
                entity.HasKey(e => new { e.Itemno, e.Jobno });

                entity.ToTable("tjitem");

                entity.HasComment("Job Item Transaction");

                entity.Property(e => e.Itemno)
                    .HasColumnName("itemno")
                    .HasComment("Item No.");

                entity.Property(e => e.Jobno)
                    .HasMaxLength(50)
                    .HasColumnName("jobno")
                    .HasComment("Job No.");

                entity.Property(e => e.Cdate)
                    .HasColumnType("datetime")
                    .HasColumnName("cdate")
                    .HasComment("Create Date");

                entity.Property(e => e.Containerno)
                    .HasMaxLength(50)
                    .HasColumnName("containerno")
                    .HasComment("Container No.");

                entity.Property(e => e.Cover)
                    .HasColumnName("cover")
                    .HasComment("Is Cover (True-Cover, False-No Cover)");

                entity.Property(e => e.Crack)
                    .HasColumnName("crack")
                    .HasComment("Is Crack (True-Crack, False-No Crack)");

                entity.Property(e => e.Dent)
                    .HasColumnName("dent")
                    .HasComment("Is Dent (True-Dent, False-No Dent)");

                entity.Property(e => e.Dimg)
                    .HasMaxLength(50)
                    .HasColumnName("dimg");

                entity.Property(e => e.Dqty).HasColumnName("dqty");

                entity.Property(e => e.Dreason)
                    .HasMaxLength(30)
                    .HasColumnName("dreason");

                entity.Property(e => e.Dstatus)
                    .HasMaxLength(20)
                    .HasColumnName("DStatus");

                entity.Property(e => e.Height)
                    .HasColumnName("height")
                    .HasComment("Item height");

                entity.Property(e => e.Hoop)
                    .HasColumnName("hoop")
                    .HasComment("Is Hoop (True-Hoop, False-No Hoop)");

                entity.Property(e => e.Isqa)
                    .HasColumnName("isqa")
                    .HasDefaultValueSql("((0))")
                    .HasComment("Is QA (QA-True, No QA-False)");

                entity.Property(e => e.ItemSkill)
                    .HasMaxLength(50)
                    .HasColumnName("itemSkill");

                entity.Property(e => e.ItemType)
                    .HasMaxLength(50)
                    .HasColumnName("itemType");

                entity.Property(e => e.Itemname)
                    .HasMaxLength(100)
                    .HasColumnName("itemname")
                    .HasComment("Item Name");

                entity.Property(e => e.Lenght)
                    .HasColumnName("lenght")
                    .HasComment("Item Lenght");

                entity.Property(e => e.Qty)
                    .HasColumnName("qty")
                    .HasComment("Item Quntity");

                entity.Property(e => e.Ref)
                    .HasMaxLength(100)
                    .HasColumnName("ref")
                    .HasComment("Reference Number (Coil Number)");

                entity.Property(e => e.Rimg)
                    .HasMaxLength(50)
                    .HasColumnName("rimg");

                entity.Property(e => e.Rmin)
                    .HasColumnName("rmin")
                    .HasComment("Is RM In (True-RM In, False-No RM In)");

                entity.Property(e => e.Rmout)
                    .HasColumnName("rmout")
                    .HasComment("Is RM Out (True-RM Out, False-No RM Out)");

                entity.Property(e => e.Rqty).HasColumnName("rqty");

                entity.Property(e => e.Rreason)
                    .HasMaxLength(30)
                    .HasColumnName("rreason");

                entity.Property(e => e.Rstatus)
                    .HasMaxLength(20)
                    .HasColumnName("RStatus");

                entity.Property(e => e.Rusty)
                    .HasColumnName("rusty")
                    .HasComment("Is Rusty (True-Rusty, False-No Rusty)");

                entity.Property(e => e.Sealno)
                    .HasMaxLength(50)
                    .HasColumnName("sealno");

                entity.Property(e => e.Unit).HasMaxLength(50);

                entity.Property(e => e.Weight)
                    .HasColumnName("weight")
                    .HasComment("Item weight");

                entity.Property(e => e.Wet)
                    .HasColumnName("wet")
                    .HasComment("Is Wet (True-Wet, False-No Wet)");

                entity.Property(e => e.Width)
                    .HasColumnName("width")
                    .HasComment("Width");
            });

            modelBuilder.Entity<Tjob>(entity =>
            {
                entity.HasKey(e => e.Jobno)
                    .HasName("PK_tjob_1");

                entity.ToTable("tjob");

                entity.HasComment("Job Transaction");

                entity.HasIndex(e => new { e.Jobstatus, e.Dduedate }, "IX_tjob_jobstatus_dduedate");

                entity.Property(e => e.Jobno)
                    .HasMaxLength(50)
                    .HasColumnName("jobno")
                    .HasComment("Job No.");

                entity.Property(e => e.Ackdate)
                    .HasColumnType("datetime")
                    .HasColumnName("ackdate");

                entity.Property(e => e.Ackduedate)
                    .HasColumnType("datetime")
                    .HasColumnName("ackduedate");

                entity.Property(e => e.Ackstatus)
                    .HasMaxLength(1)
                    .IsUnicode(false)
                    .HasColumnName("ackstatus")
                    .IsFixedLength();

                entity.Property(e => e.Amount).HasColumnType("numeric(18, 6)");

                entity.Property(e => e.Approveby)
                    .HasMaxLength(50)
                    .HasColumnName("approveby");

                entity.Property(e => e.Approvedate)
                    .HasColumnType("datetime")
                    .HasColumnName("approvedate");

                entity.Property(e => e.Attachfile)
                    .HasMaxLength(100)
                    .HasColumnName("attachfile")
                    .HasComment("Attach File");

                entity.Property(e => e.Attachname)
                    .HasMaxLength(100)
                    .HasColumnName("attachname")
                    .HasComment("Attach Name");

                entity.Property(e => e.Bpcode)
                    .HasMaxLength(20)
                    .HasColumnName("bpcode")
                    .HasComment("Customer Code");

                entity.Property(e => e.Cby)
                    .HasMaxLength(100)
                    .HasColumnName("cby")
                    .HasComment("Create by");

                entity.Property(e => e.Cdate)
                    .HasColumnType("datetime")
                    .HasColumnName("cdate")
                    .HasComment("Create date");

                entity.Property(e => e.Contactd)
                    .HasColumnName("contactd")
                    .HasComment("Contact Person - Delivery (FK mcontact.id)");

                entity.Property(e => e.Contactr)
                    .HasColumnName("contactr")
                    .HasComment("Contact Person - Receive (FK mcontact.id)");

                entity.Property(e => e.Dchkindate)
                    .HasColumnType("datetime")
                    .HasColumnName("dchkindate");

                entity.Property(e => e.Dchkinlatlng)
                    .HasMaxLength(50)
                    .HasColumnName("dchkinlatlng");

                entity.Property(e => e.Dchkoutdate)
                    .HasColumnType("datetime")
                    .HasColumnName("dchkoutdate");

                entity.Property(e => e.Dchkoutlatlng)
                    .HasMaxLength(50)
                    .HasColumnName("dchkoutlatlng");

                entity.Property(e => e.Ddate)
                    .HasColumnType("datetime")
                    .HasColumnName("ddate")
                    .HasComment("Delivery Date");

                entity.Property(e => e.Ddistance).HasColumnName("DDistance");

                entity.Property(e => e.Dduedate)
                    .HasColumnType("datetime")
                    .HasColumnName("dduedate");

                entity.Property(e => e.DeliveryUpdateDate).HasColumnType("datetime");

                entity.Property(e => e.Dlat)
                    .HasColumnType("decimal(11, 8)")
                    .HasColumnName("dlat");

                entity.Property(e => e.Dlng)
                    .HasColumnType("decimal(11, 8)")
                    .HasColumnName("dlng");

                entity.Property(e => e.Dmanimg)
                    .HasMaxLength(50)
                    .HasColumnName("dmanimg")
                    .HasComment("Man Delivery Image");

                entity.Property(e => e.Dqr).HasColumnName("dqr");

                entity.Property(e => e.Dsignimg)
                    .HasMaxLength(50)
                    .HasColumnName("dsignimg")
                    .HasComment("Delivery Signature Image");

                entity.Property(e => e.Epdate)
                    .HasColumnType("datetime")
                    .HasColumnName("epdate");

                entity.Property(e => e.Eta)
                    .HasColumnType("datetime")
                    .HasColumnName("eta");

                entity.Property(e => e.Groupid)
                    .HasMaxLength(20)
                    .HasColumnName("groupid")
                    .HasDefaultValueSql("((1000))")
                    .HasComment("Group ID");

                entity.Property(e => e.Hhid)
                    .HasMaxLength(20)
                    .HasColumnName("hhid")
                    .HasComment("Handheld");

                entity.Property(e => e.IsApprove).HasColumnName("isApprove");

                entity.Property(e => e.Ispriority)
                    .HasColumnName("ispriority")
                    .HasComment("Priority of job (1-High, 0-Low, NULL- no)");

                entity.Property(e => e.JobSkill)
                    .HasMaxLength(50)
                    .HasColumnName("jobSkill");

                entity.Property(e => e.Jobstatus)
                    .HasMaxLength(1)
                    .IsUnicode(false)
                    .HasColumnName("jobstatus")
                    .IsFixedLength()
                    .HasComment("job status (B-Blank, R-Receive, S-Send, C-Complete)");

                entity.Property(e => e.Jobtype)
                    .HasColumnName("jobtype")
                    .HasComment("Job Type (D-Delivery, R-Return Container)");

                entity.Property(e => e.LoadId)
                    .HasMaxLength(100)
                    .HasColumnName("LoadID");

                entity.Property(e => e.Payamount).HasColumnName("payamount");

                entity.Property(e => e.Paydate)
                    .HasColumnType("datetime")
                    .HasColumnName("paydate");

                entity.Property(e => e.Payref)
                    .HasMaxLength(100)
                    .HasColumnName("payref");

                entity.Property(e => e.Poidelivery)
                    .HasMaxLength(20)
                    .HasColumnName("poidelivery")
                    .HasComment("Poi Delivery (poiid FK tpoi Table)");

                entity.Property(e => e.Poireceive)
                    .HasMaxLength(20)
                    .HasColumnName("poireceive")
                    .HasComment("Poi Receive (poiid FK tpoi Table)");

                entity.Property(e => e.Printdate)
                    .HasColumnType("datetime")
                    .HasColumnName("printdate");

                entity.Property(e => e.Rchkindate)
                    .HasColumnType("datetime")
                    .HasColumnName("rchkindate");

                entity.Property(e => e.Rchkinlatlng)
                    .HasMaxLength(50)
                    .HasColumnName("rchkinlatlng");

                entity.Property(e => e.Rchkoutdate)
                    .HasColumnType("datetime")
                    .HasColumnName("rchkoutdate");

                entity.Property(e => e.Rchkoutlatlng)
                    .HasMaxLength(50)
                    .HasColumnName("rchkoutlatlng");

                entity.Property(e => e.Rdate)
                    .HasColumnType("datetime")
                    .HasColumnName("rdate")
                    .HasComment("Receive Date");

                entity.Property(e => e.Rdistance).HasColumnName("RDistance");

                entity.Property(e => e.Rduedate)
                    .HasColumnType("datetime")
                    .HasColumnName("rduedate");

                entity.Property(e => e.ReceiveUpdateDate).HasColumnType("datetime");

                entity.Property(e => e.RecognizeDate).HasColumnType("datetime");

                entity.Property(e => e.Ref1)
                    .HasMaxLength(100)
                    .HasColumnName("ref1")
                    .HasComment("Ref1 (Container Number)");

                entity.Property(e => e.Ref10)
                    .HasMaxLength(100)
                    .HasColumnName("ref10");

                entity.Property(e => e.Ref11)
                    .HasMaxLength(100)
                    .HasColumnName("ref11");

                entity.Property(e => e.Ref12)
                    .HasMaxLength(100)
                    .HasColumnName("ref12");

                entity.Property(e => e.Ref2)
                    .HasMaxLength(100)
                    .HasColumnName("ref2")
                    .HasComment("Ref2 (Trailer Number)");

                entity.Property(e => e.Ref3)
                    .HasMaxLength(100)
                    .HasColumnName("ref3")
                    .HasComment("Ref3 (Return Place)");

                entity.Property(e => e.Ref4)
                    .HasMaxLength(100)
                    .HasColumnName("ref4")
                    .HasComment("Ref4 (Chassis Number)");

                entity.Property(e => e.Ref5)
                    .HasMaxLength(100)
                    .HasColumnName("ref5");

                entity.Property(e => e.Ref6)
                    .HasMaxLength(100)
                    .HasColumnName("ref6");

                entity.Property(e => e.Ref7)
                    .HasMaxLength(100)
                    .HasColumnName("ref7");

                entity.Property(e => e.Ref8)
                    .HasMaxLength(100)
                    .HasColumnName("ref8");

                entity.Property(e => e.Ref9)
                    .HasMaxLength(100)
                    .HasColumnName("ref9");

                entity.Property(e => e.Remark)
                    .HasMaxLength(255)
                    .HasColumnName("remark")
                    .HasComment("Remark");

                entity.Property(e => e.Rlat)
                    .HasColumnType("decimal(11, 8)")
                    .HasColumnName("rlat");

                entity.Property(e => e.Rlng)
                    .HasColumnType("decimal(11, 8)")
                    .HasColumnName("rlng");

                entity.Property(e => e.Rmanimg)
                    .HasMaxLength(50)
                    .HasColumnName("rmanimg")
                    .HasComment("Man Receive Image");

                entity.Property(e => e.Rqr).HasColumnName("rqr");

                entity.Property(e => e.Rsignimg)
                    .HasMaxLength(50)
                    .HasColumnName("rsignimg")
                    .HasComment("Receive Signature Image");

                entity.Property(e => e.Seq).HasColumnName("seq");

                entity.Property(e => e.Tdistance).HasColumnName("tdistance");

                entity.Property(e => e.Ttime).HasColumnName("ttime");

                entity.Property(e => e.UpdateBy).HasMaxLength(50);

                entity.Property(e => e.UpdateDate).HasColumnType("datetime");

                entity.Property(e => e.Zone)
                    .HasMaxLength(50)
                    .HasColumnName("zone");
            });

            modelBuilder.Entity<Tpoi>(entity =>
            {
                entity.HasKey(e => e.Poiid);

                entity.ToTable("tpoi");

                entity.HasComment("POI transaction");

                entity.Property(e => e.Poiid)
                    .HasMaxLength(20)
                    .HasColumnName("poiid")
                    .HasComment("Running Number");

                entity.Property(e => e.Activestate).HasColumnName("activestate");

                entity.Property(e => e.Addrno)
                    .HasColumnName("addrno")
                    .HasComment("Address Number");

                entity.Property(e => e.Bpcode)
                    .HasMaxLength(20)
                    .HasColumnName("bpcode")
                    .HasComment("BP Code");

                entity.Property(e => e.Branchcode)
                    .HasMaxLength(20)
                    .HasColumnName("branchcode")
                    .HasComment("Branch Code");

                entity.Property(e => e.Country)
                    .HasMaxLength(100)
                    .HasColumnName("country")
                    .HasComment("Country");

                entity.Property(e => e.Createby)
                    .HasMaxLength(50)
                    .HasColumnName("createby")
                    .HasComment("Create By");

                entity.Property(e => e.Createdate)
                    .HasColumnType("datetime")
                    .HasColumnName("createdate")
                    .HasComment("Create Date");

                entity.Property(e => e.District)
                    .HasMaxLength(100)
                    .HasColumnName("district")
                    .HasComment("district");

                entity.Property(e => e.IsException).HasColumnName("isException");

                entity.Property(e => e.Isactive).HasColumnName("isactive");

                entity.Property(e => e.Isapprove)
                    .HasColumnName("isapprove")
                    .HasComment("1-Approve, 0-No Approve");

                entity.Property(e => e.Isdefault)
                    .HasColumnName("isdefault")
                    .HasComment("Default (0-No Default, 1-Default)");

                entity.Property(e => e.Isfind).HasColumnName("isfind");

                entity.Property(e => e.Lat)
                    .HasColumnType("decimal(11, 8)")
                    .HasColumnName("lat")
                    .HasComment("Latitude");

                entity.Property(e => e.Lng)
                    .HasColumnType("decimal(11, 8)")
                    .HasColumnName("lng")
                    .HasComment("Longitude");

                entity.Property(e => e.Loadtime).HasColumnName("loadtime");

                entity.Property(e => e.Name)
                    .HasMaxLength(100)
                    .HasColumnName("name")
                    .HasComment("POI Name");

                entity.Property(e => e.Province)
                    .HasMaxLength(100)
                    .HasColumnName("province")
                    .HasComment("province");

                entity.Property(e => e.Radius)
                    .HasColumnName("radius")
                    .HasComment("Error Radius");

                entity.Property(e => e.Servicetime)
                    .HasMaxLength(10)
                    .HasColumnName("servicetime");

                entity.Property(e => e.Skillpoi)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.Street)
                    .HasMaxLength(100)
                    .HasColumnName("street")
                    .HasComment("Street");

                entity.Property(e => e.Subdistrict)
                    .HasMaxLength(100)
                    .HasColumnName("subdistrict")
                    .HasComment("Subdistrict");

                entity.Property(e => e.Subtype)
                    .HasMaxLength(20)
                    .HasColumnName("subtype")
                    .HasComment("Sub Type");

                entity.Property(e => e.Truckavailable).HasColumnName("truckavailable");

                entity.Property(e => e.Type)
                    .HasMaxLength(1)
                    .IsUnicode(false)
                    .HasColumnName("type")
                    .IsFixedLength()
                    .HasComment("G-General, B-BP, R-Branch");

                entity.Property(e => e.Unloadtime).HasColumnName("unloadtime");

                entity.Property(e => e.WindowTimeEnd)
                    .HasMaxLength(50)
                    .HasColumnName("windowTimeEnd");

                entity.Property(e => e.WindowTimeStart)
                    .HasMaxLength(50)
                    .HasColumnName("windowTimeStart");

                entity.Property(e => e.Zipcode)
                    .HasMaxLength(10)
                    .HasColumnName("zipcode")
                    .HasComment("Zipcode");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
