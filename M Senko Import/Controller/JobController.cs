using System.Globalization;
using CsvHelper;
using DAL.Data;
using M_Senko_Import.Model;
using M_Senko_Import.Library;
using M_Senko_Import.DBManager;
using System.Reflection;
using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using MimeKit.Text;
using System.Data;
using System.Xml;
using DocumentFormat.OpenXml.InkML;
using DocumentFormat.OpenXml.Office2010.Excel;
using System.Numerics;
using System.Text.Json;
using System.Runtime.Intrinsics.Arm;

namespace M_Senko_Import.Controller
{
    internal class JobController
    {
        public static IConfiguration Configuration => new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile("appsettings.json").Build();

        string CompanyID = Configuration.GetConnectionString("CompanyID");
        string Jobno = string.Empty;
        string RefCreateBy = "1000";
        int Jobtype = 8;
        string Jobstatus = "B";
        string HHID = "000000000000000";
        int jobRun = 0;
        string GroupID = string.Empty;

        #region Path SAP
        DirectoryInfo SAPError = new DirectoryInfo(Configuration["SAPError"]);
        DirectoryInfo SAPJob = new DirectoryInfo(Configuration["SAPJob"]);
        DirectoryInfo SAPBackup = new DirectoryInfo(Configuration["SAPBackup"]);
        string SAPLogs = Configuration["SAPLogs"];
        #endregion

        #region Path Scale
        DirectoryInfo ScaleError = new DirectoryInfo(Configuration["ScaleError"]);
        DirectoryInfo ScaleJob = new DirectoryInfo(Configuration["ScaleJob"]);
        DirectoryInfo ScaleBackup = new DirectoryInfo(Configuration["ScaleBackup"]);
        string ScaleLogs = Configuration["ScaleLogs"];

        #endregion

