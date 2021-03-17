using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.IO;
using System.Net;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.Management;
using System.Windows.Forms;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;


namespace mJinny
{
    class SyncLog
    {
        private StreamWriter logStream;
        private readonly object lockLogWrite = new object();
        private string path;

        public SyncLog(string fullPath)
        {
            path = fullPath;
            logStream = new StreamWriter(fullPath, true);
            logStream.AutoFlush = true;
        }

        ~SyncLog()
        {
            if (null != logStream)
            {
                logStream.Close();
            }
        }

        public void Write(string strMsg)
        {
            try
            {
                lock (lockLogWrite)
                {
                    Console.WriteLine(strMsg);
                    logStream.WriteLine("[" + DateTime.Now.ToString("dd.MM.yy HH:mm:ss.fff") + "]" +
                        "[" + Thread.CurrentThread + "]" +
                        ": " + strMsg);
                }
            }
            catch { }
        }

    }
}