using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SinkDaemon.Utils;
using Quartz;
using System.Configuration;
using SinkDaemon.Models;
using System.IO;

namespace SinkDaemon.Functions
{
    /// <summary>
    /// 重启进程
    /// </summary>
    public class AutoRestart : IJob
    {
        public Task Execute(IJobExecutionContext context)
        {
            Task cycleTask = new Task(CheckAndRestartProcess);
            cycleTask.Start();
            return cycleTask;
        }

        /// <summary>
        /// 检查并重启模板中的所有应用程序
        /// </summary>
        private void CheckAndRestartProcess()
        {
            var applicationInfo = Program.applicationInfo;
            if (Process.GetProcessesByName(applicationInfo.Name).ToList().Count == 0)
            {
                Process fireProcess = new Process();
                try
                {
                    fireProcess.StartInfo.Arguments = applicationInfo.StartArgs;
                    fireProcess.StartInfo.UseShellExecute = true;
                    fireProcess.StartInfo.CreateNoWindow = false;
                    fireProcess.StartInfo.WorkingDirectory = applicationInfo.Path;
                    fireProcess.StartInfo.FileName = $"{applicationInfo.Name}.exe";
                    fireProcess.Start();
                }
                catch (Exception error)
                {
                }
                finally
                {
                    fireProcess.Dispose();
                }
            }

        }
    }
}
