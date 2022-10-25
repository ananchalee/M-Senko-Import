using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace M_Senko_Import.Library
{
    internal class Logs
    {

        public static readonly object LError = new object();
        public static readonly object LInfo = new object();

        public static IConfiguration Configuration => new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile("appsettings.json").Build();

        private static string Init(string PathLogs)
        {

            string LogPath = PathLogs + "\\";

            if (string.IsNullOrEmpty(LogPath))
            {
                LogPath = AppDomain.CurrentDomain.SetupInformation.ApplicationBase + "\\Logs\\";
            }

            if (!Directory.Exists(LogPath))
            {
                Directory.CreateDirectory(LogPath);
            }

            return LogPath;
        }

        private static string InitError()
        {

            string LogPath = Configuration["ErrorLog"];

            if (string.IsNullOrEmpty(LogPath))
            {
                LogPath = AppDomain.CurrentDomain.SetupInformation.ApplicationBase + "\\Logs\\";
            }

            if (!Directory.Exists(LogPath))
            {
                Directory.CreateDirectory(LogPath);
            }

            return LogPath;
        }

        public static void Error(string message)
        {
            new Thread(() => WriteError(message)).Start();
            lineNotify(message);
        }

        public static void Info(string PathLogs, string message)
        {
            new Thread(() => WriteInfo(PathLogs, message)).Start();
        }

        public static void LogSMS(string message, string Customer)
        {
            new Thread(() => WriteLogSMS(message, Customer)).Start();
        }

        public static void More(string PathLogs, string message, string fileName)
        {
            new Thread(() => WriteMore(PathLogs, message, fileName)).Start();
        }

        private static void WriteError(string message)
        {
            lock (LError)
            {
                string LogPath = InitError();
                string fileName = LogPath + String.Format("Error-{0:yyyyMMdd}", DateTime.Now) + ".txt";

                using (System.IO.StreamWriter sw = new System.IO.StreamWriter(fileName, true))
                {
                    sw.Write(String.Format("{0:dd/MM/yyyy-HH:mm:ss} | ", DateTime.Now));
                    sw.WriteLine(message);
                    sw.Close();
                    sw.Dispose();
                }
            }
        }

        private static void WriteInfo(string PathLogs, string message)
        {
            lock (LInfo)
            {
                string LogPath = Init(PathLogs);
                string fileName = LogPath + String.Format("Info-{0:yyyyMMdd}", DateTime.Now) + ".txt";

                using (System.IO.StreamWriter sw = new System.IO.StreamWriter(fileName, true))
                {
                    sw.Write(String.Format("{0:dd/MM/yyyy-HH:mm:ss} | ", DateTime.Now));
                    sw.WriteLine(message);
                    sw.Close();
                    sw.Dispose();
                }
            }
        }


        private static void WriteMore(string PathLogs, string message, string fileName)
        {
            lock (LInfo)
            {
                string LogPath = Init(PathLogs);
                fileName = LogPath + fileName + ".txt";
                using (System.IO.StreamWriter sw = new System.IO.StreamWriter(fileName, true))
                {
                    sw.Write(String.Format("{0:dd/MM/yyyy-HH:mm:ss} | ", DateTime.Now));
                    sw.WriteLine(message);
                    sw.Close();
                    sw.Dispose();
                }
            }
        }

        public static void DoWrite(string company, string actionBy, string moduleName, string message, string sqlcommand)
        {
            string fpath = AppDomain.CurrentDomain.SetupInformation.ApplicationBase + "\\" + company + "-" + String.Format("log-{0:ddMMyy}", DateTime.Now) + ".txt";

            using (StreamWriter sw = new StreamWriter(fpath, true))
            {
                sw.WriteLine("=======================================================================");
                sw.Write(String.Format("|{0}\r\n|{2:dd/MM/yyyy HH:mm:ss tt}{1}\r\n|MESSAGE\r\n", actionBy, ((!string.IsNullOrEmpty(moduleName)) ? "\r\n|STACK TRACE\r\n " + moduleName : ""), DateTime.Now));
                sw.WriteLine("\r\n\r\n<<======ERROR MESSAGE=======>>\r\n" + message + ((!string.IsNullOrEmpty(sqlcommand)) ? "\r\n\r\n<<======SQL COMMAND=======>>\r\n" + sqlcommand : ""));
                sw.WriteLine("=======================================================================");
                sw.Close();
            }
            GC.Collect();
        }

        public static void WriteLogRef3(string message, string Customer)
        {
            lock (LError)
            {

                string _LogPath = AppDomain.CurrentDomain.SetupInformation.ApplicationBase + "\\Log - Infomation\\";

                if (!Directory.Exists(_LogPath))
                {
                    Directory.CreateDirectory(_LogPath);
                }

                //string LogPath = Init();
                string fileName = _LogPath + String.Format(Customer + "-{0:dd-MM-yyyy}", DateTime.Now) + ".txt";

                using (System.IO.StreamWriter sw = new System.IO.StreamWriter(fileName, true))
                {
                    //sw.Write(String.Format("{0:dd/MM/yyyy-HH:mm:ss} | ", DateTime.Now));
                    sw.WriteLine(message);
                    sw.Close();
                    sw.Dispose();
                }
            }
        }

        public static void WriteLogSMS(string message, string Customer)
        {
            lock (LInfo)
            {

                string _LogPath = AppDomain.CurrentDomain.SetupInformation.ApplicationBase + "\\LogSMS\\";

                if (!Directory.Exists(_LogPath))
                {
                    Directory.CreateDirectory(_LogPath);
                }

                string fileName = _LogPath + String.Format(Customer + "-{0:dd-MM-yyyy HH}", DateTime.Now) + ".txt";

                using (System.IO.StreamWriter sw = new System.IO.StreamWriter(fileName, true))
                {
                    sw.Write(String.Format("{0:dd/MM/yyyy-HH:mm:ss} | ", DateTime.Now));
                    sw.WriteLine(message);
                    sw.Close();
                    sw.Dispose();
                }
            }
        }

        public static void lineNotify(string msg)
        {
            string token = Configuration["TokenLineNotify"];
            try
            {
                var request = (HttpWebRequest)WebRequest.Create("https://notify-api.line.me/api/notify");
                var postData = string.Format("message={0}", "M-Senko Error --> " + msg);
                var data = Encoding.UTF8.GetBytes(postData);

                request.Method = "POST";
                request.ContentType = "application/x-www-form-urlencoded";
                request.ContentLength = data.Length;
                request.Headers.Add("Authorization", "Bearer " + token);

                using (var stream = request.GetRequestStream()) stream.Write(data, 0, data.Length);
                var response = (HttpWebResponse)request.GetResponse();
                var responseString = new StreamReader(response.GetResponseStream()).ReadToEnd();
            }
            catch (Exception ex)
            {
                new Thread(() => WriteError(ex.Message)).Start();
                Console.WriteLine(ex.ToString());
            }
        }
    }
}

