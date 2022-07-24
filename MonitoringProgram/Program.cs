using System;

namespace MonitoringProgram
{
    class Program
    {
        static void Main(string[] args)
        {
            MonitoringClass mc = new MonitoringClass(args[0], Convert.ToInt32(args[1]), Convert.ToInt32(args[2]));
        }
    }
}
