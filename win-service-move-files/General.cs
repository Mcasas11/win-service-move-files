using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace win_service_move_files
{
    public static class General
    {
        public static void LogExeption(Exception ex)
        {
            string logFile = "";
            try
            {
                logFile = System.Configuration.ConfigurationManager.AppSettings["Log_Dir"] + "Test_" + DateTime.Today.ToString("ddMMyyyy") + ".txt";
                StreamWriter sw = new StreamWriter(logFile, true);
                sw.WriteLine(DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss") + "\n" + ex.Message + "\n" + ((ex.InnerException != null) ? ("Inner:" + ex.InnerException.Message) : "") + "\n\n");
                sw.WriteLine(ex.StackTrace);
                sw.Close();
            }
            catch { }
        }

        public static void LogActions(string input)
        {
            string logFile = System.Configuration.ConfigurationManager.AppSettings["Log_Dir"] + "Test_" + DateTime.Today.ToString("ddMMyyyy") + ".txt";
            try
            {
                StreamWriter file = new StreamWriter(logFile, true);
                file.WriteLine(DateTime.Now.ToString() + ": " + input);
                file.Close();
            }
            catch (Exception ex)
            {
                LogExeption(ex);
            }
        }
    }
}
