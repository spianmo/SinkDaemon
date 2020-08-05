using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Quartz;
using Quartz.Impl;
using Quartz.Impl.Triggers;


namespace SinkDaemon.Utils
{
    /// <summary>
    /// 定时任务管理器
    /// </summary>
    public class QuartzManager
    {
        /// <summary>
        /// 任务调度器工厂
        /// </summary>
        private static ISchedulerFactory SF = null;
        /// <summary>
        /// 任务调度器
        /// </summary>
        private static IScheduler Sched = null;

        static QuartzManager()
        {
            //实例化任务调度器工厂
            SF = new StdSchedulerFactory();
            //获取一个任务调度器
            Sched = SF.GetScheduler().Result;
            //启动任务调度器
            Sched.Start();
        }

        /// <summary>
        /// 添加Job 并且以定点的形式运行
        /// </summary>
        /// <typeparam name="T">继承了IJob接口的任务</typeparam>
        /// <param name="JobName">任务名</param>
        /// <param name="CronTime"></param>
        /// <param name="jobDataMap"></param>
        /// <returns></returns>
        public static DateTimeOffset AddJob<T>(string JobName, string CronTime, string jobData) where T : IJob
        {
            IJobDetail JobCheck = JobBuilder.Create<T>().WithIdentity(JobName, JobName + "_Group").UsingJobData("jobData", jobData).Build();
            ICronTrigger CronTrigger = new CronTriggerImpl(JobName + "_CronTrigger", JobName + "_TriggerGroup", CronTime);
            var Task = Sched.ScheduleJob(JobCheck, CronTrigger); 
            return Task.Result;
        }

        /// <summary>
        /// 添加Job 并且以定点的形式运行
        /// </summary>
        /// <typeparam name="T">实现了IJob接口的任务类</typeparam>
        /// <param name="JobName">任务名称</param>
        /// <param name="CronTime">时间点</param>
        /// <returns></returns>
        public static DateTimeOffset AddJob<T>(string JobName, string CronTime) where T : IJob
        {
            return AddJob<T>(JobName, CronTime, null);
        }


        /// <summary>
        /// 添加Job 并且以周期的形式运行
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="JobName"></param>
        /// <param name="StartTime"></param>
        /// <param name="SimpleTime"></param>
        /// <param name="jobDataMap"></param>
        /// <returns></returns>
        public static DateTimeOffset AddJob<T>(string JobName, DateTimeOffset StartTime, TimeSpan SimpleTime, Dictionary<string, object> map) where T : IJob
        {
            IJobDetail jobCheck = JobBuilder.Create<T>().WithIdentity(JobName, JobName + "_Group").Build();
            jobCheck.JobDataMap.PutAll(map);
            ISimpleTrigger triggerCheck = new SimpleTriggerImpl(JobName + "_SimpleTrigger", JobName + "_TriggerGroup",
                                        StartTime,
                                        null,
                                        SimpleTriggerImpl.RepeatIndefinitely,
                                        SimpleTime);
            var Task = Sched.ScheduleJob(jobCheck, triggerCheck);
            return Task.Result;
        }

        /// <summary>
        /// 添加Job 并且以周期的形式运行
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="JobName"></param>
        /// <param name="SimpleTime"></param>
        /// <returns></returns>
        public static DateTimeOffset AddJob<T>(string JobName, DateTimeOffset StartTime, TimeSpan SimpleTime) where T : IJob
        {
            return AddJob<T>(JobName, StartTime, SimpleTime, new Dictionary<string, object>());
        }

        /// <summary>
        /// 添加Job 并且以周期的形式运行
        /// </summary>
        /// <typeparam name="T">实现了IJob接口的任务</typeparam>
        /// <param name="JobName">任务名</param>
        /// <param name="SimpleTime">毫秒数</param>
        /// <returns></returns>
        public static DateTimeOffset AddJob<T>(string JobName, int SimpleTime) where T : IJob
        {
            return AddJob<T>(JobName, DateTime.UtcNow.AddMilliseconds(1), TimeSpan.FromMilliseconds(SimpleTime));
        }

