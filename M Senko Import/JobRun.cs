using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using M_Senko_Import.Controller;

namespace M_Senko_Import
{
    internal class JobRun
    {
        public static IConfiguration Configuration => new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile("appsettings.json").Build();
        private JobController JobController = new JobController();

        public void Import()
        {
            CreatePath();
            //JobController.ImportSAP_CSV();
            JobController.ImportSAP_XML();
            JobController.ImportSAP_XLSX();
            JobController.ImportScale_XLSX();

        }

        public void CreatePath()
        {
            #region SAP create path 
            string SAPError = Configuration["SAPError"];
            if (!Directory.Exists(SAPError))
            {
                Directory.CreateDirectory(SAPError);
            }
            string SAPJob = Configuration["SAPJob"];
            if (!Directory.Exists(SAPJob))
            {
                Directory.CreateDirectory(SAPJob);
            }
            string SAPBackup = Configuration["SAPBackup"];
            if (!Directory.Exists(SAPBackup))
            {
                Directory.CreateDirectory(SAPBackup);
            }
            string SAPLogs = Configuration["SAPLogs"];
            if (!Directory.Exists(SAPLogs))
            {
                Directory.CreateDirectory(SAPLogs);
            }
            #endregion

            #region Scale create path
            string ScaleError = Configuration["ScaleError"];
            if (!Directory.Exists(ScaleError))
            {
                Directory.CreateDirectory(ScaleError);
            }
            string ScaleJob = Configuration["ScaleJob"];
            if (!Directory.Exists(ScaleJob))
            {
                Directory.CreateDirectory(ScaleJob);
            }
            string ScaleBackup = Configuration["ScaleBackup"];
            if (!Directory.Exists(ScaleBackup))
            {
                Directory.CreateDirectory(ScaleBackup);
            }
            string ScaleLogs = Configuration["ScaleLogs"];
            if (!Directory.Exists(ScaleLogs))
            {
                Directory.CreateDirectory(ScaleLogs);
            }
            #endregion
        }

    }
}
