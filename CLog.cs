using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Net;
using System.Collections;
using System.IO;
using System.Data.Odbc;
using System.Data;
using System.Security.Permissions;
using System.Text;
using System.Security;
using System.Configuration;
using System.Data.SqlClient;
using System.Security.Cryptography;
using System.Net.Mail;
using System.Globalization;
using System.Security.Policy;
using System.Xml.Serialization;
using System.Security.AccessControl;


namespace CommonFunctionClassLibrary
{ /************************************************************************************
        Author :    Meghna Shetye
        created:	23 Feb 2010
        file base:	CLog.cs
        purpose:	Log Creation and Deletion
    *************************************************************************************/

   public class CLog
    {
        public string startdate;
        public string status;
        public string strLogStatus;
        public TimeSpan datediff;

        /// <summary>
        /// Added by Meghna Shetye
        /// On 23 Feb 2010
        /// Takes log file path and log status as input 
        /// and creates the log file with the log status
        /// </summary>
        /// <param name="strPath">Log File path </param>
        /// <param name="strLogStatus">Log Status</param>
       
        public void CreateOrderLog(string strPath, string strLogStatus)
        {
            StreamWriter sr = File.CreateText(strPath.ToString());
            sr.WriteLine("startdate=" + DateTime.Now);
            sr.WriteLine("status=" + strLogStatus.ToString());
            sr.Dispose();
            sr.Close();
        }

        /// <summary>
        /// Added by Meghna Shetye
        /// On 23 Feb 2010
        /// Takes logfiles path as input 
        /// and deletes the log file
        /// </summary>
        /// <param name="strPath">Logfile Path</param>
      
        public void DeleteOrderLog(string strPath)
        {
            File.Delete(strPath);
            Console.WriteLine("sucessfully");
        }

        /// <summary>
        /// Added by Meghna Shetye
        /// On 23 Feb 2010
        /// Takes logfile path  as input 
        /// and returns the log status
        /// </summary>
        /// <param name="strPath">log file path</param>
        /// <returns>strLogStatus</returns>

        public string chkOrderLogStatus(string strPath)
        {
            ArrayList errlist = new ArrayList();

            int i;
            i = 0;
            if (File.Exists(strPath))
            {
                //Console.WriteLine(path);
                using (StreamReader sr1 = new StreamReader(strPath))
                {
                    DateTime date1, date2;
                    date2 = (DateTime.Now);
                    int cnt;
                    cnt = sr1.Peek();
                    string[] line = new string[cnt];
                    Console.WriteLine("Checking Log");
                    if (sr1.Peek() == 0)
                    {
                        try
                        {
                            File.Delete(strPath);
                        }

                        catch (Exception e)
                        {
                            Console.WriteLine("The process failed: {0}", e.ToString());
                        }

                        errlist.Add("0");
                        errlist.Add("0");
                        errlist.Add("0");
                    }
                    else
                    {
                        while (sr1.Peek() > 0)
                        {
                            line[i] = sr1.ReadLine();
                            //Console.WriteLine("The value of line" + i + " " + line[i]);
                            string[] separr = new string[cnt];
                            Regex r = new Regex("(=)"); // Split on assignment.

                            separr = r.Split(line[i], 2);
                            switch (separr[0])
                            {
                                case "startdate":
                                    startdate = separr[2];
                                    date1 = Convert.ToDateTime(startdate);
                                    datediff = date2 - date1;
                                    errlist.Add(datediff.Hours.ToString());
                                    errlist.Add(datediff.Minutes.ToString());
                                    break;
                                case "status":
                                    status = separr[2];
                                    errlist.Add(status.ToString());
                                    break;
                            }
                            i++;
                        }
                    }
                    sr1.Dispose();
                    sr1.Close();
                }

                //Modified to download images of  multiple Order 
                /*Code to check if the order is in use by another method*/
                int iHr, iMin;
                iHr = Convert.ToInt32(errlist[0].ToString());
                iMin = Convert.ToInt32(errlist[1].ToString());
                if (iHr == 0)
                {
                    #region Minutes check
                    if (iMin >= 10)
                    {
                        strLogStatus = "error";
                        //Console.WriteLine("Send mail regarding the Order");
                        File.Delete(strPath);
                    }
                    else
                    {
                        if (iMin < 0)
                        {
                            File.Delete(strPath);
                            strLogStatus = "start";

                        }
                        else
                        {
                            if (errlist[2].ToString() == "start")
                            {
                                Console.WriteLine("Process not yet started");
                                strLogStatus = "start";
                            }
                            if (errlist[2].ToString() == "started")
                            {
                                Console.WriteLine("Another Process is using the order");
                                strLogStatus = "started";
                            }
                            if (errlist[2].ToString() == "done")
                            {
                                Console.WriteLine("Process Completed");
                                strLogStatus = "done";
                                File.Delete(strPath);
                            }
                            if (errlist[2].ToString() == "error")
                            {
                                Console.WriteLine("there is some problem with the order.Needs to be checked");
                                Console.WriteLine("send mail");
                                strLogStatus = "error";
                            }

                        }
                    }
                    //END
                    #endregion
                }
                else
                {
                    File.Delete(strPath);
                    strLogStatus = "start";
                }
            }
            else
            {
                errlist.Add("0");
                errlist.Add("0");
                errlist.Add("0");
                Console.WriteLine("Download order");
                strLogStatus = "start";
            }
            //Console.WriteLine("Current log Status " + strLogStatus.ToString());
            return strLogStatus;
        }

    }
}
