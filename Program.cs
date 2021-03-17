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
    class Constants
    {
        public const string ProfileDirPrefix = "Profile";
        public const string AnyProfileParentDir = "Default";

        // contains a path to a "base" empty Chrome data directory
        const string ChromeDataWorkDir = "chromeWorkData";
        const string ProfilesDataWorkDir = "profilesWorkData";
    }

    class Program
    {
        private static SyncLog syncLog;
        static void Main(string[] args)
        {
            syncLog = new SyncLog(Application.StartupPath + "\\log.txt");
            int MaxWorkers = 1; // TODO
            var WorkPool = new List<WebWorkThread>(MaxWorkers);

            for (int i = 0; i < MaxWorkers; i++)
            {
                WorkPool.Add(new WebWorkThread(i, syncLog));
                WorkPool[i].Start();
            }

            int aliveWorkers = 0;
            do
            {
                aliveWorkers = 0;
                for (int i = 0; i < MaxWorkers; i++)
                {
                    if (WorkPool[i] != null &&
                        WorkPool[i].thread.ThreadState != System.Threading.ThreadState.Stopped)
                    {
                        aliveWorkers++;
                    }

                }
                Thread.Sleep(5000);
            }
            while (aliveWorkers > 0);
            syncLog.Write("the program has exited");
        }

        static void AddLog(string strMsg)
        {
            syncLog.Write(strMsg);
        }

    }
}