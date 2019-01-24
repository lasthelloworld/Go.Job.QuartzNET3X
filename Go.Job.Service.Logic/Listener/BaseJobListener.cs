﻿using Quartz;
using Quartz.Listener;
using System;
using System.Threading;
using System.Threading.Tasks;
using Go.Job.Service.Middleware;

namespace Go.Job.Service.Logic.Listener
{
    /// <summary>
    /// job监听器基类
    /// </summary>
    public abstract class BaseJobListener : JobListenerSupport
    {
        protected readonly ILogWriter LogWriter = (ILogWriter)MidContainer.GetService(typeof(ILogWriter));

        protected Action<IJobExecutionContext> StartAction;

        protected Action<IJobExecutionContext> EndAction;


        public override string Name { get; }

        protected BaseJobListener(string name) 
        {
            Name = name;
        }
        

        public override async Task JobWasExecuted(IJobExecutionContext context, JobExecutionException jobException, CancellationToken cancellationToken = default(CancellationToken))
        {
            try
            {
                EndAction?.Invoke(context);
            }
            catch (Exception e)
            {
                LogWriter.WriteException(e, context.JobDetail.Key.Name +":"+nameof(JobWasExecuted));
            }
            await base.JobWasExecuted(context, jobException, cancellationToken);

        }

        public override async Task JobExecutionVetoed(IJobExecutionContext context, CancellationToken cancellationToken = default(CancellationToken))
        {
            await base.JobExecutionVetoed(context, cancellationToken);
        }

        public override async Task JobToBeExecuted(IJobExecutionContext context, CancellationToken cancellationToken = default(CancellationToken))
        {
            try
            {
                StartAction?.Invoke(context);
            }
            catch (Exception e)
            {
                LogWriter.WriteException(e, context.JobDetail.Key.Name + ":" + nameof(JobToBeExecuted));
            }
            await base.JobToBeExecuted(context, cancellationToken);
        }
    }
}