        public void ImportSAP_CSV()
        {
            FileInfo[] Files = SAPJob.GetFiles("*.csv");

            foreach (FileInfo i in Files)
            {
                List<Shipment> Jobs = new List<Shipment>();
                List<ErrorShipment> ErrorShipment = new List<ErrorShipment>();
                List<ImportLogs> LogImport = new List<ImportLogs>();
                List<Tjitem> __jobD = new List<Tjitem>();
                List<Tjob> __jobH = new List<Tjob>();

                string RefFileName = i.Name;
                string JobRefFullPath = SAPJob + "\\" + RefFileName;
                string PathLogs = SAPLogs;

                try
                {

                    Console.WriteLine("Start SAP Import CSV File:");
                    Console.WriteLine();
                    Console.WriteLine("start Time: " + DateTime.Now);
                    Console.WriteLine("File Name - {0}", RefFileName);

                    using (var streamReader = new StreamReader(JobRefFullPath))
                    {
                        using (var csvReader = new CsvReader(streamReader, CultureInfo.InvariantCulture))
                        {
                            csvReader.Context.RegisterClassMap<SAPCSVMapping>();
                            var shipments = csvReader.GetRecords<Shipment>().ToList();
                            Console.WriteLine("File Count " + shipments.Count);

                            if (shipments.Count == 0)
                            {
                                #region AddError
                                ErrorShipment.Add(new ErrorShipment()
                                {
                                    ImportStatus = false,
                                    FileName = RefFileName,
                                    CreateBy = RefCreateBy,
                                    Description = "Error SapCSV Empty file upload result.",
                                    CreateDate = DateTime.Now
                                });
                                #endregion

                                ImportLogs(PathLogs, ErrorShipment, RefFileName);
                                streamReader.Close();
                                MoveFile(JobRefFullPath, SAPError, RefFileName);

                                Console.WriteLine("Status: Error... " + DateTime.Now);
                                Console.WriteLine("---> Error Empty file upload result.");
                                Console.WriteLine("Move File: " + JobRefFullPath + " Move to:" + SAPError + "\\" + RefFileName);
                                Console.WriteLine("===================================================");
                                Console.WriteLine(string.Empty);

                                Console.WriteLine("Sending email...");
                                Mail(ErrorShipment, RefFileName);
                            }
                            else
                            {
                                ErrorShipment = CheckErrorSAP(shipments, RefFileName);

                                if (ErrorShipment.Count > 0)
                                {
                                    Console.WriteLine("ErrorShipment...");

                                    ImportLogs(PathLogs, ErrorShipment, RefFileName);
                                    streamReader.Close();
                                    MoveFile(JobRefFullPath, SAPError, RefFileName);

                                    Console.WriteLine("Sending email...");
                                    Mail(ErrorShipment, RefFileName);

                                    Console.WriteLine("Status: Error... " + DateTime.Now);
                                    Console.WriteLine("---> Error invalid data found.");
                                    Console.WriteLine("Move File: " + JobRefFullPath + " Move to:" + SAPError + "\\" + RefFileName);
                                    Console.WriteLine("===================================================");
                                    Console.WriteLine(string.Empty);
                                }
                                else
                                {
                                    streamReader.Close();

                                    Console.WriteLine("--> Step2 Runinng JobNO");
                                    JobsSAP(shipments, RefFileName, PathLogs);

                                    MoveFile(JobRefFullPath, SAPBackup, RefFileName);

                                    Console.WriteLine("Status: Success... " + DateTime.Now);
                                    Console.WriteLine("Move File: " + JobRefFullPath + " Move to:" + SAPBackup + "\\" + RefFileName);
                                    Console.WriteLine("===================================================");
                                    Console.WriteLine(string.Empty);

                                    //using (var db = new DatabaseManager().Prototype())

                                    //{
                                    //    Console.WriteLine("--> Step2 Runinng JobNO");

                                    //    JobsSAP(shipments, RefFileName);

                                    //    Console.WriteLine("--> Step3 Add Tjitem");
                                    //    __jobD = JobDetail(Jobs, RefFileName);

                                    //    Console.WriteLine("--> Step4 Add Tjob");
                                    //    __jobH = JobHeader(Jobs, RefFileName);

                                    //    #region Add Logs
                                    //    foreach (var H in __jobH)
                                    //    {
                                    //        LogImport.Add(new ImportLogs()
                                    //        {
                                    //            ImportStatus = true,
                                    //            FileName = RefFileName,
                                    //            JobNo = H.Jobno,
                                    //            CreateBy = RefCreateBy,
                                    //            Description = "Insert job completed,REF1=" + H.Ref1,
                                    //            CreateDate = DateTime.Now
                                    //        });
                                    //    }
                                    //    #endregion

                                    //    db.Tjob.AddRange(__jobH);
                                    //    db.Tjitems.AddRange(__jobD);
                                    //    db.ImportLogss.AddRange(LogImport);

                                    //    Console.WriteLine("--> Step5 db.Recording");
                                    //    db.SaveChanges();

                                    //    MoveFile(JobRefFullPath, SAPBackup, RefFileName);

                                    //    Console.WriteLine("Status: Success... " + DateTime.Now);
                                    //    Console.WriteLine("Move File: " + JobRefFullPath + " Move to:" + SAPBackup + "\\" + RefFileName);
                                    //    Console.WriteLine("===================================================");
                                    //    Console.WriteLine(string.Empty);
                                    //};
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    var message = ex.InnerException == null ? ex.Message : ex.Message + ", Inner Exception: " + ex.InnerException.Message;
                    #region AddError
                    ErrorShipment.Add(new ErrorShipment()
                    {
                        ImportStatus = false,
                        FileName = RefFileName,
                        CreateBy = RefCreateBy,
                        Description = message,
                        CreateDate = DateTime.Now
                    });
                    #endregion

                    ImportLogs(PathLogs, ErrorShipment, RefFileName);
                    MoveFile(JobRefFullPath, SAPError, RefFileName);

                    Console.WriteLine("Sending email...");
                    Mail(ErrorShipment, RefFileName);



                    Console.WriteLine("Status: Error... " + DateTime.Now);
                    Console.WriteLine("--->" + message);
                    Console.WriteLine("Move File: " + JobRefFullPath + " Move to:" + SAPError + "\\" + RefFileName);
                    Console.WriteLine("===================================================");
                    Console.WriteLine(string.Empty);

                    string str = ("\r\n"
                         + "Method Name:" + MethodBase.GetCurrentMethod().Name + "\r\n"
                         + "Class Name :" + this.GetType().ToString() + "\r\n"
                         + "File Name  :" + RefFileName + "\r\n"
                         + "Messsage   :" + message + "\r\n");

                    Logs.Error(str);
                }
            }
        }

        public void ImportSAP_XML()
        {
            FileInfo[] Files = SAPJob.GetFiles("*.xml");

            foreach (FileInfo i in Files)
            {
                List<Shipment> Jobs = new List<Shipment>();
                List<ErrorShipment> ErrorShipment = new List<ErrorShipment>();
                List<ImportLogs> LogImport = new List<ImportLogs>();
                List<Tjitem> __jobD = new List<Tjitem>();
                List<Tjob> __jobH = new List<Tjob>();
                List<Shipment> shipments = new List<Shipment>();

                string RefFileName = i.Name;
                string JobRefFullPath = SAPJob + "\\" + RefFileName;
                string PathLogs = SAPLogs;

                try
                {
                    Console.WriteLine("Start SAP Import XML File");
                    Console.WriteLine();
                    Console.WriteLine("start Time: " + DateTime.Now);
                    Console.WriteLine("File Name - {0}", RefFileName);

                    XmlDocument xml = new XmlDocument();
                    xml.Load(JobRefFullPath);

                    XmlNamespaceManager nsmgr = new XmlNamespaceManager(xml.NameTable);
                    nsmgr.AddNamespace("ss", "urn:schemas-microsoft-com:office:spreadsheet");

                    XmlElement root = xml.DocumentElement;
                    var RowNo = 2;
                    XmlNodeList rows = root.SelectNodes("//ss:Row", nsmgr);

                    Console.WriteLine("File Count " + rows.Count);
                    if (rows.Count == 1)
                    {
                        #region AddError
                        ErrorShipment.Add(new ErrorShipment()
                        {
                            ImportStatus = false,
                            FileName = RefFileName,
                            CreateBy = RefCreateBy,
                            Description = "Error SapXML Empty file upload result.",
                            CreateDate = DateTime.Now
                        });
                        #endregion

                        ImportLogs(PathLogs, ErrorShipment, RefFileName);

                        MoveFile(JobRefFullPath, SAPError, RefFileName);

                        Console.WriteLine("Sending email...");
                        Mail(ErrorShipment, RefFileName);

                        Console.WriteLine("Status: Error... " + DateTime.Now);
                        Console.WriteLine("---> Error Empty file upload result.");
                        Console.WriteLine("Move File: " + JobRefFullPath + " Move to:" + SAPError + "\\" + RefFileName);
                        Console.WriteLine("===================================================");
                        Console.WriteLine(string.Empty);
                    }
                    else
                    {
                        foreach (XmlNode row in rows)
                        {
                            XmlNode itemNode = row;
                            itemNode = itemNode.SelectSingleNode("following-sibling::*[1]", nsmgr);
                            if (itemNode == null) break;
                            shipments.Add(new Shipment()
                            {
                                RowNo = RowNo++,
                                Delivery = itemNode.SelectSingleNode("ss:Cell[1]/ss:Data", nsmgr).InnerText,
                                ShipTo = itemNode.SelectSingleNode("ss:Cell[3]/ss:Data", nsmgr).InnerText,
                                DPrio = itemNode.SelectSingleNode("ss:Cell[4]/ss:Data", nsmgr).InnerText,
                                Article = itemNode.SelectSingleNode("ss:Cell[5]/ss:Data", nsmgr).InnerText,
                                Description = itemNode.SelectSingleNode("ss:Cell[6]/ss:Data", nsmgr).InnerText,
                                DeliveryQuantity = Convert.ToDouble(itemNode.SelectSingleNode("ss:Cell[7]/ss:Data", nsmgr).InnerText),
                                Unit = itemNode.SelectSingleNode("ss:Cell[8]/ss:Data", nsmgr).InnerText,
                                TotalWeight = Convert.ToDouble(itemNode.SelectSingleNode("ss:Cell[9]/ss:Data", nsmgr).InnerText),
                                Volume = Convert.ToDouble(itemNode.SelectSingleNode("ss:Cell[11]/ss:Data", nsmgr).InnerText),
                                Site = itemNode.SelectSingleNode("ss:Cell[13]/ss:Data", nsmgr).InnerText,
                                PickDate = Convert.ToDateTime(itemNode.SelectSingleNode("ss:Cell[14]/ss:Data", nsmgr).InnerText),
                                DelivDate = Convert.ToDateTime(itemNode.SelectSingleNode("ss:Cell[16]/ss:Data", nsmgr).InnerText)

                            });
                        };

                        ErrorShipment = CheckErrorSAP(shipments, RefFileName);
                        if (ErrorShipment.Count > 0)
                        {
                            Console.WriteLine("ErrorShipment...");

                            ImportLogs(PathLogs, ErrorShipment, RefFileName);
                            MoveFile(JobRefFullPath, SAPError, RefFileName);

                            Console.WriteLine("Sending email...");
                            Mail(ErrorShipment, RefFileName);

                            Console.WriteLine("Status: Error... " + DateTime.Now);
                            Console.WriteLine("---> Error invalid data found.");
                            Console.WriteLine("Move File: " + JobRefFullPath + " Move to:" + SAPError + "\\" + RefFileName);
                            Console.WriteLine("===================================================");
                            Console.WriteLine(string.Empty);

                        }
                        else
                        {
                            Console.WriteLine("--> Step2 Runinng JobNO");
                            JobsSAP(shipments, RefFileName, PathLogs);

                            MoveFile(JobRefFullPath, SAPBackup, RefFileName);

                            Console.WriteLine("Status: Success... " + DateTime.Now);
                            Console.WriteLine("Move File: " + JobRefFullPath + " Move to:" + SAPBackup + "\\" + RefFileName);
                            Console.WriteLine("===================================================");
                            Console.WriteLine(string.Empty);

                            //using (var db = new DatabaseManager().Prototype())
                            //{
                            //    Console.WriteLine("--> Step2 Runinng JobNO");
                            //    Jobs = JobsSAP(shipments, RefFileName);

                            //    Console.WriteLine("--> Step3 Add Tjitem");
                            //    __jobD = JobDetail(Jobs, RefFileName);

                            //    Console.WriteLine("--> Step4 Add Tjob");
                            //    __jobH = JobHeader(Jobs, RefFileName);

                            //    #region Add Logs
                            //    foreach (var H in __jobH)
                            //    {
                            //        LogImport.Add(new ImportLogs()
                            //        {
                            //            ImportStatus = true,
                            //            FileName = RefFileName,
                            //            JobNo = H.Jobno,
                            //            CreateBy = RefCreateBy,
                            //            Description = "Insert job completed,REF1=" + H.Ref1,
                            //            CreateDate = DateTime.Now
                            //        });
                            //    }
                            //    #endregion

                            //    db.Tjob.AddRange(__jobH);
                            //    db.Tjitems.AddRange(__jobD);
                            //    db.ImportLogss.AddRange(LogImport);

                            //    Console.WriteLine("--> Step5 db.Recording");
                            //    db.SaveChanges();
                            //    MoveFile(JobRefFullPath, SAPBackup, RefFileName);

                            //    Console.WriteLine("Status: Success... " + DateTime.Now);
                            //    Console.WriteLine("Move File: " + JobRefFullPath + " Move to:" + SAPBackup + "\\" + RefFileName);
                            //    Console.WriteLine("===================================================");
                            //    Console.WriteLine(string.Empty);


                            //}

                        }
                    }


                }
                catch (Exception ex)
                {
                    var message = ex.InnerException == null ? ex.Message : ex.Message + ", Inner Exception: " + ex.InnerException.Message;

                    #region AddError
                    ErrorShipment.Add(new ErrorShipment()
                    {
                        ImportStatus = false,
                        FileName = RefFileName,
                        CreateBy = RefCreateBy,
                        Description = message,
                        CreateDate = DateTime.Now
                    });
                    #endregion

                    ImportLogs(PathLogs, ErrorShipment, RefFileName);
                    MoveFile(JobRefFullPath, SAPError, RefFileName);

                    Console.WriteLine("Sending email...");
                    Mail(ErrorShipment, RefFileName);

                    Console.WriteLine("Status: Error... " + DateTime.Now);
                    Console.WriteLine("--->" + message);
                    Console.WriteLine("Move File: " + JobRefFullPath + " Move to:" + SAPError + "\\" + RefFileName);
                    Console.WriteLine("===================================================");
                    Console.WriteLine(string.Empty);

                    string str = ("\r\n"
                         + "Method Name:" + MethodBase.GetCurrentMethod().Name + "\r\n"
                         + "Class Name :" + this.GetType().ToString() + "\r\n"
                         + "File Name  :" + RefFileName + "\r\n"
                         + "Messsage   :" + message + "\r\n");

                    Logs.Error(str);
                }

            }
        }

        public void ImportSAP_XLSX()
        {
            FileInfo[] Files = SAPJob.GetFiles("*.xlsx");

            foreach (FileInfo i in Files)
            {
                List<ErrorShipment> ErrorShipment = new List<ErrorShipment>();
                List<ImportLogs> LogImport = new List<ImportLogs>();
                List<Shipment> Jobs = new List<Shipment>();
                List<Tjob> __jobH = new List<Tjob>();
                List<Tjitem> __jobD = new List<Tjitem>();

                string RefFileName = i.Name;
                string JobRefFullPath = SAPJob + "\\" + RefFileName;
                string PathLogs = SAPLogs;

                try
                {
                    Console.WriteLine("Start SAP Import Excel File:");
                    Console.WriteLine();
                    Console.WriteLine("start Time: " + DateTime.Now);
                    Console.WriteLine("File Name - {0}", RefFileName);

                    var SapExcels = new ReadExcels().ReadExcel<SAPXLSX>(JobRefFullPath);

                    Console.WriteLine("File Count " + SapExcels.Count);

                    if (SapExcels.Count == 0)
                    {
                        #region AddError
                        ErrorShipment.Add(new ErrorShipment()
                        {
                            ImportStatus = false,
                            FileName = RefFileName,
                            CreateBy = RefCreateBy,
                            Description = "Error SapExcels Empty file upload result.",
                            CreateDate = DateTime.Now
                        });
                        #endregion

                        ImportLogs(PathLogs, ErrorShipment, RefFileName);
                        MoveFile(JobRefFullPath, SAPError, RefFileName);

                        Console.WriteLine("Sending email...");
                        Mail(ErrorShipment, RefFileName);

                        Console.WriteLine("Status: Error... " + DateTime.Now);
                        Console.WriteLine("---> Error Empty file upload result.");
                        Console.WriteLine("Move File: " + JobRefFullPath + " Move to:" + SAPError + "\\" + RefFileName);
                        Console.WriteLine("===================================================");
                        Console.WriteLine(string.Empty);
                    }
                    else
                    {
                        ErrorShipment = CheckErrorSAPExcel(SapExcels, RefFileName);
                        if (ErrorShipment.Count > 0)
                        {
                            Console.WriteLine("ErrorShipment...");

                            ImportLogs(PathLogs, ErrorShipment, RefFileName);
                            MoveFile(JobRefFullPath, SAPError, RefFileName);

                            Console.WriteLine("Sending email...");
                            Mail(ErrorShipment, RefFileName);

                            Console.WriteLine("Status: Error... " + DateTime.Now);
                            Console.WriteLine("---> Error invalid data found.");
                            Console.WriteLine("Move File: " + JobRefFullPath + " Move to:" + SAPError + "\\" + RefFileName);
                            Console.WriteLine("===================================================");
                            Console.WriteLine(string.Empty);

                        }
                        else
                        {
                            Console.WriteLine("--> Step2 Runinng JobNO");
                            JobsSAPExcel(SapExcels, RefFileName, PathLogs);

                            MoveFile(JobRefFullPath, SAPBackup, RefFileName);

                            Console.WriteLine("Status: Success... " + DateTime.Now);
                            Console.WriteLine("Move File: " + JobRefFullPath + " Move to:" + SAPBackup + "\\" + RefFileName);
                            Console.WriteLine("===================================================");
                            Console.WriteLine(string.Empty);

                            //using (var db = new DatabaseManager().Prototype())
                            //{
                            //    Console.WriteLine("--> Step2 Runinng JobNO");
                            //    JobsSAPExcel(SapExcels, RefFileName);

                            //    //Console.WriteLine("--> Step3 Add Tjitem");
                            //    //__jobD = JobDetail(Jobs, RefFileName);

                            //    //Console.WriteLine("--> Step4 Add Tjob");
                            //    //__jobH = JobHeader(Jobs, RefFileName);

                            //    //#region Add Logs
                            //    //foreach (var H in __jobH)
                            //    //{
                            //    //    LogImport.Add(new ImportLogs()
                            //    //    {
                            //    //        ImportStatus = true,
                            //    //        FileName = RefFileName,
                            //    //        JobNo = H.Jobno,
                            //    //        CreateBy = RefCreateBy,
                            //    //        Description = "Insert job completed,REF1=" + H.Ref1,
                            //    //        CreateDate = DateTime.Now
                            //    //    });
                            //    //}
                            //    //#endregion

                            //    //db.Tjob.AddRange(__jobH);
                            //    //db.Tjitems.AddRange(__jobD);
                            //    //db.ImportLogss.AddRange(LogImport);

                            //    //Console.WriteLine("--> Step5 db.Recording");
                            //    //db.SaveChanges();
                            //    MoveFile(JobRefFullPath, SAPBackup, RefFileName);

                            //    Console.WriteLine("Status: Success... " + DateTime.Now);
                            //    Console.WriteLine("Move File: " + JobRefFullPath + " Move to:" + SAPBackup + "\\" + RefFileName);
                            //    Console.WriteLine("===================================================");
                            //    Console.WriteLine(string.Empty);
                            //}
                        }
                    }
                }
                catch (Exception ex)
                {
                    var message = ex.InnerException == null ? ex.Message : ex.Message + ", Inner Exception: " + ex.InnerException.Message;

                    #region AddError
                    ErrorShipment.Add(new ErrorShipment()
                    {
                        ImportStatus = false,
                        FileName = RefFileName,
                        CreateBy = RefCreateBy,
                        Description = message,
                        CreateDate = DateTime.Now
                    });
                    #endregion

                    ImportLogs(PathLogs, ErrorShipment, RefFileName);
                    MoveFile(JobRefFullPath, SAPError, RefFileName);

                    Console.WriteLine("Sending email...");
                    Mail(ErrorShipment, RefFileName);

                    Console.WriteLine("Status: Error... " + DateTime.Now);
                    Console.WriteLine("--->" + message);
                    Console.WriteLine("Move File: " + JobRefFullPath + " Move to:" + SAPError + "\\" + RefFileName);
                    Console.WriteLine("===================================================");
                    Console.WriteLine(string.Empty);

                    string str = ("\r\n"
                          + "Method Name:" + MethodBase.GetCurrentMethod().Name + "\r\n"
                          + "Class Name :" + this.GetType().ToString() + "\r\n"
                          + "File Name  :" + RefFileName + "\r\n"
                          + "Messsage   :" + message + "\r\n");

                    Logs.Error(str);
                }
            }
        }

        public void ImportScale_XLSX()
        {
            FileInfo[] Files = ScaleJob.GetFiles("*.xlsx");
            foreach (FileInfo i in Files)
            {
                List<ErrorShipment> ErrorShipment = new List<ErrorShipment>();
                List<Shipment> Jobs = new List<Shipment>();
                //List<Tjob> __jobH = new List<Tjob>();
                //List<Tjitem> __jobD = new List<Tjitem>();
                //List<ImportLogs> LogImport = new List<ImportLogs>();

                string RefFileName = i.Name;
                string JobRefFullPath = ScaleJob + "\\" + RefFileName;
                string PathLogs = ScaleLogs;

                try
                {
                    Console.WriteLine("Start Scale Import Excel File:");
                    Console.WriteLine();
                    Console.WriteLine("start Time: " + DateTime.Now);
                    Console.WriteLine("File Name - {0}", RefFileName);

                    var ScaleExcels = new ReadExcels().ReadExcel<ScaleExcelFile>(JobRefFullPath);

                    Console.WriteLine("File Count " + ScaleExcels.Count);

                    if (ScaleExcels.Count == 0)
                    {
                        #region AddError
                        ErrorShipment.Add(new ErrorShipment()
                        {
                            ImportStatus = false,
                            FileName = RefFileName,
                            CreateBy = RefCreateBy,
                            Description = "Error Empty file upload result.",
                            CreateDate = DateTime.Now
                        });
                        #endregion
                        ImportLogs(PathLogs, ErrorShipment, RefFileName);
                        MoveFile(JobRefFullPath, ScaleError, RefFileName);

                        Console.WriteLine("Sending email...");
                        Mail(ErrorShipment, RefFileName);

                        Console.WriteLine("Status: Error... " + DateTime.Now);
                        Console.WriteLine("---> Error Empty file upload result.");
                        Console.WriteLine("Move File: " + JobRefFullPath + " Move to:" + ScaleError + "\\" + RefFileName);
                        Console.WriteLine("===================================================");
                        Console.WriteLine(string.Empty);
                    }
                    else
                    {
                        ErrorShipment = CheckErrorScaleExcel(ScaleExcels, RefFileName);
                        if (ErrorShipment.Count > 0)
                        {
                            Console.WriteLine("ErrorShipment...");

                            ImportLogs(PathLogs, ErrorShipment, RefFileName);
                            MoveFile(JobRefFullPath, ScaleError, RefFileName);

                            Console.WriteLine("Sending email...");
                            Mail(ErrorShipment, RefFileName);

                            Console.WriteLine("Status: Error... " + DateTime.Now);
                            Console.WriteLine("---> Error invalid data found.");
                            Console.WriteLine("Move File: " + JobRefFullPath + " Move to:" + ScaleError + "\\" + RefFileName);
                            Console.WriteLine("===================================================");
                            Console.WriteLine(string.Empty);

                        }
                        else
                        {
                            Console.WriteLine("--> Step2 Runinng JobNO");
                            JobsScaleExcel(ScaleExcels, RefFileName,PathLogs);

                            MoveFile(JobRefFullPath, ScaleBackup, RefFileName);

                            Console.WriteLine("Status: Success... " + DateTime.Now);
                            Console.WriteLine("Move File: " + JobRefFullPath + " Move to:" + ScaleBackup + "\\" + RefFileName);
                            Console.WriteLine("===================================================");
                            Console.WriteLine(string.Empty);

                            #region
                            //using (var db = new DatabaseManager().Prototype())
                            //{

                            //    Console.WriteLine("--> Step2 Runinng JobNO");
                            //    JobsScaleExcel(ScaleExcels, RefFileName);

                            //    //Console.WriteLine("--> Step3 Add Tjitem");
                            //    //__jobD = JobDetail(Jobs, RefFileName);

                            //    //Console.WriteLine("--> Step4 Add Tjob");
                            //    //__jobH = JobHeader(Jobs, RefFileName);

                            //    //#region Add Logs
                            //    //foreach (var H in __jobH)
                            //    //{
                            //    //    LogImport.Add(new ImportLogs()
                            //    //    {
                            //    //        ImportStatus = true,
                            //    //        FileName = RefFileName,
                            //    //        JobNo = H.Jobno,
                            //    //        CreateBy = RefCreateBy,
                            //    //        Description = "Insert job completed,REF1=" + H.Ref1,
                            //    //        CreateDate = DateTime.Now
                            //    //    });
                            //    //}
                            //    //#endregion

                            //    //db.Tjob.AddRange(__jobH);
                            //    //db.Tjitems.AddRange(__jobD);
                            //    //db.ImportLogss.AddRange(LogImport);

                            //    //Console.WriteLine("--> Step5 db.Recording");
                            //    //db.SaveChanges();

                            //    MoveFile(JobRefFullPath, ScaleBackup, RefFileName);

                            //    Console.WriteLine("Status: Success... " + DateTime.Now);
                            //    Console.WriteLine("Move File: " + JobRefFullPath + " Move to:" + ScaleBackup + "\\" + RefFileName);
                            //    Console.WriteLine("===================================================");
                            //    Console.WriteLine(string.Empty);

                            //}
                            #endregion
                        }
                    }
                }
                catch (Exception ex)
                {
                    var message = ex.InnerException == null ? ex.Message : ex.Message + ", Inner Exception: " + ex.InnerException.Message;

                    #region AddError
                    ErrorShipment.Add(new ErrorShipment()
                    {
                        ImportStatus = false,
                        FileName = RefFileName,
                        CreateBy = RefCreateBy,
                        Description = message,
                        CreateDate = DateTime.Now
                    });
                    #endregion

                    ImportLogs(PathLogs, ErrorShipment, RefFileName);
                    MoveFile(JobRefFullPath, ScaleError, RefFileName);

                    Console.WriteLine("Sending email...");
                    Mail(ErrorShipment, RefFileName);

                    Console.WriteLine("Status: Error... " + DateTime.Now);
                    Console.WriteLine("--->" + message);
                    Console.WriteLine("Move File: " + JobRefFullPath + " Move to:" + ScaleError + "\\" + RefFileName);
                    Console.WriteLine("===================================================");
                    Console.WriteLine(string.Empty);

                    string str = ("\r\n"
                         + "Method Name:" + MethodBase.GetCurrentMethod().Name + "\r\n"
                         + "Class Name :" + this.GetType().ToString() + "\r\n"
                         + "File Name  :" + RefFileName + "\r\n"
                         + "Messsage   :" + message + "\r\n");

                    Logs.Error(str);
                }


            }
        }

        public List<ErrorShipment> CheckErrorSAP(List<Shipment> shipments, string RefFileName)
        {
            List<ErrorShipment> ErrorShipment = new List<ErrorShipment>();
            try
            {
                Console.WriteLine("-->Step1 Check error file");
                #region check null Delivery
                Console.WriteLine("--> - Check null Delivery");

                shipments.Where(d => string.IsNullOrEmpty(d.Delivery)).ToList().ForEach(s =>
                {
                    ErrorShipment.Add(new ErrorShipment()
                    {
                        ImportStatus = false,
                        FileName = RefFileName,
                        Delivery = s.Delivery,
                        Row = s.RowNo,
                        CreateBy = RefCreateBy,
                        Description = "Error Delivery is null",
                        CreateDate = DateTime.Now
                    });
                });
                #endregion

                #region check null Item
                Console.WriteLine("--> - Check null Item");

                shipments.Where(d => string.IsNullOrEmpty(d.Article)).ToList().ForEach(s =>
                {
                    ErrorShipment.Add(new ErrorShipment()
                    {
                        ImportStatus = false,
                        FileName = RefFileName,
                        Delivery = s.Delivery,
                        Row = s.RowNo,
                        CreateBy = RefCreateBy,
                        Description = "Error Article is null,REF1=" + s.Delivery,
                        CreateDate = DateTime.Now
                    });
                });
                #endregion

                #region Check null Ship-to.
                Console.WriteLine("--> - Check null Ship-to.");

                shipments.Where(d => string.IsNullOrEmpty(d.ShipTo)).ToList().ForEach(s =>
                {
                    ErrorShipment.Add(new ErrorShipment()
                    {
                        ImportStatus = false,
                        FileName = RefFileName,
                        Delivery = s.Delivery,
                        Row = s.RowNo,
                        CreateBy = RefCreateBy,
                        Description = "Error=The ShipTo is null,REF1=" + s.Delivery,
                        CreateDate = DateTime.Now
                    });
                });
                #endregion

                #region Check Duplicate Delivery && Check Master Site.
                using (var db = new DatabaseManager().Prototype())
                {
                    Console.WriteLine("--> - Check Duplicate Delivery");
                    var deliverylist = shipments.Select(r => r.Delivery).Distinct().ToList();
                    var joblist_delivery = db.Tjob.Where(g => deliverylist.Contains(g.Ref1)).ToList();

                    foreach (var s in shipments)
                    {
                        var Delivery_dup = joblist_delivery.Where(g => g.Ref1 == s.Delivery).FirstOrDefault();
                        if (Delivery_dup != null)
                        {
                            ErrorShipment.Add(new ErrorShipment()
                            {
                                ImportStatus = false,
                                FileName = RefFileName,
                                Delivery = s.Delivery,
                                Row = s.RowNo,
                                CreateBy = RefCreateBy,
                                Description = "Error=The job has already exist in database,REF1=" + s.Delivery,
                                CreateDate = DateTime.Now
                            });
                        }
                    };


                    Console.WriteLine("--> - Check Master Site.");
                    var Poiid = db.Tpoi.Where(r => r.Type == "G").Select(r => r.Poiid).Distinct().ToList();
                    var Poiid_notfound = shipments.Where(g => !Poiid.Contains(g.Site)).ToList();
                    if (Poiid_notfound.Count() > 0)
                    {
                        foreach (var s in Poiid_notfound)
                        {
                            ErrorShipment.Add(new ErrorShipment()
                            {
                                ImportStatus = false,
                                FileName = RefFileName,
                                Delivery = s.Delivery,
                                Row = s.RowNo,
                                CreateBy = RefCreateBy,
                                Description = "Error=The Site id " + s.Site + "not found,REF1=" + s.Delivery,
                                CreateDate = DateTime.Now
                            });
                        };
                    }
                }
                #endregion

                #region check Delivery quantity 
                Console.WriteLine("--> - Check Delivery Quantity");

                shipments.Where(d => d.DeliveryQuantity == 0).ToList().ForEach(s =>
                {
                    ErrorShipment.Add(new ErrorShipment()
                    {
                        ImportStatus = false,
                        FileName = RefFileName,
                        Delivery = s.Delivery,
                        Row = s.RowNo,
                        CreateBy = RefCreateBy,
                        Description = "Error Quantity,REF1 = " + s.Delivery,
                        CreateDate = DateTime.Now
                    });

                });
                #endregion

                #region check pickdate > deliveryDate
                Console.WriteLine("--> - Check pickdate > deliveryDate");

                shipments.Where(d => d.PickDate > d.DelivDate).ToList().ForEach(s =>
                {
                    ErrorShipment.Add(new ErrorShipment()
                    {
                        ImportStatus = false,
                        FileName = RefFileName,
                        Delivery = s.Delivery,
                        Row = s.RowNo,
                        CreateBy = RefCreateBy,
                        Description = "Error pickdate > deliveryDate,REF1 = " + s.Delivery,
                        CreateDate = DateTime.Now
                    });

                });
                #endregion


            }
            catch (Exception ex)
            {
                var message = ex.InnerException == null ? ex.Message : ex.Message + ", Inner Exception: " + ex.InnerException.Message;
                string str = ("\r\n"
                          + "Method Name:" + MethodBase.GetCurrentMethod().Name + "\r\n"
                          + "Class Name :" + this.GetType().ToString() + "\r\n"
                          + "File Name  :" + RefFileName + "\r\n"
                          + "Messsage   :" + message + "\r\n");

                Logs.Error(str);

                Console.WriteLine(message);
                Console.ReadLine();
            }

            return ErrorShipment;
        }

        public List<ErrorShipment> CheckErrorSAPExcel(List<SAPXLSX> shipments, string RefFileName)
        {
            List<ErrorShipment> ErrorShipment = new List<ErrorShipment>();
            try
            {
                Console.WriteLine("-->Step1 Check error file");
                #region check null Delivery
                Console.WriteLine("--> - Check null Delivery");

                shipments.Where(d => string.IsNullOrEmpty(d.Delivery)).ToList().ForEach(s =>
                {
                    ErrorShipment.Add(new ErrorShipment()
                    {
                        ImportStatus = false,
                        FileName = RefFileName,
                        Delivery = s.Delivery,
                        Row = s.RowNo,
                        CreateBy = RefCreateBy,
                        Description = "Error Delivery is null",
                        CreateDate = DateTime.Now
                    });
                });
                #endregion

                #region check null Item
                Console.WriteLine("--> - Check null Item");

                shipments.Where(d => string.IsNullOrEmpty(d.Article)).ToList().ForEach(s =>
                {
                    ErrorShipment.Add(new ErrorShipment()
                    {
                        ImportStatus = false,
                        FileName = RefFileName,
                        Delivery = s.Delivery,
                        Row = s.RowNo,
                        CreateBy = RefCreateBy,
                        Description = "Error Article is null,REF1=" + s.Delivery,
                        CreateDate = DateTime.Now
                    });
                });
                #endregion

                #region Check null Ship-to.
                Console.WriteLine("--> - Check null Ship-to.");

                shipments.Where(d => string.IsNullOrEmpty(d.ShipTo)).ToList().ForEach(s =>
                {
                    ErrorShipment.Add(new ErrorShipment()
                    {
                        ImportStatus = false,
                        FileName = RefFileName,
                        Delivery = s.Delivery,
                        Row = s.RowNo,
                        CreateBy = RefCreateBy,
                        Description = "Error=The ShipTo is null,REF1=" + s.Delivery,
                        CreateDate = DateTime.Now
                    });
                });
                #endregion

                #region Check Duplicate Delivery && Check Master Site.
                using (var db = new DatabaseManager().Prototype())
                {
                    Console.WriteLine("--> - Check Duplicate Delivery");
                    var deliverylist = shipments.Select(r => r.Delivery).Distinct().ToList();
                    var joblist_delivery = db.Tjob.Where(g => deliverylist.Contains(g.Ref1)).ToList();
                    foreach (var s in shipments)
                    {
                        var Delivery_dup = joblist_delivery.Where(g => g.Ref1 == s.Delivery).FirstOrDefault();
                        if (Delivery_dup != null)
                        {
                            ErrorShipment.Add(new ErrorShipment()
                            {
                                ImportStatus = false,
                                FileName = RefFileName,
                                Delivery = s.Delivery,
                                Row = s.RowNo,
                                CreateBy = RefCreateBy,
                                Description = "Error=The job has already exist in database,REF1=" + s.Delivery,
                                CreateDate = DateTime.Now
                            });
                        }
                    };

                    Console.WriteLine("--> - Check Master Site.");
                    var Poiid = db.Tpoi.Where(r => r.Type == "G").Select(r => r.Poiid).Distinct().ToList();
                    var Poiid_notfound = shipments.Where(g => !Poiid.Contains(g.Site)).ToList();
                    if (Poiid_notfound.Count() > 0)
                    {
                        foreach (var s in Poiid_notfound)
                        {
                            ErrorShipment.Add(new ErrorShipment()
                            {
                                ImportStatus = false,
                                FileName = RefFileName,
                                Delivery = s.Delivery,
                                Row = s.RowNo,
                                CreateBy = RefCreateBy,
                                Description = "Error=The Site id " + s.Site + "not found,REF1=" + s.Delivery,
                                CreateDate = DateTime.Now
                            });
                        };
                    }
                }
                #endregion

                #region check Delivery quantity 
                Console.WriteLine("--> - Check Delivery Quantity");

                shipments.Where(d => d.DeliveryQuantity == 0).ToList().ForEach(s =>
                {
                    ErrorShipment.Add(new ErrorShipment()
                    {
                        ImportStatus = false,
                        FileName = RefFileName,
                        Delivery = s.Delivery,
                        Row = s.RowNo,
                        CreateBy = RefCreateBy,
                        Description = "Error Quantity,REF1 = " + s.Delivery,
                        CreateDate = DateTime.Now
                    });

                });
                #endregion

                #region check pickdate > deliveryDate
                Console.WriteLine("--> - Check pickdate > deliveryDate");
                shipments.Where(d => d.PickDate > d.DelivDate).ToList().ForEach(s =>
                {
                    ErrorShipment.Add(new ErrorShipment()
                    {
                        ImportStatus = false,
                        FileName = RefFileName,
                        Delivery = s.Delivery,
                        Row = s.RowNo,
                        CreateBy = RefCreateBy,
                        Description = "Error pickdate > deliveryDate,REF1 = " + s.Delivery,
                        CreateDate = DateTime.Now
                    });

                });
                #endregion
            }
            catch (Exception ex)
            {
                var message = ex.InnerException == null ? ex.Message : ex.Message + ", Inner Exception: " + ex.InnerException.Message;
                string str = ("\r\n"
                         + "Method Name:" + MethodBase.GetCurrentMethod().Name + "\r\n"
                         + "Class Name :" + this.GetType().ToString() + "\r\n"
                         + "File Name  :" + RefFileName + "\r\n"
                         + "Messsage   :" + message + "\r\n");

                Logs.Error(str);

                Console.WriteLine(message);
                Console.ReadLine();
            }

            return ErrorShipment;
        }

        public List<ErrorShipment> CheckErrorScaleExcel(List<ScaleExcelFile> excels, string RefFileName)
        {
            List<ErrorShipment> ErrorShipment = new List<ErrorShipment>();
            try
            {
                Console.WriteLine("-->Step1 Check error file");
                #region check null Delivery
                Console.WriteLine("--> - Check null Delivery");
                excels.Where(d => string.IsNullOrEmpty(d.DELIVERY_ORDER)).ToList().ForEach(s =>
                {
                    ErrorShipment.Add(new ErrorShipment()
                    {
                        ImportStatus = false,
                        FileName = RefFileName,
                        Row = s.RowNo,
                        CreateBy = RefCreateBy,
                        Description = "Error Delivery is null",
                        CreateDate = DateTime.Now
                    });
                });
                #endregion

                #region check null Item
                Console.WriteLine("--> - Check null Item");

                excels.Where(d => string.IsNullOrEmpty(d.ITEM)).ToList().ForEach(s =>
                {
                    ErrorShipment.Add(new ErrorShipment()
                    {
                        ImportStatus = false,
                        FileName = RefFileName,
                        Delivery = s.DELIVERY_ORDER,
                        Row = s.RowNo,
                        CreateBy = RefCreateBy,
                        Description = "Error Item is null,REF1=" + s.DELIVERY_ORDER,
                        CreateDate = DateTime.Now
                    });
                });
                #endregion

                #region check null Ship-to.
                Console.WriteLine("--> - Check null Ship-to.");

                excels.Where(d => string.IsNullOrEmpty(d.SHIP_TO)).ToList().ForEach(s =>
                {
                    ErrorShipment.Add(new ErrorShipment()
                    {
                        ImportStatus = false,
                        FileName = RefFileName,
                        Delivery = s.DELIVERY_ORDER,
                        Row = s.RowNo,
                        CreateBy = RefCreateBy,
                        Description = "Error=The ShipTo is null,REF1=" + s.DELIVERY_ORDER,
                        CreateDate = DateTime.Now
                    });
                });
                #endregion

                #region Check Duplicate Delivery && Check Master Site.
                Console.WriteLine("--> - Check Duplicate Delivery");
                using (var db = new DatabaseManager().Prototype())
                {
                    var deliverylist = excels.Select(r => r.DELIVERY_ORDER).Distinct().ToList();
                    var joblist = db.Tjob.Where(g => deliverylist.Contains(g.Ref1)).ToList();                  

                    foreach (var s in excels)
                    {
                        var Delivery_dup = joblist.Where(g => g.Ref1 == s.DELIVERY_ORDER).FirstOrDefault();
                        if (Delivery_dup != null)
                        {
                            ErrorShipment.Add(new ErrorShipment()
                            {
                                ImportStatus = false,
                                FileName = RefFileName,
                                Delivery = s.DELIVERY_ORDER,
                                ShipmentID = s.SHIPMENT,
                                Row = s.RowNo,
                                CreateBy = RefCreateBy,
                                Description = "Error=The job has already exist in database,REF1=" + s.DELIVERY_ORDER,
                                CreateDate = DateTime.Now
                            });
                        }
                    };

                    Console.WriteLine("--> - Check Master Site.");
                    var Poiid = db.Tpoi.Where(r => r.Type == "G").Select(r => r.Poiid).Distinct().ToList();
                    var Poiid_notfound = excels.Where(g => !Poiid.Contains(g.Site)).ToList();
                    if (Poiid_notfound.Count() > 0)
                    {
                        foreach (var s in Poiid_notfound)
                        {
                            ErrorShipment.Add(new ErrorShipment()
                            {
                                ImportStatus = false,
                                FileName = RefFileName,
                                Delivery = s.DELIVERY_ORDER,
                                Row = s.RowNo,
                                CreateBy = RefCreateBy,
                                Description = "Error=The Site id " + s.Site + "not found,REF1=" + s.DELIVERY_ORDER,
                                CreateDate = DateTime.Now
                            });
                        };
                    }
                }
                #endregion

                #region check Delivery quantity 
                Console.WriteLine("--> - Check Delivery Quantity ");
                excels.Where(d => d.ALLOCATED_QTY == 0).ToList().ForEach(s =>
                {
                    ErrorShipment.Add(new ErrorShipment()
                    {
                        ImportStatus = false,
                        FileName = RefFileName,
                        Delivery = s.DELIVERY_ORDER,
                        ShipmentID = s.SHIPMENT,
                        Row = s.RowNo,
                        CreateBy = RefCreateBy,
                        Description = "Error Allocated_qty ,REF1 = " + s.DELIVERY_ORDER,
                        CreateDate = DateTime.Now
                    });
                });
                #endregion

                #region check pickdate > deliveryDate
                Console.WriteLine("--> - check pickdate > deliveryDate");
                excels.Where(d => d.ORDER_DATE > d.PLANNED_SHIP_DATE).ToList().ForEach(s =>
                {
                    ErrorShipment.Add(new ErrorShipment()
                    {
                        ImportStatus = false,
                        FileName = RefFileName,
                        Delivery = s.DELIVERY_ORDER,
                        ShipmentID = s.SHIPMENT,
                        Row = s.RowNo,
                        CreateBy = RefCreateBy,
                        Description = "Error Order_date > Planned_ship_date,REF1 = " + s.DELIVERY_ORDER,
                        CreateDate = DateTime.Now
                    });
                });
                #endregion
            }
            catch (Exception ex)
            {
                var message = ex.InnerException == null ? ex.Message : ex.Message + ", Inner Exception: " + ex.InnerException.Message;
                string str = ("\r\n"
                         + "Method Name:" + MethodBase.GetCurrentMethod().Name + "\r\n"
                         + "Class Name :" + this.GetType().ToString() + "\r\n"
                         + "File Name  :" + RefFileName + "\r\n"
                         + "Messsage   :" + message + "\r\n");

                Logs.Error(str);

                Console.WriteLine(message);
                Console.ReadLine();
            }

            return ErrorShipment;
        }


        public List<ErrorShipment> RunningJobno(string RefFileName)
        {
            List<ErrorShipment> ErrorShipment = new List<ErrorShipment>();
            DateTime DateTimeZone = DateTime.Now;

            try
            {
                //string DODate = "DO" + DateTime.Parse(DateTime.Now.ToString()).ToString("yyMMdd", new CultureInfo("en-GB"));

                //using (var db = new DatabaseManager().Prototype())
                //{
                //    var JobDateAll = db.Tjob.Where(o => o.Jobno.Substring(0, 9) == DODate).Count();
                //    if (JobDateAll > 0)
                //    {
                //        jobRun = JobDateAll;
                //    }
                //    jobRun++;

                //    Jobno = DODate + jobRun.ToString("D5");
                //}

                using (var db = new DatabaseManager().Prototype())
                {
                    //var oPrefix = db.Mdocruns.Where(o => o.Prefix == mPrefix && o.Type.Equals("job")).FirstOrDefault();
                    var oPrefix = db.Mdocrun.Where(o => o.Isdefault == true && o.Type.Equals("job")).FirstOrDefault();

                    if (oPrefix != null)
                    {
                        int JobRunning = int.Parse(oPrefix.Currentrun);

                        if (JobRunning == 0)
                        {
                            var CoutOfJobno = db.Tjob.Count(r => r.Jobno.Substring(0, oPrefix.Prefix.Length) == oPrefix.Prefix);
                            if (CoutOfJobno > 0)
                            {
                                JobRunning = CoutOfJobno;
                            }
                        }

                        int PrefixLength = oPrefix.Prefix.Length;
                        DateTime? LastUpdatePrefix = oPrefix.Lastcreate;

                        string Delimiter = string.Empty;
                        string DateFormat = string.Empty;
                        string JobFormat = "0000";

                        if (!string.IsNullOrEmpty(oPrefix.Docformat))
                        {
                            JobFormat = oPrefix.Docformat;
                        }

                        string[] datas = JobFormat.Split('-');

                        List<string> ftypes = new List<string>();

                        if (datas.Count() > 1)
                        {
                            Delimiter = JobFormat.Substring(PrefixLength, 1);
                            Delimiter = (Delimiter.Equals("-")) ? Delimiter : "";
                        }

                        int xstart, xlen = 0;

                        if (oPrefix.Docformat.Substring(oPrefix.Prefix.Length + 1, 1) == "Y")
                        {
                            if (oPrefix.Prefix.Length == datas[0].Length)
                            {
                                xstart = oPrefix.Prefix.Length + Delimiter.Length;
                                xlen = oPrefix.Docformat.Length - xstart - (oPrefix.Startrun.Length + Delimiter.Length);
                                DateFormat = oPrefix.Docformat.Substring(xstart, xlen).ToLower().Replace("mm", "MM");
                            }
                            else
                            {
                                xstart = oPrefix.Prefix.Length;
                                xlen = oPrefix.Docformat.Length - xstart - (oPrefix.Startrun.Length);
                                DateFormat = oPrefix.Docformat.Substring(xstart, xlen).ToLower().Replace("mm", "MM");
                            }
                        }

                        if (string.IsNullOrEmpty(DateFormat))
                        {
                            JobRunning++;
                        }

                        else
                        {
                            if (LastUpdatePrefix != null)
                            {
                                var sd1 = DateTimeZone.ToString(DateFormat);
                                var sd2 = ((DateTime)oPrefix.Lastcreate).ToString(DateFormat);
                                if (sd1.Equals(sd2))
                                {
                                    JobRunning++;
                                }
                                else
                                {
                                    JobRunning = 1;
                                    oPrefix.Lastcreate = DateTimeZone;
                                }
                            }
                            else
                            {
                                JobRunning = 1;
                                oPrefix.Lastcreate = DateTimeZone;
                            }
                        }

                        oPrefix.Currentrun = JobRunning.ToString("00");

                        if ((oPrefix.Startrun.Length - oPrefix.Currentrun.Length) < 0)
                        {
                            //rwm.message = "Max length prefix format. </br> Please renew prefix or create new.";
                            //return rwm;
                            ErrorShipment.Add(new ErrorShipment()
                            {
                                ImportStatus = false,
                                FileName = RefFileName,
                                CreateBy = RefCreateBy,
                                Description = "Max length prefix format. </br> Please renew prefix or create new.",
                                CreateDate = DateTime.Now
                            });
                            return ErrorShipment;
                        }

                        var Nextformat = oPrefix.Startrun.Substring(0, oPrefix.Startrun.Length - oPrefix.Currentrun.Length) + oPrefix.Currentrun;

                        if (string.IsNullOrEmpty(DateFormat))
                        {
                            Jobno = oPrefix.Prefix + Delimiter + Nextformat;
                        }
                        else
                        {
                            if (Delimiter.Contains("-"))
                            {
                                Jobno = oPrefix.Prefix + Delimiter + DateTime.Parse(DateTime.Now.ToString()).ToString(DateFormat, new CultureInfo("en-GB")) + Delimiter + Nextformat;
                            }
                            else
                            {
                                Jobno = oPrefix.Prefix + DateTime.Parse(DateTime.Now.ToString()).ToString(DateFormat, new CultureInfo("en-GB")) + Nextformat;
                            }

                        }
                    }

                    db.SaveChanges();

                }
            }
            catch (Exception ex)
            {
                var message = ex.InnerException == null ? ex.Message : ex.Message + ", Inner Exception: " + ex.InnerException.Message;

                string str = ("\r\n"
                         + "Method Name:" + MethodBase.GetCurrentMethod().Name + "\r\n"
                         + "Class Name :" + this.GetType().ToString() + "\r\n"
                         + "File Name  :" + RefFileName + "\r\n"
                         + "Messsage   :" + message + "\r\n");

                Logs.Error(str);

                Console.WriteLine(message);
                Console.ReadLine();
            }
            return ErrorShipment;
        }

        public void JobsSAP(List<Shipment> shipments, string RefFileName,string PathLogs)
        {
            List<Shipment> Jobs = new List<Shipment>();
            List<ErrorShipment> ErrorShipment = new List<ErrorShipment>();
            List<ImportLogs> LogImport = new List<ImportLogs>();
            List<Tjob> __jobH = new List<Tjob>();
            List<Tjitem> __jobD = new List<Tjitem>();

            try
            {
                using (var db = new DatabaseManager().Prototype())
                {
                    var Shipto = shipments.Select(r => r.ShipTo).Distinct().ToList();
                    var Bpcode = db.Mbpd.Where(g => Shipto.Contains(g.Bpcode)).ToList();

                    shipments.GroupBy(s => new
                    {
                        s.Delivery,
                        s.ShipTo,
                        s.DPrio,
                        s.Site,
                        s.PickDate,
                        s.DelivDate,
                    }).Select(s => new { Delivery = s }).ToList().ForEach(_job =>
                    {
                        var indexitem = 1;
                        RunningJobno(RefFileName);
                        _job.Delivery.ToList().ForEach(J =>
                        {
                            var joblist_Bpcode = Bpcode.Where(g => g.Bpcode == J.ShipTo).Select(r => r.Bpcode).FirstOrDefault();
                            //พบ BPcode
                            //GroupID = (!string.IsNullOrEmpty(joblist_Bpcode)) ? "1000" : "9999";
                            if (joblist_Bpcode != null)
                            {
                                GroupID = "1000";
                            }
                            else
                            {
                                GroupID = "9999";
                                // Add error ส่ง mail 
                                ErrorShipment.Add(new ErrorShipment()
                                {
                                    ImportStatus = false,
                                    FileName = RefFileName,
                                    Delivery = J.Delivery,
                                    Row = J.RowNo,
                                    CreateBy = RefCreateBy,
                                    Description = "Error=The Ship to id = " + J.ShipTo + " not found,REF1=" + J.Delivery,
                                    CreateDate = DateTime.Now
                                });
                            }

                            Jobs.Add(new Shipment()
                            {
                                Jobno = Jobno,
                                Itemno = indexitem++,
                                Lenght = 1,
                                Height = 1,
                                Jobtype = Jobtype,
                                Jobstatus = Jobstatus,
                                Delivery = J.Delivery,
                                ShipTo = J.ShipTo,
                                DPrio = J.DPrio,
                                Site = J.Site,
                                PickDate = Convert.ToDateTime(J.PickDate),
                                DelivDate = Convert.ToDateTime(J.DelivDate),
                                Cby = RefCreateBy,
                                Article = J.Article,
                                Description = (string.IsNullOrEmpty(J.Description)) ? J.Article : J.Description,
                                DeliveryQuantity = J.DeliveryQuantity,
                                Unit = J.Unit,
                                TotalWeight = J.TotalWeight,
                                Volume = J.Volume,
                                Hhid = HHID,
                                Groupid = GroupID

                            });
                            __jobD = JobDetail(Jobs, RefFileName);
                        });
                    });

                    __jobH = JobHeader(Jobs, RefFileName);

                    #region Add Logs
                    foreach (var H in __jobH)
                    {
                        LogImport.Add(new ImportLogs()
                        {
                            ImportStatus = true,
                            FileName = RefFileName,
                            JobNo = H.Jobno,
                            CreateBy = RefCreateBy,
                            Description = "Insert job completed,REF1=" + H.Ref1,
                            CreateDate = DateTime.Now
                        });
                    }
                    #endregion

                    db.Tjob.AddRange(__jobH);
                    db.Tjitem.AddRange(__jobD);
                    db.ImportLogs.AddRange(LogImport);


                    Console.WriteLine("--> db.Recording");
                    db.SaveChanges();
                }

                
                if (ErrorShipment.Count > 0)
                {
                    Console.WriteLine("--> Sent mail Non Shipto.");
                    Mail(ErrorShipment, RefFileName);
                    ImportLogs(PathLogs, ErrorShipment, RefFileName);
                }

            }
            catch (Exception ex)
            {
                var message = ex.InnerException == null ? ex.Message : ex.Message + ", Inner Exception: " + ex.InnerException.Message;

                string str = ("\r\n"
                         + "Method Name:" + MethodBase.GetCurrentMethod().Name + "\r\n"
                         + "Class Name :" + this.GetType().ToString() + "\r\n"
                         + "File Name  :" + RefFileName + "\r\n"
                         + "Messsage   :" + message + "\r\n");

                Logs.Error(str);

                Console.WriteLine(message);
                Console.ReadLine();
            }

            //return Jobs;
        }

        public void JobsSAPExcel(List<SAPXLSX> SapExcels, string RefFileName,string PathLogs)
        {
            List<Shipment> Jobs = new List<Shipment>();
            List<ErrorShipment> ErrorShipment = new List<ErrorShipment>();
            List<Tjob> __jobH = new List<Tjob>();
            List<Tjitem> __jobD = new List<Tjitem>();
            List<ImportLogs> LogImport = new List<ImportLogs>();

            try
            {
                using (var db = new DatabaseManager().Prototype())
                {
                    var Shipto = SapExcels.Select(r => r.ShipTo).Distinct().ToList();
                    var Bpcode = db.Mbpd.Where(g => Shipto.Contains(g.Bpcode)).ToList();
                
                    SapExcels.GroupBy(s => new
                    {
                        s.Delivery,
                        s.ShipTo,
                        s.DPrio,
                        s.Site,
                        s.PickDate,
                        s.DelivDate,
                    }).Select(s => new { Delivery = s }).ToList().ForEach(_job =>
                    {

                        var indexitem = 1;
                        RunningJobno(RefFileName);
                        _job.Delivery.ToList().ForEach(J =>
                        {
                            var joblist_Bpcode = Bpcode.Where(g => g.Bpcode == J.ShipTo).Select(r => r.Bpcode).FirstOrDefault();
                            //พบ BPcode
                            //GroupID = (!string.IsNullOrEmpty(joblist_Bpcode)) ? "1000" : "9999";
                            if (joblist_Bpcode != null)
                            {
                                GroupID = "1000";
                            }
                            else
                            {
                                GroupID = "9999";
                                // Add error ส่ง mail 
                                ErrorShipment.Add(new ErrorShipment()
                                {
                                    ImportStatus = false,
                                    FileName = RefFileName,
                                    Delivery = J.Delivery,
                                    Row = J.RowNo,
                                    CreateBy = RefCreateBy,
                                    Description = "Error=The Ship to id = " + J.ShipTo + " not found,REF1=" + J.Delivery,
                                    CreateDate = DateTime.Now
                                });
                            }

                            Jobs.Add(new Shipment()
                            {
                                Jobno = Jobno,
                                Itemno = indexitem++,
                                Lenght = 1,
                                Height = 1,
                                Jobtype = Jobtype,
                                Jobstatus = Jobstatus,
                                Delivery = J.Delivery,
                                ShipTo = J.ShipTo,
                                DPrio = J.DPrio,
                                Site = J.Site,
                                PickDate = Convert.ToDateTime(J.PickDate),
                                DelivDate = Convert.ToDateTime(J.DelivDate),
                                Cby = RefCreateBy,
                                Article = J.Article,
                                //Description = J.Description,
                                Description = (string.IsNullOrEmpty(J.Description)) ? J.Article : J.Description,
                                DeliveryQuantity = J.DeliveryQuantity,
                                Unit = J.Unit,
                                TotalWeight = J.TotalWeight,
                                Volume = J.Volume,
                                Hhid = HHID,
                                Groupid = GroupID

                            });

                            __jobD = JobDetail(Jobs, RefFileName);
                        });
                    });

                    __jobH = JobHeader(Jobs, RefFileName);

                    #region Add Logs
                    foreach (var H in __jobH)
                    {
                        LogImport.Add(new ImportLogs()
                        {
                            ImportStatus = true,
                            FileName = RefFileName,
                            JobNo = H.Jobno,
                            CreateBy = RefCreateBy,
                            Description = "Insert job completed,REF1=" + H.Ref1,
                            CreateDate = DateTime.Now
                        });
                    }
                    #endregion

                    db.Tjob.AddRange(__jobH);
                    db.Tjitem.AddRange(__jobD);
                    db.ImportLogs.AddRange(LogImport);


                    Console.WriteLine("--> db.Recording");
                    db.SaveChanges();

                }

                if (ErrorShipment.Count > 0)
                {
                    Console.WriteLine("--> Sent mail Non Shipto.");
                    Mail(ErrorShipment, RefFileName);
                    ImportLogs(PathLogs, ErrorShipment, RefFileName);
                }
            }
            catch (Exception ex)
            {
                var message = ex.InnerException == null ? ex.Message : ex.Message + ", Inner Exception: " + ex.InnerException.Message;

                string str = ("\r\n"
                         + "Method Name:" + MethodBase.GetCurrentMethod().Name + "\r\n"
                         + "Class Name :" + this.GetType().ToString() + "\r\n"
                         + "File Name  :" + RefFileName + "\r\n"
                         + "Messsage   :" + message + "\r\n");

                Logs.Error(str);

                Console.WriteLine(message);
                Console.ReadLine();
            }

            //return Jobs;
        }
        public void JobsScaleExcel(List<ScaleExcelFile> ScaleExcels, string RefFileName,string PathLogs)
        {
            List<Shipment> Jobs = new List<Shipment>();
            List<MapGroupID> mapGroupIDs = new List<MapGroupID>();
            List<ErrorShipment> ErrorShipment = new List<ErrorShipment>();
            List<Tjob> __jobH = new List<Tjob>();
            List<Tjitem> __jobD = new List<Tjitem>();
            List<ImportLogs> LogImport = new List<ImportLogs>();
            string Remark = string.Empty;

            try
            {
                string json = File.ReadAllText("ShiptoMapGroupID.json");
                var listGroupid = JsonSerializer.Deserialize<List<MapGroupID>>(json);

                using (var db = new DatabaseManager().Prototype())
                {
                    var Shipto = ScaleExcels.Select(r => r.SHIP_TO).Distinct().ToList();
                    var Bpcode = db.Mbpd.Where(g => Shipto.Contains(g.Bpcode)).ToList();
                    ScaleExcels.GroupBy(s => new
                    {
                        s.DELIVERY_ORDER,
                        s.SHIPMENT,
                        s.SHIP_TO,
                        s.Site,
                        s.ORDER_DATE,
                        s.PLANNED_SHIP_DATE,
                    }).Select(s => new { Delivery = s }).ToList().ForEach(_job =>
                    {
                        var indexitem = 1;
                        RunningJobno(RefFileName);

                        _job.Delivery.ToList().ForEach(J =>
                        {

                            #region Check Customer
                            var joblist_Bpcode = Bpcode.Where(g => g.Bpcode == J.SHIP_TO).FirstOrDefault();
                            //พบ BPcode
                            if (joblist_Bpcode != null)
                            {
                                GroupID = listGroupid.Where(l => J.SHIP_TO.StartsWith(l.ShipTo)).Select(g => g.GroupID).FirstOrDefault();
                            }
                            else
                            {
                                GroupID = "9999";
                                // Add error ส่ง mail 
                                ErrorShipment.Add(new ErrorShipment()
                                {
                                    ImportStatus = false,
                                    FileName = RefFileName,
                                    Delivery = J.DELIVERY_ORDER,
                                    ShipmentID = J.SHIPMENT,
                                    Row = J.RowNo,
                                    CreateBy = RefCreateBy,
                                    Description = "Error=The Ship to id = " + J.SHIP_TO + " not found,REF1=" + J.DELIVERY_ORDER,
                                    CreateDate = DateTime.Now
                                });
                            }
                            #endregion

                            #region Check Shipment 

                            if (!string.IsNullOrEmpty(J.SHIPMENT) )
                            {
                                GroupID = "8888";
                                Remark = "Shipment ="+ J.SHIPMENT;
                            }
                            #endregion

                            Jobs.Add(new Shipment()
                            {
                                Jobno = Jobno,
                                Itemno = indexitem++,
                                Lenght = J.ITEM_LENGTH,
                                Height = J.ITEM_HEIGHT,
                                Unit = J.QUANTITY_UM,
                                Weight = J.ITEM_WEIGHT,
                                Width = J.ITEM_WIDTH,
                                Jobtype = Jobtype,
                                Jobstatus = Jobstatus,
                                ShipmentID = J.SHIPMENT,
                                //Hhid = (!string.IsNullOrEmpty(J.SHIPMENT)) ? "10000005" : HHID,
                                Remark = Remark,
                                Hhid = HHID,
                                Delivery = J.DELIVERY_ORDER,
                                ShipTo = J.SHIP_TO,
                                Site = J.Site,
                                PickDate = Convert.ToDateTime(J.ORDER_DATE),
                                DelivDate = Convert.ToDateTime(J.PLANNED_SHIP_DATE),
                                Cby = RefCreateBy,
                                Article = J.ITEM,
                                Description = J.ITEM,
                                DeliveryQuantity = J.ALLOCATED_QTY,
                                Groupid = (!string.IsNullOrEmpty(GroupID)) ? GroupID : "1025",
                            });

                            __jobD = JobDetail(Jobs, RefFileName);
                        });
                    });

                    __jobH = JobHeader(Jobs, RefFileName);

                    #region Add Logs
                    foreach (var H in __jobH)
                    {
                        LogImport.Add(new ImportLogs()
                        {
                            ImportStatus = true,
                            FileName = RefFileName,
                            JobNo = H.Jobno,
                            CreateBy = RefCreateBy,
                            Description = "Insert job completed,REF1=" + H.Ref1,
                            CreateDate = DateTime.Now
                        });
                    }
                    #endregion

                    db.Tjob.AddRange(__jobH);
                    db.Tjitem.AddRange(__jobD);
                    db.ImportLogs.AddRange(LogImport);


                    Console.WriteLine("--> db.Recording");
                    db.SaveChanges();

                }

                if (ErrorShipment.Count() > 0)
                {
                    Console.WriteLine("--> Sent mail Non Shipto.");
                    Mail(ErrorShipment, RefFileName);
                    ImportLogs(PathLogs, ErrorShipment, RefFileName);
                }
                
            }
            catch (Exception ex)
            {
                var message = ex.InnerException == null ? ex.Message : ex.Message + ", Inner Exception: " + ex.InnerException.Message;

                string str = ("\r\n"
                         + "Method Name:" + MethodBase.GetCurrentMethod().Name + "\r\n"
                         + "Class Name :" + this.GetType().ToString() + "\r\n"
                         + "File Name  :" + RefFileName + "\r\n"
                         + "Messsage   :" + message + "\r\n");


                Logs.Error(str);

                Console.WriteLine(message);
                Console.ReadLine();
            }

            //return Jobs;
        }

        public List<Tjitem> JobDetail(List<Shipment> Jobs, string RefFileName)
        {
            List<Tjitem> __jobD = new List<Tjitem>();
            try
            {
                Jobs.GroupBy(o => o.Delivery).Select(o => new { Delivery = o }).ToList().ForEach(_jobD =>
                {
                    _jobD.Delivery.ToList().ForEach(D =>
                    {
                        var Weight = D.TotalWeight != 0 ? D.TotalWeight / D.DeliveryQuantity : D.Weight;
                        if (Double.IsNaN(Weight))
                        {
                            Weight = 0;
                        }
                        __jobD.Add(new Tjitem()
                        {
                            Jobno = D.Jobno,
                            Itemno = D.Itemno,
                            Containerno = D.Article,
                            Itemname = D.Description,
                            Qty = D.DeliveryQuantity,
                            Unit = D.Unit,
                            Weight = Weight,
                            Width = D.Volume,
                            Lenght = D.Lenght,
                            Height = D.Height,
                            Cdate = DateTime.Now
                        });

                    });

                });
            }
            catch (Exception ex)
            {
                var message = ex.InnerException == null ? ex.Message : ex.Message + ", Inner Exception: " + ex.InnerException.Message;

                string str = ("\r\n"
                         + "Method Name:" + MethodBase.GetCurrentMethod().Name + "\r\n"
                         + "Class Name :" + this.GetType().ToString() + "\r\n"
                         + "File Name  :" + RefFileName + "\r\n"
                         + "Messsage   :" + message + "\r\n");

                Logs.Error(str);

                Console.WriteLine(message);
                Console.ReadLine();
            }

            return __jobD;
        }

        public List<Tjob> JobHeader(List<Shipment> Jobs, string RefFileName)
        {
            List<Tjob> __jobH = new List<Tjob>();
            try
            {
                var _jobH = Jobs.GroupBy(o => new
                {
                    o.Delivery,
                    o.ShipTo,
                    o.DPrio,
                    o.Site,
                    o.PickDate,
                    o.DelivDate,
                    o.Jobstatus,
                    o.Jobno,
                    o.Jobtype,
                    o.Hhid,
                    o.Cby,
                    o.Groupid,
                    o.Remark
                }).Select(s => new
                {
                    s.Key.Delivery,
                    s.Key.ShipTo,
                    s.Key.DPrio,
                    s.Key.Site,
                    s.Key.PickDate,
                    s.Key.DelivDate,
                    s.Key.Jobstatus,
                    s.Key.Jobno,
                    s.Key.Jobtype,
                    s.Key.Hhid,
                    s.Key.Cby,
                    s.Key.Groupid,
                    s.Key.Remark
                }).ToList();
                foreach (var H in _jobH)
                {
                    __jobH.Add(new Tjob
                    {
                        Jobno = H.Jobno,
                        Jobtype = H.Jobtype,
                        Jobstatus = H.Jobstatus,
                        Ref1 = H.Delivery,
                        Bpcode = H.ShipTo,
                        Poidelivery = H.ShipTo,
                        Ref2 = H.DPrio,//DPrio ช่วงเวลาในการจัดส่ง
                        Poireceive = H.Site,
                        Groupid = H.Groupid,
                        Rduedate = H.PickDate,
                        Dduedate = H.DelivDate,
                        Cdate = DateTime.Now,
                        Cby = H.Cby,
                        Hhid = H.Hhid,
                        Remark=H.Remark,
                    });
                }
            }
            catch (Exception ex)
            {
                var message = ex.InnerException == null ? ex.Message : ex.Message + ", Inner Exception: " + ex.InnerException.Message;
                string str = ("\r\n"
                         + "Method Name:" + MethodBase.GetCurrentMethod().Name + "\r\n"
                         + "Class Name :" + this.GetType().ToString() + "\r\n"
                         + "File Name  :" + RefFileName + "\r\n"
                         + "Messsage   :" + message + "\r\n");

                Logs.Error(str);

                Console.WriteLine(message);
                Console.ReadLine();
            }

            return __jobH;
        }

        public void ImportLogs(string PathLogs, List<ErrorShipment> ErrorShipment, string RefFileName)
        {

            List<ImportLogs> LogImport = new List<ImportLogs>();
            try
            {
                var str = string.Empty;
                using (var db = new DatabaseManager().Prototype())
                {
                    ErrorShipment.ToList().ForEach(e =>
                    {
                        LogImport.Add(new ImportLogs()
                        {
                            ImportStatus = e.ImportStatus,
                            FileName = e.FileName,
                            CreateBy = e.CreateBy,
                            Row = e.Row,
                            Description = e.Description,
                            CreateDate = DateTime.Now
                        });

                         str += ("\r\n"
                         + "Messsage :" + e.Description +",Row = "+e.Row);


                    });

                    db.ImportLogs.AddRange(LogImport);
                    db.SaveChanges();

                    //var Description = ErrorShipment.Select(r => r.Description).ToList();

                    Logs.More(PathLogs, str, RefFileName);

                };
            }
            catch (Exception ex)
            {
                var message = ex.InnerException == null ? ex.Message : ex.Message + ", Inner Exception: " + ex.InnerException.Message;
                string str = ("\r\n"
                         + "Method Name:" + MethodBase.GetCurrentMethod().Name + "\r\n"
                         + "Class Name :" + this.GetType().ToString() + "\r\n"
                         + "File Name  :" + RefFileName + "\r\n"
                         + "Messsage   :" + message + "\r\n");

                Logs.Error(str);

                Console.WriteLine(message);
                Console.ReadLine();
            }
        }


        //public void WriteLogsCSV(string _FileName)
        //{
        //    try
        //    {
        //        if (!Directory.Exists(_rdm.LogDirectory + "\\Test"))
        //        {
        //            Directory.CreateDirectory(_rdm.LogDirectory + "\\Test");
        //        }

        //        ////สรุป Logs ทั้งหมด
        //        string FilePath = _rdm.LogDirectory + "\\Test\\" + DateTime.Now.ToString("yyyyMMddHHmmss_") + _FileName;

        //        using (var writer = new StreamWriter(FilePath, true, Encoding.UTF8))
        //        {
        //            var csv = new CsvWriter(writer);
        //            csv.Configuration.Delimiter = ",";
        //            csv.WriteRecords(this.LogImport.Where(r => r.ImportStatus == false && r.FileName == _FileName).Select(r => new { No = r.No, FilseName = r.FileName, JobNo = r.JobNo, ImportStatus = r.ImportStatus, Row = r.Row, Col = r.Col, Description = r.Description, CreateDate = r.CreateDate, CreateBy = 1000 }));

        //            writer.Close();
        //            writer.Flush();
        //            writer.Dispose();
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        var mesg = "WriteErrorLogs CSV : " + this.RefFileName + " CompunyID : " + this.CompanyID + " " + ex.Message;
        //        if (ex.InnerException != null)
        //        {
        //            mesg += ex.InnerException.Message;
        //        }
        //        //comment
        //        //Logs.Error(mesg);
        //    }
        //}


        public void MoveFile(string JobRefFullPath, DirectoryInfo Directory, string RefFileName)
        {
            try
            {
                if (File.Exists(Directory + "\\" + RefFileName))
                {
                    File.Delete(Directory + "\\" + RefFileName);
                }
                File.Move(JobRefFullPath, Directory + "\\" + RefFileName);
            }
            catch (Exception ex)
            {
                var message = ex.InnerException == null ? ex.Message : ex.Message + ", Inner Exception: " + ex.InnerException.Message;
                string str = ("\r\n"
                         + "Method Name:" + MethodBase.GetCurrentMethod().Name + "\r\n"
                         + "Class Name :" + this.GetType().ToString() + "\r\n"
                         + "File Name  :" + RefFileName + "\r\n"
                         + "Messsage   :" + message + "\r\n");

                Logs.Error(str);

                Console.WriteLine(message);
                Console.ReadLine();
            }

        }

        public void Mail(List<ErrorShipment> Errors, string RefFileName)
        {
            try
            {
                #region dotNet Core 5
                var errorList = Errors.Where(o => o.ImportStatus == false).GroupBy(o => new { o.Delivery }).Count();

                var email = new MimeMessage();

                email.From.Add(new MailboxAddress("Sky Frog Support", Configuration["EMail_SkyFrog_Support"]));
                email.To.Add(MailboxAddress.Parse(Configuration["EMail_Customer"]));
                email.Cc.Add(MailboxAddress.Parse(Configuration["EMail_Customer_cc"]));
                email.Subject = "[" + CompanyID + "] Failure to imported file" + DateTime.Now.ToString("dd-MM-yyyy hh:mm tt");
                int errorindex = 1;
                var text = "";

                Errors.Where(i => i.ImportStatus == false).ToList().ForEach(e =>
                {
                    text += "" + (errorindex++) + ". { Row:  \"" + e.Row + "\", Description : \"" + e.Description + "\" }" + "<br>";
                });

                email.Body = new TextPart(TextFormat.Html)
                {
                    Text = "<p>Dear All</p>" +
                    "<br><p>The file has been failure import. See more details below</p>" +
                    "<br><p>File name :" + RefFileName +
                    "<br>Import date :" + DateTime.Now.ToString("dd-MM-yyyy hh:mm tt") +
                    "<br>Error :" + errorList + " Job" + (errorList > 1 ? "s" : "") + "<br/>"
                    + (errorList < 30 ? "<br/>TOP 30 ERROR <br/>" + text : "")
                };

                SmtpClient smtp = new SmtpClient();

                smtp.ServerCertificateValidationCallback = (s, c, h, e) => true;
                smtp.Connect("103.87.217.251", 25, SecureSocketOptions.Auto);
                smtp.Authenticate("skyfrog@smtp-hgc-251.debutmail.com", "9%cP#6Wa");

                smtp.Send(email);
                smtp.Disconnect(true);
                #endregion
            }

            catch (Exception ex)
            {
                var message = ex.InnerException == null ? ex.Message : ex.Message + ", Inner Exception: " + ex.InnerException.Message;

                string str = ("\r\n"
                     + "Method Name:" + MethodBase.GetCurrentMethod().Name + "\r\n"
                     + "Class Name :" + this.GetType().ToString() + "\r\n"
                     + "File Name  :" + RefFileName + "\r\n"
                     + "Messsage   :" + message + "\r\n");

                Logs.Error(str);

                Console.WriteLine(message);
                Console.ReadLine();
            }

            Console.WriteLine("Sent E-Mail success." + DateTime.Now);
        }

    }
}