        /// <summary>
        /// 添加Job 并且以周期的形式运行
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="JobName"></param>
        /// <param name="SimpleTime">毫秒数</param>
        /// <returns></returns>
        public static DateTimeOffset AddJob<T>(string JobName, DateTimeOffset StartTime, int SimpleTime) where T : IJob
        {
            return AddJob<T>(JobName, StartTime, TimeSpan.FromMilliseconds(SimpleTime));
        }



        /// <summary>
        /// 添加Job 并且以周期的形式运行
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="JobName"></param>
        /// <param name="StartTime"></param>
        /// <param name="SimpleTime">毫秒数</param>
        /// <param name="jobDataMap"></param>
        /// <returns></returns>
        public static DateTimeOffset AddJob<T>(string JobName, DateTimeOffset StartTime, int SimpleTime, string MapKey, object MapValue) where T : IJob
        {
            Dictionary<string, object> map = new Dictionary<string, object>();
            map.Add(MapKey, MapValue);
            return AddJob<T>(JobName, StartTime, TimeSpan.FromMilliseconds(SimpleTime), map);
        }



        /// <summary>
        /// 修改触发器时间,需要job名,以及修改结果
        /// CronTriggerImpl类型触发器
        /// </summary>
        public static void UpdateTime(string jobName, string CronTime)
        {
            TriggerKey TKey = new TriggerKey(jobName + "_CronTrigger", jobName + "_TriggerGroup");
            CronTriggerImpl CTI = Sched.GetTrigger(TKey).Result as CronTriggerImpl;
            CTI.CronExpression = new CronExpression(CronTime);
            Sched.RescheduleJob(TKey, CTI);
        }

        /// <summary>
        /// 修改触发器时间,需要job名,以及修改结果
        /// SimpleTriggerImpl类型触发器
        /// </summary>
        /// <param name="jobName"></param>
        /// <param name="SimpleTime">分钟数</param>
        public static void UpdateTime(string jobName, int SimpleTime)
        {
            UpdateTime(jobName, TimeSpan.FromMinutes(SimpleTime));
        }

        /// <summary>
        /// 修改触发器时间,需要job名,以及修改结果
        /// SimpleTriggerImpl类型触发器
        /// </summary>
        public static void UpdateTime(string jobName, TimeSpan SimpleTime)
        {
            TriggerKey TKey = new TriggerKey(jobName + "_SimpleTrigger", jobName + "_TriggerGroup");
            SimpleTriggerImpl STI = Sched.GetTrigger(TKey).Result as SimpleTriggerImpl;
            STI.RepeatInterval = SimpleTime;
            Sched.RescheduleJob(TKey, STI);
        }

        /// <summary>
        /// 暂停所有Job
        /// 暂停功能Quartz提供有很多,以后可扩充
        /// </summary>
        public static void PauseAll()
        {
            Sched.PauseAll();
        }

        /// <summary>
        /// 恢复所有Job
        /// 恢复功能Quartz提供有很多,以后可扩充
        /// </summary>
        public static void ResumeAll()
        {
            Sched.ResumeAll();
        }

        /// <summary>
        /// 删除Job
        /// 删除功能Quartz提供有很多,以后可扩充
        /// </summary>
        /// <param name="JobName"></param>
        public static void DeleteJob(string JobName)
        {
            JobKey JK = new JobKey(JobName, JobName + "_Group");
            Sched.DeleteJob(JK);
        }

        /// <summary>
        /// 卸载定时器
        /// </summary>
        /// <param name="waitForJobsToComplete">是否等待job执行完成</param>
        public static void Shutdown(bool waitForJobsToComplete)
        {
            Sched.Shutdown(waitForJobsToComplete);
        }
    }
}
