using Quartz;
using Quartz.Spi;

namespace Application.Common.Interfaces;

public class SingletonJobFactory : IJobFactory
{
    private readonly IServiceProvider _serviceProvider;

    public SingletonJobFactory(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
    }

    public IJob NewJob(TriggerFiredBundle bundle, IScheduler scheduler)
    {
        Type type = bundle.JobDetail.JobType;
        return _serviceProvider.GetService(type) as IJob;
    }

    public void ReturnJob(IJob job)
    {
        if (job is IDisposable disposableJob)
        {
            disposableJob.Dispose();
        }
    }
}
