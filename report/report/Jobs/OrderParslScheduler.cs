using Quartz;
using Quartz.Impl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace report.Jobs
{
    public class OrderParslScheduler
    {
        public static async void Start()
        {
            IScheduler scheduler = await StdSchedulerFactory.GetDefaultScheduler();
            await scheduler.Start();

            IJobDetail OrderParsJob = JobBuilder.Create<OrderPars>().Build();
            IJobDetail TrackingJob = JobBuilder.Create<Tracking>().Build();
            IJobDetail OrderJob = JobBuilder.Create<ShopOrders>().Build();
            IJobDetail ToysiJob = JobBuilder.Create<Toysi>().Build();
            IJobDetail ParseJob = JobBuilder.Create<Pars>().Build();

            ITrigger ParsTrigger = TriggerBuilder.Create()
               .WithIdentity("ParsTrigger", "ParsTriggergroup")
               .StartNow()
               .WithSimpleSchedule(x => x
                   .WithIntervalInMinutes(1)
                   .RepeatForever())
               .Build();

            ITrigger OrderParsTrigger = TriggerBuilder.Create()
                .WithIdentity("OrderParsTrigger", "OrderParsTriggergroup")
                .StartNow()
                .WithSimpleSchedule(x => x
                    .WithIntervalInMinutes(1)
                    .RepeatForever())
                .Build();

            ITrigger ToysiTrigger = TriggerBuilder.Create()
                .WithIdentity("ToysiTrigger", "ToysiTriggergroup")
                .StartNow()
                .WithSimpleSchedule(x => x
                    .WithIntervalInHours(24)
                    .RepeatForever())
                .Build();

            ITrigger TrackingTrigger = TriggerBuilder.Create()
                .WithIdentity("TrackingTrigger", "TrackingTriggergroup")
                .StartNow()
                .WithSimpleSchedule(x => x
                    .WithIntervalInHours(3)
                    .RepeatForever())
                .Build();

            ITrigger OrderTrigger = TriggerBuilder.Create()
                .WithIdentity("OrderTrigger", "OrderTriggergroup")
                .StartNow()
                .WithSimpleSchedule(x => x
                    .WithIntervalInMinutes(1)
                    .RepeatForever())
                .Build();


            await scheduler.ScheduleJob(ToysiJob, ToysiTrigger);
            await scheduler.ScheduleJob(OrderParsJob, OrderParsTrigger);
            await scheduler.ScheduleJob(TrackingJob, TrackingTrigger);
            await scheduler.ScheduleJob(OrderJob, OrderTrigger);
            await scheduler.ScheduleJob(ParseJob, ParsTrigger);
        }
    }
}