using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.IO;

namespace MonitoringProgram
{
    //Help logging our text into file if it is created or console 
    internal delegate void DelegateMessage(string text);
    class MonitoringClass
    {
        private string nameProc;
        private int timeWork;
        private int checkFrequency;

        private Process[] proc;

        private DelegateMessage writer;
        private StreamWriter sw;

        public MonitoringClass(string nameProc, int timeWork, int checkFrequency)
        {
            if (!(timeWork >= 0 && checkFrequency >= 0))
                Console.WriteLine("Your time in minutes and frequency should be valid: more then zero");
            else
            {
                this.nameProc = nameProc;
                this.timeWork = timeWork;
                this.checkFrequency = checkFrequency;

                InitSW();
                writer.Invoke("Monitoring start");
                sw.Flush();
                MonitoringProcess();
            }
        }

        ///Create log file
        private void InitSW(string pathTo = "ProcessMonitoringLog.txt")
        {
            try
            {
                sw = new StreamWriter(pathTo);
            }
            catch (IOException ex)
            {
                writer = OnFailureFileCreation;
                writer.Invoke(string.Format("Log: File creation process is failed with " + ex.Message));
                sw.Flush();
            }
            finally
            {
                writer = OnSucceedFileCreation;
                writer.Invoke("Log file was successfully created");
                sw.Flush();
            }
        }

        //Log on failure file creation
        private void OnFailureFileCreation(string text)
        {
            Console.WriteLine("{0:t}\t{1}", DateTime.Now, text);
        }

        //Log on succeed file creation
        private void OnSucceedFileCreation(string text)
        {
            Console.WriteLine("{0:t}\t{1}", DateTime.Now, text);
            sw.WriteLine("{0}\t{1}", DateTime.Now, text);
        }

        //Start of monitoring process
        void MonitoringProcess()
        {
            proc = Process.GetProcessesByName(nameProc);

            try
            {
                if (!Process.GetProcessesByName(nameProc).Any())
                    Console.WriteLine("Process is not found");

                int passedMinutes = 0;

                while (ShouldBeMonitored(passedMinutes))
                {
                    passedMinutes += checkFrequency;
                    writer.Invoke(string.Format("Process work: " + 
                        passedMinutes + " out of "+ timeWork +" minutes"));
                    sw.Flush();
                }
            }
            catch (Exception ex)
            {
                writer.Invoke(string.Format("Exception: " + ex.Message));
                sw.Flush();
            }
            finally
            {
                writer.Invoke("Monitoring end");
                sw.Close();
            }
        }

        //Check if time is passed then process should be killed otherwise continue monitoring
        public bool ShouldBeMonitored(int passedMinutes)
        {
            if (timeWork <= passedMinutes)
            {
                foreach (Process p in proc)
                {
                    p.Kill();
                    Console.WriteLine("Process with id: " + p.Id + " was killed");
                }
                return false;
            }
            Thread.Sleep(checkFrequency * 60000);
            return true;
        }
    }
}
