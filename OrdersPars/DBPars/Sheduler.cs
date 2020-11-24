using Quartz;
using Quartz.Impl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrdersPars
{
    class Sheduler
    {
        public static async void Start()
        {
            IScheduler scheduler = await StdSchedulerFactory.GetDefaultScheduler();
            await scheduler.Start();

            var Off = DateTime.Today;
            Off = Off.AddHours(00);
            Off = Off.AddMinutes(3);


            var DOn = DateTime.Today;
            DOn = DOn.AddHours(9);
            DOn = DOn.AddMinutes(0);
            var DOff = DateTime.Today;
            DOff = DOff.AddHours(20);
            DOff = DOff.AddMinutes(00);

            IJobDetail jobOn = JobBuilder.Create<Job>().Build();
            IJobDetail jobOff = JobBuilder.Create<Job>().Build();
            IJobDetail jobOff2 = JobBuilder.Create<Job>().Build();


            ITrigger trigger = TriggerBuilder.Create()  
                .WithIdentity("trigger1", "group1")    
                .StartAt(DOn)
                .EndAt(DOff)
                .ForJob(jobOn)
                .Build();

            ITrigger trigger2 = TriggerBuilder.Create()
                .WithIdentity("trigger2", "group2")
                .StartAt(DOff)
                .ForJob(jobOff)
                .Build();

            ITrigger trigger3 = TriggerBuilder.Create()
                .WithIdentity("trigger3", "group3")
                .StartAt(Off)
                .WithSimpleSchedule(x => x
                        .WithMisfireHandlingInstructionNextWithExistingCount())
                .EndAt(DOn)
                .ForJob(jobOff2)
                .Build();

            await scheduler.ScheduleJob(jobOn, trigger);
            await scheduler.ScheduleJob(jobOff, trigger2);
            await scheduler.ScheduleJob(jobOff2, trigger3);
        }
    }
}
