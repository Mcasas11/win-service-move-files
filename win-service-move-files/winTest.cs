using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
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
            string conn = ConfigurationManager.ConnectionStrings["DbConnect"].ConnectionString;

            if (flag)
            {
                return;
            }
            try
            {
                flag = true;

                General.LogActions("Starting with file paths.");

                string InputPath = ConfigurationManager.AppSettings["InputPath"].ToString();
                string ProcessPath = ConfigurationManager.AppSettings["ProcessPath"].ToString();
                string OutPath = ConfigurationManager.AppSettings["OutPath"].ToString();

                //Directory access
                DirectoryInfo InPath = new DirectoryInfo(InputPath);
                DirectoryInfo PrPath = new DirectoryInfo(ProcessPath);
                DirectoryInfo OuPath = new DirectoryInfo(OutPath);
                //Move files
                foreach (var file in InPath.GetFiles("*", SearchOption.AllDirectories))
                {
                    if (File.Exists(PrPath + file.Name))
                    {
                        File.SetAttributes(PrPath + file.Name, FileAttributes.Normal);
                        File.Delete(PrPath + file.Name);
                        General.LogActions("File exists in destination path so, we need to delete it.");
                    }

                    General.LogActions("Copying file.");
                    //File.Copy(InPath + file.Name, PrPath + file.Name);
                    File.Move(InPath + file.Name, PrPath + file.Name);
                    File.SetAttributes(PrPath + file.Name, FileAttributes.Normal);
                    General.LogActions("File copied.");
                    File.Delete(InPath + file.Name);
                    General.LogActions("File deleted from origin path.");

                    if (File.Exists(PrPath + file.Name))
                    {
                        General.LogActions("File copy successful.");
                    }
                    else
                    {
                        General.LogActions("Error in process. Can't copy file.");
                    }
                }

                foreach (var file in PrPath.GetFiles("*", SearchOption.AllDirectories))
                {
                    if (File.Exists(PrPath + file.Name))
                    {
                        string fileName = @PrPath + file.Name;
                        General.LogActions("Starting process to insert data.");
                        InsertData(fileName, conn);
                        General.LogActions("Process finished successfully. Now starting to move file to process folder.");
                        MoveToProcess(PrPath + file.Name, OuPath + file.Name);
                        General.LogActions("Moving file to process folder successfully.");
                    }
                }
            }
            catch (Exception ex)
            {
                General.LogExeption(ex);
            }
        }

        #region InsertData
        private void InsertData(string fileName, string conn)
        {
            try
            {
                DataTable dataInput = new DataTable();
                dataInput.Columns.AddRange(new DataColumn[4]
                {
                    new DataColumn("Month_Countable",typeof(int)),
                    new DataColumn("Client_RUT", typeof(string)),
                    new DataColumn("Trx_Count", typeof(string)),
                    new DataColumn("Service_Code", typeof(string))
                });

                string lines = File.ReadAllText(fileName);

                General.LogActions("File.");

                foreach (string row in lines.Split('\r', '\n'))
                {
                    if (!string.IsNullOrEmpty(row))
                    {
                        dataInput.Rows.Add();
                        int i = 0;
                        foreach (string cell in row.Split(';'))
                        {
                            dataInput.Rows[dataInput.Rows.Count - 1][i] = cell.Trim();
                            i++;
                        }
                    }
                }

                using (SqlConnection sql = new SqlConnection(conn))
                {
                    using (SqlBulkCopy sqlBC = new SqlBulkCopy(sql))
                    {
                        sqlBC.DestinationTableName = "dbo.tbl_Interface";
                        sql.Open();
                        sqlBC.WriteToServer(dataInput);
                        sql.Close();
                    }
                }
            }
            catch(Exception ex)
            {
                General.LogExeption(ex);
            }
        }


        #endregion

        #region MoveToProcessed
        public void MoveToProcess(string originPath, string destinationPath)
        {
             if (File.Exists(destinationPath))
             {
                 File.SetAttributes(destinationPath, FileAttributes.Normal);
                 File.Delete(destinationPath);
             }

             File.Move(originPath, destinationPath);
             File.SetAttributes(destinationPath, FileAttributes.Normal);
             File.Delete(originPath);

             if (File.Exists(destinationPath))
             {
                 EventLog.WriteEntry("File copied successful.", EventLogEntryType.Information);
             }
             else
             {
                 EventLog.WriteEntry("File can't copy.", EventLogEntryType.Information);
             }
        }
        #endregion
    }
}
