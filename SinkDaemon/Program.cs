using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using SinkDaemon.Utils;
using SinkDaemon.Functions;
using SinkDaemon.Models;
using System.IO;
using System.Text;
using System.Diagnostics;

namespace SinkDaemon
{
    static class Program
    {
        public static Applications applicationInfo = new Applications()
        {
            Name = "cmake_gui",
            Path = @"D:\Program Files\CMake\bin\",
            StartArgs = "Fuck",
            Cycle = "4000"
        };

        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            if (!onlyOneMutex())
            {
                return;
            }
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            if (args.Length >= 4)
            {
                applicationInfo = new Applications()
                {
                    Name = args[0],
                    Path = args[1],
                    StartArgs = obainExtraArgs(args),
                    Cycle = args[args.Length-1]
                };
                MessageBox.Show(applicationInfo.StartArgs);
            }

            if (!File.Exists($@"{applicationInfo.Path}\{applicationInfo.Name}.exe"))
            {
                MessageBox.Show($"程序{applicationInfo.Name}.exe 不存在,守护进程失败！");
                return;
            }

            QuartzManager.AddJob<AutoRestart>("AutoRestart", int.Parse(applicationInfo.Cycle) == 0 ? 10000 : int.Parse(applicationInfo.Cycle));
            Application.Run(new frmProtect());
        }

        static bool onlyOneMutex()
        {
            Process[] myProcesses = Process.GetProcessesByName(Application.ProductName);
            return myProcesses.Length > 1 ? false:true ;
        }

        static string obainExtraArgs(string[] args)
        {
            StringBuilder arg_s = new StringBuilder();
            for(int i = 0; i < args.Length; i++)
            {
                if(i!=0 && i != 1 && i != args.Length - 1)
                {
                    arg_s.Append(args[i]);
                    arg_s.Append(" ");
                }
            }
            return arg_s.ToString();
        }
    }
}
