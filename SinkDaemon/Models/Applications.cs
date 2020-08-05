using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SinkDaemon.Models
{
    public class Applications
    {
        /// <summary>
        /// App名字
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// App打开路径
        /// </summary>
        public string Path { get; set; }
        /// <summary>
        /// App启动参数
        /// </summary>
        public string StartArgs { get; set; }
        /// <summary>
        /// 轮询时间间隔(单位:毫秒)
        /// </summary>
        public string Cycle { get; set; }
    }
}
