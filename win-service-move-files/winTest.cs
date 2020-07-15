using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;


namespace win_service_move_files
{
    partial class winTest : ServiceBase
    {
        bool flag = false;
        public winTest()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            tmrWin.Start();
        }

        protected override void OnStop()
        {
            tmrWin.Stop();
        }

        private void tmrWin_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            if (flag)
            {
                return;
            }
            try
            {
                flag = true;

                //EventLog.WriteEntry("Copy to Input Folder.", EventLogEntryType.Information);

                string InputPath = ConfigurationManager.AppSettings["InputPath"].ToString();
                string ProcessPath = ConfigurationManager.AppSettings["ProcessPath"].ToString();
                string OutPath = ConfigurationManager.AppSettings["OutPath"].ToString();

                //Directory access
                DirectoryInfo InPath = new DirectoryInfo(InputPath);
                DirectoryInfo PrPath = new DirectoryInfo(ProcessPath);
                DirectoryInfo OuPath = new DirectoryInfo(OutPath);
                //Move files
                foreach(var file in InPath.GetFiles("*", SearchOption.AllDirectories))
                {
                    if(File.Exists(PrPath + file.Name))
                    {
                        File.SetAttributes(PrPath + file.Name, FileAttributes.Normal);
                        File.Delete(PrPath + file.Name);
                    }

                    File.Copy(InPath + file.Name, PrPath + file.Name);
                    File.SetAttributes(PrPath + file.Name, FileAttributes.Normal);
                    
                    if(File.Exists(PrPath + file.Name))
                    {
                        EventLog.WriteEntry("File copied successful.", EventLogEntryType.Information);
                    }
                    else
                    {
                        EventLog.WriteEntry("File copy in process", EventLogEntryType.Information);
                    }
                }

                EventLog.WriteEntry("Copying process completed");

            }
            catch(Exception ex)
            {
                EventLog.WriteEntry(ex.Message,EventLogEntryType.Error);
            }
        }
    }
}
